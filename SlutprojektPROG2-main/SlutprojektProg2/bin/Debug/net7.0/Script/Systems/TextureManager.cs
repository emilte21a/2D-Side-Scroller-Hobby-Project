public static class TextureManager
{
    private static readonly Dictionary<string, Texture2D> textures = new();

    public static Texture2D LoadTexture(string path)
    {
        if (!textures.ContainsKey(path))
            textures[path] = Raylib.LoadTexture(path);
        return textures[path];
    }
}