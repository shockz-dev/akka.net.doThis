using Akka.Actor;

namespace GithubActors.Actors
{
  public class RepoResultsActor : ReceiveActor
  {
    private DataGridView _userDg;
    private ToolStripStatusLabel _statusLabel;
    private ToolStripProgressBar _progressBar;
    private bool _hasSetProgress = false;

    public RepoResultsActor(DataGridView userDg, ToolStripStatusLabel statusLabel, ToolStripProgressBar progressBar)
    {
      _userDg = userDg;
      _statusLabel = statusLabel;
      _progressBar = progressBar;
      InitialReceives();
    }

    private void InitialReceives()
    {
      Receive<GithubProgressStats>(stats =>
      {
        if (!_hasSetProgress && stats.ExpectedUsers > 0)
        {
          _progressBar.Minimum = 0;
          _progressBar.Step = 1;
          _progressBar.Maximum = stats.ExpectedUsers;
          _progressBar.Value = stats.UsersThusFar;
          _progressBar.Visible = true;
          _statusLabel.Visible = true;
        }

        _statusLabel.Text = string.Format("{0} out of {1} users ({2} failures) [{3} elapsed]", stats.UsersThusFar, stats.ExpectedUsers, stats.QueryFailures, stats.Elapsed);
        _progressBar.Value = stats.UsersThusFar + stats.QueryFailures;
      });

      Receive<IEnumerable<SimilarRepo>>(repos =>
      {
        foreach (var similarRepo in repos)
        {
          var repo = similarRepo.Repo;
          var row = new DataGridViewRow();
          row.CreateCells(_userDg);
          row.Cells[0].Value = repo.Owner.Login;
          row.Cells[1].Value = repo.Name;
          row.Cells[2].Value = repo.HtmlUrl;
          row.Cells[3].Value = similarRepo.SharedStarrers;
          row.Cells[4].Value = repo.OpenIssuesCount;
          row.Cells[5].Value = repo.StargazersCount;
          row.Cells[6].Value = repo.ForksCount;
          _userDg.Rows.Add(row);
        }
      });

      Receive<GithubCoordinatorActor.JobFailed>(failed =>
      {
        _progressBar.Visible = true;
        _progressBar.ForeColor = Color.Red;
        _progressBar.Maximum = 1;
        _progressBar.Value = 1;
        _statusLabel.Visible = true;
        _statusLabel.Text = string.Format("Failed to gather data for Github repository {0} / {1}", failed.Repo.Owner, failed.Repo.Repo);
      });
    }
  }
}