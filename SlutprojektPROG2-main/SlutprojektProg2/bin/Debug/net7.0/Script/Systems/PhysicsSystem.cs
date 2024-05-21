public class PhysicsSystem : GameSystem
{
    private float terminalVelocity = 50;
    public override void Update()
    {
        foreach (Entity e in Game.entities)
        {
            PhysicsBody? physicsBody = e.GetComponent<PhysicsBody>();

            if (physicsBody != null && physicsBody.UseGravity == PhysicsBody.Gravity.enabled)
            {
                //Lägg till acceleration
                physicsBody.acceleration += physicsBody.gravity * Raylib.GetFrameTime() * 4;

                if (physicsBody.airState != AirState.grounded)
                {
                    //Updatera velociteten  
                    physicsBody.velocity.Y += physicsBody.acceleration.Y * Raylib.GetFrameTime() * 10;
                }

                //Clampa maxhastigheten 
                physicsBody.velocity.Y = Raymath.Clamp(physicsBody.velocity.Y, -terminalVelocity, terminalVelocity);

                //Uppdatera positionen
                e.position += physicsBody.velocity * Raylib.GetFrameTime() * 100;

                //Återställ accelerationen
                physicsBody.acceleration = Vector2.Zero;
            }
        }
    }
}