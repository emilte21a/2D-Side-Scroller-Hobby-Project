

public class Inventory
{
    public static Inventory current;
    public Dictionary<Item, int> itemsInInventory;

    private Slot[] inventoryHotbar;
    private Slot[,] inventoryBackpack;

    private Texture2D _hotbarTexture = Raylib.LoadTexture("Images/Hotbar.png");

    private Texture2D _itemChosenTexture = Raylib.LoadTexture("Images/itemChosen.png");

    private Texture2D _itemFrameTexture = Raylib.LoadTexture("Images/itemFrame.png");

    private bool _shouldShowInventory = false;

    public Item currentActiveItem;

    public Inventory()
    {
        itemsInInventory = new Dictionary<Item, int>();
        inventoryHotbar = new Slot[5];
        inventoryBackpack = new Slot[5, 3];
    }

    int xIndex = 0;
    int yIndex = 0;

    public void Update()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Tab) && !_shouldShowInventory)
            _shouldShowInventory = true;

        else if (Raylib.IsKeyPressed(KeyboardKey.Tab) && _shouldShowInventory)
            _shouldShowInventory = false;

        UpdateInventoryBackpack();

        if (itemsInInventory.Count != 0)
        {

            currentActiveItem = inventoryHotbar[CurrentItemIndex()].item;
        }
        else
            currentActiveItem = null;
    }


    int itemPos = 0;
    public void Draw()
    {
        Raylib.DrawTexture(_hotbarTexture, Game.ScreenWidth / 2 - _hotbarTexture.Width / 2, Game.ScreenHeight - 100, Color.White);

        foreach (Slot slot in inventoryHotbar)
        {
            if (itemPos < inventoryHotbar.Length)
            {
                Raylib.DrawTexture(_itemChosenTexture, Game.ScreenWidth / 2 - _hotbarTexture.Width / 2 + 11 + CurrentItemIndex() * 58, Game.ScreenHeight - 89, Color.White);

                if (slot.item != null && itemsInInventory.ContainsKey(slot.item))
                {
                    Raylib.DrawTexture(slot.item.texture, Game.ScreenWidth / 2 - _hotbarTexture.Width / 2 + 11 + 58 * slot.index, Game.ScreenHeight - 89, Color.White);
                    Raylib.DrawText($"{itemsInInventory[slot.item]}", Game.ScreenWidth / 2 - _hotbarTexture.Width / 2 + 54 + 58 * slot.index, Game.ScreenHeight - 64, 10, Color.White);
                }

                itemPos++;
            }

            if (itemPos >= inventoryHotbar.Length)
                itemPos = FindFirstEmptySlot();

        }

        if (_shouldShowInventory)
        {
            for (int x = 0; x < inventoryBackpack.GetLength(0); x++)
            {
                for (int y = 0; y < inventoryBackpack.GetLength(1); y++)
                {
                    Raylib.DrawTexture(_itemFrameTexture, x * 80 + Game.ScreenWidth / 2 - inventoryBackpack.GetLength(0) * 80 / 2 + 40, y * 80 + 200, Color.White);
                    if (inventoryBackpack[x, y].item != null)
                    {
                        Raylib.DrawTexture(inventoryBackpack[x, y].item.texture, x * 80 + 305, y * 80 + 100, Color.White);
                        Raylib.DrawText($"{itemsInInventory[inventoryBackpack[x, y].item]}", 305 + 80 + 80 * x, 100 + 80 + 80 * y, 10, Color.White);
                    }
                }
            }
        }
    }

    int activeitemIndex;
    public int CurrentItemIndex()
    {
        KeyboardKey keyPressed = (KeyboardKey)Raylib.GetKeyPressed();

        switch (keyPressed)
        {
            case KeyboardKey.One:
                activeitemIndex = 0;
                break;
            case KeyboardKey.Two:
                activeitemIndex = 1;
                break;
            case KeyboardKey.Three:
                activeitemIndex = 2;
                break;
            case KeyboardKey.Four:
                activeitemIndex = 3;
                break;
            case KeyboardKey.Five:
                activeitemIndex = 4;
                break;
        }
        return activeitemIndex;
    }

    public void AddToInventory(Item item, int quantity)
    {
        bool x = itemsInInventory.Keys.Any(k => k.ID.Equals(item.ID));

        if (x && item.stackable)
        {
            foreach (var kvp in itemsInInventory)
            {
                if (item.ID.Equals(kvp.Key.ID))
                {
                    itemsInInventory[kvp.Key] += quantity;
                }
            }
        }
        else
            itemsInInventory[item] = 1;

        if (itemsInInventory.Count <= inventoryHotbar.Length)
        {
            int emptySlotIndex = FindFirstEmptySlot();
            if (emptySlotIndex != -1)
                inventoryHotbar[emptySlotIndex] = new Slot(item, emptySlotIndex);

        }
    }

    public void RemoveFromInventory(Item item)
    {
        if (itemsInInventory.ContainsKey(item))
            itemsInInventory.Remove(item);
    }

    public int FindFirstEmptySlot()
    {
        for (int i = 0; i < inventoryHotbar.Length; i++)
        {
            if (inventoryHotbar[i].item == null)
            {
                return i;
            }
            continue;
        }
        return -1;
    }

    public bool CanCraft(Item item)
    {

        foreach (KeyValuePair<Item, int> ingredient in item.recipe)
        {
            if (!itemsInInventory.ContainsKey(ingredient.Key) || itemsInInventory[ingredient.Key] < ingredient.Value)
            {
                return false;
            }
        }
        return true;
    }

    public void CraftItem(Item item)
    {
        if (CanCraft(item))
        {
            foreach (KeyValuePair<Item, int> ingredient in item.recipe)
            {
                itemsInInventory[ingredient.Key] -= ingredient.Value;
                if (itemsInInventory[ingredient.Key] <= 0)
                {
                    itemsInInventory.Remove(ingredient.Key);
                }
            }

            AddToInventory(item, 1);
        }
    }

    private void UpdateInventoryBackpack()
    {
        for (int i = 0; i < itemsInInventory.Count; i++)
        {
            if (i < inventoryHotbar.Length)
                continue;


            if (xIndex >= inventoryBackpack.GetLength(0))
            {
                xIndex = 0;
                yIndex++;


                if (yIndex >= inventoryBackpack.GetLength(1))
                    break;
            }

            inventoryBackpack[xIndex, yIndex].item = itemsInInventory.Keys.ElementAt(i);

            xIndex++;
        }

        xIndex = 0;
        yIndex = 0;
    }
}

public struct Slot
{
    public Vector2 position;
    public int index;
    public Item item;
    public Slot(Item item1, int slotIndex)
    {
        this.item = item1;
        this.index = slotIndex;
    }
}