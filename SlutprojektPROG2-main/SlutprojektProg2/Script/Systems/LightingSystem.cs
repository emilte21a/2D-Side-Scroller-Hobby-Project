public class LightingSystem
{
    private int[,] lightMap; //En array av ints som bestämmer ljusnivån på varje position
    private Image _lightMapImage; //En bild som man ritar på för att visa rätt värden
    public RenderTexture2D lightMapTexture; //En texture som man ritar ut som overlay över världen

    int minLight = 5;
    int maxLight = 15;
    float decayFactor = 0.9f;

    private void InitializeLightmap(TilePref[,] tileMap)
    {
        int width = tileMap.GetLength(0);
        int height = tileMap.GetLength(1);
        lightMap = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (tileMap[x, y] is ILightSource && IsWithinBounds(x, y, width, height))
                {
                    SetLightLevel(x, y, maxLight);
                }
                else if (tileMap[x, y] == null && y < height - 2 && y * 80 <= WorldGeneration.spawnPoints[x].Y)
                {
                    SetLightLevel(x, y, maxLight);
                }
                else if (IsWithinBounds(x, y, width, height))
                {
                    float newLightLevel = decayFactor * (
                        GetLightLevel(x - 1, y) +
                        GetLightLevel(x + 1, y) +
                        GetLightLevel(x, y - 1) +
                        GetLightLevel(x, y + 1)
                    ) / 4;
                    lightMap[x, y] = (int)Raymath.Clamp(newLightLevel, minLight, maxLight);
                }
            }
        }
    }

    private Image CreateLightMapImage()
    {
        int width = lightMap.GetLength(0);
        int height = lightMap.GetLength(1);
        Image img = Raylib.GenImageColor(width, height, Color.Blank);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int lightLevel = lightMap[x, y];
                Color pixelColor = new Color(0, 0, 0, 255 - lightLevel * 17);
                Raylib.ImageDrawPixel(ref img, x, y, pixelColor);
            }
        }
        return img;
    }

    public void InstantiateLightMap()
    {
        InitializeLightmap(WorldGeneration.tilemap);
        _lightMapImage = CreateLightMapImage();

        Raylib.ImageFlipVertical(ref _lightMapImage);

        lightMapTexture = Raylib.LoadRenderTexture(_lightMapImage.Width, _lightMapImage.Height);
        Raylib.SetTextureFilter(lightMapTexture.Texture, TextureFilter.Bilinear);

        Raylib.BeginTextureMode(lightMapTexture);
        Raylib.DrawTexture(Raylib.LoadTextureFromImage(_lightMapImage), 0, 0, Color.White);
        Raylib.EndTextureMode();

        Raylib.UnloadImage(_lightMapImage); 
    }

    public void UpdateLightmap(Vector2 pos)
    {
        InitializeLightmap(WorldGeneration.tilemap);
        Raylib.BeginTextureMode(lightMapTexture);
        Raylib.DrawCircleGradient((int)pos.X, (int)pos.Y, 100, Color.White, new Color(255, 255, 255, 0));
        Raylib.EndTextureMode();
    }

    private bool IsWithinBounds(int x, int y, int width, int height)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    private int GetLightLevel(int x, int y)
    {
        if (x < 0 || y < 0 || x >= lightMap.GetLength(0) || y >= lightMap.GetLength(1))
            return minLight;
        return lightMap[x, y];
    }

    private void SetLightLevel(int x, int y, int level)
    {
        if (x >= 0 && y >= 0 && x < lightMap.GetLength(0) && y < lightMap.GetLength(1))
        {
            lightMap[x, y] = level;

            if (level == maxLight)
            {
                SpreadLight(x, y);
            }
        }
    }

    private void SpreadLight(int x, int y)
    {
        if (x > 0) lightMap[x - 1, y] = maxLight - 1;
        if (x < lightMap.GetLength(0) - 1) lightMap[x + 1, y] = maxLight - 1;
        if (y > 0) lightMap[x, y - 1] = maxLight - 1;
        if (y < lightMap.GetLength(1) - 1) lightMap[x, y + 1] = maxLight - 1;
    }

    public void UpdateRegion(Rectangle area)
    {
        int startX = (int)area.X;
        int startY = (int)area.Y;
        int endX = startX + (int)area.Width;
        int endY = startY + (int)area.Height;

        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                if (IsWithinBounds(x, y, lightMap.GetLength(0), lightMap.GetLength(1)))
                {
                    float newLightLevel = decayFactor * (
                        GetLightLevel(x - 1, y) +
                        GetLightLevel(x + 1, y) +
                        GetLightLevel(x, y - 1) +
                        GetLightLevel(x, y + 1)
                    ) / 4;
                    lightMap[x, y] = (int)Raymath.Clamp(newLightLevel, minLight, maxLight);
                }
            }
        }
    }
}