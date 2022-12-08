using Akka.Actor;

namespace ChartApp.Actors
{
  /// <summary>
  /// Actor responsible for managing button toggles
  /// </summary>
  public class ButtonToggleActor : UntypedActor
  {
    #region message types

    /// <summary>
    /// Toggles this button on or off and sends an appropriate messages
    /// to the <see cref="PerformanceCounterCoordinatorActor"/>
    /// </summary>
    public class Toggle
    { }

    #endregion message types

    private readonly CounterType _counterType;
    private bool _isToggledOn;
    private readonly Button _button;
    private readonly IActorRef _coordinatorActor;

    public ButtonToggleActor(IActorRef coordinatorActor, Button button, CounterType counterType, bool isToggledOn = false)
    {
      _coordinatorActor = coordinatorActor;
      _button = button;
      _isToggledOn = isToggledOn;
      _counterType = counterType;
    }

    protected override void OnReceive(object message)
    {
      if (message is Toggle && _isToggledOn)
      {
        // toggle is currently on

        // stop watching this counter
        _coordinatorActor.Tell(new PerformanceCounterCoordinatorActor.Unwatch(_counterType));

        FlipToggle();
      }
      else if (message is Toggle && !_isToggledOn)
      {
        // toggle is currently off

        // start watching this counter
        _coordinatorActor.Tell(new PerformanceCounterCoordinatorActor.Watch(_counterType));

        FlipToggle();
      }
      else
      {
        Unhandled(message);
      }
    }

    private void FlipToggle()
    {
      // flip the toggle
      _isToggledOn = !_isToggledOn;

      // change the text of the button
      _button.Text = string.Format("{0} ({1})", _counterType.ToString().ToUpperInvariant(), _isToggledOn ? "ON" : "OFF");
    }
  }
}