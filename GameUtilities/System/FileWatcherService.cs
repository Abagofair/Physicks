namespace GameUtilities.System;

public class FileWatcherService
{
    private readonly string _folder;
    private readonly FileSystemWatcher _watcher;
    private Dictionary<string, Watch> _actionsByFileName;

    public FileWatcherService(string folderToWatch)
    {
        _folder = folderToWatch;
        _actionsByFileName = new Dictionary<string, Watch>();

        _watcher = new FileSystemWatcher(folderToWatch)
        {
            EnableRaisingEvents = true
        };

        _watcher.Changed += OnChanged;
    }

    public void WatchFile(string fileName, Action<string> onChange)
    {
        if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException(nameof(fileName));
        if (onChange == null) throw new ArgumentNullException(nameof(onChange));

        var watch = new Watch()
        {
            action = onChange
        };

        _actionsByFileName.Add(fileName, watch);
    }

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        if (e.ChangeType != WatcherChangeTypes.Changed)
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(e.Name) && e.Name.Contains('.'))
        {
            var result = _actionsByFileName[e.Name];
            if (!result.executed)
            {
                result.executed = true;
                _actionsByFileName[e.Name] = result;
                result.action(Path.Combine(_folder, e.Name));
            }
            else
            {
                result.executed = false;
                _actionsByFileName[e.Name] = result;
            }
        }
    }

    private struct Watch
    {
        public bool executed;
        public Action<string> action;
    }
}