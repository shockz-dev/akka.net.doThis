using System.Windows.Forms.DataVisualization.Charting;

using Akka.Actor;

namespace ChartApp.Actors
{
  public class ChartingActor : UntypedActor
  {
    #region messages

    public class InitializeChart
    {
      public InitializeChart(Dictionary<string, Series> initialSeries)
      {
        InitialSeries = initialSeries;
      }

      public Dictionary<string, Series> InitialSeries { get; private set; }
    }

    #endregion messages

    private readonly Chart _chart;
    private Dictionary<string, Series> _seriesIndex;

    public ChartingActor(Chart chart) : this(chart, new Dictionary<string, Series>())
    {
    }

    public ChartingActor(Chart chart, Dictionary<string, Series> seriesIndex)
    {
      _chart = chart;
      _seriesIndex = seriesIndex;
    }

    protected override void OnReceive(object message)
    {
      if (message is InitializeChart)
      {
        var ic = message as InitializeChart;
        HandleInitialize(ic);
      }
    }

    #region individual message type handlers

    private void HandleInitialize(InitializeChart ic)
    {
      if (ic.InitialSeries != null)
      {
        _seriesIndex = ic.InitialSeries;
      }

      _chart.Series.Clear();

      if (_seriesIndex.Any())
      {
        foreach (var series in _seriesIndex)
        {
          series.Value.Name = series.Key;
          _chart.Series.Add(series.Value);
        }
      }
    }

    #endregion individual message type handlers
  }
}