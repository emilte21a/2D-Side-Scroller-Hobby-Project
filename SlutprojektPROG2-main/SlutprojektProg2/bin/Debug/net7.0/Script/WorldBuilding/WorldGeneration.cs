

public class WorldGeneration : IDrawable
{
    private ushort _worldSize = 80; //Världsstorlek
    private ushort _tileSize = 80; //Varje tiles storlek

    private ushort _chunkSize = 16;

    #region  Listor för alla tiles och prefabs i världen
    public static List<TilePref> gameObjectsInWorld = new List<TilePref>();
    public static List<TilePref> gameObjectsThatShouldRender = new List<TilePref>();

    public List<Chunk> chunks = new List<Chunk>();

    #endregion

    #region Variabler som bestämmer hur världen ser ut
    private short _seed;
    private ushort _caveThreshold = 160;
    private float _surfaceThreshold = 0.5f;
    private ushort _heightMultiplier = 4;

    #endregion

    public static Vector2[] spawnPoints; //Varje möjliga plats att spawna en entity på
    public static string[] tileOccupation; //En array där man kan se ifall en position på ytan ockuperas av en prefab
    public static TilePref[,] tilemap;

    public WorldGeneration()
    {
        spawnPoints = new Vector2[_worldSize];
        tileOccupation = new string[spawnPoints.Length];
        tilemap = new TilePref[_worldSize, _worldSize];
        _seed = (short)Random.Shared.Next(-10000, 10000); //Skapa nytt seed varje gång man startar spelet

        // for (int x = 0; x < _chunkSize * 3; x += _chunkSize)
        // {
        //     chunks.Add(new Chunk(new Vector2(x, 0), _chunkSize, 100));
        // }
    }

    //Metod som procedurellt genererar en ny värld varje gång man startar spelet.
    private void GenerateTiles()
    {
        Image heightImage = Raylib.GenImagePerlinNoise(_worldSize, _worldSize, _seed, _seed, 1f); //En perlinnoise bild som bestämmer hur "len" ytan ska vara. Desto större scale, desto "galnare"
        Image noiseImage = Raylib.GenImagePerlinNoise(_worldSize, _worldSize, _seed, _seed, 10f); //Bestämmer hur grottorna ska se ut.



        for (int x = 0; x < noiseImage.Width; x++)
        {
            int height = Raylib.GetImageColor(heightImage, (int)(x * _surfaceThreshold), (int)_surfaceThreshold).R / _heightMultiplier;
            //Hämta en höjd för varje x värde i världen genom att ta det röda värdet på varje x pixel.

            for (int y = height; y < _worldSize; y++)
            {
                SpawnTilePrefab(new BackgroundTile(new Vector2(x * _tileSize, y * _tileSize))); //Background tile på varje plats i världen

                if (y == height)
                {
                    spawnPoints[x] = new Vector2(x * _tileSize, y * _tileSize);

                }

                if (Raylib.GetImageColor(noiseImage, x, y).R < _caveThreshold) //Placera endast ut ett block om det röda värdet på varje pixel på noiseimage är mindre än en threshold
                {
                    if (y == height) //Spawna gräs
                    {
                        GrassTile grassTile = new GrassTile(new Vector2(x * _tileSize, y * _tileSize));
                        SpawnTilePrefab(grassTile);
                        tilemap[x, y] = grassTile;
                    }

                    else if (y == height + 1) //Spawna jord under gräs
                    {
                        DirtTile dirtTile = new DirtTile(new Vector2(x * _tileSize, y * _tileSize));
                        SpawnTilePrefab(dirtTile);
                        tilemap[x, y] = dirtTile;
                    }

                    else //Annars spawna sten
                    {
                        StoneTile stoneTile = new StoneTile(new Vector2(x * _tileSize, y * _tileSize));
                        SpawnTilePrefab(stoneTile);
                        tilemap[x, y] = stoneTile;
                    }
                }
                else //Gör varje plats där det inte finns en tile till tom på tilemap. Detta används i lightingsystem
                    tilemap[x, y] = null;
            }
        }

        // Ladda ur båda bilderna från CPU för att frigöra minne

        // foreach (var chunk in chunks)
        // {
        //     for (int x = 0; x < _chunkSize; x++)
        //     {
        //         for (int y = 0; y < _worldSize; y++)
        //         {
        //             int worldX = (int)chunk.position.X + x;
        //             int worldY = (int)chunk.position.Y + y;

        //             int height = Raylib.GetImageColor(heightImage, x, 0).R * _heightMultiplier;

        //             for (int h = height; h < _worldSize; h++)
        //             {
        //                 // Spawn tiles within the chunk
        //                 if (Raylib.GetImageColor(heightImage, x, h).R < _caveThreshold)
        //                 {
        //                     if (h == height) // Grass
        //                     {
        //                         GrassTile grassTile = new GrassTile(new Vector2(x * _tileSize, h));
        //                         chunk.tilesInChunk[x, y] = grassTile;
        //                         SpawnTilePrefab(grassTile);
        //                     }
        //                     else if (h == height + 1) // Dirt
        //                     {
        //                         DirtTile dirtTile = new DirtTile(new Vector2(x * _tileSize, h));
        //                         chunk.tilesInChunk[x, y] = dirtTile;
        //                         SpawnTilePrefab(dirtTile);
        //                     }
        //                     else // Stone
        //                     {
        //                         StoneTile stoneTile = new StoneTile(new Vector2(x * _tileSize, h));
        //                         chunk.tilesInChunk[x, y] = stoneTile;
        //                         SpawnTilePrefab(stoneTile);
        //                     }
        //                 }
        //             }
        //         }
        //     }
        // }


        Raylib.UnloadImage(noiseImage);
        Raylib.UnloadImage(heightImage);
    }

