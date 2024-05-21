

public class PlacementSystem
{
    public static Vector2 WorldToTile(Vector2 worldPosition, int cellSize)
    {
        int column = (int)worldPosition.X / cellSize;
        int row = (int)worldPosition.Y / cellSize;

        return new Vector2(column, row);
    }
}