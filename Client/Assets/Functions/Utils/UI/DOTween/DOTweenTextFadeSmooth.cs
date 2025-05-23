using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Utils.UI.DOTween
{
    public class DOTweenTextFadeSmooth
    {
        private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ";

        public async UniTask AnimateTextChangeAsync(TextMeshProUGUI textMesh, string newText, float charDelay,
            CancellationTokenSource cts)
        {
            if (cts == null)
                throw new System.ArgumentNullException(nameof(cts), "CancellationTokenSource must not be null.");
            if (textMesh == null)
                throw new System.ArgumentNullException(nameof(textMesh), "TextMeshProUGUI must not be null.");
            await UniTask.WaitForEndOfFrame(cancellationToken: cts.Token);
            string current = textMesh.text;
            int maxLength = Mathf.Max(current.Length, newText.Length);

            char[] currentChars = current.PadRight(maxLength).ToCharArray();
            char[] targetChars = newText.PadRight(maxLength).ToCharArray();

            UniTask[] tasks = new UniTask[maxLength];

            for (int i = 0; i < maxLength; i++)
            {
                int index = i;
                tasks[i] = AnimateCharacter(index, currentChars, targetChars, textMesh, charDelay, cts.Token);
            }

            await UniTask.WhenAll(tasks).AttachExternalCancellation(cts.Token);
        }

        private async UniTask AnimateCharacter(
            int index,
            char[] current,
            char[] target,
            TMP_Text textMesh,
            float delay,
            CancellationToken token)
        {
            char curChar = char.ToUpper(current[index]);
            char targetChar = char.ToUpper(target[index]);

            int curIdx = Alphabet.IndexOf(curChar);
            int targetIdx = Alphabet.IndexOf(targetChar);

            if (curIdx == -1) curIdx = 0;
            if (targetIdx == -1) targetIdx = 0;

            while (curIdx != targetIdx)
            {
                token.ThrowIfCancellationRequested();

                curIdx = (curIdx + 1) % Alphabet.Length;
                current[index] = Alphabet[curIdx];

                if (textMesh != null)
                    textMesh.text =
                        new string(current); // vẫn sẽ có flicker nhẹ, chỉ có thể gom chung lại nếu muốn smooth hơn.

                await UniTask.Delay((int)(delay * 1000), cancellationToken: token);
            }
        }
    }
}