using UnityEngine;

public class ItemSpriteManager : MonoBehaviour
{
    public Sprite WoodenStropSprite;
    public Sprite MetalStropSprite;
    public Sprite TorchSprite;
    public Sprite BulbSprite;
    public Sprite Bulbitemsprite;
    public Sprite TracksSprite;

    public Sprite CoalSprite;
    public Sprite WoodSprite;
    public Sprite FuelSprite;
    public Sprite MetalSprite;
    
    public Sprite GetSpriteByItemType(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.WoodenStrop:
                return WoodenStropSprite;
            case ItemType.MetalStrop:
                return MetalStropSprite;
            case ItemType.Torch:
                return TorchSprite;
            case ItemType.Bulb:
                return BulbSprite;
            case ItemType.Tracks:
                return TracksSprite;
            case ItemType.Coal:
                return CoalSprite;
            case ItemType.Wood:
                return WoodSprite;
            case ItemType.Fuel:
                return FuelSprite;
            case ItemType.Metal:
                return MetalSprite;
            default:
                return null;
        }
    }

}
