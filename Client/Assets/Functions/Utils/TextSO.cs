using UnityEngine;

namespace Utils
{
    [CreateAssetMenu(fileName = "TextSO", menuName = "TextSO")]
    public class TextSO : ScriptableObject
    {
        [TextArea(10, 50)] public string Content;
    }
}