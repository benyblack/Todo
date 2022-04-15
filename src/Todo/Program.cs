using System.IO.Abstractions;
using Todo.Lib.TodoText;
// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hi!");
var taskItemManager = new TaskItemManager(new FileSystem());
taskItemManager.AddTask("3 @home2 @work2 +project3 +project4");
taskItemManager.AddTask("x Salam @home2 @work2 +project3 +project4");
taskItemManager.AddTask("Hi There @home2 @work2 +project3 +project4");
var taskItems = taskItemManager.GetAll();
foreach (var item in taskItems)
{
	Console.WriteLine(item.ToString());
}
File.Delete("todo.txt");
