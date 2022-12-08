using Akka.Actor;

namespace ChartApp.Actors
{
  #region reporting

  /// <summary>
  /// Signal used to indicate that it's time to simple all counters
  /// </summary>
  public class GatherMetrics
  { }

  /// <summary>
  /// Metric data at the time of simple
  /// </summary>
  public class Metric
  {
    public Metric(string series, float counterValue)
    {
      Series = series;
      CounterValue = counterValue;
    }

    public string Series { get; private set; }
    public float CounterValue { get; private set; }
  }

  #endregion reporting

  #region performance counter management

  /// <summary>
  /// All types of counters supported by this example
  /// </summary>
  public enum CounterType
  {
    Cpu,
    Memory,
    Disk
  }

  /// <summary>
  /// Enables a counter and begins publishing values to <see cref="Subscriber"/>.
  /// </summary>
  public class SubscribeCounter
  {
    public SubscribeCounter(CounterType counter, IActorRef subscriber)
    {
      Counter = counter;
      Subscriber = subscriber;
    }

    public CounterType Counter { get; private set; }
    public IActorRef Subscriber { get; private set; }
  }

  /// <summary>
  /// Unsubscribes <see cref="Subscriber"/> from receiving updates
  /// for a given counter
  /// </summary>
  public class UnsubscriberCounter
  {
    public UnsubscriberCounter(CounterType counter, IActorRef subscriber)
    {
      Counter = counter;
      Subscriber = subscriber;
    }

    public CounterType Counter { get; private set; }
    public IActorRef Subscriber { get; private set; }
  }

  #endregion performance counter management
}