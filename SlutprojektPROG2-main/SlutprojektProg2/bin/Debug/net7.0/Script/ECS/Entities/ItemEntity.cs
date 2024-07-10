public class ItemEntity : Entity
{
    public Item itemDrop;
    public Texture2D texture;
    Collider collider;
    PhysicsBody physicsBody;

    public ItemEntity(Vector2 pos, Item item)
    {
        itemDrop = item;

        texture = itemDrop.texture;

        rectangle = new Rectangle(0, 0, texture.Width, texture.Height);
        position = pos;
        rectangle.X = position.X;
        rectangle.Y = position.Y;

        components = new List<Component>();
        collider = AddComponent<Collider>();
        physicsBody = AddComponent<PhysicsBody>();

        collider.boxCollider = rectangle;

        physicsBody.UseGravity = PhysicsBody.Gravity.enabled;
    }
}