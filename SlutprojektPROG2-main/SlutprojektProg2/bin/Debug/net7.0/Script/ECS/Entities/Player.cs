public sealed class Player : Entity, IDrawable
{
    public Camera2D camera { get; init; }

    //Skapa spelarens egna physicsbody
    public PhysicsBody physicsBody;

    //Skapa en renderer för att rita ut spelaren
    public Renderer renderer;

    //Skapa spelarens collider
    public Collider collider;

    public AudioPlayer audioPlayer;

    public Animator animator;

    public Inventory inventory;

    public PlayerAction playerAction;

    private ushort _playerSpeed = 3;

    public ushort healthPoints;

    private Texture2D spriteSheet = Raylib.LoadTexture("Images/SpriteSheet.png");

    public static PlayerState playerState;

    public Player()
    {
        #region Lägg till spelarens komponenter
        components = new List<Component>();
        physicsBody = AddComponent<PhysicsBody>();
        collider = AddComponent<Collider>();
        renderer = AddComponent<Renderer>();
        audioPlayer = AddComponent<AudioPlayer>();
        animator = AddComponent<Animator>();
        inventory = new Inventory();
        playerAction = new PlayerAction();
        #endregion

        inventory.AddToInventory(new WoodPickaxe(), 1);
        inventory.AddToInventory(new CraftingTableItem(), 1);
        inventory.AddToInventory(new TorchItem(), 1);

        healthPoints = 100;
        tag = "Player";
        playerState = PlayerState.inGame;

        renderer.sprite = Raylib.LoadTexture("Images/CharacterSprite.png");

        rectangle = new Rectangle(0, 0, renderer.sprite.Width, renderer.sprite.Height);

        position = new Vector2(rectangle.X, rectangle.Y);
        physicsBody.UseGravity = PhysicsBody.Gravity.enabled;
        collider.boxCollider = new Rectangle(rectangle.X, rectangle.Y, rectangle.Width - 40, rectangle.Height - 20);
    }

    TilePref collidingTile;

    public override void Update()
    {
        for (int i = 0; i < WorldGeneration.gameObjectsThatShouldRender.Count; i++)
        {
            if (Raylib.CheckCollisionRecs(rectangle, WorldGeneration.gameObjectsThatShouldRender[i].rectangle) && WorldGeneration.gameObjectsThatShouldRender[i] != null)
            {
                collidingTile = WorldGeneration.gameObjectsThatShouldRender[i];
            }
        }

        if (collidingTile is IInteractable interactable)
        {
            if (Raylib.IsKeyPressed(KeyboardKey.E))
            {
                interactable.OnInteract();
            }
        }

        if (playerState == PlayerState.inGame)
        {
            collider.boxCollider.X = rectangle.X + 20;
            collider.boxCollider.Y = rectangle.Y + 20;

            MovePlayer(physicsBody, _playerSpeed);

            playerAction.origin = position;

            if (Raylib.IsMouseButtonPressed(0))
                playerAction.OnClick(position, (int)Raymath.Clamp(Raylib.GetScreenToWorld2D(Raylib.GetMousePosition(), Game.camera).X - position.X, -1, 1), inventory);

            if (Raylib.IsKeyPressed(KeyboardKey.Space) && physicsBody.airState == AirState.grounded)
                physicsBody.Jump(physicsBody, 12);

            if (Raylib.IsKeyPressed(KeyboardKey.H))
                healthPoints = 100;

            playerAction.Update();

            if (ItemManager.itemsOnGround.Count > 0)
            {
                for (int i = 0; i < ItemManager.itemsOnGround.Count; i++)
                {
                    if (Raylib.CheckCollisionRecs(rectangle, ItemManager.itemsOnGround[i].rectangle))
                    {
                        inventory.AddToInventory(ItemManager.itemsOnGround[i].itemDrop, ItemManager.itemsOnGround[i].itemDrop.dropAmount);

                        Game.gameObjectsToDestroy.Add(ItemManager.itemsOnGround[i]);

                        Game.entities.Remove(ItemManager.itemsOnGround[i]);
                        ItemManager.itemsOnGround.Remove(ItemManager.itemsOnGround[i]);

                    }
                }
            }

        }
        inventory.Update();
    }

    public void MovePlayer(PhysicsBody physicsBody, float speed)
    {
        physicsBody.velocity.X = Raymath.Clamp(InputManager.GetAxisX() * speed, -5f, 5f);
    }

    private int FallDamage()
    {
        return (int)physicsBody.velocity.Y > 10 ? (int)physicsBody.velocity.Y : 0;
    }

    public override void Draw()
    {
        Vector2 newPos = new Vector2(position.X - 20, position.Y - 20);
        if (physicsBody.velocity.X != 0)
            animator.PlayAnimation(spriteSheet, (int)InputManager.GetLastDirectionDelta(), 4, newPos);

        else
            Raylib.DrawTextureRec(renderer.sprite, new Rectangle(0, 0, renderer.sprite.Width * InputManager.GetLastDirectionDelta(), renderer.sprite.Height), newPos, Color.White);

        if (inventory.currentActiveItem != null)
        {
            if (inventory.currentActiveItem.usable)
                DrawCurrentItemAnimation();
        }
    }

    private void DrawCurrentItemAnimation()
    {
        Texture2D texture = inventory.currentActiveItem.texture;

        // Calculate rotation origin
        Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);

        // Draw the texture with rotation around the player
        Raylib.DrawTexturePro(
        texture,
        new Rectangle(0, 0, texture.Width * playerAction.xScale, texture.Height),
        new Rectangle(position.X + rectangle.Width / 2 + 20 * playerAction.xScale, position.Y - 20,
        texture.Width, texture.Height),
        origin,
        playerAction.rotation,
        Color.White
        );
    }
}

public enum PlayerState
{
    inInventory,
    inGame,
    inMenu
}