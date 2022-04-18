using System.Text.RegularExpressions;

namespace Todo.Lib.TodoText;

public class TaskItem
{
	public bool IsCompleted { get; private set; } = false;
	public string? Priority { get; private set; }
	public DateTime? CompletionDate { get; private set; }
	public DateTime? CreateDate { get; private set; }
	public string? Description { get; private set; }
	public string[]? Projects { get; private set; }
	public string[]? Contexts { get; private set; }
	public Dictionary<string, string>? Metadata { get; private set; }

	public TaskItem()
	{

	}

	public TaskItem(string line)
	{
		Import(line);
	}

	public void Import(string line)
	{
		var priorityRegex = new Regex(@"\(([A-Z])\)");
		var completedRegex = new Regex(@"^x");
		var creationDateRegex = new Regex(@"\d{4}-\d{2}-\d{2}");
		var completionDateRegex = new Regex(@"\d{4}-\d{2}-\d{2}");
		var descriptionRegex = new Regex(@"^x\s.*$");
		var projectRegex = new Regex(@"(\s+|^)\+([^\s]+)");
		var contextRegex = new Regex(@"(\s+|^)@([^\s]+)");
		var metadataRegex = new Regex(@"(\s+|^)(\S+):(\S+)");

		var completed = completedRegex.Match(line)?.Value;
		IsCompleted = (completed == "x");
		Priority = priorityRegex.Match(line).Groups[1]?.Value;
		var creationDate = creationDateRegex.Match(line).Value;
		var completionDate = completionDateRegex.Match(line).Value;
		CompletionDate = !string.IsNullOrEmpty(completionDate) ? DateTime.Parse(completionDate) : null;
		CreateDate = !string.IsNullOrEmpty(creationDate) ? DateTime.Parse(creationDate) : null;

		Projects = projectRegex.Matches(line).Select(x => x.Groups[2].Value).Distinct().ToArray();
		Contexts = contextRegex.Matches(line).Select(x => x.Groups[2].Value).Distinct().ToArray();

		var metaKeyVals = metadataRegex.Matches(line).Select(x => x.Value).ToArray();
		Metadata = metaKeyVals.Distinct()
							  .Select(x => x.Split(':'))
							  .ToDictionary(x => x[0].Trim(), x => x[1].Trim());

		var description = completedRegex.Replace(line, "", 1);
		description = priorityRegex.Replace(description, "", 1);
		description = creationDateRegex.Replace(description, "", 1);
		description = completionDateRegex.Replace(description, "", 1);
		Description = description.Trim();

	}

	public override string ToString()
	{
		List<string> items = new();
		if (IsCompleted)
			items.Add("x");
		if (!string.IsNullOrEmpty(this.Priority))
			items.Add($"({this.Priority})");
		if (this.CompletionDate != null)
			items.Add($"{this.CompletionDate.Value.ToString("yyyy-MM-dd")}");
		if (this.CreateDate != null)
			items.Add($"{this.CreateDate.Value.ToString("yyyy-MM-dd")}");
		if (!string.IsNullOrEmpty(this.Description))
			items.Add(this.Description);
		return string.Join(" ", items);
	}

	internal void Mark()
	{
		IsCompleted = !IsCompleted;
	}
}