    public void SpawnTilePrefab(TilePref tilePref)
    {
        gameObjectsInWorld.Add(tilePref);
    }

    //Metod som ritar ut varje tile och prefab som finns i världen
    public void Draw()
    {
        gameObjectsThatShouldRender.ForEach(TP => Raylib.DrawTexture(TP.renderer.sprite, (int)TP.position.X, (int)TP.position.Y, Color.White));
        // gameObjectsInWorld.ForEach(TP => Raylib.DrawTexture(TP.renderer.sprite, (int)TP.position.X, (int)TP.position.Y, Color.White));
    }

    public TilePref GetTileAt(Vector2 position)
    {
        if (position.X / _tileSize >= 0 && position.X / _tileSize <= tilemap.GetLength(0) && position.Y / _tileSize >= 0 && position.Y / _tileSize <= tilemap.GetLength(1))
        {
            return tilemap[(int)position.X / _tileSize, (int)position.Y / _tileSize];
        }
        return null;
    }

    public Vector2 WorldPositionToTilePosition(Vector2 position)
    {
        if (position.X / _tileSize >= 0 && position.X / _tileSize <= tilemap.GetLength(0) && position.Y / _tileSize >= 0 && position.Y / _tileSize <= tilemap.GetLength(1))
            return new Vector2((int)position.X / _tileSize, (int)position.Y / _tileSize);

        return Vector2.Zero;
    }
    //Metod som genererar prefabs
    private void GeneratePrefabs()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            tileOccupation[i] = "Empty";
            if (Random.Shared.Next(0, 10) == 1 && tileOccupation[i] == "Empty" && spawnPoints[i].Y != 0)
            {
                TilePref tree = new Tree(new Vector2(i * _tileSize - _tileSize, spawnPoints[i].Y));
                SpawnTilePrefab(tree);
                tileOccupation[i] = "Tree";
                tilemap[(int)spawnPoints[i].X / _tileSize, (int)spawnPoints[i].Y / _tileSize] = tree;
            }

            else if (Random.Shared.Next(0, 10) == 1 && tileOccupation[i] == "Empty" && spawnPoints[i].Y != 0)
            {
                TilePref rock = new Rock(new Vector2(i * _tileSize, spawnPoints[i].Y));
                SpawnTilePrefab(rock);
                tileOccupation[i] = "Rock";
                tilemap[(int)spawnPoints[i].X / _tileSize, (int)spawnPoints[i].Y / _tileSize] = rock;
            }
        }
    }

    /*
        För varje position som är giltid på världen
            Gör alla till tomma

            10% att spawna ett träd på varje tile om den är tom 
                Spawna ett träd på den positionen och gör att den platsen ockuperas av ett träd

            10% att spawna en sten på varje tile om den är tom 
                spawna en sten på den postitionen och gör att den platsen ockuperas av en sten
    */

    public void GenerateWorld()
    {
        GenerateTiles();
        GeneratePrefabs();
    }
}

public class Chunk
{
    public Tile[,] tilesInChunk { get; private set; }
    public Vector2 position { get; private set; }

    public Chunk(Vector2 pos, int xSize, int ySize)
    {
        position = pos;
        tilesInChunk = new Tile[xSize, ySize];
    }
}

public class SpawnManager
{
    public static void SpawnEntityAt(Entity entity, Vector2 position)
    {
        entity.position = position;
    }
}