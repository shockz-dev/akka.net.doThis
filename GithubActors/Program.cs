using Akka.Actor;

namespace GithubActors
{
  internal static class Program
  {
    public static ActorSystem GithubActors;

    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
      GithubActors = ActorSystem.Create("GithubActors");

      // To customize application configuration such as set high DPI settings or default font,
      // see https://aka.ms/applicationconfiguration.
      ApplicationConfiguration.Initialize();
      Application.Run(new GithubAuth());
    }
  }
}