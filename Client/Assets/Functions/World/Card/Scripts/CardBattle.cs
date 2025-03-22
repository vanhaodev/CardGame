using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using World.Card;

public partial class CardBattle : MonoBehaviour
{
    [SerializeField] private bool _isDead;
    public bool IsDead => _isDead;

    [SerializeField] private SerializedDictionary<AttributeType, int> _attributes;
    [SerializeField] private CardVital _vital;
    public CardVital Vital => _vital;
    [SerializeField] CardEffect _effect;

    public void SetupBattle(Card card)
    {
        _isDead = false;
        // Xóa dữ liệu cũ để tránh lỗi trùng key
        _attributes.Clear();

        // Chuyển List thành Dictionary thủ công
        foreach (var attr in card.CardModel.CalculatedAttributes)
        {
            _attributes[attr.Type] = attr.Value;
        }

        SetupHpMp(AttributeType.Hp, AttributeType.HpMax);
        SetupHpMp(AttributeType.Mp, AttributeType.MpMax);
        _vital.UpdateHp(_attributes[AttributeType.Hp], _attributes[AttributeType.HpMax]);
        _vital.UpdateMp(_attributes[AttributeType.Mp], _attributes[AttributeType.MpMax]);
        _effect.PlaySpawn();
    }

    private void SetupHpMp(AttributeType current, AttributeType max)
    {
        if (_attributes.TryGetValue(max, out var maxAttr))
        {
            if (_attributes.ContainsKey(current))
            {
                _attributes[current] = maxAttr; // Cập nhật nếu đã có
            }
            else
            {
                _attributes[current] = maxAttr; // Thêm mới
            }
        }
    }

    public void OnTakeDamage(int damage)
    {
        if (_isDead) return;
        _attributes[AttributeType.Hp] -= damage;
        if (_attributes[AttributeType.Hp] <= 0)
        {
            _isDead = true;
            _effect.PlayDie();
        }
        _vital.UpdateHp(_attributes[AttributeType.Hp], _attributes[AttributeType.HpMax]);
    }
}