

public class PlacementSystem : GameSystem
{
    public static Vector2 WorldToTile(Vector2 worldPosition, int cellSize)
    {
        int column = (int)worldPosition.X / cellSize;
        int row = (int)worldPosition.Y / cellSize;

        return new Vector2(column, row);
    }



    public override void Update()
    {

    }


    public void PlaceTile(Player player, WorldGeneration worldGeneration, IPlaceable placeableItem)
    {

        Vector2 pos = WorldToTile(Raylib.GetScreenToWorld2D(Raylib.GetMousePosition(), Game.camera), 80);

        int posX = (int)pos.X;
        int posY = (int)pos.Y;

        if (posX >= 0 && posX < WorldGeneration.tilemap.GetLength(0) && posY >= 0 && posY < WorldGeneration.tilemap.GetLength(1))
        {
            TilePref currentTile = WorldGeneration.tilemap[posX, posY];
            Console.WriteLine(currentTile);

            bool cantPlace = WorldGeneration.tilemap[posX, posY + 1] == null && WorldGeneration.tilemap[posX + 1, posY] == null && WorldGeneration.tilemap[posX - 1, posY] == null && WorldGeneration.tilemap[posX, posY - 1] == null;

            if ((currentTile == null || currentTile.tag == "BackgroundTile") && !cantPlace && !Raylib.CheckCollisionPointRec(Raylib.GetScreenToWorld2D(Raylib.GetMousePosition(), Game.camera), player.collider.boxCollider))
            {
                TilePref tile = placeableItem.TilePrefToPlace(pos * 80);
                worldGeneration.SpawnTilePrefab(tile);
                WorldGeneration.tilemap[posX, posY] = tile;
                player.inventory.itemsInInventory[player.inventory.currentActiveItem]--;
            }
        }

    }
}