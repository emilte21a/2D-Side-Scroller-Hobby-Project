public class CraftingSystem
{
    public static CraftingSystem current;
    public List<Item> craftables;

    private CraftingSystem()
    {
        current = new CraftingSystem();
        craftables = new List<Item>() { new StickItem(), new StoneAxe(), new WoodPickaxe() };
    }
}