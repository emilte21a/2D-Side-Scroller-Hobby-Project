
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
    public StoneItem()
    {
        name = "Stone";
        ID = 0;
        craftable = false;
        stackable = true;
        usable = false;
        itemType = ItemType.stone;
        dropAmount = 1;
        texture = TextureManager.LoadTexture("Images/rockTexture.png");
    }
    public TilePref TilePrefToPlace(Vector2 pos)
    {
        return new StoneTile(pos);
    }
}

public sealed class GrassItem : Item, IPlaceable
{
    public GrassItem()
    {
        name = "Grass";
        ID = 1;
        craftable = false;
        stackable = true;
        usable = false;
        dropAmount = 1;
        itemType = ItemType.grass;
        texture = TextureManager.LoadTexture("Images/grassTexture.png");
    }
    public TilePref TilePrefToPlace(Vector2 pos)
    {
        return new GrassTile(pos);
    }
}

public sealed class DirtItem : Item, IPlaceable
{
    public DirtItem()
    {
        name = "Dirt";
        ID = 2;
        craftable = false;
        stackable = true;
        usable = false;
        dropAmount = 1;
        itemType = ItemType.dirt;

        texture = TextureManager.LoadTexture("Images/dirtTexture.png");
    }
    public TilePref TilePrefToPlace(Vector2 pos)
    {
        return new DirtTile(pos);
    }
}

public sealed class WoodItem : Item
{
    public WoodItem()
    {
        name = "Wood";
        ID = 3;
        craftable = false;
        stackable = true;
        usable = false;
        itemType = ItemType.wood;
        dropAmount = 6;

        texture = TextureManager.LoadTexture("Images/woodTexture.png");
    }

}

public sealed class StickItem : Item
{
    public StickItem()
    {
        name = "Stick";
        ID = 4;
        craftable = true;
        stackable = true;
        usable = false;
        itemType = ItemType.stick;

        texture = TextureManager.LoadTexture("Images/stickTexture.png");


        recipe = new() { { new WoodItem(), 1 } };
    }
}

public sealed class WoodPickaxe : Item
{
    public WoodPickaxe()
    {
        name = "Wooden Pickaxe";
        ID = 5;
        stackable = false;
        craftable = true;
        usable = true;
        itemDamage = 10;
        itemType = ItemType.woodenPickaxe;

        texture = TextureManager.LoadTexture("Images/woodenPickaxeTexture.png");

        recipe = new() { { new StickItem(), 1 }, { new WoodItem(), 2 } };
    }
}
public sealed class StoneAxe : Item
{
    public StoneAxe()
    {
        name = "Stone Axe";
        ID = 6;
        stackable = false;
        craftable = true;
        usable = true;
        itemDamage = 25;
        itemType = ItemType.stoneAxe;

        texture = TextureManager.LoadTexture("Images/stoneAxeTexture.png");

        recipe = new() { { new StickItem(), 1 }, { new StoneItem(), 2 } };
    }
}

public sealed class CraftingTableItem : Item, IPlaceable
{
    public CraftingTableItem()
    {
        name = "Crafting Table";
        ID = 7;
        stackable = false;
        craftable = true;
        usable = true;

        texture = TextureManager.LoadTexture("Images/craftingTableIcon.png");
        recipe = new() { { new WoodItem(), 4 } };
    }

    public TilePref TilePrefToPlace(Vector2 pos)
    {
        return new CraftingTable(pos);
    }
}

public sealed class TorchItem : Item, IPlaceable
{
    public TorchItem()
    {
        name = "Torch";
        ID = 8;
        stackable = true;
        craftable = true;
        usable = false;
      
        texture = TextureManager.LoadTexture("Images/torchTexture.png");

        recipe = new() { { new StickItem(), 1 } };
    }

    public TilePref TilePrefToPlace(Vector2 pos)
    {
        return new Torch(pos);
    }
}