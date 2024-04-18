class Command
{
	public string CommandWord { get; init; }
	public string SecondWord { get; init; }
	
	public Command(string first, string second)
	{
		CommandWord = first;
		SecondWord = second;
	}

	public bool IsUnknown()
	{
		return CommandWord == null;
	}

	public bool HasSecondWord()
	{
		return SecondWord != null;
	}
}
