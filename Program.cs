using System;

using Akka.Actor;

namespace WinTail
{
  #region Program

  public class Program
  {
    public static ActorSystem MyActorSystem;

    private static void Main(string[] args)
    {
      // initialize MyActorSystem
      MyActorSystem = ActorSystem.Create("MyActorSystem");

      // typeof 로 넘기면 compile time 에서 잡아내기 힘듬
      //Props fakeActorProps = Props.Create(typeof(FakeActor));
      //IActorRef fakeActor = MyActorSystem.ActorOf(fakeActorProps, "fakeActor");

      //Props consoleWriterProps = Props.Create(typeof(ConsoleWriterActor)); // dont' do
      Props consoleWriterProps = Props.Create<ConsoleWriterActor>();
      IActorRef consoleWriterActor = MyActorSystem.ActorOf(consoleWriterProps, "consoleWriterActor");

      Props validationActorProps = Props.Create(() => new ValidationActor(consoleWriterActor));
      IActorRef validationActor = MyActorSystem.ActorOf(validationActorProps, "validationActor");

      Props consoleReaderProps = Props.Create<ConsoleReaderActor>(validationActor);
      IActorRef consoleReaderActor = MyActorSystem.ActorOf(consoleReaderProps, "consoleReaderActor");

      // tell console reader to begin
      //YOU NEED TO FILL IN HERE
      consoleReaderActor.Tell(ConsoleReaderActor.StartCommand);

      // blocks the main thread from exiting until the actor system is shut down
      MyActorSystem.WhenTerminated.Wait();
    }
  }

  #endregion Program

  public class FakeActor { }
}