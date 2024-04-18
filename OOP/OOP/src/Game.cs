using System;
using System.Reflection.Metadata;

class Game
{
	private Parser parser;
	private Player player;
	private bool keyUsed = false;
	Item diamond = new Item(15, "diamond");

	Item medkit = new Item(20, "bandage");

	public Game()
	{
		parser = new Parser();
		player = new Player();
		CreateRooms();
	}

	private void CreateRooms()
	{
		Room outside = new Room("You are outside at the entrance of a train station");
		Room theatre = new Room("In the theater room in the train station");
		Room pub = new Room("In the train station pub");
		Room office = new Room("In the computing train station admin office");
		Room hallway = new Room("In the train station hallway");
		Room up = new Room("In the second floor of the train station");
		Room storage = new Room("In the storage room");

		outside.AddExit("hallway", hallway);

		hallway.AddExit("theatre", theatre);
		hallway.AddExit("office", office);
		hallway.AddExit("pub", pub);
		hallway.AddExit("stairs_up", up);
		
		theatre.AddExit("hallway", hallway);
		office.AddExit("hallway", hallway);
		pub.AddExit("hallway", hallway);

		theatre.AddExit("office", office);
		office.AddExit("pub", pub);
		pub.AddExit("theatre", theatre);

		up.AddExit("stairs_down", hallway);
		up.AddExit("storageroom", storage);

		storage.AddExit("stairs_down", hallway);

		storage.Chest.Put("diamond", diamond);

		hallway.Chest.Put("bandage", medkit);

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
		Console.WriteLine("Thank you for playing.");
		Console.WriteLine("Press [Enter] to continue.");
		Console.ReadLine();
	}

	private void PrintWelcome()
	{
		Console.WriteLine();
		Console.WriteLine("You are outside at the entrance from an old train station.");
		Console.WriteLine("Your goal is to find the hidden diamond, and use it.");
		Console.WriteLine("But be aware, the rooms have a poisonous smell.");
		Console.WriteLine("So don't stay inside for too long, because it's bad for your health.");
		Console.WriteLine("Write 'help' if you need any help.");
		Console.WriteLine();
		Console.WriteLine(player.CurrentRoom.GetLongDescription(player));
	}

private bool ProcessCommand(Command command)
{
	bool wantToQuit = false;

	if (!player.IsAlive() && command.CommandWord != "quit")
	{
        Console.WriteLine("You are poisonous, you have died...");
		Console.WriteLine("You are only able to use the command:");
		Console.WriteLine("quit");
		return wantToQuit;
	}

	if (keyUsed && command.CommandWord != "quit") 
	{
		Console.WriteLine("You have won the game, you are only able to use the command:");
		Console.WriteLine("quit");
		return wantToQuit;
	}

	if (command.IsUnknown())
	{
		Console.WriteLine("I don't know what you mean...");
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
			UseItem(command, out keyUsed); 
			break;
		case "quit":
			wantToQuit = true;
			break;
	}

	return wantToQuit;
}

	private void PrintHelp()
	{
		Console.WriteLine("you are searching alone at the old train station.");
		Console.WriteLine();
		parser.PrintValidCommands();
	}

	private void Look()
	{
		Console.WriteLine(player.CurrentRoom.GetLongDescription(player));

		Dictionary<string, Item> roomItems = player.CurrentRoom.Chest.GetItems();
		if (roomItems.Count > 0)
		{
			Console.WriteLine("Items the room:");
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
		Console.WriteLine($"You have {player.GetHealth()} health");

		Dictionary<string, Item> items = player.GetItems();

		if (items.Count > 0)
		{
			Console.WriteLine("Your current items:");

			foreach (var itemEntry in items)
			{
				Console.WriteLine($"- {itemEntry.Key}: Weight {itemEntry.Value.Weight}");
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
			return;
		}

		string direction = command.SecondWord;

		Room nextRoom = player.CurrentRoom.GetExit(direction);
		if (nextRoom == null)
		{
			Console.WriteLine("There isn't a door to "+direction+"!");
			return;
		}

		player.Damage(20);
		player.CurrentRoom = nextRoom;
		Console.WriteLine(player.CurrentRoom.GetLongDescription(player));
		
		if (!player.IsAlive()) 
		{
			Console.WriteLine("you got poisoned, you have died...");
		}
	}

    private void UseItem(Command command, out bool keyUsed)
    {
        if (!command.HasSecondWord())
        {
            Console.WriteLine("What do you want to use?");
            keyUsed = false;
            return;
        }

        string itemName = command.SecondWord.ToLower();

        bool itemUsed = player.Use(itemName, out keyUsed);

        if (itemUsed)
        {
            if (keyUsed)
            {
                this.keyUsed = true; 
				Console.WriteLine("Congratulations, you have used and found the hidden diamond.");
				Console.WriteLine("You have won the game!");
				Console.WriteLine("You are only able to use the command:");
				Console.WriteLine("quit");
            }
        }
    }
}
