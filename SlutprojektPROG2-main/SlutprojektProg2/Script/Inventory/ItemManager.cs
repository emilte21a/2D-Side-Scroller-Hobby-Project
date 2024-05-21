public class ItemManager
{
    public static T EnumToItem<T>(Item itemType)
    {
        Type enumType = typeof(T);

        T value = (T)Enum.ToObject(enumType, itemType);
        if (Enum.IsDefined(enumType, value) == false)
        {
            throw new NotSupportedException("Unable to convert value from database to the type: " + enumType.ToString());
        }
        return (T)value;
    }

    // public Enum ItemToEnum(Item itemType)
    // {

    //     Enum @enum = Enum.TryParse(itemType.name, out ItemType)

    //     itemTypes.Add(@enum);
    // }
}
public enum ItemType
{
    wood, stone, dirt, grass, stick, woodenPickaxe, stoneAxe
}