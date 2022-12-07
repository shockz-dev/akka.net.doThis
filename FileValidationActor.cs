using System.IO;

using Akka.Actor;

namespace WinTail
{
  /// <summary>
  /// Actor that validates user input and signals result to others.
  /// </summary>
  public class FileValidationActor : UntypedActor
  {
    private readonly IActorRef _consoleWriterActor;

    public FileValidationActor(IActorRef consoleWriterActor)
    {
      _consoleWriterActor = consoleWriterActor;
    }

    protected override void OnReceive(object message)
    {
      var msg = message as string;
      if (string.IsNullOrEmpty(msg))
      {
        _consoleWriterActor.Tell(new Messages.NullInputError("Input was blan. Please try agin.\n"));
        Sender.Tell(new Messages.ContinueProcessing());
      }
      else
      {
        var valid = IsFileUri(msg);
        if (valid)
        {
          _consoleWriterActor.Tell(new Messages.InputSuccess(string.Format("Starting processing for {0}", msg)));
          //_tailCoordinatorActor.Tell(new TailCoordinatorActor.StartTail(msg, _consoleWriterActor));
          Context.ActorSelection("akka://MyActorSystem/user/tailCoordinatorActor").Tell(new TailCoordinatorActor.StartTail(msg, _consoleWriterActor));
        }
        else
        {
          _consoleWriterActor.Tell(new Messages.ValidationError(string.Format("{0} is not an existing URI on disk.", msg, msg)));
          Sender.Tell(new Messages.ContinueProcessing());
        }
      }
    }

    /// <summary>
    /// Checks if file exists at path provided by user.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private static bool IsFileUri(string path)
    {
      return File.Exists(path);
    }
  }
}