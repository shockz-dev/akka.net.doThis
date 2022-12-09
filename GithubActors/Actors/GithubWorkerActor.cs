using Akka.Actor;

using Octokit;

namespace GithubActors.Actors
{
  public class GithubWorkerActor : ReceiveActor
  {
    #region messages

    public class QueryStarrers
    {
      public QueryStarrers(RepoKey key)
      {
        Key = key;
      }

      public RepoKey Key { get; private set; }
    }

    public class QueryStarrer
    {
      public QueryStarrer(string login)
      {
        Login = login;
      }

      public string Login { get; private set; }
    }

    public class StarredReposForUser
    {
      public StarredReposForUser(string login, IEnumerable<Repository> repos)
      {
        Repos = repos;
        Login = login;
      }

      public IEnumerable<Repository> Repos { get; private set; }
      public string Login { get; private set; }
    }

    #endregion messages

    private IGitHubClient _gitHubClient;
    private readonly Func<IGitHubClient> _gitHubClientFactory;

    public GithubWorkerActor(Func<IGitHubClient> gitHubClientFactory)
    {
      _gitHubClientFactory = gitHubClientFactory;
      InitialReceves();
    }

    protected override void PreStart()
    {
      _gitHubClient = _gitHubClientFactory();
    }

    private void InitialReceves()
    {
      Receive<RetryableQuery>(query => query.Query is QueryStarrer, query =>
      {
        var starrer = (query.Query as QueryStarrer).Login;

        var sender = Sender;
        _gitHubClient.Activity.Starring.GetAllForUser(starrer).ContinueWith<object>(tr =>
        {
          if (tr.IsFaulted || tr.IsCanceled)
          {
            return query.NextTry();
          }

          return new StarredReposForUser(starrer, tr.Result);
        }).PipeTo(sender);
      });

      Receive<RetryableQuery>(query => query.Query is QueryStarrers, query =>
      {
        var starrers = (query.Query as QueryStarrers).Key;

        var sender = Sender;
        _gitHubClient.Activity.Starring.GetAllStargazers(starrers.Owner, starrers.Repo).ContinueWith<object>(tr =>
        {
          if (tr.IsFaulted || tr.IsCanceled)
          {
            return query.NextTry();
          }

          return tr.Result.ToArray();
        }).PipeTo(sender);
      });
    }
  }
}