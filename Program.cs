using System;

using Akka.Actor;

namespace WinTail
{
  #region Program

  internal class Program
  {
    public static ActorSystem MyActorSystem;

    private static void Main(string[] args)
    {
      // initialize MyActorSystem
      // YOU NEED TO FILL IN HERE
      MyActorSystem = ActorSystem.Create("MyActorSystem");

      // time to make your first actors!
      //YOU NEED TO FILL IN HERE
      // make consoleWriterActor using these props: Props.Create(() => new ConsoleWriterActor())
      var consoleWriterActor = MyActorSystem.ActorOf(Props.Create(() => new ConsoleWriterActor()));
      // make consoleReaderActor using these props: Props.Create(() => new ConsoleReaderActor(consoleWriterActor))
      var consoleReaderActor = MyActorSystem.ActorOf(Props.Create(() => new ConsoleReaderActor(consoleWriterActor)));

      // tell console reader to begin
      //YOU NEED TO FILL IN HERE
      consoleReaderActor.Tell(ConsoleReaderActor.StartCommand);

      // blocks the main thread from exiting until the actor system is shut down
      MyActorSystem.WhenTerminated.Wait();
    }
  }

  #endregion Program
}