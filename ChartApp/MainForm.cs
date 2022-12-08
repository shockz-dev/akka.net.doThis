using System.Windows.Forms.DataVisualization.Charting;

using Akka.Actor;
using Akka.Util.Internal;

using ChartApp.Actors;

namespace ChartApp
{
  public partial class MainForm : Form
  {
    private IActorRef _chartActor;
    private readonly AtomicCounter _seriesCounter = new AtomicCounter(1);

    public MainForm()
    {
      InitializeComponent();
    }

    #region initialization

    private void MainForm_Load(object sender, EventArgs e)
    {
      _chartActor = Program.ChartActors.ActorOf(Props.Create(() => new ChartingActor(sysChart)), "charting");
      var series = ChartDataHelper.RandomSeries("FakeSeries" + _seriesCounter.GetAndIncrement());
      _chartActor.Tell(new ChartingActor.InitializeChart(new Dictionary<string, Series>()
      {
        {series.Name, series }
      }));
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      _chartActor.Tell(PoisonPill.Instance);
      Program.ChartActors.Terminate();
    }

    #endregion initialization
  }
}