public class CraftingInterface
{
    public List<Item> craftables;
    public List<CraftingSlot> craftingSlots;

    public CraftingInterface()
    {
        craftables = new List<Item>() { new StickItem(), new StoneAxe(), new WoodPickaxe(), new TorchItem() };
        craftingSlots = new List<CraftingSlot>();
        for (int i = 0; i < craftables.Count; i++)
        {
            Vector2 slotPosition = new Vector2(i * 80 + Game.ScreenWidth / 2 - craftables.Count * 80 / 2 + 40, Game.ScreenHeight / 2 - 31);
            CraftingSlot craftingSlot = new CraftingSlot(craftables[i], slotPosition);
            craftingSlots.Add(craftingSlot);
        }
    }



    public void Draw()
    {
        for (int i = 0; i < craftingSlots.Count; i++)
        {
            Raylib.DrawRectangleRec(craftingSlots[i].rectangle, Color.Black);
            Raylib.DrawTextureV(craftingSlots[i].itemToCraft.texture, craftingSlots[i].position + new Vector2(31, 31), Color.White);
        }

        Raylib.DrawRectangle(Game.ScreenWidth - 400, 100, 300, Game.ScreenHeight - 200, Color.Black);
    }
}


public class CraftingSlot
{
    public Item itemToCraft;
    public Rectangle rectangle;
    public Vector2 position;

    public CraftingSlot(Item slotItem, Vector2 pos)
    {
        itemToCraft = slotItem;
        position = pos;
        rectangle = new Rectangle(position.X, position.Y, 62, 62);
    }
}