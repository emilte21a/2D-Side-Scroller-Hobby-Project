
public abstract class Component { }

public class PhysicsBody : Component
{
    #region variabler

    public Vector2 acceleration = Vector2.Zero;

    public Vector2 velocity = Vector2.Zero;

    public Vector2 gravity = new Vector2(0, 50f);

    public Gravity UseGravity;

    public AirState airState;

    #endregion

    public enum Gravity
    {
        enabled,
        disabled
    }

    public void Jump(PhysicsBody physicsBody, float jumpForce)
    {
        physicsBody.velocity.Y = -jumpForce;
    }
}

public enum AirState
{
    inAir,
    grounded
}

public class Collider : Component
{
    public Rectangle boxCollider;
}

public class Renderer : Component
{
    public Texture2D sprite { get; set; }
}

public class AudioPlayer : Component
{
    public Sound audioClip { get; set; }

    public float audioVolume { get; set; }
}

public class Animator : Component
{
    private int _frame;
    private float _timer;
    private int _maxTime = 2;

    public void PlayAnimation(Texture2D spriteSheet, int direction, int maxFrames, Vector2 position)
    {
        _timer += Raylib.GetFrameTime() * 10;

        if (_timer >= _maxTime)
        {
            _timer = 0;
            _frame++;
        }
        _frame %= maxFrames;

        Raylib.DrawTextureRec(spriteSheet, new Rectangle(_frame * spriteSheet.Width / maxFrames, 0, spriteSheet.Width / maxFrames * direction, spriteSheet.Height), position, Color.White);
    }
}