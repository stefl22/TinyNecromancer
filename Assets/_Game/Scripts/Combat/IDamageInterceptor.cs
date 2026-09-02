using UnityEngine;

public interface IDamageInterceptor
{
    bool TryInterceptDamage(DamageInfo damageInfo);
}