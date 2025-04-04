using System.Linq.Expressions;

public class PlayerAction
{

    public Rectangle handRectangle;
    Vector2 position;
    float timer = 2;
    public float rotation = 0;
    public Vector2 origin;

    public RotationDirection rotationDirection = RotationDirection.down;

    public int attackCount = 0;

    public PlayerAction()
    {
        position = new Vector2(0, 0);
        handRectangle = new Rectangle(0, 0, 20, 20);
    }

    public void Update()
    {
        if (isRotating)
        {
            // Increment rotation towards the target rotation
            if (rotation != targetRotation)
            {
                rotation = Raymath.Lerp(rotation, targetRotation * xScale, 16f * Raylib.GetFrameTime());
            }
            else
            {
                isRotating = false;
            }
        }

        if (attackCount % 2 == 1)
        {
            rotationDirection = RotationDirection.down;
        }
        else
            rotationDirection = RotationDirection.up;
    }

    private bool isRotating = false;
    private float targetRotation = -90f;
    private const float rotationSpeed = 90f; // Degrees per second

    public int yScale = 1;
    public int xScale = 1;

    public void OnClick(Vector2 playerPosition, int direction, Inventory inventory)
    {
        attackCount++;
        origin = playerPosition + new Vector2(40, 40);
        position = Raylib.GetScreenToWorld2D(Raylib.GetMousePosition(), Game.camera);

        handRectangle.X = position.X;
        handRectangle.Y = position.Y;
        xScale = direction;
        isRotating = true;
        targetRotation *= -1;
        yScale *= -1;

        if (Vector2.Distance(origin, position) <= 240)
        {
            TilePref GO = WorldGeneration.gameObjectsThatShouldRender.Find(g => Raylib.CheckCollisionPointRec(position, g.rectangle) && g.tag != "BackgroundTile");

            if (GO != null && inventory.currentActiveItem != null)
            {
                Item item = GO.dropType;

                GO.HP -= inventory.currentActiveItem.itemDamage;

                if (GO.HP <= 0)
                {
                    //inventory.AddToInventory(item, item.dropAmount);
                    if (item != null)
                    {
                        Vector2 itemPos = GO.position + new Vector2(item.texture.Width / 2, item.texture.Height / 2);
                        ItemEntity itemEntity = new ItemEntity(itemPos, item);
                        ItemManager.itemsOnGround.Add(itemEntity);
                        Game.entities.Add(itemEntity);
                    }

                    Game.gameObjectsToDestroy.Add(GO);
                    WorldGeneration.gameObjectsInWorld.Remove(GO);
                    WorldGeneration.tilemap[(int)position.X / 80, (int)position.Y / 80] = null;
                }
            }
        }
    }
}


public enum RotationDirection
{
    up, down
}
