public sealed class GrassTile : TilePref
{
    static Texture2D grassTexture;
    public GrassTile(Vector2 pos)
    {
        components = new();
        renderer = AddComponent<Renderer>();
        tag = "Tile";

        position = pos;
        if (grassTexture.Id == 0)
            grassTexture = Raylib.LoadTexture("Images/GrassTile.png");


        renderer.sprite = grassTexture;
        rectangle = new Rectangle(position.X, position.Y, renderer.sprite.Width, renderer.sprite.Height);
        HP = 100;

        dropType = new GrassItem();
    }
}

public sealed class StoneTile : TilePref
{
    static Texture2D stoneTexture;

    public StoneTile(Vector2 pos)
    {
        components = new();
        renderer = AddComponent<Renderer>();
        tag = "Tile";
        position = pos;
        if (stoneTexture.Id == 0)
        {
            stoneTexture = Raylib.LoadTexture("Images/StoneTile.png");
        }

        renderer.sprite = stoneTexture;
        rectangle = new Rectangle(position.X, position.Y, renderer.sprite.Width, renderer.sprite.Height);
        HP = 100;

        dropType = new StoneItem();
    }
}

public sealed class DirtTile : TilePref
{
    static Texture2D dirtTexture;

    public DirtTile(Vector2 pos)
    {
        components = new();
        renderer = AddComponent<Renderer>();
        tag = "Tile";
        rectangle = new Rectangle(0, 0, 80, 80);
        position = pos;
        if (dirtTexture.Id == 0)
        {
            dirtTexture = Raylib.LoadTexture("Images/DirtTile.png");
        }
        renderer.sprite = dirtTexture;

        rectangle = new Rectangle(position.X, position.Y, renderer.sprite.Width, renderer.sprite.Height);
        HP = 100;

        dropType = new DirtItem();
    }
}

public sealed class BackgroundTile : TilePref
{
    static Texture2D backgroundTexture;

    public BackgroundTile(Vector2 pos)
    {
        components = new();
        renderer = AddComponent<Renderer>();
        tag = "BackgroundTile";
        rectangle = new Rectangle(0, 0, 80, 80);
        position = pos;
        if (backgroundTexture.Id == 0)
            backgroundTexture = Raylib.LoadTexture("Images/BackgroundTile.png");

        renderer.sprite = backgroundTexture;
        rectangle = new Rectangle(position.X, position.Y, renderer.sprite.Width, renderer.sprite.Height);

        HP = 100;
    }
}

public sealed class CraftingTable : TilePref, IInteractable
{
    static Texture2D craftingTableTexture;

    public CraftingTable(Vector2 pos)
    {
        components = new();
        renderer = AddComponent<Renderer>();
        tag = "CraftingTable";
        rectangle = new Rectangle(0, 0, 80, 80);
        position = pos;
        if (craftingTableTexture.Id == 0)
            craftingTableTexture = Raylib.LoadTexture("Images/craftingtable.png");

        renderer.sprite = craftingTableTexture;
        rectangle = new Rectangle(position.X, position.Y, renderer.sprite.Width, renderer.sprite.Height);

        HP = 100;
    }

    public void OnInteract()
    {
        
    }
}
