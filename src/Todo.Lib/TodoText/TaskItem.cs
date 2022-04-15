using System.Text.RegularExpressions;

namespace Todo.Lib.TodoText;

public class TaskItem
{
	public bool IsCompleted { get; set; } = false;
	public string? Priority { get; set; }
	public DateTime? CompletionDate { get; set; }
	public DateTime? CreateDate { get; set; }
	public string? Description { get; set; }
	public string[]? Projects { get; set; }
	public string[]? Contexts { get; set; }
	public Dictionary<string, string>? Metadata { get; set; }

	public void Import(string line)
	{
		var priorityRegex = new Regex(@"\(([A-Z])\)");
		var completedRegex = new Regex(@"^x");
		var creationDateRegex = new Regex(@"\d{4}-\d{2}-\d{2}");
		var completionDateRegex = new Regex(@"\d{4}-\d{2}-\d{2}");
		var descriptionRegex = new Regex(@"^x\s.*$");
		var projectRegex = new Regex(@"\+([^\s]+)");
		var contextRegex = new Regex(@"@([^\s]+)");
		var metadataRegex = new Regex(@"(\s+|^)(\S+):(\S+)");

		var completed = completedRegex.Match(line).Value;
		if (completed == "x")
		{
			IsCompleted = true;
		}
		this.Priority = priorityRegex.Match(line).Groups[1]?.Value;

		var creationDate = creationDateRegex.Match(line).Value;
		var completionDate = completionDateRegex.Match(line).Value;
		this.CompletionDate = !string.IsNullOrEmpty(completionDate) ? DateTime.Parse(completionDate) : null;
		this.CreateDate = !string.IsNullOrEmpty(creationDate) ? DateTime.Parse(creationDate) : null;

		this.Projects = projectRegex.Matches(line).Select(x => x.Groups[1].Value).ToArray();
		this.Contexts = contextRegex.Matches(line).Select(x => x.Groups[1].Value).ToArray();

		var metaKeyVals = metadataRegex.Matches(line).Select(x => x.Value).ToArray();
		this.Metadata = metaKeyVals.Select(x => x.Split(':')).ToDictionary(x => x[0].Trim(), x => x[1].Trim());

		var description = completedRegex.Replace(line, "", 1);
		description = priorityRegex.Replace(description, "", 1);
		description = creationDateRegex.Replace(description, "", 1);
		description = completionDateRegex.Replace(description, "", 1);
		this.Description = description.Trim();
	}

	public override string ToString()
	{
		throw new NotImplementedException();
	}
}
