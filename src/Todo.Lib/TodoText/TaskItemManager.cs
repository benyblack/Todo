using System.IO.Abstractions;

namespace Todo.Lib.TodoText;
public class TaskItemManager
{

	private readonly IFileSystem _fileSystem;
	private readonly string _filePath;
	public TaskItemManager(IFileSystem fileSystem, string filePath = "todo.txt")
	{
		_fileSystem = fileSystem;
		_filePath = filePath;
	}

	public List<TaskItem> GetAll()
	{
		var lines = _fileSystem.File.ReadAllLines(_filePath);
		var taskItems = new List<TaskItem>();
		foreach (var line in lines)
		{
			var taskItem = new TaskItem();
			taskItem.Import(line);
			taskItems.Add(taskItem);
		}
		return taskItems;
	}

	public void AddTask(string taskItemString)
	{
		TaskItem task = new(taskItemString);
		if (!_fileSystem.File.Exists(_filePath))
		{
			_fileSystem.File.Create(_filePath).Close();
		}
		var text = _fileSystem.File.ReadAllText(_filePath);
		string line = task.ToString() + Environment.NewLine;
		if (!text.EndsWith(Environment.NewLine) && !string.IsNullOrEmpty(text))
		{
			line = Environment.NewLine + line;
		}
		_fileSystem.File.AppendAllText(_filePath, line);
	}
}
