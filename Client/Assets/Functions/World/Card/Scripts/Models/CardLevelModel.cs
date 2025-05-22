using GameConfigs;
using Globals;
using Sirenix.OdinInspector;
using UnityEngine;

namespace World.TheCard.Skill
{
    [System.Serializable]
    public class CardLevelModel
    {
        /// <summary>
        /// lưu cấp
        /// </summary>
        private ushort _level;

        /// <summary>
        /// phần chính, exp
        /// </summary>
        [SerializeField] private uint _exp;

        /// <summary>
        /// exp của lần cập nhật cuối
        /// </summary>
        private uint _expOfLastUpdate;

        /// <summary>
        /// exp để đạt level tiếp theo
        /// </summary>
        private uint _expNext;

        /// <summary>
        /// %
        /// </summary>
        private float _progress;

        [Button]
        private void Update()
        {
            if (_exp == _expOfLastUpdate && _exp != 0) return;
            var updateData = Global.Instance.Get<GameConfig>().GetLevelProgressAndNextExp(_exp);
            _level = updateData.level;
            _expOfLastUpdate = _exp;
            _expNext = updateData.expNext;
            _progress = updateData.progressPercent;
        }

        public ushort GetLevel()
        {
            Update();
            return _level;
        }

        public uint GetExp()
        {
            Update();
            return _exp;
        }

        public uint GetExpNext()
        {
            Update();
            return _expNext;
        }

        public float GetProgress()
        {
            Update();
            return _progress;
        }
    }
}