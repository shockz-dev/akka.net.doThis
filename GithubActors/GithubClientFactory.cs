using Octokit;
using Octokit.Internal;

namespace GithubActors
{
  public static class GithubClientFactory
  {
    public static string OAuthToken { get; set; }

    public static GitHubClient GetUnauthenticatedClient()
    {
      return new GitHubClient(new ProductHeaderValue("AkkaBootcamp-Unit3"));
    }

    public static GitHubClient GetClient()
    {
      return new GitHubClient(new ProductHeaderValue("AkkaBootcamp-Unit3"), new InMemoryCredentialStore(new Credentials(OAuthToken)));
    }
  }
}