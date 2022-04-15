using System;
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
}
