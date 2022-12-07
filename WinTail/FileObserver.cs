using System;
using System.IO;

using Akka.Actor;

namespace WinTail
{
  /// <summary>
  /// Turns <see cref="FileSystemWatcher"/> events about a specific file into message for <see cref="TailActor"/>
  /// </summary>
  public class FileObserver : IDisposable
  {
    private readonly IActorRef _tailActor;
    private readonly string _absoluteFilePath;
    private FileSystemWatcher _watcher;
    private readonly string _fileDir;
    private readonly string _fileNameOnly;

    public FileObserver(IActorRef tailActor, string absoluteFilePath)
    {
      _tailActor = tailActor;
      _absoluteFilePath = absoluteFilePath;
      _fileDir = Path.GetDirectoryName(_absoluteFilePath);
      _fileNameOnly = Path.GetFileName(_absoluteFilePath);
    }

    /// <summary>
    /// Begin monitoring file.
    /// </summary>
    public void Start()
    {
      _watcher = new FileSystemWatcher(_fileDir, _fileNameOnly);
      _watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;

      _watcher.Changed += OnFileChanged;
      _watcher.Error += OnFileError;

      _watcher.EnableRaisingEvents = true;
    }

    /// <summary>
    /// Callback for <see cref="FileSystemWatcher"/> file change events.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
      if (e.ChangeType == WatcherChangeTypes.Changed)
      {
        _tailActor.Tell(new TailActor.FileWrite(e.Name), ActorRefs.NoSender);
      }
    }

    /// <summary>
    /// Callback for <see cref="FileSystemWatcher"/> file error events.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnFileError(object sender, ErrorEventArgs e)
    {
      _tailActor.Tell(new TailActor.FileError(_fileNameOnly, e.GetException().Message), ActorRefs.NoSender);
    }

    public void Dispose()
    {
      _watcher.Dispose();
    }
  }
}