using Akka.Actor;

namespace ChartApp
{
  internal static class Program
  {
    public static ActorSystem ChartActors;

    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      // To customize application configuration such as set high DPI settings or default font,
      // see https://aka.ms/applicationconfiguration.
      ChartActors = ActorSystem.Create("ChartActors");

      ApplicationConfiguration.Initialize();
      Application.Run(new MainForm());
    }
  }
}