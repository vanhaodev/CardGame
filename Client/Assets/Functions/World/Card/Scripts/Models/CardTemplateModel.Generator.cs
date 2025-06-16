#if UNITY_EDITOR //===================EDITOR======================//
using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Globals;
using Sirenix.OdinInspector;
using UnityEngine;
using World.TheCard.Skill;
using Random = UnityEngine.Random;

namespace World.TheCard
{
    public partial class CardTemplateModel : ScriptableObject
    {
        [Button]
        public void MakeAttribute()
        {
            Attributes = new List<AttributeModel>();

            // Sinh chỉ số cơ bản theo class
            switch (Class)
            {
                case ClassType.Crusher:
                    GenerateCrusherAttributes();
                    break;
                case ClassType.Assassin:
                    GenerateAssassinAttributes();
                    break;
                case ClassType.Demolisher:
                    GenerateDemolisherAttributes();
                    break;
                case ClassType.Tactician:
                    GenerateTacticianAttributes();
                    break;
                case ClassType.Guardian:
                    GenerateGuardianAttributes();
                    break;
                case ClassType.Medic:
                    GenerateMedicAttributes();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // Sau khi sinh chỉ số class xong, mới gắn element tương khắc
            GenerateElementalResistant();
        }

        private void GenerateCrusherAttributes()
        {
            switch (Element)
            {
                case ElementType.Metal:
                    Attributes.Add(new AttributeModel { Type = AttributeType.HpMax, Value = Random.Range(220, 260) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Attack, Value = Random.Range(46, 56) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Defense, Value = Random.Range(28, 36) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.CriticalRate, Value = Random.Range(200, 300) }); // = 2–3%
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.ToughnessHitRate, Value = Random.Range(300, 500) }); // tầm ~3–5%
                    break;

                case ElementType.Wood:
                    Attributes.Add(new AttributeModel { Type = AttributeType.HpMax, Value = Random.Range(210, 250) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Attack, Value = Random.Range(44, 54) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Defense, Value = Random.Range(24, 32) });
                    Attributes.Add(
                        new AttributeModel
                            { Type = AttributeType.DodgeRate, Value = Random.Range(150, 250) }); // = 1.5–2.5%
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.ToughnessHitRate, Value = Random.Range(300, 450) });
                    break;

                case ElementType.Water:
                    Attributes.Add(new AttributeModel { Type = AttributeType.HpMax, Value = Random.Range(230, 270) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Attack, Value = Random.Range(45, 55) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Defense, Value = Random.Range(26, 36) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.EffectResistRate, Value = Random.Range(200, 300) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.ToughnessHitRate, Value = Random.Range(250, 400) });
                    break;

