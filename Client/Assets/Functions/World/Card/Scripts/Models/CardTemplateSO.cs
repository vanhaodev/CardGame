using UnityEngine;
using UnityEngine.Serialization;

namespace World.Card
{
    [CreateAssetMenu(fileName = "CardTemplateSO", menuName = "World/Card/CardTemplateSO")]
    public class CardTemplateSO : ScriptableObject
    {
        public string Name;
        [TextArea(3, 30)]
        public string History;
        /// <summary>
        /// Skin auto change by star level
        /// </summary>
        public Sprite[] StarSkins;
        /// <summary>
        /// Custom skin when player equipped
        /// </summary>
        public CardSkinModel[] CustomSkins;
        [FormerlySerializedAs("Infor")] public CardTemplateModel Info;
    }
}
