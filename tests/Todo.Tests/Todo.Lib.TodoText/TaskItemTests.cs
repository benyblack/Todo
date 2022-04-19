using System;
using System.Collections.Generic;
using Todo.Lib.TodoText;
using Xunit;
using Xunit.Categories;

namespace Todo.Tests.Todo.Lib.TodoText;
public class TaskItemTests
{

	public static IEnumerable<object[]> Data =>
		new List<object[]>
		{
			new object[] {"description of the +test @home task @home @work +project1 +project2"},
			new object[] {"description"},
			new object[] {"description of the @home task @home @work"},
			new object[] {"description of the +test @home task @home @work +project1 +project2 meta:data"},
			new object[] {"description of the +test @home task @home @work meta1:data +project1 +project2 meta:data"},
		};

	[Theory]
	[MemberData(nameof(Data))]
	public void Import_GivenLineWithDescription_ShouldSetFullDescriptionAndDescription(string description)
	{
		// Arrange
		var taskItem = new TaskItem();
		string completed = "x";
		string priority = "(A)";
		string dueDate = "2022-01-01";
		string createDate = "2022-01-01";
		//string taskFullDescription = "description of the +test @home task @home @work +project1 +project2";

		// Act
		taskItem.Import($"{completed} {priority} {dueDate} {createDate} {description}");

		// Assert
		Assert.Equal(description, taskItem.Description);
	}

	[Fact]
	public void Import_GivenLineWithDates_ShouldSetDates()
	{
		// Arrange
		var taskItem = new TaskItem();
		string completed = "x";
		string priority = "(A)";
		string dueDate = "2022-01-01";
		string createDate = "2022-01-01";
		string taskDescription = "description of the task +test @home @work +project1 +project2";

		// Act
		taskItem.Import($"{completed} {priority} {dueDate} {createDate} {taskDescription}");

		// Assert
		Assert.Equal(DateTime.Parse(dueDate), taskItem.CompletionDate);
		Assert.Equal(DateTime.Parse(createDate), taskItem.CreateDate);
	}

	[Fact]
	public void Import_GivenLineWithPriority_ShouldSetPriority()
	{
		// Arrange
		var taskItem = new TaskItem();

		// Act
		taskItem.Import("(A) 1 +test @home @work +project1 +project2");

		// Assert
		Assert.Equal("A", taskItem.Priority);
	}

	[Fact]
	public void Import_GivenLineMarkedCompleted_ShouldSetCompleted()
	{
		// Arrange
		var taskItem = new TaskItem();

		// Act
		taskItem.Import("x 1 +test @home @work +project1 +project2");

		// Assert
		Assert.True(taskItem.IsCompleted);
	}

	[Fact]
	public void Import_GivenLineWithProjects_ShouldSetProjects()
	{
		// Arrange
		var taskItem = new TaskItem();

		// Act
		taskItem.Import("x 1 +test @home @work +project1 +project2");

		// Assert
		Assert.Equal(new [] { "test", "project1", "project2" }, taskItem.Projects);
	}

	[Fact]
	[Trait("Label", "Duplicates")]
	public void Import_GivenLineWithDuplicatedProjects_ShouldSetProjectsAndSkipDuplicates()
	{
		// Arrange
		var taskItem = new TaskItem();

		// Act
		taskItem.Import("x 1 +test +test @home @work +project1 +project2 +project2");

		// Assert
		Assert.Equal(new [] { "test", "project1", "project2" }, taskItem.Projects);
	}

	[Fact]
	public void Import_GivenLineWithContexts_ShouldSetContexts()
	{
		// Arrange
		var taskItem = new TaskItem();

		// Act
		taskItem.Import("x 1 +test @home @work +project1 +project2");

		// Assert
		Assert.Equal(new [] { "home", "work" }, taskItem.Contexts);
	}

	[Fact]
	[Trait("Label", "Duplicates")]
	public void Import_GivenLineWithContextsDuplicated_ShouldSetContextsAndSkipDuplicates()
	{
		// Arrange
		var taskItem = new TaskItem();

		// Act
		taskItem.Import("x 1 +test @home @work @work +project1 @work +project2");

		// Assert
		Assert.Equal(new [] { "home", "work" }, taskItem.Contexts);
	}

	[Fact]
	[Category("Metadata")]
	public void Import_GivenLineWithMetadata_ShouldSetMetadata()
	{
		// Arrange
		var taskItem = new TaskItem();
		var expected = new Dictionary<string, string>() { { "tag1", "val1" }, { "tag2", "val2" } };


		// Act
		taskItem.Import("x 1 +test @home @work tag1:val1 +project1 +project2 tag2:val2");

		// Assert
		Assert.Equal(expected, taskItem.Metadata);
	}

	[Fact]
	[Trait("Label", "Duplicates")]
	[Category("Metadata")]
	public void Import_GivenLineWithDuplicatedMetadata_ShouldNotThrowException()
	{
		// Arrange
		var taskItem = new TaskItem();
		var expected = new Dictionary<string, string>() { { "tag1", "val1" }, { "tag2", "val2" } };


		// Act
		taskItem.Import("x 1 +test @home @work tag1:val1 +project1 +project2 tag2:val2 tag2:val2");

		// Assert
		Assert.Equal(expected, taskItem.Metadata);
	}

	[Fact]
	public void ToString_GivenLineFull_ShouldReturnTheSame()
	{
		// Arrange
		var taskItem = new TaskItem();
		string completed = "x";
		string priority = "(A)";
		string dueDate = "2022-01-01";
		string createDate = "2022-01-01";
		string taskDescription = "description of the task +test @home @work +project1 +project2";
		string raw = $"{completed} {priority} {dueDate} {createDate} {taskDescription}";

		// Act
		taskItem.Import(raw);
		var result = taskItem.ToString();

		// Assert
		Assert.Equal(raw, result);
	}
}
