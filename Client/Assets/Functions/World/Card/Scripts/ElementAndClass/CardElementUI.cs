using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.UI;

namespace World.TheCard
{
    public class CardElementUI : MonoBehaviour
    {
        [SerializeField] SerializedDictionary<ElementType, Sprite> _sprites = new SerializedDictionary<ElementType, Sprite>();
        [SerializeField] private Image _image;
        public void Init(ElementType elementType)
        {
            _image.sprite = _sprites[elementType];
        }
    }
}
