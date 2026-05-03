using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{

    public TileBase tile;
    public ItemType itemType;
    public ActionType actionType;
    public Vector2Int range = new(5, 4);

    public bool stackable = true;

    public Sprite image;
    public int maxStackCount = 1;

    [Header("Weapon Hand Type")]
    public WeaponHandType weaponHandType = WeaponHandType.None;
}

public enum ItemType
{
    Weapon,
    Armour,
    Consumable,

    //Specific Equipment
    Helmet,
    BodyArmour,
    Gloves,
    Leggings,
    Boots,
    Ring,
    Amulet,
    RightHanded,
    LeftHanded
}

public enum WeaponHandType
{
    None,
    OneHanded,
    TwoHanded,
    ShieldOrQuiver
}

public enum ActionType
{
    Equippable,
    Consumable
}
