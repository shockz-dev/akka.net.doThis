using Akka.Actor;

namespace GithubActors.Actors
{
  public class GithubCommanderActor : ReceiveActor
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

    public GithubCommanderActor()
    {
      Receive<CanAcceptJob>(job =>
      {
        _canAcceptJobSender = Sender;
        _coordinator.Tell(job);
      });

      Receive<UnableToAcceptJob>(job =>
      {
        _canAcceptJobSender.Tell(job);
      });

      Receive<AbleToAcceptJob>(job =>
      {
        _canAcceptJobSender.Tell(job);

        _coordinator.Tell(new GithubCoordinatorActor.BeginJob(job.Repo));

        Context.ActorSelection(ActorPaths.MainFormActor.Path).Tell(new MainFormActor.LaunchRepoResultsWindow(job.Repo, Sender));
      });
    }

    protected override void PreStart()
    {
      _coordinator = Context.ActorOf(Props.Create(() => new GithubCoordinatorActor()), ActorPaths.GithubCoordinatorActor.Name);
      base.PreStart();
    }

    protected override void PreRestart(Exception reason, object message)
    {
      _coordinator.Tell(PoisonPill.Instance);
      base.PreRestart(reason, message);
    }
  }
}