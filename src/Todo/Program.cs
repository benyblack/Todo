using System.IO.Abstractions;
using Todo.Lib.TodoText;
// See https://aka.ms/new-console-template for more information
var taskItemManager = new TaskItemManager(new FileSystem());
Console.WriteLine("Your Todo items:");
var taskItems = taskItemManager.GetAll();
for (int i = 0; i < taskItems.Count; i++)
{
	Console.WriteLine($"{i + 1}. {taskItems[i]}");
}
Console.WriteLine("==========================================================");
string? input = "";
while (input != "quit" && input != "q")
{
	Console.WriteLine("What would you like to do? [(a)dd, (l)ist, (r)emove, (e)dit, (q)uit]");
	input = Console.ReadLine()?.ToLower();
	switch (input)
	{
		case "a":
		case "add":
			AddTask();
			break;
		case "l":
		case "list":
			PrintAllTasks();
			break;
		case "r":
		case "remove":
			RemoveTask();
			break;
		case "e":
		case "edit":
			EditTask();
			break;
		case "q":
		case "quit":
			break;
		default:
			Console.WriteLine("Invalid input");
			break;
	}
	taskItems = taskItemManager.GetAll();
}

void AddTask()
{
	Console.WriteLine("Enter a description:");
	var description = Console.ReadLine();
	if (description is { Length: > 0 })
		taskItemManager.AddTask(description);
}

void RemoveTask()
{
	Console.WriteLine($"Enter a task number to remove (1 to {taskItems.Count}):");
	var taskNumber = Console.ReadLine();
	if (taskNumber is { Length: > 0 })
		taskItemManager.RemoveTask(int.Parse(taskNumber));
}

void EditTask()
{
	Console.WriteLine($"Enter a task number to edit (1 to {taskItems.Count}):");
	var taskNumberToEdit = Console.ReadLine();
	Console.WriteLine($"Enter a new description:");
	var newDescription = Console.ReadLine();
	if (taskNumberToEdit is { Length: > 0 } && newDescription is { Length: > 0 })
		taskItemManager.EditTask(int.Parse(taskNumberToEdit), newDescription);
}

void PrintAllTasks()
{
	var taskItems = taskItemManager.GetAll();
	for (int i = 0; i < taskItems.Count; i++)
	{
		Console.WriteLine($"{i + 1}. {taskItems[i]}");
	}
}
