public sealed class Tree : TilePref
{
    private List<Texture2D> textures = new();
    private static Texture2D oakTexture;
    private static Texture2D birchTexture;
    public Tree(Vector2 pos)
    {
        textures.Add(oakTexture);
        textures.Add(birchTexture);
        position = pos;
        components = new();
        renderer = AddComponent<Renderer>();

        if (oakTexture.Id == 0)
            oakTexture = Raylib.LoadTexture("Images/oakTree.png");

        if (birchTexture.Id == 0)
            birchTexture = Raylib.LoadTexture("Images/birchTree.png");

        renderer.sprite = textures[Random.Shared.Next(0, textures.Count)];
        rectangle = new Rectangle(0, 0, renderer.sprite.Width, renderer.sprite.Height);

        position.Y -= renderer.sprite.Height;
        rectangle.X = position.X;
        rectangle.Y = position.Y;
        HP = 100;

        dropType = new WoodItem();
    }
}

public sealed class Rock : TilePref
{
    private static Texture2D rockTexture;
    public Rock(Vector2 pos)
    {
        position = pos;
        components = new();
        renderer = AddComponent<Renderer>();

        if (rockTexture.Id == 0)
            rockTexture = Raylib.LoadTexture("Images/rock.png");

        renderer.sprite = rockTexture;
        rectangle = new Rectangle(0, 0, renderer.sprite.Width, renderer.sprite.Height);

        position.Y -= renderer.sprite.Height;
        rectangle.X = position.X;
        rectangle.Y = position.Y;

        HP = 120;
        dropType = new StoneItem();
    }
}

public sealed class Torch : TilePref, ILightSource
{
    private static Texture2D torchTexture;
    public Animator animator;

    public Torch(Vector2 pos)
    {
        position = pos;
        components = new();
        renderer = AddComponent<Renderer>();
        animator = AddComponent<Animator>();

        if (torchTexture.Id == 0)
            torchTexture = Raylib.LoadTexture("Images/torchAnimation.png");

        renderer.sprite = torchTexture;
        rectangle = new Rectangle(0, 0, renderer.sprite.Width, renderer.sprite.Height);

        position.Y -= renderer.sprite.Height / 2;
        rectangle.X = position.X;
        rectangle.Y = position.Y;

        HP = 1;
        dropType = new TorchItem();
    }
}