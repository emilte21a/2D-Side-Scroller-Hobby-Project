global using Raylib_cs;
global using System.Numerics;
global using System.Collections.Generic;
global using System.Collections.Concurrent;
global using System.Linq;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Specialized;
using Raylib_CsLo;
using Raylib = Raylib_cs.Raylib;
using Color = Raylib_cs.Color;
using KeyboardKey = Raylib_cs.KeyboardKey;
using Camera2D = Raylib_cs.Camera2D;
using System.Formats.Asn1;
using Rectangle = Raylib_cs.Rectangle;

public class Game
{
    public static int ScreenWidth = 1600;
    public static int ScreenHeight = 1024;

    //Klass-instanser
    SceneHandler sceneHandler;
    Player player;
    WorldGeneration worldGeneration;
    ParallaxManager parallaxManager;
    DayNightSystem dayNightSystem;
    GUIcontroller gUIcontroller;
    LightingSystem lightingSystem;

    public static Camera2D camera;

    List<IDrawable> drawables;
    List<GameSystem> gameSystems;

    public static List<Entity> entities;

    //public static List<GameObject> gameObjects; //Tom för nu

    public ObservableCollection<GameObject> gameObjects;
    public static List<GameObject> gameObjectsToDestroy;



    //Multithreading/Task
    Task updateDayNightCycle;

    public Game()
    {
        Raylib.InitWindow(ScreenWidth, ScreenHeight, "game");
        Raylib.SetExitKey(KeyboardKey.Null);
        Raylib.InitAudioDevice();
        gameObjectsToDestroy = new List<GameObject>();
        InitializeInstances();
        lightingSystem.InstantiateLightMap(WorldGeneration.tilemap); // Måste köras efter initwindow

        // gameObjects = new ObservableCollection<GameObject>(WorldGeneration.gameObjectsInWorld);


        drawables = new List<IDrawable>();
        drawables.Add(worldGeneration);
        drawables.Add(player);
    }

    private void InitializeInstances()
    {
        gameSystems = new List<GameSystem>();
        gameSystems.Add(new PhysicsSystem());
        gameSystems.Add(new CollisionSystem());
        gameSystems.Add(new AudioSystem());

        camera = new()
        {
            Target = new Vector2(0, 0),
            Offset = new Vector2(ScreenWidth / 2, ScreenHeight / 2 + 60),
            Zoom = 0.9f
        };
        sceneHandler = new();
        dayNightSystem = new DayNightSystem();
        gUIcontroller = new GUIcontroller();
        worldGeneration = new WorldGeneration();
        player = new Player() { camera = camera };
        parallaxManager = new ParallaxManager();
        lightingSystem = new LightingSystem();

        worldGeneration.GenerateWorld();


        entities = new();
        entities.Add(player);
    }

    public void Run()
    {
        //SpawnManager.SpawnEntityAt(player, new Vector2(WorldGeneration.spawnPoints[100].X, WorldGeneration.spawnPoints[100].Y - player.collider.boxCollider.Height));

        while (!Raylib.WindowShouldClose())
        {
            Update();
            Draw();
        }
        Raylib.CloseWindow();
    }

    bool paused = false;

    private void Update()
    {
        if (sceneHandler.currentScene == SceneHandler.CurrentSceneState.Start)
        {

        }
        else if (sceneHandler.currentScene == SceneHandler.CurrentSceneState.Game && !paused)
        {

            updateDayNightCycle = new Task(dayNightSystem.Update);
            updateDayNightCycle.Start();

            if (Raylib.IsKeyPressed(KeyboardKey.Z))
                camera.Zoom -= 0.05f;

            else if (Raylib.IsKeyPressed(KeyboardKey.X))
                camera.Zoom += 0.05f;

            gameSystems[0].Update(); //Fysik
            player.Update(); //Spelaren
            gameSystems[1].Update(); //Kollisioner

            camera.Target = Raymath.Vector2Lerp(camera.Target, player.position, 0.6f);
            parallaxManager.Update(player);

            if (Raylib.IsMouseButtonPressed(Raylib_cs.MouseButton.Right) && player.inventory.currentActiveItem is IPlaceable && player.inventory.currentActiveItem != null)
            {
                Vector2 pos = PlacementSystem.WorldToTile(Raylib.GetScreenToWorld2D(Raylib.GetMousePosition(), camera), 80);

                int posX = (int)pos.X;
                int posY = (int)pos.Y;

                if (posX >= 0 && posX < WorldGeneration.tilemap.GetLength(0) && posY >= 0 && posY < WorldGeneration.tilemap.GetLength(1))
                {
                    TilePref currentTile = WorldGeneration.tilemap[posX, posY];
                    System.Console.WriteLine(currentTile);
                    if (currentTile == null || currentTile.tag == "BackgroundTile")
                    {
                        Prefab tile = new Torch(pos * 80);
                        worldGeneration.SpawnTilePrefab(tile);
                        WorldGeneration.tilemap[posX, posY] = tile;
                        LightingSystem.shouldUpdateLightingSystem = true;
                    }
                }
            }

            if (Raylib.IsKeyPressed(KeyboardKey.O))
            {
                WorldGeneration.gameObjectsInWorld.Clear();
                worldGeneration.GenerateWorld();
            }

            if (Raylib.IsKeyPressed(KeyboardKey.Down))
            {
                System.Console.WriteLine("");
                System.Console.WriteLine(GC.GetTotalMemory(true));
            }

            if (LightingSystem.shouldUpdateLightingSystem)
            {
                lightingSystem.InstantiateLightMap(WorldGeneration.tilemap);
                LightingSystem.shouldUpdateLightingSystem = false;
            }
        }
        else if (sceneHandler.currentScene == SceneHandler.CurrentSceneState.Gameover)
            System.Console.WriteLine("Gameover");
    }

