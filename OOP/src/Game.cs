using System;
using System.Reflection.Metadata;

class Game
{
	private Parser parser;
	private Player player;
	Item Diamond = new Item(15, "diamond");
	Item Apple = new Item(20, "apple");

	// Constructor
	public Game()
	{
		parser = new Parser();
		player = new Player();
		CreateRooms();
	}

	// Init rooms + items
	private void CreateRooms()
	{
		Room outside = new Room("You are outside at the entrance of a train station");
		Room theatre = new Room("In the theater room in the train station");
		Room pub = new Room("In the train station pub");
		Room office = new Room("In the computing train station admin office");
		Room hallway = new Room("In the train station hallway");
		Room up = new Room("In the second floor of the train station");
		Room storage = new Room("In the storage room");
		// Initialise room exits
		outside.AddExit("hallway", hallway);

		hallway.AddExit("theatre", theatre);
		hallway.AddExit("office", office);
		hallway.AddExit("pub", pub);
		hallway.AddExit("stairs_up", up);
		
		theatre.AddExit("hallway", hallway);
		office.AddExit("hallway", hallway);
		pub.AddExit("hallway", hallway);

		up.AddExit("stairs_down", hallway);
		up.AddExit("storageroom", storage);

		storage.AddExit("stairs_down", hallway);

		storage.Chest.Put("diamond", Diamond);

		theatre.Chest.Put("apple", Apple);
		office.Chest.Put("apple", Apple);
		pub.Chest.Put("apple", Apple);

		player.CurrentRoom = outside;
	}

	public void Play()
	{
		PrintWelcome();

		bool finished = false;
		while (!finished)
		{
			Command command = parser.GetCommand();
			finished = ProcessCommand(command);
		}
		Console.WriteLine("Thank you for playing!");
		Console.WriteLine("Press [Enter] to continue.");
		Console.ReadLine();
	}

	private void PrintWelcome()
	{
		// Console.WriteLine("");
		Console.WriteLine(player.CurrentRoom.GetLongDescription(player));
		Console.WriteLine("(Write 'help' if you need any help)");
	}

private bool ProcessCommand(Command command)
{
    bool wantToQuit = false;

    if (!player.IsAlive() && command.CommandWord != "quit")
    {
        Console.WriteLine("you bled to death...");
		Console.WriteLine("You can only use command:");
		Console.WriteLine("quit");
        return wantToQuit;
    }

    if(command.IsUnknown())
    {
        Console.WriteLine("Hmmm, what do you mean..?");
        return wantToQuit; 
    }

    switch (command.CommandWord)
    {
        case "help":
            PrintHelp();
            break;
        case "look":
            Look();
            break;
		case "take":
			Take(command);
			break;
		case "drop":
			Drop(command);
			break;
        case "status":
            Health();
            break;
        case "go":
            GoRoom(command);
            break;
		case "use":
			UseItem(command);
			break;
        case "quit":
            wantToQuit = true;
            break;
    }

    return wantToQuit;
}

	//  Commands
	private void PrintHelp()
	{
		Console.WriteLine("You are lost. You are alone.");
		Console.WriteLine("You wander around at the train station.");
		Console.WriteLine();
		parser.PrintValidCommands();
	}

	private void Look()
	{
		Console.WriteLine(player.CurrentRoom.GetLongDescription(player));

		Dictionary<string, Item> roomItems = player.CurrentRoom.Chest.GetItems();
		if (roomItems.Count > 0)
		{
			Console.WriteLine("Items in this room:");
			foreach (var itemEntry in roomItems)
			{
				Console.WriteLine($"{itemEntry.Value.Description} - ({itemEntry.Value.Weight} kg)");
			}
		}
	}


	private void Take(Command command)
	{
		if (!command.HasSecondWord())
		{
			Console.WriteLine("What do you want to take?");
			return;
		}

		string itemName = command.SecondWord.ToLower();

		bool success = player.TakeFromChest(itemName);

	}

	private void Drop(Command command)
	{
		if (!command.HasSecondWord())
		{
			Console.WriteLine("What do you want to drop?");
			return;
		}

		string itemName = command.SecondWord.ToLower();

		bool success = player.DropToChest(itemName);


	}

	private void Health()
	{
		Console.WriteLine($"You have: {player.GetHealth()} health");

		Dictionary<string, Item> items = player.GetItems();

		if (items.Count > 0)
		{
			Console.WriteLine("Your current items:");

			// Iterate over elk item in player zijn inv
			foreach (var itemEntry in items)
			{
				Console.WriteLine($"- {itemEntry.Key}: ({itemEntry.Value.Weight} kg)");
			}
		}
		else
		{
			Console.WriteLine("You don't have any items in your inventory");
		}
	}

	private void GoRoom(Command command)
	{
		if(!command.HasSecondWord())
		{
			Console.WriteLine("Where do you want to go?");
			// On no params
			return;
		}

		string direction = command.SecondWord;

		Room nextRoom = player.CurrentRoom.GetExit(direction);
		if (nextRoom == null)
		{
			Console.WriteLine("There is no door to "+direction+"!");
			return;
		}

		player.Damage(20);
		player.CurrentRoom = nextRoom;
		Console.WriteLine(player.CurrentRoom.GetLongDescription(player));
		
		if (!player.IsAlive()) 
		{
			Console.WriteLine("You are bleeding, you died...");
		}
	}

	private void UseItem(Command command)
	{
		if (!command.HasSecondWord())
		{
			Console.WriteLine("What do you want to use?");
			return;
		}

		string itemName = command.SecondWord.ToLower();

		player.Use(itemName);
	}

}

