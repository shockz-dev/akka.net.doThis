using System;

using Akka.Actor;

namespace WinTail
{
  /// <summary>
  /// Actor responsible for reading FROM the console.
  /// Also responsible for calling <see cref="ActorSystem.Terminate"/>.
  /// </summary>
  public class ConsoleReaderActor : UntypedActor
  {
    public const string StartCommand = "start";
    public const string ExitCommand = "exit";

    protected override void OnReceive(object message)
    {
      if (message.Equals(StartCommand))
      {
        DoPrintInstructions();
      }

      GetAndValidateInput();
    }

    #region internal methods

    /// <summary>
    /// Reads input from console, validation it, then signals appropriate response
    /// (continue processing, error, success, etc.).
    /// </summary>
    private void GetAndValidateInput()
    {
      var message = Console.ReadLine();
      if (!string.IsNullOrEmpty(message) && string.Equals(message, ExitCommand, StringComparison.OrdinalIgnoreCase))
      {
        Context.System.Terminate();
        return;
      }

      //_validationActor.Tell(message);
      Context.ActorSelection("akka://MyActorSystem/user/validationActor").Tell(message);
    }

    private void DoPrintInstructions()
    {
      //Console.WriteLine("Write whatever you want into the console!");
      //Console.WriteLine("Some entries will pass validation, and some won't...\n\n");
      //Console.WriteLine("Type 'exit' to quit this application at any time.\n");

      Console.WriteLine("Please provide the URI of a log file on disk.\n");
    }

    /// <summary>
    /// Validates <see cref="message"/>
    /// Currently says messages are valid if contain even number of characters.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private static bool IsValid(string message)
    {
      var valid = message.Length % 2 == 0;
      return valid;
    }

    #endregion internal methods
  }
}