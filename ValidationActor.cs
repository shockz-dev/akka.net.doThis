using Akka.Actor;

namespace WinTail
{
  /// <summary>
  /// Actor that validates user input and signals result to other.
  /// </summary>
  public class ValidationActor : UntypedActor
  {
    private readonly IActorRef _consoleWriterActor;

    public ValidationActor(IActorRef consoleWriterActor)
    {
      _consoleWriterActor = consoleWriterActor;
    }

    protected override void OnReceive(object message)
    {
      var msg = message as string;
      if (string.IsNullOrEmpty(msg))
      {
        _consoleWriterActor.Tell(new Messages.NullInputError("No input received."));
      }
      else
      {
        var valid = IsValid(msg);
        if (valid)
        {
          _consoleWriterActor.Tell(new Messages.InputSuccess("Thank you! Message was valid."));
        }
        else
        {
          _consoleWriterActor.Tell(new Messages.ValidationError("Invalid: input had odd number of characters."));
        }
      }

      Sender.Tell(new Messages.ContinueProcessing());
    }

    /// <summary>
    /// Determines if the message received is valid.
    /// Checks if number of chars in message received is even.
    /// </summary>
    /// <param name="msg"></param>
    /// <returns></returns>
    private static bool IsValid(string msg)
    {
      var valid = msg.Length % 2 == 0;
      return valid;
    }
  }
}