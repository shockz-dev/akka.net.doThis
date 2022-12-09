using Akka.Actor;

using Octokit;

namespace GithubActors.Actors
{
  public class GithubCoordinatorActor : ReceiveActor
  {
    #region messages

    public class BeginJob
    {
      public BeginJob(RepoKey repo)
      {
        Repo = repo;
      }

      public RepoKey Repo { get; private set; }
    }

    public class SubscribeToProgressUpdates
    {
      public SubscribeToProgressUpdates(IActorRef subscriber)
      {
        Subscriber = subscriber;
      }

      public IActorRef Subscriber { get; private set; }
    }

    public class PublishUpdate
    {
      private PublishUpdate()
      { }

      public static readonly PublishUpdate _instance = new PublishUpdate();
      public static PublishUpdate Instance => _instance;
    }

    public class JobFailed
    {
      public JobFailed(RepoKey repo)
      {
        Repo = repo;
      }

      public RepoKey Repo { get; private set; }
    }

    #endregion messages

    private IActorRef _githubWorker;
    private RepoKey _currentRepo;
    private Dictionary<string, SimilarRepo> _similarRepos;
    private HashSet<IActorRef> _subscribers;
    private ICancelable _publishTimer;
    private GithubProgressStats _githubProgressStats;
    private bool _receivedInitialUser = false;

    public GithubCoordinatorActor()
    {
      Waiting();
    }

    protected override void PreStart()
    {
      _githubWorker = Context.ActorOf(Props.Create(() => new GithubWorkerActor(GithubClientFactory.GetClient)));
    }

    private void Waiting()
    {
      Receive<GithubCommanderActor.CanAcceptJob>(job => Sender.Tell(new GithubCommanderActor.AbleToAcceptJob(job.Repo)));
      Receive<BeginJob>(job =>
      {
        BecomeWorking(job.Repo);
        _githubWorker.Tell(new RetryableQuery(new GithubWorkerActor.QueryStarrers(job.Repo), 4));
      });
    }

    private void BecomeWorking(RepoKey repo)
    {
      _receivedInitialUser = false;
      _currentRepo = repo;
      _subscribers = new HashSet<IActorRef>();
      _similarRepos = new Dictionary<string, SimilarRepo>();
      _publishTimer = new Cancelable(Context.System.Scheduler);
      _githubProgressStats = new GithubProgressStats();
      Become(Working);
    }

    private void BecomeWaiting()
    {
      _publishTimer.Cancel();
      Become(Waiting);
    }

    private void Working()
    {
      Receive<GithubWorkerActor.StarredReposForUser>(user =>
      {
        _githubProgressStats = _githubProgressStats.UserQueriesFinished();
        foreach (var repo in user.Repos)
        {
          if (!_similarRepos.ContainsKey(repo.HtmlUrl))
          {
            _similarRepos[repo.HtmlUrl] = new SimilarRepo(repo);
          }

          _similarRepos[repo.HtmlUrl].SharedStarrers++;
        }
      });

      Receive<PublishUpdate>(update =>
      {
        if (_receivedInitialUser && _githubProgressStats.IsFinished)
        {
          _githubProgressStats = _githubProgressStats.Finish();
          var sortedSimilarRepos = _similarRepos.Values.Where(x => x.Repo.Name != _currentRepo.Repo).OrderByDescending(x => x.SharedStarrers).ToList();

          foreach (var subscriber in _subscribers)
          {
            subscriber.Tell(sortedSimilarRepos);
          }
          BecomeWaiting();
        }

        foreach (var subscriber in _subscribers)
        {
          subscriber.Tell(_githubProgressStats);
        }
      });

      Receive<User[]>(users =>
      {
        _receivedInitialUser = true;
        _githubProgressStats = _githubProgressStats.SetExpectedUserCount(users.Length);

        foreach (var user in users)
        {
          _githubWorker.Tell(new RetryableQuery(new GithubWorkerActor.QueryStarrer(user.Login), 3));
        }
      });

      Receive<GithubCommanderActor.CanAcceptJob>(job => Sender.Tell(new GithubCommanderActor.UnableToAcceptJob(job.Repo)));

      Receive<SubscribeToProgressUpdates>(update =>
      {
        if (_subscribers.Count == 0)
        {
          Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100), Self, PublishUpdate.Instance, Self, _publishTimer);
        }

        _subscribers.Add(update.Subscriber);
      });

      Receive<RetryableQuery>(query => query.CanRetry, query => _githubWorker.Tell(query));

      Receive<RetryableQuery>(query => !query.CanRetry && query.Query is GithubWorkerActor.QueryStarrers, query =>
      {
        _receivedInitialUser = true;
        foreach (var subscriber in _subscribers)
        {
          subscriber.Tell(new JobFailed(_currentRepo));
        }
        BecomeWaiting();
      });

      Receive<RetryableQuery>(query => !query.CanRetry && query.Query is GithubWorkerActor.QueryStarrer, query => _githubProgressStats.IncrementFailures());
    }
  }
}