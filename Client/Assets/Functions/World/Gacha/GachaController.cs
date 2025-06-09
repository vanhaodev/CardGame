using System.Collections.Generic;
using UnityEngine;

namespace Functions.World.Gacha
{
    public class GachaController
    {
        /// <summary>
        /// Rút thưởng từ danh sách theo tỉ lệ, ưu tiên vật phẩm hiếm trước (Rate thấp hơn).
        /// </summary>
        /// <param name="rewardPool">Danh sách phần thưởng, không cần sắp xếp.</param>
        /// <param name="accumulatedBonus">
        /// Bonus giảm giá trị random để tăng tỉ lệ trúng item hiếm.
        /// VD: nếu random ra 50000, bonus = 10000 thì roll thực tế là 40000.
        /// </param>
        /// <returns>Index phần thưởng trúng trong rewardPool, hoặc -1 nếu không có phần thưởng.</returns>
        public int PlayGacha(List<GachaRewardModel> rewardPool, int accumulatedBonus = 0)
        {
            if (rewardPool == null || rewardPool.Count == 0)
            {
                Debug.LogWarning("PlayGacha: rewardPool is empty.");
                return -1;
            }

            // Bước 1: Random một số từ 1 đến 100000 (tương đương 100%)
            int rawRoll = UnityEngine.Random.Range(1, 100001);

            // Bước 2: Trừ bonus để tăng tỉ lệ ra item hiếm
            int adjustedRoll = Mathf.Max(1, rawRoll - accumulatedBonus);

            // Bước 3: Duyệt ngược từ cuối (Rate thấp = item hiếm) để ưu tiên trúng item hiếm trước
            for (int i = rewardPool.Count - 1; i >= 0; i--)
            {
                if (adjustedRoll <= rewardPool[i].Rate)
                {
                    return i;
                }
            }

            // Không trúng item nào (nên chỉ xảy ra nếu tổng Rate < 100000), fallback:
            return 0;
        }
    }
}