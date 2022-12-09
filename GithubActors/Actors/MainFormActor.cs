using Akka.Actor;

namespace GithubActors.Actors
{
  public class MainFormActor : ReceiveActor, IWithUnboundedStash
  {
    #region messages

    public class LaunchRepoResultsWindow
    {
      public LaunchRepoResultsWindow(RepoKey repo, IActorRef coordinator)
      {
        Repo = repo;
        Coordinator = coordinator;
      }

      public RepoKey Repo { get; private set; }
      public IActorRef Coordinator { get; private set; }
    }

    #endregion messages

    private readonly Label _validationLabel;

    public MainFormActor(Label validationLabel)
    {
      _validationLabel = validationLabel;
      Ready();
    }

    private void Ready()
    {
      Receive<ProcessRepo>(repo =>
      {
        Context.ActorSelection(ActorPaths.GithubValidatorActor.Path).Tell(new GithubValidatorActor.ValidateRepo(repo.RepoUri));
        BecomeBusy(repo.RepoUri);
      });

      Receive<LaunchRepoResultsWindow>(window =>
      {
        var form = new RepoResultsForm(window.Coordinator, window.Repo);
        form.Show();
      });
    }

    private void BecomeBusy(string repoUri)
    {
      _validationLabel.Visible = true;
      _validationLabel.Text = string.Format("Validating {0}...", repoUri);
      _validationLabel.ForeColor = Color.Gold;
      Become(Busy);
    }

    private void Busy()
    {
      Receive<GithubValidatorActor.RepoIsValid>(valid => BecomeReady("Valid"));
      Receive<GithubValidatorActor.InvalidRepo>(invalid => BecomeReady(invalid.Reason, false));
      Receive<GithubCommanderActor.UnableToAcceptJob>(job => BecomeReady(string.Format("{0}/{1} is a valid repo, but system can't accept additional jobs", job.Repo.Owner, job.Repo.Repo), false));
      Receive<GithubCommanderActor.AbleToAcceptJob>(job => BecomeReady(string.Format("{0}/{1} is a valid repo - starting job!", job.Repo.Owner, job.Repo.Repo)));
      Receive<LaunchRepoResultsWindow>(window => Stash.Stash());
    }

    private void BecomeReady(string message, bool isValid = true)
    {
      _validationLabel.Text = message;
      _validationLabel.ForeColor = isValid ? Color.Green : Color.Red;
      Stash.UnstashAll();
      Become(Ready);
    }

    public IStash Stash { get; set; }
  }
}