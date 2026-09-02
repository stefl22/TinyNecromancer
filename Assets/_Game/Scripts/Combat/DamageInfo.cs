using UnityEngine;

public enum DamageCategory
{
    Physical,
    Magic
}

public enum DamageType
{
    None,
    Slash,
    Pierce,
    Blunt,
    Fire,
    Ice,
    Arcane,
    Soul
}

public struct DamageInfo
{
    public float Amount;

    public DamageCategory Category;
    public DamageType Type;

    public GameObject Source;

    public Vector3 HitPoint;

    // Direction the attack travels:
    // attacker -> target
    public Vector3 AttackDirection;

    public bool CanBeBlocked;
    public bool CanBeParried;

    public DamageInfo(
        float amount,
        DamageCategory category,
        DamageType type,
        GameObject source,
        Vector3 hitPoint,
        Vector3 attackDirection,
        bool canBeBlocked = true,
        bool canBeParried = true)
    {
        Amount = amount;
        Category = category;
        Type = type;
        Source = source;
        HitPoint = hitPoint;
        AttackDirection = attackDirection.normalized;
        CanBeBlocked = canBeBlocked;
        CanBeParried = canBeParried;
    }
}