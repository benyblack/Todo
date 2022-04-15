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
}
