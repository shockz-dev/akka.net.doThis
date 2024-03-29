﻿using Akka.Actor;

using GithubActors.Actors;

namespace GithubActors
{
  public partial class LauncherForm : Form
  {
    private IActorRef _mainFormActor;

    public LauncherForm()
    {
      InitializeComponent();
    }

    private void LauncherForm_Load(object sender, EventArgs e)
    {
      _mainFormActor = Program.GithubActors.ActorOf(Props.Create(() => new MainFormActor(lblIsValid)), ActorPaths.MainFormActor.Name);
      Program.GithubActors.ActorOf(Props.Create(() => new GithubValidatorActor(GithubClientFactory.GetClient())), ActorPaths.GithubValidatorActor.Name);
      Program.GithubActors.ActorOf(Props.Create(() => new GithubCommanderActor()), ActorPaths.GithubCommanderActor.Name);
    }

    private void LauncherForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      Application.Exit();
    }

    private void btnLaunch_Click(object sender, EventArgs e)
    {
      _mainFormActor.Tell(new ProcessRepo(tbRepoUrl.Text));
    }
  }
}