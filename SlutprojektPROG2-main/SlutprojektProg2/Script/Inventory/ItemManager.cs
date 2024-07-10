public class ItemManager
{
    public static List<ItemEntity> itemsOnGround = new List<ItemEntity>();

    public static void Draw()
    {
        for (int i = 0; i < itemsOnGround.Count; i++)
        {
            Raylib.DrawTextureV(itemsOnGround[i].texture, itemsOnGround[i].position, Color.White);
        }
    }
}
public enum ItemType
{
    wood, stone, dirt, grass, stick, woodenPickaxe, stoneAxe
}