                case ElementType.Fire:
                    Attributes.Add(new AttributeModel { Type = AttributeType.HpMax, Value = Random.Range(200, 240) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Attack, Value = Random.Range(50, 60) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.CriticalRate, Value = Random.Range(200, 300) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.CriticalDamage, Value = Random.Range(500, 700) }); // = 5–7%
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.ToughnessHitRate, Value = Random.Range(300, 450) });
                    break;

                case ElementType.Earth:
                    Attributes.Add(new AttributeModel { Type = AttributeType.HpMax, Value = Random.Range(260, 320) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Attack, Value = Random.Range(42, 50) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Defense, Value = Random.Range(36, 48) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.HealingReceived, Value = Random.Range(300, 500) }); // = 3–5%
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.ToughnessHitRate, Value = Random.Range(300, 500) });
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void GenerateAssassinAttributes()
        {
            switch (Element)
            {
                case ElementType.Metal:
                    Attributes.Add(new AttributeModel { Type = AttributeType.HpMax, Value = Random.Range(160, 200) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Attack, Value = Random.Range(52, 64) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.CriticalRate, Value = Random.Range(250, 300) }); // 2.5–3%
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.ArmorPenetrationRate, Value = Random.Range(200, 300) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Speed, Value = Random.Range(160, 200) });
                    break;

                case ElementType.Wood:
                    Attributes.Add(new AttributeModel { Type = AttributeType.HpMax, Value = Random.Range(150, 190) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Attack, Value = Random.Range(48, 60) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.CriticalRate, Value = Random.Range(200, 300) });
                    Attributes.Add(
                        new AttributeModel { Type = AttributeType.DodgeRate, Value = Random.Range(200, 300) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Speed, Value = Random.Range(180, 200) });
                    break;

                case ElementType.Water:
                    Attributes.Add(new AttributeModel { Type = AttributeType.HpMax, Value = Random.Range(160, 200) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Attack, Value = Random.Range(50, 62) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.EffectHitRate, Value = Random.Range(200, 300) });
                    Attributes.Add(
                        new AttributeModel { Type = AttributeType.LifeSteal, Value = Random.Range(300, 500) }); // 3–5%
                    Attributes.Add(new AttributeModel { Type = AttributeType.Speed, Value = Random.Range(150, 180) });
                    break;

                case ElementType.Fire:
                    Attributes.Add(new AttributeModel { Type = AttributeType.HpMax, Value = Random.Range(150, 190) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Attack, Value = Random.Range(56, 68) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.CriticalRate, Value = Random.Range(250, 300) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.CriticalDamage, Value = Random.Range(600, 700) }); // 6–7%
                    Attributes.Add(new AttributeModel { Type = AttributeType.Speed, Value = Random.Range(140, 170) });
                    break;

                case ElementType.Earth:
                    Attributes.Add(new AttributeModel { Type = AttributeType.HpMax, Value = Random.Range(180, 230) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Attack, Value = Random.Range(50, 60) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.ToughnessHitRate, Value = Random.Range(300, 450) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.CriticalRate, Value = Random.Range(180, 250) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Defense, Value = Random.Range(24, 32) });
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void GenerateDemolisherAttributes()
        {
            switch (Element)
            {
                case ElementType.Metal:
                    Attributes.Add(new AttributeModel { Type = AttributeType.HpMax, Value = Random.Range(200, 240) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Attack, Value = Random.Range(48, 60) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Defense, Value = Random.Range(20, 28) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.ToughnessHitRate, Value = Random.Range(350, 500) }); // 3.5–5%
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.ArmorPenetrationRate, Value = Random.Range(250, 300) }); // 2.5–3%
                    break;

                case ElementType.Wood:
                    Attributes.Add(new AttributeModel { Type = AttributeType.HpMax, Value = Random.Range(190, 230) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Attack, Value = Random.Range(46, 58) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Speed, Value = Random.Range(140, 180) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.ToughnessHitRate, Value = Random.Range(300, 450) });
                    Attributes.Add(
                        new AttributeModel { Type = AttributeType.DodgeRate, Value = Random.Range(200, 300) });
                    break;

                case ElementType.Water:
                    Attributes.Add(new AttributeModel { Type = AttributeType.HpMax, Value = Random.Range(200, 240) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Attack, Value = Random.Range(46, 58) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.ToughnessHitRate, Value = Random.Range(350, 500) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.EffectHitRate, Value = Random.Range(200, 300) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.UPRegeneration, Value = Random.Range(300, 500) }); // 3–5%
                    break;

                case ElementType.Fire:
                    Attributes.Add(new AttributeModel { Type = AttributeType.HpMax, Value = Random.Range(180, 220) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Attack, Value = Random.Range(52, 64) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.ToughnessHitRate, Value = Random.Range(350, 500) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.CriticalDamage, Value = Random.Range(600, 700) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.ArmorPenetrationDamage, Value = Random.Range(500, 700) });
                    break;

                case ElementType.Earth:
                    Attributes.Add(new AttributeModel { Type = AttributeType.HpMax, Value = Random.Range(240, 300) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Attack, Value = Random.Range(44, 54) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Defense, Value = Random.Range(28, 36) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.ToughnessHitRate, Value = Random.Range(400, 500) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.HealingReceived, Value = Random.Range(300, 500) });
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void GenerateTacticianAttributes()
        {
            switch (Element)
            {
                case ElementType.Metal:
                    Attributes.Add(new AttributeModel { Type = AttributeType.HpMax, Value = Random.Range(180, 220) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Attack, Value = Random.Range(36, 46) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Defense, Value = Random.Range(20, 28) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.EffectHitRate, Value = Random.Range(250, 300) }); // 2.5–3%
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.UPRegeneration, Value = Random.Range(300, 500) }); // 3–5%
                    break;

                case ElementType.Wood:
                    Attributes.Add(new AttributeModel { Type = AttributeType.HpMax, Value = Random.Range(170, 210) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Attack, Value = Random.Range(34, 44) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Speed, Value = Random.Range(150, 190) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.EffectHitRate, Value = Random.Range(240, 280) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.OutgoingHealing, Value = Random.Range(300, 500) }); // 3–5%
                    break;

                case ElementType.Water:
                    Attributes.Add(new AttributeModel { Type = AttributeType.HpMax, Value = Random.Range(180, 220) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Attack, Value = Random.Range(34, 44) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.EffectHitRate, Value = Random.Range(270, 300) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.UPRegeneration, Value = Random.Range(400, 500) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.EffectResistRate, Value = Random.Range(200, 300) });
                    break;

                case ElementType.Fire:
                    Attributes.Add(new AttributeModel { Type = AttributeType.HpMax, Value = Random.Range(160, 200) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Attack, Value = Random.Range(38, 48) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.CriticalRate, Value = Random.Range(200, 300) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.EffectHitRate, Value = Random.Range(230, 280) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.UPRegeneration, Value = Random.Range(300, 500) });
                    break;

                case ElementType.Earth:
                    Attributes.Add(new AttributeModel { Type = AttributeType.HpMax, Value = Random.Range(200, 250) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Attack, Value = Random.Range(32, 42) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Defense, Value = Random.Range(24, 32) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.OutgoingHealing, Value = Random.Range(400, 500) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.EffectResistRate, Value = Random.Range(200, 300) });
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void GenerateGuardianAttributes()
        {
            switch (Element)
            {
                case ElementType.Metal:
                    Attributes.Add(new AttributeModel { Type = AttributeType.HpMax, Value = Random.Range(240, 300) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Attack, Value = Random.Range(32, 42) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Defense, Value = Random.Range(36, 48) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.DodgeDamage, Value = Random.Range(300, 500) }); // 3–5%
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.ToughnessHitRate, Value = Random.Range(300, 450) });
                    break;

                case ElementType.Wood:
                    Attributes.Add(new AttributeModel { Type = AttributeType.HpMax, Value = Random.Range(220, 270) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Attack, Value = Random.Range(32, 42) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Speed, Value = Random.Range(140, 180) });
                    Attributes.Add(
                        new AttributeModel { Type = AttributeType.DodgeRate, Value = Random.Range(200, 300) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Defense, Value = Random.Range(28, 36) });
                    break;

                case ElementType.Water:
                    Attributes.Add(new AttributeModel { Type = AttributeType.HpMax, Value = Random.Range(250, 320) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Attack, Value = Random.Range(32, 42) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Defense, Value = Random.Range(32, 44) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.HealingReceived, Value = Random.Range(300, 500) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.EffectResistRate, Value = Random.Range(200, 300) });
                    break;

                case ElementType.Fire:
                    Attributes.Add(new AttributeModel { Type = AttributeType.HpMax, Value = Random.Range(210, 270) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Attack, Value = Random.Range(36, 46) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Defense, Value = Random.Range(28, 38) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.CriticalRate, Value = Random.Range(200, 300) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.DodgeDamage, Value = Random.Range(300, 500) });
                    break;

                case ElementType.Earth:
                    Attributes.Add(new AttributeModel { Type = AttributeType.HpMax, Value = Random.Range(280, 350) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Attack, Value = Random.Range(30, 40) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Defense, Value = Random.Range(40, 55) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.HealingReceived, Value = Random.Range(400, 500) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.ToughnessHitRate, Value = Random.Range(300, 500) });
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void GenerateMedicAttributes()
        {
            switch (Element)
            {
                case ElementType.Metal:
                    Attributes.Add(new AttributeModel { Type = AttributeType.HpMax, Value = Random.Range(260, 320) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Attack, Value = Random.Range(26, 36) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.UPRegeneration, Value = Random.Range(400, 500) }); // 4–5%
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.OutgoingHealing, Value = Random.Range(400, 500) }); // 4–5%
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.EffectHitRate, Value = Random.Range(200, 300) });
                    break;

                case ElementType.Wood:
                    Attributes.Add(new AttributeModel { Type = AttributeType.HpMax, Value = Random.Range(240, 300) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Attack, Value = Random.Range(28, 38) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.UPRegeneration, Value = Random.Range(350, 500) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.OutgoingHealing, Value = Random.Range(400, 500) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Speed, Value = Random.Range(160, 200) });
                    break;

                case ElementType.Water:
                    Attributes.Add(new AttributeModel { Type = AttributeType.HpMax, Value = Random.Range(270, 330) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Attack, Value = Random.Range(26, 36) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.UPRegeneration, Value = Random.Range(400, 500) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.OutgoingHealing, Value = Random.Range(450, 500) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.EffectResistRate, Value = Random.Range(250, 300) });
                    break;

                case ElementType.Fire:
                    Attributes.Add(new AttributeModel { Type = AttributeType.HpMax, Value = Random.Range(230, 280) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Attack, Value = Random.Range(32, 44) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.UPRegeneration, Value = Random.Range(350, 500) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.OutgoingHealing, Value = Random.Range(350, 500) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.CriticalRate, Value = Random.Range(200, 300) });
                    break;

                case ElementType.Earth:
                    Attributes.Add(new AttributeModel { Type = AttributeType.HpMax, Value = Random.Range(300, 350) });
                    Attributes.Add(new AttributeModel { Type = AttributeType.Attack, Value = Random.Range(26, 34) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.UPRegeneration, Value = Random.Range(400, 500) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.OutgoingHealing, Value = Random.Range(400, 500) });
                    Attributes.Add(new AttributeModel
                        { Type = AttributeType.HealingReceived, Value = Random.Range(300, 500) });
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void GenerateElementalResistant()
        {
            switch (Element)
            {
                case ElementType.Metal:
                    Attributes.Add(new AttributeModel
                    {
                        Type = AttributeType.WoodResistant, // Kim khắc Mộc
                        Value = 500
                    });
                    break;
                case ElementType.Wood:
                    Attributes.Add(new AttributeModel
                    {
                        Type = AttributeType.EarthResistant, // Mộc khắc Thổ
                        Value = 500
                    });
                    break;
                case ElementType.Water:
                    Attributes.Add(new AttributeModel
                    {
                        Type = AttributeType.FireResistant, // Thủy khắc Hỏa
                        Value = 500
                    });
                    break;
                case ElementType.Fire:
                    Attributes.Add(new AttributeModel
                    {
                        Type = AttributeType.MetalResistant, // Hỏa khắc Kim
                        Value = 500
                    });
                    break;
                case ElementType.Earth:
                    Attributes.Add(new AttributeModel
                    {
                        Type = AttributeType.WaterResistant, // Thổ khắc Thủy
                        Value = 500
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
#endif