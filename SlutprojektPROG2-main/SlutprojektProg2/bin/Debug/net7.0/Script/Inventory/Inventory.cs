using System.Data.Common;
using System.IO.Pipes;
using System.Runtime.CompilerServices;

public class Inventory
{
    public Dictionary<Item, int> itemsInInventory;

    private Slot[] inventoryHotbar;
    private Slot[] inventoryBackpack;

    private bool _inventoryChanged = false;

    private bool _shouldShowInventory = false;

    public Item currentActiveItem;

    public Inventory()
    {
        itemsInInventory = new Dictionary<Item, int>();
        inventoryHotbar = new Slot[5];
        inventoryBackpack = new Slot[15];
    }

    public void Update()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Tab))
            _shouldShowInventory = !_shouldShowInventory;

        if (itemsInInventory.Count != 0)
            currentActiveItem = inventoryHotbar[CurrentItemIndex()].item;

        else
            currentActiveItem = null;

        if (_shouldShowInventory)
            UpdateSlots();

        if (_inventoryChanged)
        {
            // Update hotbar slots
            for (int i = 0; i < inventoryHotbar.Length; i++)
            {
                if (inventoryHotbar[i].item != null)
                {
                    if (itemsInInventory.ContainsKey(inventoryHotbar[i].item))
                    {
                        inventoryHotbar[i].Count = (ushort)itemsInInventory[inventoryHotbar[i].item];
                    }
                    else
                    {

                        inventoryHotbar[i].item = null;
                        inventoryHotbar[i].Count = 0;
                    }
                }

                if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), new Rectangle(inventoryHotbar[i].position.X, inventoryHotbar[i].position.Y, 62, 62)))
                {
                    inventoryHotbar[i].slotColor = Color.Gray;
                }
                else if (activeitemIndex == i)
                    inventoryHotbar[i].slotColor = Color.Gold;
                else
                {
                    inventoryHotbar[i].slotColor = Color.Black;
                }
            }
            _inventoryChanged = false;
        }

        for (int i = 0; i < itemsInInventory.Count; i++)
        {
            var itemAtIndex = itemsInInventory.ElementAt(i);
            if (itemAtIndex.Value == 0)
            {
                inventoryHotbar[i].item = null;
                RemoveFromInventory(itemAtIndex.Key);
                break;
            }
        }

    }

    private int activeitemIndex;
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
        _inventoryChanged = true;
        return activeitemIndex;
    }

    public void Draw()
    {
        for (int i = 0; i < inventoryHotbar.Length; i++)
        {
            Vector2 slotPosition = new Vector2(i * 80 + Game.ScreenWidth / 2 - inventoryHotbar.Length * 80 / 2 + 40, Game.ScreenHeight - 100);
            inventoryHotbar[i].position = slotPosition;
            Raylib.DrawRectangleV(slotPosition, new Vector2(62, 62), inventoryHotbar[i].slotColor);

            if (inventoryHotbar[i].item != null && !inventoryHotbar[i].isDragging)
            {
                Raylib.DrawTextureV(inventoryHotbar[i].item.texture, slotPosition, Color.White);
                Raylib.DrawText($"{inventoryHotbar[i].Count}", (int)(slotPosition.X + 30), (int)(slotPosition.Y + 30), 10, Color.White);
            }

            if (inventoryHotbar[i].index >= inventoryHotbar.Length)
                inventoryHotbar[i].index = (ushort)FindFirstEmptySlot(inventoryHotbar);
        }

        if (_shouldShowInventory)
        {
            Rectangle inventoryRect = new Rectangle(Game.ScreenWidth / 2 - inventoryBackpack.Length * 80 / 2 + 30, 190, inventoryBackpack.GetLength(0) * 80, 80);
            Raylib.DrawRectangleRec(inventoryRect, Color.DarkPurple);
            DrawSlots();
        }
    }



    private void DrawSlots()
    {
        // Draw backpack slots
        for (int x = 0; x < inventoryBackpack.Length; x++)
        {
            Vector2 slotPosition = new Vector2(x * 80 + Game.ScreenWidth / 2 - inventoryBackpack.GetLength(0) * 80 / 2 + 40, 80 + 200);
            inventoryBackpack[x].position = slotPosition;
            Raylib.DrawRectangleV(slotPosition, new Vector2(62, 62), inventoryBackpack[x].slotColor);
            Raylib.DrawRectangleLines((int)slotPosition.X, (int)slotPosition.Y, 62, 62, Color.Gray);
            if (inventoryBackpack[x].item != null && !inventoryBackpack[x].isDragging)
            {
                Raylib.DrawTextureV(inventoryBackpack[x].item.texture, slotPosition, Color.White);
                Raylib.DrawText($"{inventoryBackpack[x].Count}", (int)(slotPosition.X + 30), (int)(slotPosition.Y + 30), 10, Color.White);
            }
        }

        // Draw dragging items on top
        for (int x = 0; x < inventoryBackpack.Length; x++)
        {
            if (inventoryBackpack[x].isDragging)
            {
                Vector2 dragPosition = Raylib.GetMousePosition() - inventoryBackpack[x].dragOffset;
                Raylib.DrawTextureV(inventoryBackpack[x].item.texture, dragPosition, Color.White);
                Raylib.DrawText($"{inventoryBackpack[x].Count}", (int)(dragPosition.X + 30), (int)(dragPosition.Y + 30), 10, Color.White);
            }
        }

        for (int i = 0; i < inventoryHotbar.Length; i++)
        {
            if (inventoryHotbar[i].isDragging)
            {
                Vector2 dragPosition = Raylib.GetMousePosition() - inventoryHotbar[i].dragOffset;
                Raylib.DrawTextureV(inventoryHotbar[i].item.texture, dragPosition, Color.White);
                Raylib.DrawText($"{inventoryHotbar[i].Count}", (int)(dragPosition.X + 30), (int)(dragPosition.Y + 30), 10, Color.White);
            }
        }
    }

    private void UpdateSlots()
    {
        Vector2 mousePos = Raylib.GetMousePosition();

        // Update backpack slots
        for (int x = 0; x < inventoryBackpack.Length; x++)
        {

            if (inventoryBackpack[x].item != null)
            {
                inventoryBackpack[x].Count = (ushort)itemsInInventory[inventoryBackpack[x].item];
            }

            if (Raylib.CheckCollisionPointRec(mousePos, new Rectangle(inventoryBackpack[x].position.X, inventoryBackpack[x].position.Y, 62, 62)))
            {
                inventoryBackpack[x].slotColor = Color.Gray;
            }
            else
            {
                inventoryBackpack[x].slotColor = Color.Black;
            }

        }



        // Handle dragging start
        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            for (int x = 0; x < inventoryBackpack.Length; x++)
            {

                if (Raylib.CheckCollisionPointRec(mousePos, new Rectangle(inventoryBackpack[x].position.X, inventoryBackpack[x].position.Y, 62, 62)))
                {
                    inventoryBackpack[x].isDragging = true;
                    inventoryBackpack[x].dragOffset = new Vector2(mousePos.X - inventoryBackpack[x].position.X, mousePos.Y - inventoryBackpack[x].position.Y);
                    break;
                }

            }

            for (int i = 0; i < inventoryHotbar.Length; i++)
            {
                if (Raylib.CheckCollisionPointRec(mousePos, new Rectangle(inventoryHotbar[i].position.X, inventoryHotbar[i].position.Y, 62, 62)))
                {
                    inventoryHotbar[i].isDragging = true;
                    inventoryHotbar[i].dragOffset = new Vector2(mousePos.X - inventoryHotbar[i].position.X, mousePos.Y - inventoryHotbar[i].position.Y);
                    break;
                }
            }
        }

        // Handle dragging end
        if (Raylib.IsMouseButtonReleased(MouseButton.Left))
        {
            // Check backpack slots
            for (int x = 0; x < inventoryBackpack.Length; x++)
            {

                if (inventoryBackpack[x].isDragging)
                {
                    bool swapped = false;

                    // Snap to backpack slots
                    for (int nx = 0; nx < inventoryBackpack.Length; nx++)
                    {

                        if (Raylib.CheckCollisionPointRec(mousePos, new Rectangle(inventoryBackpack[nx].position.X, inventoryBackpack[nx].position.Y, 62, 62)))
                        {
                            SwapSlots(ref inventoryBackpack[x], ref inventoryBackpack[nx]);
                            swapped = true;
                            break;
                        }

                        if (swapped) break;
                    }

                    // Snap to hotbar slots
                    if (!swapped)
                    {
                        for (int ni = 0; ni < inventoryHotbar.Length; ni++)
                        {
                            if (Raylib.CheckCollisionPointRec(mousePos, new Rectangle(inventoryHotbar[ni].position.X, inventoryHotbar[ni].position.Y, 62, 62)))
                            {
                                SwapSlots(ref inventoryBackpack[x], ref inventoryHotbar[ni]);
                                swapped = true;
                                break;
                            }
                        }
                    }

                    inventoryBackpack[x].isDragging = false;
                }

            }

            // Check hotbar slots
            for (int i = 0; i < inventoryHotbar.Length; i++)
            {
                if (inventoryHotbar[i].isDragging)
                {
                    bool swapped = false;

                    // Snap to hotbar slots
                    for (int ni = 0; ni < inventoryHotbar.Length; ni++)
                    {
                        if (Raylib.CheckCollisionPointRec(mousePos, new Rectangle(inventoryHotbar[ni].position.X, inventoryHotbar[ni].position.Y, 62, 62)))
                        {
                            SwapSlots(ref inventoryHotbar[i], ref inventoryHotbar[ni]);
                            swapped = true;
                            break;
                        }
                    }

                    // Snap to backpack slots
                    if (!swapped)
                    {
                        for (int nx = 0; nx < inventoryBackpack.Length; nx++)
                        {
                            if (Raylib.CheckCollisionPointRec(mousePos, new Rectangle(inventoryBackpack[nx].position.X, inventoryBackpack[nx].position.Y, 62, 62)))
                            {
                                SwapSlots(ref inventoryHotbar[i], ref inventoryBackpack[nx]);
                                swapped = true;
                                break;
                            }

                            if (swapped) break;
                        }
                    }

                    inventoryHotbar[i].isDragging = false;
                }
            }
        }

        // Handle dragging move
        if (Raylib.IsMouseButtonDown(MouseButton.Left))
        {
            for (int x = 0; x < inventoryBackpack.Length; x++)
            {

                if (inventoryBackpack[x].isDragging)
                {
                    inventoryBackpack[x].position.X = mousePos.X - inventoryBackpack[x].dragOffset.X;
                    inventoryBackpack[x].position.Y = mousePos.Y - inventoryBackpack[x].dragOffset.Y;
                }

            }

            for (int i = 0; i < inventoryHotbar.Length; i++)
            {
                if (inventoryHotbar[i].isDragging)
                {
                    inventoryHotbar[i].position.X = mousePos.X - inventoryHotbar[i].dragOffset.X;
                    inventoryHotbar[i].position.Y = mousePos.Y - inventoryHotbar[i].dragOffset.Y;
                }
            }
        }
    }

    private void SwapSlots(ref Slot slot1, ref Slot slot2)
    {
        Slot temp = slot1;
        slot1 = slot2;
        slot2 = temp;

        // Snap the items to the center of the slots
        slot1.SnapPosition();
        slot2.SnapPosition();
    }

    public void AddToInventory(Item item, int quantity)
    {
        bool itemAlreadyExistsInInventory = itemsInInventory.Keys.Any(k => k.ID.Equals(item.ID));

        if (itemAlreadyExistsInInventory && item.stackable)
        {
            foreach (KeyValuePair<Item, int> kvp in itemsInInventory)
            {
                if (item.ID.Equals(kvp.Key.ID))
                {
                    itemsInInventory[kvp.Key] += quantity;
                }
                continue;
            }
        }
        else if (item.stackable && !itemAlreadyExistsInInventory)
            itemsInInventory[item] = quantity;

        else
            itemsInInventory.Add(item, 1);


        if (itemsInInventory.Count <= inventoryHotbar.Length)
        {
            int emptySlotIndex = FindFirstEmptySlot(inventoryHotbar);
            if (emptySlotIndex != -1 && (!itemAlreadyExistsInInventory || !item.stackable))
            {
                inventoryHotbar[emptySlotIndex] = new Slot(item);
                inventoryHotbar[emptySlotIndex].index = (ushort)emptySlotIndex;
            }
        }

        else
        {
            int emptySlotIndex = FindFirstEmptySlot(inventoryBackpack);
            if (emptySlotIndex != -1 && (!itemAlreadyExistsInInventory || !item.stackable))
            {
                inventoryBackpack[FindFirstEmptySlot(inventoryBackpack)] = new Slot(item);
                inventoryBackpack[FindFirstEmptySlot(inventoryBackpack)].Count = (ushort)itemsInInventory[item];
            }
        }
        _inventoryChanged = true;
    }

    public void RemoveFromInventory(Item item)
    {
        if (itemsInInventory.ContainsKey(item))
            itemsInInventory.Remove(item);

        _inventoryChanged = true;
    }

    public int FindFirstEmptySlot(Slot[] items)
    {
        for (int i = 0; i < items.Length; i++)
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
        if (item != null)
        {
            foreach (KeyValuePair<Item, int> ingredient in item.recipe)
            {
                if (!itemsInInventory.Any(i => ingredient.Key.ID == i.Key.ID && i.Value >= ingredient.Value))
                    return false;
            }
            return true;
        }

        return false;
    }

    public void CraftItem(Item item)
    {
        if (CanCraft(item))
        {
            foreach (KeyValuePair<Item, int> ingredient in item.recipe)
            {
                //if (!itemsInInventory.Any(i => ingredient.Key.ID == i.Key.ID && i.Value >= ingredient.Value))

                if (itemsInInventory.Any(i => i.Key.ID == ingredient.Key.ID))
                {
                    itemsInInventory[GetItemByID(ingredient.Key.ID).Key] -= ingredient.Value;

                    if (itemsInInventory[GetItemByID(ingredient.Key.ID).Key] <= 0)
                    {
                        itemsInInventory.Remove(GetItemByID(ingredient.Key.ID).Key);
                    }
                }

            }
            AddToInventory(item, 1);
        }
    }

    public KeyValuePair<Item, int> GetItemByID(ushort ID)
    {
        return itemsInInventory.FirstOrDefault(item => item.Key.ID == ID);
    }

    // private Vector2 FindFirstEmptySlotBackpack()
    // {
    //     for (int x = 0; x < inventoryBackpack.GetLength(1); x++)
    //     {
    //         for (int y = 0; y < inventoryBackpack.GetLength(0); y++)
    //         {
    //             if (inventoryBackpack[y, x].item == null)
    //                 return new Vector2(y, x);
    //         }
    //     }
    //     return -Vector2.One;
    // }

    public Slot? GetSlotAtPosition(Vector2 position, Slot[] items)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (Raylib.CheckCollisionPointRec(position, new Rectangle(items[i].position.X, items[i].position.Y, 62, 62)))
            {
                return items[i];
            }
        }

        // for (int x = 0; x < inventoryBackpack.GetLength(0); x++)
        // {
        //     for (int y = 0; y < inventoryBackpack.GetLength(1); y++)
        //     {
        //         if (Raylib.CheckCollisionPointRec(position, new Rectangle(inventoryBackpack[x, y].position.X, inventoryBackpack[x, y].position.Y, 62, 62)))
        //         {
        //             return inventoryBackpack[x, y];
        //         }
        //     }
        // }
        return null;
    }
}

public struct Slot
{
    public Color slotColor = Color.Black;
    public bool isDragging { get; set; }
    public Vector2 dragOffset { get; set; }
    public Rectangle rectangle = new Rectangle(0, 0, 62, 62);
    public Vector2 position;
    public ushort Count;
    public ushort index;
    public Item item;

    public Slot(Item item1)
    {
        item = item1;
        isDragging = false;
        dragOffset = Vector2.Zero;
        position = Vector2.Zero;
    }

    public void SnapPosition()
    {
        // Snap to the center of the slot
        position.X = (int)(position.X / 80) * 80 + 40;
        position.Y = (int)(position.Y / 80) * 80 + 200;
    }
}