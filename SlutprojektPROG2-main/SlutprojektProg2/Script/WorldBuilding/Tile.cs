public sealed class GrassTile : TilePref
{
    public GrassTile(Vector2 pos)
    {
        components = new();
        renderer = AddComponent<Renderer>();
        tag = "Tile";

        position = pos;

        renderer.sprite = TextureManager.LoadTexture("Images/GrassTile.png");
        rectangle = new Rectangle(position.X, position.Y, renderer.sprite.Width, renderer.sprite.Height);
        HP = 100;

        dropType = new GrassItem();
    }
}

public sealed class StoneTile : TilePref
{
    public StoneTile(Vector2 pos)
    {
        components = new();
        renderer = AddComponent<Renderer>();
        tag = "Tile";
        position = pos;

        renderer.sprite = TextureManager.LoadTexture("Images/StoneTile.png");
        rectangle = new Rectangle(position.X, position.Y, renderer.sprite.Width, renderer.sprite.Height);
        HP = 100;

        dropType = new StoneItem();
    }
}

public sealed class DirtTile : TilePref
{
    public DirtTile(Vector2 pos)
    {
        components = new();
        renderer = AddComponent<Renderer>();
        tag = "Tile";
        rectangle = new Rectangle(0, 0, 80, 80);
        position = pos;
        
        renderer.sprite = TextureManager.LoadTexture("Images/DirtTile.png");

        rectangle = new Rectangle(position.X, position.Y, renderer.sprite.Width, renderer.sprite.Height);
        HP = 100;

        dropType = new DirtItem();
    }
}

public sealed class BackgroundTile : TilePref
{
    public BackgroundTile(Vector2 pos)
    {
        components = new();
        renderer = AddComponent<Renderer>();
        tag = "BackgroundTile";
        rectangle = new Rectangle(0, 0, 80, 80);
        position = pos;

        renderer.sprite = TextureManager.LoadTexture("Images/BackgroundTile.png");
        rectangle = new Rectangle(position.X, position.Y, renderer.sprite.Width, renderer.sprite.Height);

        HP = 100;
    }
}

public sealed class CraftingTable : TilePref, IInteractable
{
    public CraftingTable(Vector2 pos)
    {
        components = new();
        renderer = AddComponent<Renderer>();
        tag = "CraftingTable";
        rectangle = new Rectangle(0, 0, 80, 80);
        position = pos;

        renderer.sprite = TextureManager.LoadTexture("Images/craftingtable.png");
        rectangle = new Rectangle(position.X, position.Y, renderer.sprite.Width, renderer.sprite.Height);

        HP = 100;
    }

    public void OnInteract()
    {
        if (!Game.shouldShowCraftingInterface) Game.shouldShowCraftingInterface = true;

        else if (Game.shouldShowCraftingInterface) Game.shouldShowCraftingInterface = false;

    }
}
