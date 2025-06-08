using System.Collections.Generic;
using UnityEngine;

namespace Functions.World.Gacha
{
    public class GachaController
    {
        public GachaRewardModel PlayGacha(List<GachaRewardModel> rewards, int accumulatedBonus = 0)
        {
            // Sinh ra một số ngẫu nhiên từ 0 đến 99999
            // Giá trị càng nhỏ càng hiếm.
            var rollResult = UnityEngine.Random.Range(0, 100000); 
    
            // Áp dụng bonus: trừ bonus từ rollResult
            // Điều này làm cho rollResult nhỏ hơn, dễ đạt được các vật phẩm hiếm (có Rate thấp) hơn.
            rollResult -= accumulatedBonus; 

            // Đảm bảo rollResult không âm (ngưỡng thấp nhất)
            // Nếu Rate thấp nhất là 0, thì rollResult không nên thấp hơn 0
            rollResult = Mathf.Max(0, rollResult); 

            // Duyệt qua danh sách phần thưởng từ hiếm nhất đến phổ biến nhất
            // Giả định rewards[i].Rate là ngưỡng trên cho vật phẩm đó (VD: 1000 cho 1%)
            // Và danh sách rewards được sắp xếp theo Rate tăng dần (hiếm -> phổ biến)
            for (int i = 0; i < rewards.Count; i++) // Vòng lặp từ đầu đến cuối
            {
                if (rollResult < rewards[i].Rate)
                {
                    return rewards[i];
                }
            }

            // Nếu không khớp với bất kỳ Rate nào (ví dụ, rollResult quá cao),
            // trả về phần thưởng phổ biến nhất.
            // Điều này giả định rewards[rewards.Count - 1] là phần thưởng phổ biến nhất.
            return rewards[rewards.Count - 1]; 
        }
    }
}