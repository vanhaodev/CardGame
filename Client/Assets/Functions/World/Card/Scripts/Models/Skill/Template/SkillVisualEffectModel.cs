using UnityEngine.AddressableAssets;

namespace World.TheCard.Skill
{
    [System.Serializable]
    public class SkillVisualEffectModel
    {
        public enum VisualEffectType
        {
            OnSend, OnReceive, Projectile
        }

        public VisualEffectType Type;
        public AssetReference VfxPrefab;
    }
}