using Akka.Actor;

using Octokit;

namespace GithubActors.Actors
{
  public class GithubValidatorActor : ReceiveActor
  {
    #region messages

    public class ValidateRepo
    {
      public ValidateRepo(string repoUri)
      {
        RepoUri = repoUri;
      }

      public string RepoUri { get; private set; }
    }

    public class InvalidRepo
    {
      public InvalidRepo(string repoUri, string reason)
      {
        Reason = reason;
        RepoUri = repoUri;
      }

      public string Reason { get; private set; }
      public string RepoUri { get; private set; }
    }

    public class SystemBusy
    { }

    public class RepoIsValid
    {
      private RepoIsValid()
      { }

      private static readonly RepoIsValid _instance = new RepoIsValid();

      public static RepoIsValid Instance
      {
        get { return _instance; }
      }
    }

    #endregion messages

    private readonly IGitHubClient _gitHubClient;

    public GithubValidatorActor(IGitHubClient gitHubClient)
    {
      _gitHubClient = gitHubClient;
      ReadyToValidate();
    }

    private void ReadyToValidate()
    {
      Receive<ValidateRepo>(repo => string.IsNullOrEmpty(repo.RepoUri) || !Uri.IsWellFormedUriString(repo.RepoUri, UriKind.Absolute), repo => Sender.Tell(new InvalidRepo(repo.RepoUri, "Not a valid absolute URI")));

      Receive<ValidateRepo>(repo =>
      {
        var userOwner = SplitIntoOwnerAndRepo(repo.RepoUri);
        var sender = Sender;
        _gitHubClient.Repository.Get(userOwner.Item1, userOwner.Item2).ContinueWith<object>(t =>
        {
          if (t.IsCanceled)
          {
            return new InvalidRepo(repo.RepoUri, "Repo lookup timed out");
          }
          if (t.IsFaulted)
          {
            return new InvalidRepo(repo.RepoUri, t.Exception != null ? t.Exception.GetBaseException().Message : "Unknown Octokit error");
          }

          return t.Result;
        }).PipeTo(Self, sender);
      });

      Receive<InvalidRepo>(repo => Sender.Forward(repo));

      Receive<Repository>(repository =>
      {
        Context.ActorSelection(ActorPaths.GithubCommanderActor.Path).Tell(new GithubCommanderActor.CanAcceptJob(new RepoKey(repository.Owner.Login, repository.Name)));
      });

      Receive<GithubCommanderActor.UnableToAcceptJob>(job => Context.ActorSelection(ActorPaths.MainFormActor.Path).Tell(job));
      Receive<GithubCommanderActor.AbleToAcceptJob>(job => Context.ActorSelection(ActorPaths.MainFormActor.Path).Tell(job));
    }

    public static Tuple<string, string> SplitIntoOwnerAndRepo(string repoUri)
    {
      var split = new Uri(repoUri, UriKind.Absolute).PathAndQuery.TrimEnd('/').Split('/').Reverse().ToList();
      return Tuple.Create(split[1], split[0]); // user, repo
    }
  }
}