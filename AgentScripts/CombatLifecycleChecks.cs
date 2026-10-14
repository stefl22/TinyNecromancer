using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

// Run with Pipeline run_script in Play Mode. Temporary actors never alter saved scenes.
public static class CombatLifecycleChecks
{
    static readonly BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
    static void Set(object target, string field, object value) =>
        target.GetType().GetField(field, Private).SetValue(target, value);

    static async Task Wait(float seconds)
    {
        float end = Time.time + seconds;
        float deadline = Time.realtimeSinceStartup + 10f;
        while (Time.time < end)
        {
            if (!Application.isPlaying || Time.realtimeSinceStartup > deadline)
                throw new InvalidOperationException("Play Mode stopped or paused during checks.");
            await Task.Delay(20);
        }
    }

    public static async Task<object> Main()
    {
        if (!Application.isPlaying || Time.timeScale == 0f)
            throw new InvalidOperationException("Run in unpaused Play Mode.");
        bool background = Application.runInBackground;
        try
        {
            Application.runInBackground = true;
            return await RunChecks();
        }
        finally { Application.runInBackground = background; }
    }

    static async Task<object> RunChecks()
    {
        var results = new List<object>();
        foreach (bool enemy in new[] { false, true })
        foreach (string scenario in new[] { "alive", "death", "disable" })
        {
            var actor = new GameObject("LifecycleCheck_Attacker");
            var victim = new GameObject("LifecycleCheck_Target");
            actor.SetActive(false);
            victim.SetActive(false);
            try
            {
                actor.transform.position = new Vector3(1000f, 0f, 1000f);
                victim.transform.position = actor.transform.position + Vector3.forward;
                var targetCollider = victim.AddComponent<CapsuleCollider>();
                targetCollider.center = Vector3.up * 0.5f;
                var targetHealth = victim.AddComponent<Health>();
                var attackerHealth = actor.AddComponent<Health>();
                var visual = new GameObject("CheckVisual").transform;
                visual.SetParent(actor.transform, false);
                var hand = new GameObject("CheckHand").transform;
                hand.SetParent(visual, false);
                MonoBehaviour combat;
                int hitEvents = 0;
                int cancelledEvents = 0;
                if (enemy)
                {
                    var ai = actor.AddComponent<EnemyAI>();
                    Set(actor.GetComponent<EnemyPerception>(), "target", victim.transform);
                    Set(ai, "nextAttackTime", float.MaxValue);
                    Set(ai, "moveSpeed", 0f);
                    Set(ai, "attackWindup", 0.25f);
                    ai.AttackHit += () => hitEvents++;
                    ai.AttackCancelled += () => cancelledEvents++;
                    Set(actor.AddComponent<EnemySwordSwing>(), "rightHandPoint", hand);
                    Set(actor.AddComponent<EnemyAttackFeedback>(), "visual", visual);
                    combat = ai;
                }
                else
                {
                    var player = actor.AddComponent<PlayerCombat>();
                    Set(player, "attackPoint", actor.transform);
                    Set(player, "attackWindup", 0.25f);
                    player.AttackHit += () => hitEvents++;
                    player.AttackCancelled += () => cancelledEvents++;
                    Set(actor.AddComponent<PlayerAttackFeedback>(), "visual", visual);
                    combat = player;
                }
                victim.SetActive(true);
                actor.SetActive(true);
                Physics.SyncTransforms();
                await Wait(0.05f);
                var routine = (IEnumerator)combat.GetType().GetMethod("AttackRoutine", Private).Invoke(combat, null);
                combat.StartCoroutine(routine);
                await Wait(0.05f);
                if (scenario == "death") attackerHealth.TakeDamage(1000f);
                if (scenario == "disable") combat.enabled = false;
                bool cancellationResetPose = scenario == "alive" ||
                    (visual.localPosition == Vector3.zero && visual.localScale == Vector3.one
                    && Quaternion.Angle(hand.localRotation, Quaternion.identity) < 0.01f);
                await Wait(0.8f);
                float expected = scenario == "alive" ? (enemy ? 90f : 75f) : 100f;
                bool passed = Mathf.Approximately(targetHealth.CurrentHealth, expected)
                    && hitEvents == (scenario == "alive" ? 1 : 0)
                    && cancelledEvents == (scenario == "alive" ? 0 : 1)
                    && cancellationResetPose
                    && !(bool)combat.GetType().GetField("isAttacking", Private).GetValue(combat);
                results.Add(new { actor = enemy ? "enemy" : "player", scenario, passed,
                    expectedHealth = expected, actualHealth = targetHealth.CurrentHealth, hitEvents,
                    cancelledEvents, cancellationResetPose });
            }
            finally
            {
                UnityEngine.Object.Destroy(actor);
                UnityEngine.Object.Destroy(victim);
            }
            await Wait(0.02f);
        }
        var defender = new GameObject("LifecycleCheck_Defender");
        defender.SetActive(false);
        try
        {
            var health = defender.AddComponent<Health>();
            var defense = defender.AddComponent<PlayerDefense>();
            defender.SetActive(true);
            // Isolate damage rules from mouse polling; exercise the actual Health interceptor chain.
            foreach (string scenario in new[] { "front", "rear", "magic", "unblockable", "iframe" })
            {
                health.Heal(100f);
                Set(defense, "<IsBlocking>k__BackingField", scenario != "iframe");
                health.SetInvulnerable(scenario == "iframe");
                var hit = new DamageInfo(10f,
                    scenario == "magic" ? DamageCategory.Magic : DamageCategory.Physical,
                    DamageType.Slash, null, defender.transform.position,
                    scenario == "rear" ? Vector3.forward : Vector3.back,
                    scenario != "unblockable", true);
                health.TakeDamage(hit);
                float expected = scenario == "front" || scenario == "iframe" ? 100f : 90f;
                results.Add(new { actor = "defender", scenario,
                    passed = Mathf.Approximately(health.CurrentHealth, expected),
                    expectedHealth = expected, actualHealth = health.CurrentHealth });
            }
        }
        finally { UnityEngine.Object.Destroy(defender); }
        return results;
    }
}
