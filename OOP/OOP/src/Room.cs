using System.Collections.Generic;

class Room
{
	private string description;
	private Inventory chest;
	private Dictionary<string, Room> exits;


    public Inventory Chest
    {
        get { return chest; }
    }

    public Room(string desc)
    {
        description = desc;
        chest = new Inventory(99999);
        exits = new Dictionary<string, Room>();
    }

	public void AddExit(string direction, Room neighbor)
	{
		exits.Add(direction, neighbor);
	}

	public string GetShortDescription()
	{
		return description;
	}

    public string GetLongDescription(Player player)
    {
        string str = "You are ";
        str += description;
        str += ".\n";
        if (player.IsAlive()) 
        {
            str += GetExitString();
        }
        return str;
    }

	public Room GetExit(string direction)
	{
		if (exits.ContainsKey(direction))
		{
			return exits[direction];
		}
		return null;
	}

	private string GetExitString()
	{
		string str = "Exits:";

		int countCommas = 0;
		foreach (string key in exits.Keys)
		{
			if (countCommas != 0)
			{
				str += ",";
			}
			str += " " + key;
			countCommas++;
		}

		return str;
	}
}
