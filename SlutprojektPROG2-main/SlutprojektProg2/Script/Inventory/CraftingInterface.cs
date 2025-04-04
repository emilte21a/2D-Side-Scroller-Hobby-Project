public class CraftingInterface
{
    public List<Item> craftables;
    public List<CraftingSlot> craftingSlots;
    Player player;

    public CraftingInterface(Player player)
    {
        this.player = player;
        craftables = new List<Item>() { new StickItem(), new StoneAxe(), new WoodPickaxe(), new TorchItem() };
        craftingSlots = new List<CraftingSlot>();
        for (int i = 0; i < craftables.Count; i++)
        {
            Vector2 slotPosition = new Vector2(i * 80 + Game.ScreenWidth / 2 - craftables.Count * 80 / 2 + 40, Game.ScreenHeight / 2 - 31);
            CraftingSlot craftingSlot = new CraftingSlot(craftables[i], slotPosition);
            craftingSlots.Add(craftingSlot);
        }
    }

    Item? _itemToCraft;
    Rectangle craftButtonRectangle = new Rectangle(Game.ScreenWidth - 475, Game.ScreenHeight - 250, 250, 120);
    Color craftButtonColor = Color.Orange;


    public void UpdateCraftingInterface()
    {
        Vector2 mousePos = Raylib.GetMousePosition();

        for (int i = 0; i < craftingSlots.Count; i++)
        {
            if (Raylib.CheckCollisionPointRec(mousePos, craftingSlots[i].rectangle) && Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                _itemToCraft = craftingSlots[i].itemToCraft;
            }

            if (Raylib.CheckCollisionPointRec(mousePos, craftingSlots[i].rectangle))
            {
                craftingSlots[i].slotColor = Color.Gray;
            }
            else
            {
                craftingSlots[i].slotColor = Color.Black;
            }
        }

        if (Raylib.CheckCollisionPointRec(mousePos, craftButtonRectangle))
        {
            craftButtonColor = Color.Yellow;
            if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                player.inventory.CraftItem(_itemToCraft);
            }
        }
        else
            craftButtonColor = Color.Orange;


    }



    public void Draw()
    {


        for (int i = 0; i < craftingSlots.Count; i++)
        {
            Raylib.DrawRectangleRec(craftingSlots[i].rectangle, craftingSlots[i].slotColor);
            Raylib.DrawTextureV(craftingSlots[i].itemToCraft.texture, craftingSlots[i].position, Color.White);
        }

        Raylib.DrawRectangle(Game.ScreenWidth - 500, 100, 300, Game.ScreenHeight - 200, Color.Black);
        Raylib.DrawRectangleLines(Game.ScreenWidth - 500, 100, 300, Game.ScreenHeight - 200, Color.Gray);
        Raylib.DrawRectangleRec(craftButtonRectangle, craftButtonColor);
        Raylib.DrawText("Craft", (int)craftButtonRectangle.X + 20, (int)craftButtonRectangle.Y + 10, 20, Color.White);

        if (_itemToCraft != null)
        {
            int xOffset = 0;
            int spacing = 150;
            Raylib.DrawTexture(_itemToCraft.texture, (int)craftButtonRectangle.X, (int)(craftButtonRectangle.Y - 160), Color.White);
            Raylib.DrawText(_itemToCraft.name, (int)craftButtonRectangle.X, (int)(craftButtonRectangle.Y - 200), 20, Color.White);
            Raylib.DrawText("Required:", (int)craftButtonRectangle.X, (int)(craftButtonRectangle.Y - 100), 20, Color.White);

            foreach (var ingredient in _itemToCraft.recipe)
            {
                Color canCraftItemColor = Color.White;
                if (!player.inventory.CanCraft(_itemToCraft))
                    canCraftItemColor = Color.Red;

                else
                    canCraftItemColor = Color.White;

                Raylib.DrawTexture(ingredient.Key.texture, (int)craftButtonRectangle.X + xOffset, (int)(craftButtonRectangle.Y - 75), Color.White);
                Raylib.DrawText($":{ingredient.Value}", (int)(craftButtonRectangle.X + 60 + xOffset), (int)(craftButtonRectangle.Y - 75), 60, canCraftItemColor);
                xOffset += spacing;
            }
            // Raylib.DrawText($"{player.inventory.GetItemByID(_itemToCraft.ID).Value}", (int)(craftButtonRectangle.X + 100), (int)(craftButtonRectangle.Y - 100), 60, Color.White);
        }

    }

}


public class CraftingSlot
{
    public Item itemToCraft;
    public Rectangle rectangle;
    public Vector2 position;
    public Color slotColor = Color.Black;

    public CraftingSlot(Item slotItem, Vector2 pos)
    {
        itemToCraft = slotItem;
        position = pos;
        rectangle = new Rectangle(position.X, position.Y, 62, 62);
    }
}