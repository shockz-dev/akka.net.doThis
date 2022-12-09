using Akka.Actor;

using GithubActors.Actors;

namespace GithubActors
{
  public partial class RepoResultsForm : Form
  {
    private IActorRef _formActor;
    private IActorRef _githubCoordinator;
    private RepoKey _repo;

    public RepoResultsForm(IActorRef githubCoordinator, RepoKey repo)
    {
      _githubCoordinator = githubCoordinator;
      _repo = repo;
      InitializeComponent();
    }

    private void RepoResultsForm_Load(object sender, EventArgs e)
    {
      _formActor = Program.GithubActors.ActorOf(Props.Create(() => new RepoResultsActor(dgUsers, tsStatus, tsProgress))
        .WithDispatcher("akka.actor.synchronized-dispatcher"));

      Text = string.Format("Repo Similar to {0} / {1}", _repo.Owner, _repo.Repo);

      _githubCoordinator.Tell(new GithubCoordinatorActor.SubscribeToProgressUpdates(_formActor));
    }

    private void RepoResultsForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      _formActor.Tell(PoisonPill.Instance);
    }
  }
}