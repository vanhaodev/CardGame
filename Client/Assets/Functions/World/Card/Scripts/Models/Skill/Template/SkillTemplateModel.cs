using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using World.Requirement;

namespace World.TheCard.Skill
{
    [System.Serializable]
    public class SkillTemplateModel
    {
        public string Name;
        public string Description;
        public SkillTargetType TargetType;

        /// <summary>
        /// max 5 stars
        /// </summary>
        public List<SkillOptionTemplateModel> Levels;

        public bool IsValidTarget(int senderFactionIndex, int targetFactionIndex, int senderMemberIndex,
            int targetMemberIndex)
        {
            // Kiểm tra xem có cùng phe không
            bool isSameFaction = senderFactionIndex == targetFactionIndex;

            // Kiểm tra xem có phải bản thân không (cùng phe và cùng chỉ số thành viên)
            bool isSelf = isSameFaction && senderMemberIndex == targetMemberIndex;

            switch (TargetType)
            {
                case SkillTargetType.Self:
                    // Chỉ cho phép chọn chính bản thân
                    return isSelf;

                case SkillTargetType.Team:
                    // Chỉ cho phép chọn đồng đội, không bao gồm bản thân
                    return isSameFaction && !isSelf;

                case SkillTargetType.TeamAndSelf:
                    // Cho phép chọn bất kỳ ai trong phe (bao gồm bản thân)
                    return isSameFaction;

                case SkillTargetType.Enemy:
                    // Chỉ cho phép chọn mục tiêu khác phe
                    return !isSameFaction;

                default:
                    // Trường hợp không xác định (an toàn: không cho phép)
                    return false;
            }
        }
    }
}