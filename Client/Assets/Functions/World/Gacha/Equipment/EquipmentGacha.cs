using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Functions.World.Gacha
{
    public class EquipmentGacha : CardGacha
    {
        [SerializeField] private Image _imageChest;

        public void SetChestSprite(Sprite sprite)
        {
            _imageChest.sprite = sprite;
        }
    }
}