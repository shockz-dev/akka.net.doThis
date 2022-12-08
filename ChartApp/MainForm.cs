using Akka.Actor;
using Akka.Util.Internal;

using ChartApp.Actors;

namespace ChartApp
{
  public partial class MainForm : Form
  {
    private IActorRef _chartActor;
    private readonly AtomicCounter _seriesCounter = new AtomicCounter(1);
    private IActorRef _coordinatorActor;
    private Dictionary<CounterType, IActorRef> _toggleActors = new Dictionary<CounterType, IActorRef>();

    public MainForm()
    {
      InitializeComponent();
    }

    #region initialization

    private void MainForm_Load(object sender, EventArgs e)
    {
      //_chartActor = Program.ChartActors.ActorOf(Props.Create(() => new ChartingActor(sysChart)), "charting");
      //var series = ChartDataHelper.RandomSeries("FakeSeries" + _seriesCounter.GetAndIncrement());
      //_chartActor.Tell(new ChartingActor.InitializeChart(new Dictionary<string, Series>()
      //{
      //  {series.Name, series }
      //}));

      _chartActor = Program.ChartActors.ActorOf(Props.Create(() => new ChartingActor(sysChart)), "charting");
      _chartActor.Tell(new ChartingActor.InitializeChart(null));
      _coordinatorActor = Program.ChartActors.ActorOf(Props.Create(() => new PerformanceCounterCoordinatorActor(_chartActor)), "counters");

      _toggleActors[CounterType.Cpu] = Program.ChartActors.ActorOf(Props.Create(() => new ButtonToggleActor(_coordinatorActor, btnCpu, CounterType.Cpu, false)).WithDispatcher("akka.actor.synchronized-dispatcher"));
      _toggleActors[CounterType.Memory] = Program.ChartActors.ActorOf(Props.Create(() => new ButtonToggleActor(_coordinatorActor, btnMemory, CounterType.Memory, false)).WithDispatcher("akka.actor.synchronized-dispatcher"));
      _toggleActors[CounterType.Disk] = Program.ChartActors.ActorOf(Props.Create(() => new ButtonToggleActor(_coordinatorActor, btnDisk, CounterType.Disk, false)).WithDispatcher("akka.actor.synchronized-dispatcher"));

      _toggleActors[CounterType.Cpu].Tell(new ButtonToggleActor.Toggle());
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      _chartActor.Tell(PoisonPill.Instance);
      Program.ChartActors.Terminate();
    }

    #endregion initialization

    private void btnCpu_Click(object sender, EventArgs e)
    {
      _toggleActors[CounterType.Cpu].Tell(new ButtonToggleActor.Toggle());
    }

    private void btnMemory_Click(object sender, EventArgs e)
    {
      _toggleActors[CounterType.Memory].Tell(new ButtonToggleActor.Toggle());
    }

    private void btnDisk_Click(object sender, EventArgs e)
    {
      _toggleActors[CounterType.Disk].Tell(new ButtonToggleActor.Toggle());
    }
  }
}