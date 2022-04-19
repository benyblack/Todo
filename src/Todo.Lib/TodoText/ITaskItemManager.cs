namespace Todo.Lib.TodoText;

public interface ITaskItemManager
{
	void AddTask(string taskItemString);
	void EditTask(int lineNumber, string newDescrioption);
	List<TaskItem> GetAll();
	void Mark(int lineNumber);
	void RemoveTask(int lineNumber);
}
