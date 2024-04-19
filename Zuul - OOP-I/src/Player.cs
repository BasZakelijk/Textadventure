class Player
{
    public Room CurrentRoom { get; set; }
    private int health;
    private Inventory backpack;

    public Player()
    {
        CurrentRoom = null; 
        health = 100;
        backpack = new Inventory(25);
    }
    
    public Dictionary<string, Item> GetItems()
    {
        return backpack.GetItems();
    }

    public int GetHealth()
    {
        return health;
    }


    public void Damage(int amount)
    {
        health -= amount;
        if (health < 0)
            health = 0;
    }

    public void Heal(int amount)
    {
        health += amount;
        if (health > 100)
            health = 100;
    }

    public bool IsAlive()
    {
        return health > 0;
    }

    public bool TakeFromChest(string itemName)
    {
        Item item = CurrentRoom.Chest.Get(itemName);

        if (item != null) 
        {
            bool success = backpack.Put(itemName, item);
            
            if (success)
            {
                CurrentRoom.Chest.Get(itemName);
                Console.WriteLine($"You took the {itemName}.");
                return true;
            }
            else
            {
                CurrentRoom.Chest.Put(itemName, item);
                Console.WriteLine("You can't carry that. It's too heavy.");
                return false;
            }
        }
        else
        {
            Console.WriteLine($"There is no {itemName} here.");
            return false;
        }
    }

    public bool DropToChest(string itemName)
    {
        Item item = backpack.Get(itemName);

        if (item != null)
        {
            bool success = CurrentRoom.Chest.Put(itemName, item);

            if (success)
            {
                backpack.RemoveWeight(item.Weight);

                Console.WriteLine($"You have dropped the {itemName}.");
                return true;
            }
            else
            {
                backpack.Put(itemName, item);
                return false;
            }
        }
        else
        {
            Console.WriteLine($"You don't have {itemName} in your backpack.");
            return false;
        }
    }


    public bool Use(string itemName, out bool keyUsed)
    {
        keyUsed = false; 
        if (backpack.GetItems().ContainsKey(itemName))
        {
            if (itemName.ToLower() == "bandage")
            {
                health += 60;
                if (health > 100)
                {
                    health = 100;
                }

                backpack.GetItems().Remove(itemName);
                backpack.RemoveWeight(20);
                Console.WriteLine($"You have used the bandage. You have now {health} health.");
                return true; 
            }
            else if (itemName.ToLower() == "diamond")
            {
                keyUsed = true; 
                backpack.GetItems().Remove(itemName);
                Console.WriteLine("You used the key.");
                backpack.RemoveWeight(15);
                return true; 
            }
            else
            {
                Console.WriteLine("You aren't able to use that item.");
                return false; 
            }
        }
        else
        {
            Console.WriteLine("You don't have that item in your inventory.");
            return false; 
        }
    }

}

