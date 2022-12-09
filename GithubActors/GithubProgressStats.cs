using Octokit;

namespace GithubActors
{
  public class SimilarRepo : IComparable<SimilarRepo>
  {
    public SimilarRepo(Repository repo)
    {
      Repo = repo;
    }

    public Repository Repo { get; private set; }
    public int SharedStarrers { get; set; }

    public int CompareTo(SimilarRepo? other)
    {
      return SharedStarrers.CompareTo(other.SharedStarrers);
    }
  }

  public class GithubProgressStats
  {
    public int ExpectedUsers { get; private set; }
    public int UsersThusFar { get; private set; }
    public int QueryFailures { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime? EndTime { get; private set; }

    public TimeSpan Elapsed
    {
      get
      {
        return ((EndTime.HasValue ? EndTime.Value : DateTime.UtcNow) - StartTime);
      }
    }

    public bool IsFinished
    {
      get { return ExpectedUsers == UsersThusFar + QueryFailures; }
    }

    public GithubProgressStats()
    {
      StartTime = DateTime.UtcNow;
    }

    private GithubProgressStats(DateTime startTime, int expectedUsers, int usersThusFar, int queryFailures, DateTime? endTime)
    {
      EndTime = endTime;
      QueryFailures = queryFailures;
      UsersThusFar = usersThusFar;
      ExpectedUsers = expectedUsers;
      StartTime = startTime;
    }

    public GithubProgressStats UserQueriesFinished(int delta = 1)
    {
      return Copy(usersThusFar: UsersThusFar + delta);
    }

    public GithubProgressStats SetExpectedUserCount(int totalExpectedUsers)
    {
      return Copy(expectedUsers: totalExpectedUsers);
    }

    public GithubProgressStats IncrementFailures(int delta = 1)
    {
      return Copy(queryFailures: QueryFailures + delta);
    }

    public GithubProgressStats Finish()
    {
      return Copy(endTime: DateTime.UtcNow);
    }

    public GithubProgressStats Copy(int? expectedUsers = null, int? usersThusFar = null, int? queryFailures = null, DateTime? startTime = null, DateTime? endTime = null)
    {
      return new GithubProgressStats(startTime ?? StartTime, expectedUsers ?? ExpectedUsers, usersThusFar ?? UsersThusFar, queryFailures ?? QueryFailures, endTime ?? EndTime);
    }
  }
}