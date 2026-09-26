using System;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float starHealth;

    protected float Health { get; private set; }
    protected bool IsDead;

    public event Action OnDeath;

    protected virtual void Start()
    {
        Health = starHealth;
    }

    public virtual void TakeDamage(float damage)
    {
        Health -= damage;

        if (Health > 0 || IsDead) return;

        Die();
    }

    // 供本身及子類別安全觸發死亡邏輯
    protected virtual void Die()
    {
        if (IsDead) return;
        IsDead = true;
        OnDeath?.Invoke();
        Destroy(gameObject);
    }
}