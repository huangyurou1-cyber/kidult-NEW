using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : LivingEntity
{
    // 這裡一定要加上 
    public static readonly List ActivePlayers = new List();

    // 雙人共享 5 顆心
    public static float SharedHealth { get; private set; } = 5f;
    public static event Action OnSharedHealthChanged;
    public static event Action OnSharedDeath;

    [SerializeField] private float initialSharedHearts = 5f;

    private void Awake()
    {
        SharedHealth = initialSharedHearts;
    }

    private void OnEnable()
    {
        ActivePlayers.Add(this);
    }

    private void OnDisable()
    {
        ActivePlayers.Remove(this);
    }

    public override void TakeDamage(float damage)
    {
        if (SharedHealth <= 0) return;

        SharedHealth = Mathf.Max(0, SharedHealth - damage);
        Debug.Log($"玩家受傷！剩餘共享生命：{SharedHealth}");

        // 通知 UI 更新（帶有剩餘心數的參數）
        OnSharedHealthChanged?.Invoke(SharedHealth);

        if (SharedHealth <= 0)
        {
            Debug.Log("生命歸零，Game Over！");
            OnSharedDeath?.Invoke();

            // 呼叫父類別的 Die() 安全觸發死亡
            Die();
        }
    }
}