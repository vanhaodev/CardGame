using GameConfigs;
using Globals;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace World.TheCard.Skill
{
    [System.Serializable]
    public class CardLevelModel
    {
        /// <summary>
        /// lưu cấp
        /// </summary>
        public ushort Level;

        /// <summary>
        /// phần chính, exp
        /// </summary>
        public uint Exp;

        /// <summary>
        /// exp của lần cập nhật cuối
        /// </summary>
        public uint ExpOfLastUpdate;

        /// <summary>
        /// Exp tính từ mốc level hiện tại
        /// </summary>
        public uint ExpCurrent;

        /// <summary>
        /// exp để đạt level tiếp theo
        /// </summary>
        public uint ExpNext;

        /// <summary>
        /// %
        /// </summary>
        public float Progress;

        [Button]
        private void Update()
        {
            if (Exp == ExpOfLastUpdate && Exp != 0) return;
            var updateData = Global.Instance.Get<GameConfig>().GetLevelProgressAndNextExp(Exp);
            Level = updateData.level;
            ExpOfLastUpdate = Exp;
            ExpCurrent = updateData.expCurrent;
            ExpNext = updateData.expNext;
            Progress = updateData.progressPercent;
        }

        public ushort GetLevel(bool isUpdate = true)
        {
            if (isUpdate) Update();
            return Level;
        }

        public void SetExp(uint exp)
        {
            Exp = exp;
            Update();
        }

        public uint GetExp(bool isUpdate = true)
        {
            if (isUpdate) Update();
            return Exp;
        }

        public uint GetExpCurrent(bool isUpdate = true)
        {
            if (isUpdate) Update();
            return ExpCurrent;
        }

        public uint GetExpNext(bool isUpdate = true)
        {
            if (isUpdate) Update();
            return ExpNext;
        }

        public float GetProgress(bool isUpdate = true)
        {
            if (isUpdate) Update();
            return Progress;
        }
    }
}