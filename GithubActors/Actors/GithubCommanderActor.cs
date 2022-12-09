using Akka.Actor;
using Akka.Routing;

namespace GithubActors.Actors
{
  public class GithubCommanderActor : ReceiveActor, IWithUnboundedStash
  {
    #region messages

    public class CanAcceptJob
    {
      public CanAcceptJob(RepoKey repo)
      {
        Repo = repo;
      }

      public RepoKey Repo { get; private set; }
    }

    public class AbleToAcceptJob
    {
      public AbleToAcceptJob(RepoKey repo)
      {
        Repo = repo;
      }

      public RepoKey Repo { get; private set; }
    }

    public class UnableToAcceptJob
    {
      public UnableToAcceptJob(RepoKey repo)
      {
        Repo = repo;
      }

      public RepoKey Repo { get; private set; }
    }

    #endregion messages

    private IActorRef _coordinator;
    private IActorRef _canAcceptJobSender;
    private int pendingJobReplies;
    private RepoKey _repoJob;

    public IStash Stash { get; set; }

    public GithubCommanderActor()
    {
      Ready();
    }

    private void Ready()
    {
      Receive<CanAcceptJob>(job =>
      {
        _coordinator.Tell(job);
        _repoJob = job.Repo;
        BecomeAsking();
      });
    }

    private void BecomeAsking()
    {
      _canAcceptJobSender = Sender;
      //pendingJobReplies = 3;
      pendingJobReplies = _coordinator.Ask<Routees>(new GetRoutees()).Result.Members.Count();
      Become(Asking);

      Context.SetReceiveTimeout(TimeSpan.FromSeconds(3));
    }

    private void Asking()
    {
      Receive<CanAcceptJob>(job => Stash.Stash());

      Receive<UnableToAcceptJob>(job =>
      {
        pendingJobReplies--;
        if (pendingJobReplies == 0)
        {
          _canAcceptJobSender.Tell(job);

          BecomeReady();
        }
      });

      Receive<AbleToAcceptJob>(job =>
      {
        _canAcceptJobSender.Tell(job);

        Sender.Tell(new GithubCoordinatorActor.BeginJob(job.Repo));

        Context.ActorSelection(ActorPaths.MainFormActor.Path).Tell(new MainFormActor.LaunchRepoResultsWindow(job.Repo, Sender));

        BecomeReady();
      });

      Receive<ReceiveTimeout>(timeout =>
      {
        _canAcceptJobSender.Tell(new UnableToAcceptJob(_repoJob));
        BecomeReady();
      });
    }

    private void BecomeReady()
    {
      Become(Ready);
      Stash.UnstashAll();

      Context.SetReceiveTimeout(null);
    }

    protected override void PreStart()
    {
      //var c1 = Context.ActorOf(Props.Create(() => new GithubCoordinatorActor()), ActorPaths.GithubCoordinatorActor.Name + "1");
      //var c2 = Context.ActorOf(Props.Create(() => new GithubCoordinatorActor()), ActorPaths.GithubCoordinatorActor.Name + "2");
      //var c3 = Context.ActorOf(Props.Create(() => new GithubCoordinatorActor()), ActorPaths.GithubCoordinatorActor.Name + "3");

      //_coordinator = Context.ActorOf(Props.Empty.WithRouter(new BroadcastGroup(ActorPaths.GithubCoordinatorActor.Path + "1", ActorPaths.GithubCoordinatorActor.Path + "2", ActorPaths.GithubCoordinatorActor.Path + "3")));
      //base.PreStart();

      _coordinator = Context.ActorOf(Props.Create(() => new GithubCoordinatorActor()).WithRouter(FromConfig.Instance), ActorPaths.GithubCoordinatorActor.Name);
      base.PreStart();
    }

    protected override void PreRestart(Exception reason, object message)
    {
      _coordinator.Tell(PoisonPill.Instance);
      base.PreRestart(reason, message);
    }
  }
}