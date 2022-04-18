using System.IO.Abstractions;
using Spectre.Console;
using Todo.Lib.TodoText;
// See https://aka.ms/new-console-template for more information
var taskItemManager = new TaskItemManager(new FileSystem());
var taskItems = taskItemManager.GetAll();
PrintAllTasks();
char? input = ' ';
while (input != 'q')
{
	//Console.WriteLine("What would you like to do? [(a)dd, (l)ist, (r)emove, (e)dit, (q)uit]");
	AnsiConsole.MarkupLine("==========================================================");
	AnsiConsole.MarkupLine("What would you like to do?");
	AnsiConsole.MarkupLine("([bold]a[/])dd, ([bold]r[/])emove, ([bold]e[/])dit, ([bold]q[/])uit, ([bold]m[/])ark as done/undone?");
	input = char.ToLower(Console.ReadKey().KeyChar);
	Console.WriteLine();
	switch (input)
	{
		case 'a':
			AddTask();
			break;
		case 'r':
			RemoveTask();
			break;
		case 'e':
			EditTask();
			break;
		case 'm':
			MarkTask();
			break;
		case 'q':
			break;
		default:
			Console.WriteLine("Invalid input");
			break;
	}
	taskItems = taskItemManager.GetAll();
	PrintAllTasks();
}

void MarkTask()
{
	var taskNumber = GetSelectedTaskNumber();
	taskItemManager.Mark(taskNumber);
}

void AddTask()
{
	AnsiConsole.MarkupLine("Enter a [bold]description[/]:");
	var description = Console.ReadLine();
	if (description is { Length: > 0 })
		taskItemManager.AddTask(description);
	else
		AnsiConsole.MarkupLine("[red]Invalid input[/]");
}

void RemoveTask()
{
	var taskNumber = GetSelectedTaskNumber();
	taskItemManager.RemoveTask(taskNumber);
}

void EditTask()
{
	var taskNumberToEdit = GetSelectedTaskNumber();
	Console.WriteLine($"Enter a new description:");
	var newDescription = Console.ReadLine();
	if (newDescription is { Length: > 0 })
		taskItemManager.EditTask(taskNumberToEdit, newDescription);
	else
		Console.WriteLine("Invalid input");
}

void PrintAllTasks()
{
	Console.Clear();
	AnsiConsole.MarkupLine("===============================");
	AnsiConsole.MarkupLine("Task List");
	AnsiConsole.MarkupLine("===============================");
	for (int i = 0; i < taskItems.Count; i++)
	{
		string tik = taskItems[i].IsCompleted ? "[bold][green]✓[/][/]" : " ";
		AnsiConsole.MarkupLine($"[[{tik}]] {i + 1}. {taskItems[i].Description}");
	}
}

int GetSelectedTaskNumber()
{
	return AnsiConsole.Prompt(
			new TextPrompt<int>($"Enter a task number to edit (1 to {taskItems.Count}):")
			.PromptStyle("blue")
			.ValidationErrorMessage($"[red]The input needs to be a number and between 1 to {taskItems.Count}[/]")
			.Validate(i =>
			{
				var max = taskItems.Count;
				if (i < 1 || i > max)
					return ValidationResult.Error($"The input needs to be a number and between 1 to {max}");
				return ValidationResult.Success();
			}));
}
