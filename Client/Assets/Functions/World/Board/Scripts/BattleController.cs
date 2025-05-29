using System;
using Cysharp.Threading.Tasks;
using Globals;
using UnityEngine;
using World.Board;

public class BattleController : MonoBehaviour, IDisposable
{
    [SerializeField] GameObject _objBoard;
    private void Start()
    {
        InitBattle();
    }

    public async UniTask InitBattle()
    {
        await Global.Instance.AddComponent(GetComponent<BattleData>());
        _objBoard.SetActive(true);
    }

    private void OnDestroy()
    {
        Dispose();
    }

    private void OnDisable()
    {
        Dispose();
    }

    public void Dispose()
    {
        Global.Instance.RemoveComponent(GetComponent<BattleData>());
        _objBoard.SetActive(false);
    }
}