using UnityEngine;
using UnityEngine.Serialization;

namespace World.Board
{
    public class SkillPointUI : MonoBehaviour
    {
        /// <summary>
        /// Mảng chứa 5 viên đá hoặc biểu tượng tương ứng với điểm kỹ năng (0 đến 5).
        /// </summary>
        [SerializeField] private GameObject[] _objSkillPoints;

        /// <summary>
        /// Thiết lập số điểm kỹ năng và cập nhật hiển thị tương ứng.
        /// </summary>
        /// <param name="point">Số điểm kỹ năng (0 đến 5)</param>
        public void SetPoint(int point)
        {
            point = Mathf.Clamp(point, 0, _objSkillPoints.Length);

            for (int i = 0; i < _objSkillPoints.Length; i++)
            {
                _objSkillPoints[i].SetActive(i < point);
            }
        }
    }

}