

public abstract class Entity : GameObject
{
    public Rectangle rectangle;

    public Vector2 lastDirection;

    public Vector2 position
    {
        get => new Vector2(rectangle.X, rectangle.Y);
        set
        {
            rectangle.X = value.X;
            rectangle.Y = value.Y;
        }

    }
    public virtual void Update() { }
    public virtual void Draw() { }

}
