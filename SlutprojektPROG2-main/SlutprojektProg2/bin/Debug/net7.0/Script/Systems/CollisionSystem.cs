

public class CollisionSystem : GameSystem
{
    public override void Update()
    {
        foreach (Entity e in Game.entities)
        {
            #region Hämta potentiella komponenter hos entitites
            PhysicsBody? physicsBody = e.GetComponent<PhysicsBody>();
            Collider? collider = e.GetComponent<Collider>();
            #endregion

            List<TilePref> tilesCloseToEntity; //Lista med alla rektanglar som screenRectangle kolliderar med
            List<TilePref> floorCollision;

            #region Rektanglar som kollar spelarens kollisioner
            Rectangle ScreenRectangle = new Rectangle(e.position.X - Game.ScreenWidth / 2 - 100, e.position.Y - Game.ScreenHeight / 2 - 150, Game.ScreenWidth + 200, Game.ScreenWidth + 300); //Magiska nummer för offset
            Rectangle floorCollider = new Rectangle(e.position.X, e.position.Y + collider.boxCollider.Height, collider.boxCollider.Width, 2);
            #endregion

            if (collider != null && physicsBody != null) //Kolla så att entityns physicsbody och collider inte är null
            {
                tilesCloseToEntity = WorldGeneration.gameObjectsInWorld.Where(tile => Raylib.CheckCollisionRecs(ScreenRectangle, tile.rectangle)).ToList();
                floorCollision = WorldGeneration.gameObjectsInWorld.Where(tile => Raylib.CheckCollisionRecs(tile.rectangle, floorCollider) && tile.tag == "Tile").ToList();

                if (e.tag == "Player")
                    WorldGeneration.gameObjectsThatShouldRender = tilesCloseToEntity;

                #region bestäm senaste riktningen samt om entityn nuddar marken eller ej
                if (physicsBody.velocity.X > 0) { e.lastDirection.X = 1; }

                else if (physicsBody.velocity.X < 0) { e.lastDirection.X = -1; }

                if (physicsBody.velocity.Y > 0) { e.lastDirection.Y = 1; }

                else if (physicsBody.velocity.Y < 0) { e.lastDirection.Y = -1; }

                physicsBody.airState = floorCollision.Count > 0 ? AirState.grounded : AirState.inAir;
                #endregion

                #region Korrigera y positionen 
                foreach (TilePref tilePref in tilesCloseToEntity)
                {
                    //Räkna ut spelarens y-position i nästa frame med en rektangel
                    Rectangle nextBounds = new Rectangle(e.position.X, e.position.Y + physicsBody.velocity.Y, collider.boxCollider.Width, collider.boxCollider.Height);
                    Rectangle collisionRectangle = Raylib.GetCollisionRec(tilePref.rectangle, nextBounds);
                    
                    if (collisionRectangle.Width > collisionRectangle.Height && tilePref.tag == "Tile")
                    {
                        if (physicsBody.velocity.Y > 0 || e.lastDirection.Y == 1)
                        {
                            //Korrigera Y hastigheten ifall spelaren faller
                            e.position = new Vector2(e.position.X, tilePref.rectangle.Y - collider.boxCollider.Height);
                            physicsBody.velocity.Y = 0;
                        }
                        else if (physicsBody.velocity.Y < 0 || e.lastDirection.Y == -1)
                        {
                            //Korrigera Y hastigheten ifall spelaren hoppar
                            e.position = new Vector2(e.position.X, tilePref.rectangle.Y + tilePref.rectangle.Height);
                            physicsBody.velocity.Y = 0;
                        }
                    }
                }
                #endregion

                #region Korrigera x positionen
                foreach (TilePref tilePref in tilesCloseToEntity)
                {
                    //Räkna ut spelarens x-position i nästa frame med en rektangel
                    Rectangle nextBounds = new Rectangle(e.position.X + physicsBody.velocity.X, e.position.Y, collider.boxCollider.Width, collider.boxCollider.Height);

                    if (Raylib.CheckCollisionRecs(nextBounds, tilePref.rectangle) && tilePref.tag == "Tile") //Kolla kollisioner mellan alla tiles som är inom ett visst område av spelaren och spelarens rektangel i nästa frame
                    {
                        if (physicsBody.velocity.X > 0 || e.lastDirection.X == 1)
                        {
                            //Korrigera spelarens hastighet åt höger när den kolliderar
                            e.position = new Vector2(tilePref.rectangle.X - collider.boxCollider.Width, e.position.Y);
                            physicsBody.velocity.X = 0;
                        }

                        else if (physicsBody.velocity.X < 0 || e.lastDirection.X == -1)
                        {
                            //Korrigera spelarens hastighet åt vänster när den kolliderar
                            e.position = new Vector2(tilePref.rectangle.X + tilePref.rectangle.Width, e.position.Y);
                            physicsBody.velocity.X = 0;
                        }
                    }
                }
                #endregion
            }
        }
    }
}