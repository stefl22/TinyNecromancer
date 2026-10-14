using System.Collections.Generic;
using UnityEngine;

// Presentation and swept weapon contact for the authored First Descent rigs.
// Each character owns its controller/clips; gameplay remains on the existing root.
[DefaultExecutionOrder(100)]
[RequireComponent(typeof(Health))]
public class CombatAnimator : MonoBehaviour
{
    public Animator animator;
    public Transform bladeBase;
    public Transform bladeTip;
    public bool weaponContact = true;
    public float attackDuration = .85f;
    public float damage = 25f;
    public float bladeRadius = .11f;
    [Range(0, 1)] public float contactStart = .42f;
    [Range(0, 1)] public float contactEnd = .68f;

    Health health;
    PlayerCombat player;
    EnemyAI enemy;
    PlayerMovement movement;
    PlayerDefense defense;
    Vector3 lastPosition, previousBase, previousTip;
    readonly HashSet<Health> struck = new HashSet<Health>();
    float attackStarted = -100f, hitUntil;
    string state;
    bool dead, windowWasOpen;
    public bool UsesWeaponContact => enabled && weaponContact && bladeBase != null && bladeTip != null;
    public int ContactCount { get; private set; }

    void Awake()
    {
        health = GetComponent<Health>();
        player = GetComponent<PlayerCombat>(); enemy = GetComponent<EnemyAI>();
        movement = GetComponent<PlayerMovement>(); defense = GetComponent<PlayerDefense>();
        lastPosition = transform.position;
        if (animator != null) animator.applyRootMotion = false;
    }
    void OnEnable()
    {
        health.Died += Die; health.Damaged += Hit;
        if (player != null) { player.AttackStarted += Attack; player.AttackCancelled += Cancel; }
        if (enemy != null) { enemy.AttackStarted += Attack; enemy.AttackCancelled += Cancel; }
    }
    void OnDisable()
    {
        health.Died -= Die; health.Damaged -= Hit;
        if (player != null) { player.AttackStarted -= Attack; player.AttackCancelled -= Cancel; }
        if (enemy != null) { enemy.AttackStarted -= Attack; enemy.AttackCancelled -= Cancel; }
        Cancel();
    }
    void Play(string next, float blend = .10f)
    {
        if (animator == null || state == next) return;
        state = next;
        animator.CrossFadeInFixedTime(next, blend, 0, 0);
    }
    void Attack()
    {
        if (health.IsDead) return;
        attackStarted = Time.time; struck.Clear(); windowWasOpen = false;
        state = null; Play("Attack", .055f);
    }
    void Cancel() { attackStarted = -100; windowWasOpen = false; struck.Clear(); state = null; }
    void Die() { dead = true; Cancel(); Play("Death", .08f); }
    void Hit(float amount)
    {
        if (health.IsDead || Time.time - attackStarted < attackDuration) return;
        hitUntil = Time.time + .32f; state = null; Play("Hit", .04f);
    }
    void Update()
    {
        Vector3 delta = transform.position - lastPosition; delta.y = 0;
        float speed = delta.magnitude / Mathf.Max(Time.deltaTime, .0001f);
        lastPosition = transform.position;
        if (dead || health.IsDead) return;
        if (Time.time - attackStarted < attackDuration || Time.time < hitUntil) return;
        if (movement != null && movement.IsDodging) { Play("Dodge", .025f); return; }
        if (defense != null && defense.IsBlocking) { Play("Block"); return; }
        Play(speed > 3.5f ? "Run" : speed > .12f ? "Walk" : "Idle");
    }
    void LateUpdate()
    {
        if (!UsesWeaponContact || dead || health.IsDead) return;
        float phase = (Time.time - attackStarted) / Mathf.Max(.01f, attackDuration);
        if (phase < contactStart || phase > contactEnd) { windowWasOpen = false; return; }
        Vector3 a = bladeBase.position, b = bladeTip.position;
        if (!windowWasOpen) { previousBase = a; previousTip = b; }
        int steps = Mathf.Clamp(Mathf.CeilToInt(Mathf.Max(Vector3.Distance(a, previousBase),
            Vector3.Distance(b, previousTip)) / Mathf.Max(.03f, bladeRadius)), 1, 24);
        for (int i = 1; i <= steps; i++)
        {
            float t = (float)i / steps;
            foreach (var collider in Physics.OverlapCapsule(Vector3.Lerp(previousBase, a, t),
                Vector3.Lerp(previousTip, b, t), bladeRadius, ~0, QueryTriggerInteraction.Ignore))
            {
                var target = collider.GetComponentInParent<Health>();
                if (target == null || target == health || target.IsDead || struck.Contains(target)) continue;
                if (player != null && target.GetComponent<EnemyAI>() == null) continue;
                if (enemy != null && target.GetComponent<PlayerMovement>() == null) continue;
                Vector3 point = collider.ClosestPoint((a + b) * .5f);
                Vector3 origin = transform.position + Vector3.up;
                // Do not deal damage through cave walls.
                if (Physics.Linecast(origin, point, out var obstruction, ~0, QueryTriggerInteraction.Ignore)
                    && obstruction.collider.GetComponentInParent<Health>() != target
                    && obstruction.collider.GetComponentInParent<Health>() != health) continue;
                struck.Add(target); ContactCount++;
                target.TakeDamage(new DamageInfo(damage, DamageCategory.Physical, DamageType.Slash,
                    gameObject, point, target.transform.position - transform.position, true, true));
            }
        }
        previousBase = a; previousTip = b; windowWasOpen = true;
    }
}
