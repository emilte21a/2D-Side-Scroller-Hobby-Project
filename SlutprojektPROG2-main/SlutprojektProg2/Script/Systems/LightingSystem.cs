public class LightingSystem
{
    public static bool shouldUpdateLightingSystem = false;
    private int[,] lightMap; //En array av ints som bestämmer ljusnivån på varje position
    private Image _lightMapImage; //En bild som man ritar på för att visa rätt värden
    public RenderTexture2D lightMapTexture; //En texture som man ritar ut som overlay över världen

    private void InitializeLightmap(TilePref[,] tileMap)
    {
        int width = tileMap.GetLength(0);
        int height = tileMap.GetLength(1);

        lightMap = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //|| tileMap[x, y].tag == "BackgroundTile"
                if ((tileMap[x, y] == null) && y < height - 2 && y * 80 <= WorldGeneration.spawnPoints[x].Y)
                {
                    //Om det inte finns en t §ile på x och y och om y värdet är mindre än höjden -1
                    //Gör denna positionen ljusnivå till 15
                    lightMap[x, y] = 15;
                    lightMap[x, y + 1] = 15;
                    lightMap[x, y + 2] = 15;
                }
                else if (tileMap[x, y] is ILightSource)
                    lightMap[x, y] = 15;
                else
                {
                    if (y > 1 && y < height - 1 && x > 0 && x < width - 1)
                    {
                        float newLightLevel = (lightMap[x, y - 1] + lightMap[x, y + 1] + lightMap[x - 1, y] + lightMap[x + 1, y]) / 3;
                        // lightMap[x, y] = (int)Raymath.Clamp(lightMap[x, y - 1] - 1, 5, 15);
                        lightMap[x, y] = (int)Raymath.Clamp(newLightLevel, 5, 15);
                    }
                    if ((x == 0 || x == width - 1) && y < height - 1)
                        lightMap[x, y] = (int)Raymath.Clamp(lightMap[x, y + 1] - 1, 5, 15);
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
                Color pixelColor = new Color(0, 0, 0, 255 - lightMap[x, y] * 17);
                Raylib.ImageDrawPixel(ref img, x, y, pixelColor);
            }
        }
        return img;
    }

    public void InstantiateLightMap(TilePref[,] tileMap)
    {
        InitializeLightmap(tileMap);
        _lightMapImage = CreateLightMapImage();
        lightMapTexture.Texture = Raylib.LoadTextureFromImage(_lightMapImage);
        Raylib.BeginTextureMode(lightMapTexture);
        Raylib.SetTextureFilter(lightMapTexture.Texture, TextureFilter.Bilinear);
        Raylib.EndTextureMode();
        Raylib.UnloadImage(_lightMapImage);
    }

    public void UpdateLightmap(Vector2 pos)
    {
        Raylib.BeginTextureMode(lightMapTexture);
        Raylib.DrawCircleGradient((int)pos.X, (int)pos.Y, 100, new Color(255, 255, 255, 255), new Color(255, 255, 255, 0));
        Raylib.EndTextureMode();
    }
}