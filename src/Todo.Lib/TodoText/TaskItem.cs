namespace Todo.Lib.TodoText;

public class TaskItem
{
	public bool IsCompleted { get; set; }
	public string? Priority { get; set; }
	public DateTime? CompletionDate { get; set; }
	public DateTime? CreationDate { get; set; }
	public string? Description { get; set; }
	public string[]? Projects { get; set; }
	public string[]? Contexts { get; set; }
	public Dictionary<string, string>? Metadata { get; set; }

}
