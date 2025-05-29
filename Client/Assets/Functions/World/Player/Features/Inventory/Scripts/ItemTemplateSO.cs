using Functions.World.Player.Inventory;
using UnityEngine;

namespace World.Player.PopupCharacter
{
    [CreateAssetMenu(fileName = "ItemTemplateSO", menuName = "SO/ItemTemplateSO")]
    public class ItemTemplateSO : ScriptableObject
    {
       [SerializeReference] public ItemTemplateModel Model;
    }
}