    private void Draw()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.SkyBlue);
        if (sceneHandler.currentScene == SceneHandler.CurrentSceneState.Start)
        {
            bool pressed = RayGui.GuiButton(new Raylib_CsLo.Rectangle(ScreenWidth / 2 - 100, ScreenHeight / 2 - 40, 200, 80), "Press to play");
            RayGui.GuiTextInputBox(new Raylib_CsLo.Rectangle(ScreenWidth / 2 - 100, ScreenHeight / 2 - 40 + 400, 200, 80), "Input seed", $"{Raylib.GetKeyPressed()}", "", "", 15);



            if (pressed) sceneHandler.ChangeScene(SceneHandler.CurrentSceneState.Game);
        }

        else if (sceneHandler.currentScene == SceneHandler.CurrentSceneState.Game)
        {
            lightingSystem.UpdateLightmap(player.position);
            dayNightSystem.Draw();
            parallaxManager.Draw();
            Raylib.BeginMode2D(camera);
            drawables.ForEach(d => d.Draw());
            Raylib.DrawTextureEx(lightingSystem.lightMapTexture.Texture, new Vector2(0, 0), 0, 80, Color.White);
            // entities.ForEach(e => Raylib.DrawRectangleRec(e.GetComponent<Collider>().boxCollider, new Color(0, 255, 50, 100)));
            Raylib.EndMode2D();
            dayNightSystem.DrawNightOverlay();
            player.inventory.Draw();
            gUIcontroller.Draw(player.healthPoints);
            Raylib.DrawText($"Pos: {player.position}", 20, 60, 30, Color.White);
            Raylib.DrawText($"{player.lastDirection}", ScreenWidth - 100, 120, 30, Color.White);
            Raylib.DrawText($"{InputManager.GetAxisX()}", 20, 90, 30, Color.White);
            Raylib.DrawText($"Vel: {player.physicsBody.velocity}", 20, 120, 30, Color.White);
            Raylib.DrawText($"Accl: {player.physicsBody.acceleration}", 20, 150, 30, Color.White);
            Raylib.DrawText($"{player.physicsBody.airState}", 20, 180, 30, Color.White);
            Raylib.DrawText($"{camera.Zoom}", 20, 210, 30, Color.White);
            Raylib.DrawText($"Time: {dayNightSystem.currentTime}", 20, 240, 30, Color.White);
            Raylib.DrawText($"rotDir: {player.playerAction.rotationDirection}", 20, 300, 30, Color.White);
            Raylib.DrawText($"rot: {player.playerAction.rotation}", 20, 360, 30, Color.White);

            Raylib.DrawFPS(20, 20);

            if (Raylib.IsKeyPressed(KeyboardKey.Escape))
            {
                paused = true;
            }
            if (paused)
            {
                RayGui.GuiLabel(new Raylib_CsLo.Rectangle(ScreenWidth / 2 - 400, ScreenHeight / 2 - 400, 800, 800), "settings");
                bool resume = RayGui.GuiButton(new Raylib_CsLo.Rectangle(ScreenWidth / 2 - 200, ScreenHeight / 2 - 200, 400, 100), "Resume");
                bool close = RayGui.GuiButton(new Raylib_CsLo.Rectangle(ScreenWidth / 2 - 200, ScreenHeight / 2 - 50, 400, 100), "Exit");

                if (resume)
                    paused = false;

                if (close)
                    Raylib.CloseWindow();
            }

        }
        Raylib.EndDrawing();
    }
}
