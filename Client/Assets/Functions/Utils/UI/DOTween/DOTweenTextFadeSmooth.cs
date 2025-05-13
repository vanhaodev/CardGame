using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
namespace Utils.UI.DOTween
{
    public class DOTweenTextFadeSmooth 
    {
        // Bảng chữ cái + số + khoảng trắng (có thể thêm ký tự đặc biệt tùy ý)
        private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ";

        public async UniTask AnimateTextChangeAsync(TextMeshProUGUI textMesh, string newText, float charDelay = 0.02f)
        {
            string current = textMesh.text;
            int maxLength = Mathf.Max(current.Length, newText.Length);

            // Đảm bảo cả 2 chuỗi có cùng độ dài
            char[] currentChars = current.PadRight(maxLength).ToCharArray();
            char[] targetChars = newText.PadRight(maxLength).ToCharArray();

            // Tạo danh sách task
            UniTask[] tasks = new UniTask[maxLength];

            for (int i = 0; i < maxLength; i++)
            {
                int index = i;
                tasks[i] = AnimateCharacter(index, currentChars, targetChars, textMesh, charDelay);
            }

            await UniTask.WhenAll(tasks);
        }

        private async UniTask AnimateCharacter(int index, char[] current, char[] target, TMP_Text textMesh, float delay)
        {
            char curChar = char.ToUpper(current[index]);
            char targetChar = char.ToUpper(target[index]);

            int curIdx = Alphabet.IndexOf(curChar);
            int targetIdx = Alphabet.IndexOf(targetChar);

            if (curIdx == -1) curIdx = 0;
            if (targetIdx == -1) targetIdx = 0;

            while (curIdx != targetIdx)
            {
                curIdx = (curIdx + 1) % Alphabet.Length;
                current[index] = Alphabet[curIdx];
                textMesh.text = new string(current);
                await UniTask.Delay((int)(delay * 1000));
            }
        }
    }

}