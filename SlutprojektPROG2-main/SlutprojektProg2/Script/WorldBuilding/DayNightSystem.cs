using System.Globalization;

public class DayNightSystem
{
    public float currentTime;

    public int dayDuration = 24;

    Rectangle skyRectangle = new Rectangle(0, 0, Game.ScreenWidth, Game.ScreenHeight);

    Color dayColor = new Color(135, 206, 235, 255);
    Color nightColor = new Color(0, 8, 20, 255);

    Texture2D sunSprite = Raylib.LoadTexture("Images/sunSprite.png");
    Texture2D moonSprite = Raylib.LoadTexture("Images/moonSprite.png");

    private float _rotation = 0f;

    private int _timePerHour = 10;

    public DayNightSystem()
    {
        currentTime = 6;
    }

    private int _overlayAlpha;

    public void Update()
    {
        currentTime += Raylib.GetFrameTime() / _timePerHour; //Varje "timme" är 10 sekunder

        if (currentTime >= dayDuration)
        {
            currentTime = 0;
        }

        _rotation = currentTime * 15 - 90;

        nightColor.A = (byte)Raymath.Clamp((float)CalculateRotation(currentTime), 0, 255);
        _overlayAlpha = (int)Raymath.Clamp((float)CalculateRotation(currentTime), 0, 180);
    }

    public void Draw()
    {
        Raylib.DrawRectangle(0, 0, Game.ScreenWidth, Game.ScreenHeight, dayColor);
        Raylib.DrawRectangleRec(skyRectangle, nightColor);
        DrawCelestialBodies();
    }

    private void DrawCelestialBodies()
    {
        Raylib.DrawTexturePro(sunSprite,
            new Rectangle(0, 0, sunSprite.Width, sunSprite.Height), // Source rectangle (whole texture)
            new Rectangle(Game.ScreenWidth / 2, Game.ScreenHeight / 2, sunSprite.Width, sunSprite.Height), // Destination rectangle
            new Vector2(600, 250 / 2), // Origin (center of texture)
            _rotation,
            Color.White);

        Raylib.DrawTexturePro(moonSprite,
            new Rectangle(0, 0, sunSprite.Width, sunSprite.Height), // Source rectangle (whole texture)
            new Rectangle(Game.ScreenWidth / 2, Game.ScreenHeight / 2, sunSprite.Width, sunSprite.Height), // Destination rectangle
            new Vector2(600, 250 / 2), // Origin (center of texture)
            _rotation + 180,
            Color.White);
    }

    public static double CalculateRotation(float x)
    {
        return -180 * Math.Cos((x - 12) * Math.PI / 12);
    }

    public void DrawNightOverlay()
    {
        Raylib.DrawRectangle(0, 0, Game.ScreenWidth, Game.ScreenHeight, new Color(nightColor.R, nightColor.G, nightColor.B, _overlayAlpha));
        Raylib.BeginBlendMode(BlendMode.Additive);
        for (int i = 0; i < WorldGeneration.gameObjectsThatShouldRender.Count; i++)
        {
            if (WorldGeneration.gameObjectsThatShouldRender[i] is ILightSource lightSource)
            {
                Vector2 pos = Raylib.GetWorldToScreen2D(new Vector2(WorldGeneration.gameObjectsThatShouldRender[i].position.X, WorldGeneration.gameObjectsThatShouldRender[i].position.Y), Game.camera);
                Raylib.DrawCircleGradient((int)pos.X + 40, (int)pos.Y + 40, 150, new Color(Color.Orange.R, Color.Orange.G, Color.Orange.B, (byte)130), Color.Blank);
            }
        }
        Raylib.EndBlendMode();
    }
}