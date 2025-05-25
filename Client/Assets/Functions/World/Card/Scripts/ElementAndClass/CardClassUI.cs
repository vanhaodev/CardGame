using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.UI;

namespace World.TheCard
{
    public class CardClassUI : MonoBehaviour
    {
        [SerializeField] SerializedDictionary<ClassType, Sprite> _sprites = new SerializedDictionary<ClassType, Sprite>();
        [SerializeField] private Image _image;

        public void Init(ClassType classType)
        {
            _image.sprite = _sprites[classType];
        }
    }
}