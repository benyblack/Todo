using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using Moq;
using Todo.Lib.TodoText;
using Xunit;

namespace Todo.Tests.Todo.Lib.TodoText;

public class TaskItemManagerTests
{
	[Fact]
	public void GetAll_GivenFileWithItems_ShouldReturnAllItems()
	{
		// Arrange
		string fileText = $"1 @home @work +project1 +project2{Environment.NewLine}2 +test2 @home2 @work2 +project3 +project4";
		var fileSystem = new Mock<IFileSystem>();
		fileSystem.Setup(x => x.File.Exists(It.IsAny<string>())).Returns(true);
		fileSystem.Setup(x => x.File.ReadAllText(It.IsAny<string>())).Returns(fileText);
		fileSystem.Setup(x => x.File.ReadAllLines(It.IsAny<string>())).Returns(fileText.Split(Environment.NewLine));
		var taskItemManager = new TaskItemManager(fileSystem.Object);

		// Act
		var taskItems = taskItemManager.GetAll();

		// Assert
		Assert.Equal(2, taskItems.Count);

		Assert.Equal("home", taskItems[0].Contexts[0]);
		Assert.Equal("work", taskItems[0].Contexts[1]);
		Assert.Equal("project1", taskItems[0].Projects[0]);
		Assert.Equal("project2", taskItems[0].Projects[1]);

		Assert.Equal("home2", taskItems[1].Contexts[0]);
		Assert.Equal("work2", taskItems[1].Contexts[1]);
		Assert.Equal("test2", taskItems[1].Projects[0]);
		Assert.Equal("project3", taskItems[1].Projects[1]);
	}

	[Fact]
	public void GetAll_GivenFileWDoesNotExists_ShouldReturnEmptyList()
	{
		// Arrange
		var fileSystem = new Mock<IFileSystem>();
		fileSystem.Setup(x => x.File.Exists(It.IsAny<string>())).Returns(false);
		fileSystem.Setup(x => x.File.ReadAllLines(It.IsAny<string>())).Throws(new FileNotFoundException());
		var taskItemManager = new TaskItemManager(fileSystem.Object);

		// Act
		var taskItems = taskItemManager.GetAll();

		// Assert
		Assert.Empty(taskItems);
	}
	[Fact]
	public void AddTask_GivenTaskItem_ShouldAddTaskItemToFile()
	{
		// Arrange
		string fileText = $"1 @home @work +project1 +project2{Environment.NewLine}2 +test2 @home2 @work2 +project3 +project4";
		var fileSystem = new Mock<IFileSystem>();
		fileSystem.Setup(x => x.File.Exists(It.IsAny<string>())).Returns(true);
		fileSystem.Setup(x => x.File.ReadAllText(It.IsAny<string>())).Returns(fileText);

		string appendedLine = "";
		fileSystem.Setup(x => x.File.AppendAllText(It.IsAny<string>(), It.IsAny<string>())).Callback((string path, string text) =>
		{
			appendedLine = text;
		});
		var taskItemManager = new TaskItemManager(fileSystem.Object);

		// Act
		var taskItemString = "3 @home2 @work2 +project3 +project4";
		taskItemManager.AddTask(taskItemString);
		string expected = $"{Environment.NewLine}{taskItemString}{Environment.NewLine}";

		// Assert
		fileSystem.Verify(x => x.File.AppendAllText(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
		Assert.Equal(expected, appendedLine);
	}

	[Fact]
	public void AddTask_GivenFileIsEmpty_ShouldAddTaskNotAddNewlineAtBeginning()
	{
		// Arrange
		string fileText = $"";
		var fileSystem = new Mock<IFileSystem>();
		fileSystem.Setup(x => x.File.Exists(It.IsAny<string>())).Returns(true);
		fileSystem.Setup(x => x.File.ReadAllText(It.IsAny<string>())).Returns(fileText);

		string appendedLine = "";
		fileSystem.Setup(x => x.File.AppendAllText(It.IsAny<string>(), It.IsAny<string>())).Callback((string path, string text) =>
		{
			appendedLine = text;
		});
		var taskItemManager = new TaskItemManager(fileSystem.Object);

		// Act
		var taskItemString = "3 @home2 @work2 +project3 +project4";
		taskItemManager.AddTask(taskItemString);
		string expected = $"{taskItemString}{Environment.NewLine}";

		// Assert
		fileSystem.Verify(x => x.File.AppendAllText(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
		Assert.Equal(expected, appendedLine);
	}

	[Fact]
	public void RemoveTask_GivenRowNumber_RemoveTheLineFromFile()
	{
		// Arrange
		string fileText = $"1 @home @work +project1 +project2{Environment.NewLine}2 +test2 @home2 @work2 +project3 +project4";
		var fileSystem = new Mock<IFileSystem>();
		fileSystem.Setup(x => x.File.Exists(It.IsAny<string>())).Returns(true);
		fileSystem.Setup(x => x.File.ReadAllLines(It.IsAny<string>())).Returns(fileText.Split(Environment.NewLine));

		string editedFiletext = "";
		fileSystem.Setup(x => x.File.WriteAllLines(It.IsAny<string>(), It.IsAny<List<string>>()))
						.Callback((string path, IEnumerable<string> lines) =>
						{
							editedFiletext = string.Join(Environment.NewLine, lines);
						}).Verifiable();


		var taskItemManager = new TaskItemManager(fileSystem.Object);

		// Act
		taskItemManager.RemoveTask(2);
		string expected = $"1 @home @work +project1 +project2";

		// Assert
		fileSystem.Verify(x => x.File.WriteAllLines(It.IsAny<string>(), It.IsAny<List<string>>()), Times.Once);
		Assert.Equal(expected, editedFiletext);
	}

	[Fact]
	public void EditTask_GivenRowNumberAndText_OverwriteTheLineFromFile()
	{
		// Arrange
		string fileText = $"1 @home @work +project1 +project2{Environment.NewLine}2 +test2 @home2 @work2 +project3 +project4";
		var fileSystem = new Mock<IFileSystem>();
		fileSystem.Setup(x => x.File.Exists(It.IsAny<string>())).Returns(true);
		fileSystem.Setup(x => x.File.ReadAllLines(It.IsAny<string>())).Returns(fileText.Split(Environment.NewLine));

		string editedFiletext = "";
		fileSystem.Setup(x => x.File.WriteAllLines(It.IsAny<string>(), It.IsAny<List<string>>()))
						.Callback((string path, IEnumerable<string> lines) =>
						{
							editedFiletext = string.Join(Environment.NewLine, lines);
						}).Verifiable();


		var taskItemManager = new TaskItemManager(fileSystem.Object);

		// Act
		taskItemManager.EditTask(2, "3 @home2 @work2 +project3 +project4");
		string expected = $"1 @home @work +project1 +project2{Environment.NewLine}3 @home2 @work2 +project3 +project4";

		// Assert
		fileSystem.Verify(x => x.File.WriteAllLines(It.IsAny<string>(), It.IsAny<List<string>>()), Times.Once);
		Assert.Equal(expected, editedFiletext);
	}
}
