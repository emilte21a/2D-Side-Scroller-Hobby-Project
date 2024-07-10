
public abstract class Item : GameObject
{
    public string name;
    public ushort ID;
    public bool craftable;
    public bool stackable;
    public Texture2D texture;
    public ushort itemDamage;
    public bool usable;
    public Dictionary<Item, int> recipe;
    public ItemType itemType;

    public ushort dropAmount;
}

public sealed class StoneItem : Item, IPlaceable
{
    static Texture2D tex;
    public StoneItem()
    {
        name = "Stone";
        ID = 0;
        craftable = false;
        stackable = true;
        usable = false;
        itemType = ItemType.stone;
        dropAmount = 1;
        if (tex.Id == 0)
            tex = Raylib.LoadTexture("Images/rockTexture.png");

        texture = tex;
    }
    public TilePref TilePrefToPlace(Vector2 pos)
    {
        return new StoneTile(pos);
    }
}

public sealed class GrassItem : Item, IPlaceable
{
    static Texture2D tex;
    public GrassItem()
    {
        name = "Grass";
        ID = 1;
        craftable = false;
        stackable = true;
        usable = false;
        dropAmount = 1;
        itemType = ItemType.grass;
        if (tex.Id == 0)
            tex = Raylib.LoadTexture("Images/grassTexture.png");

        texture = tex;
    }
    public TilePref TilePrefToPlace(Vector2 pos)
    {
        return new GrassTile(pos);
    }
}

public sealed class DirtItem : Item, IPlaceable
{
    static Texture2D tex;
    public DirtItem()
    {
        name = "Dirt";
        ID = 2;
        craftable = false;
        stackable = true;
        usable = false;
        dropAmount = 1;
        itemType = ItemType.dirt;
        if (tex.Id == 0)
            tex = Raylib.LoadTexture("Images/dirtTexture.png");

        texture = tex;
    }
    public TilePref TilePrefToPlace(Vector2 pos)
    {
        return new DirtTile(pos);
    }
}

public sealed class WoodItem : Item
{
    static Texture2D tex;
    public WoodItem()
    {
        name = "Wood";
        ID = 3;
        craftable = false;
        stackable = true;
        usable = false;
        itemType = ItemType.wood;
        dropAmount = 6;
        if (tex.Id == 0)
            tex = Raylib.LoadTexture("Images/woodTexture.png");

        texture = tex;
    }

}

public sealed class StickItem : Item
{
    static Texture2D tex;
    public StickItem()
    {
        name = "Stick";
        ID = 4;
        craftable = true;
        stackable = true;
        usable = false;
        itemType = ItemType.stick;

        if (tex.Id == 0)
            tex = Raylib.LoadTexture("Images/stickTexture.png");
        texture = tex;


        recipe = new() { { new WoodItem(), 1 } };
    }
}

public sealed class WoodPickaxe : Item
{
    static Texture2D tex;
    public WoodPickaxe()
    {
        name = "Wooden Pickaxe";
        ID = 5;
        stackable = false;
        craftable = false;
        usable = true;
        itemDamage = 10;
        itemType = ItemType.woodenPickaxe;

        if (tex.Id == 0)
            tex = Raylib.LoadTexture("Images/woodenPickaxeTexture.png");
        texture = tex;

        recipe = new() { { new StickItem(), 1 }, { new WoodItem(), 2 } };
    }
}
public sealed class StoneAxe : Item
{
    static Texture2D tex;
    public StoneAxe()
    {
        name = "Stone Axe";
        ID = 6;
        stackable = false;
        craftable = false;
        usable = true;
        itemDamage = 15;
        itemType = ItemType.stoneAxe;

        if (tex.Id == 0)
            tex = Raylib.LoadTexture("Images/stoneAxeTexture.png");
        texture = tex;

        recipe = new() { { new StickItem(), 1 }, { new StoneItem(), 2 } };
    }
}

public sealed class CraftingTableItem : Item, IPlaceable
{
    static Texture2D tex;

    public CraftingTableItem()
    {
        name = "Crafting Table";
        ID = 7;
        stackable = false;
        craftable = true;
        usable = true;

        if (tex.Id == 0)
            tex = Raylib.LoadTexture("Images/craftingTableIcon.png");

        texture = tex;
        recipe = new() { { new WoodItem(), 4 } };
    }

    public TilePref TilePrefToPlace(Vector2 pos)
    {
        return new CraftingTable(pos);
    }
}

public sealed class TorchItem : Item, IPlaceable
{
    static Texture2D tex;

    public TorchItem()
    {
        name = "Torch";
        ID = 8;
        stackable = true;
        craftable = true;
        usable = false;
        if (tex.Id == 0)
            tex = Raylib.LoadTexture("Images/torchTexture.png");

        texture = tex;

        recipe = new() { { new StickItem(), 1 } };
    }

    public TilePref TilePrefToPlace(Vector2 pos)
    {
        return new Torch(pos);
    }
}