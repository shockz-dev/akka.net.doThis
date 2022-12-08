using System.Windows.Forms.DataVisualization.Charting;

using Akka.Actor;

namespace ChartApp.Actors
{
  public class ChartingActor : ReceiveActor
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

    /// <summary>
    /// Add a new <see cref="Series"/> to the chart
    /// </summary>
    public class AddSeries
    {
      public AddSeries(Series series)
      {
        Series = series;
      }

      public Series Series { get; private set; }
    }

    public class RemoveSeries
    {
      public RemoveSeries(string seriesName)
      {
        SeriesName = seriesName;
      }

      public string SeriesName { get; private set; }
    }

    public class TogglePause
    {
    }

    #endregion messages

    /// <summary>
    /// Maximum number of points we will allow in a series
    /// </summary>
    public const int MaxPoints = 250;

    /// <summary>
    /// Incrementing counter we use to plot along the X-axis
    /// </summary>
    private int xPosCounter = 0;

    private readonly Chart _chart;
    private Dictionary<string, Series> _seriesIndex;

    private readonly Button _pauseButton;

    public ChartingActor(Chart chart, Button pausedButton) : this(chart, new Dictionary<string, Series>(), pausedButton)
    {
    }

    public ChartingActor(Chart chart, Dictionary<string, Series> seriesIndex, Button pausedButton)
    {
      _chart = chart;
      _seriesIndex = seriesIndex;
      _pauseButton = pausedButton;

      Charting();
    }

    private void Charting()
    {
      Receive<InitializeChart>(ic => HandleInitialize(ic));
      Receive<AddSeries>(addSeries => HandleAddSeries(addSeries));
      Receive<RemoveSeries>(removeSeries => HandleRemoveSeries(removeSeries));
      Receive<Metric>(metric => HandleMetrics(metric));

      Receive<TogglePause>(pause =>
      {
        SetPauseButtonText(true);
        BecomeStacked(Paused);
      });
    }

    private void Paused()
    {
      Receive<Metric>(metric => HandleMetricsPaused(metric));
      Receive<TogglePause>(pause =>
      {
        SetPauseButtonText(false);
        UnbecomeStacked();
      });
    }

    #region individual message type handlers

    private void HandleInitialize(InitializeChart ic)
    {
      if (ic.InitialSeries != null)
      {
        _seriesIndex = ic.InitialSeries;
      }
      _chart.Series.Clear();

      var area = _chart.ChartAreas[0];
      area.AxisX.IntervalType = DateTimeIntervalType.Number;
      area.AxisY.IntervalType = DateTimeIntervalType.Number;

      SetChartBoundaries();

      if (_seriesIndex.Any())
      {
        foreach (var series in _seriesIndex)
        {
          series.Value.Name = series.Key;
          _chart.Series.Add(series.Value);
        }
      }

      SetChartBoundaries();
    }

    private void HandleAddSeries(AddSeries series)
    {
      if (!string.IsNullOrEmpty(series.Series.Name) && !_seriesIndex.ContainsKey(series.Series.Name))
      {
        _seriesIndex.Add(series.Series.Name, series.Series);
        _chart.Series.Add(series.Series);
        SetChartBoundaries();
      }
    }

    private void HandleRemoveSeries(RemoveSeries series)
    {
      if (!string.IsNullOrEmpty(series.SeriesName) && !_seriesIndex.ContainsKey(series.SeriesName))
      {
        var seriesToRemove = _seriesIndex[series.SeriesName];
        _seriesIndex.Remove(series.SeriesName);
        _chart.Series.Remove(seriesToRemove);
        SetChartBoundaries();
      }
    }

    private void HandleMetrics(Metric metric)
    {
      if (!string.IsNullOrEmpty(metric.Series) && _seriesIndex.ContainsKey(metric.Series))
      {
        var series = _seriesIndex[metric.Series];

        if (series.Points == null) return;

        series.Points.AddXY(xPosCounter++, metric.CounterValue);

        while (series.Points.Count > MaxPoints)
        {
          series.Points.RemoveAt(0);
          SetChartBoundaries();
        }
      }
    }

    private void HandleMetricsPaused(Metric metric)
    {
      if(!string.IsNullOrEmpty(metric.Series) && _seriesIndex.ContainsKey(metric.Series))
      {
        var series = _seriesIndex[metric.Series];
        // set the Y value to zero when we're paused
        series.Points.AddXY(xPosCounter++, 0.0d);
        while (series.Points.Count > MaxPoints)
        {
          series.Points.RemoveAt(0);
          SetChartBoundaries();
        }
      }
    }

    #endregion individual message type handlers

    private void SetChartBoundaries()
    {
      double maxAxisX, maxAxisY, minAxisX, minAxisY = 0.0d;
      var allPoints = _seriesIndex.Values.SelectMany(series => series.Points).ToList();
      var yValues = allPoints.SelectMany(point => point.YValues).ToList();
      maxAxisX = xPosCounter;
      minAxisX = xPosCounter - MaxPoints;
      maxAxisY = yValues.Count > 0 ? Math.Ceiling(yValues.Max()) : 1.0d;
      minAxisY = yValues.Count > 0 ? Math.Floor(yValues.Min()) : 0.0d;

      if (minAxisY == maxAxisY)
      {
        maxAxisY++;
      }

      var area = _chart.ChartAreas[0];
      area.AxisY.Minimum = minAxisY;
      area.AxisY.Maximum = Math.Max(1.0d, maxAxisY);

      if (allPoints.Count > 2)
      {
        area.AxisX.Minimum = minAxisX;
        area.AxisX.Maximum = maxAxisX;
      }
    }

    private void SetPauseButtonText(bool paused)
    {
      _pauseButton.Text = string.Format("{0}", !paused ? "PAUSE ||" : "RESUME ->");
    }
  }
}