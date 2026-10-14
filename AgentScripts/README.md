# Live combat checks

`CombatLifecycleChecks.cs` is a Pipeline `run_script` fixture outside Assets. It compiles in memory and uses actual game components, coroutines, collision queries and damage interceptors in the open Unity Editor.

Start from a saved scene, enter Play Mode, and run:

```powershell
unity command run_script --file AgentScripts/CombatLifecycleChecks.cs --caller plugin --skill unity-cli --project-path C:\TINY_NECROMANCER\TinyNecromancer --format json
```

Inspect every result's `passed` field; the outer command succeeding only means the script ran. Exit Play Mode afterward. The fixture creates temporary actors far from the prototype, destroys them in finally blocks, and temporarily enables background simulation, restoring its original value. It does not save or edit scenes/assets. Do not interact with combat controls during the check.

## Verified 2026-10-14

Before the fix, both normal-attack controls passed and all four death/disable cancellation cases failed: player damage remained 25 and enemy damage remained 10 after cancellation should have occurred.

After the fix, 11/11 checks passed:
- Normal player and enemy attacks: target HP 75 and 90; one attack-hit event each.
- Death during windup, for both attackers: target HP 100, zero hit events, one cancellation event, idle attack state and immediately reset visual pose.
- Component disable during windup, for both attackers: same cancellation guarantees.
- Frontal physical block and invulnerability: target HP 100.
- Rear physical hit, magic and explicitly unblockable attack: target HP 90.

Unity compilation succeeded. Final live Console counters were zero errors and zero warnings. Two import-worker shutdown errors predated the change and remained in Pipeline's historical buffer.

## Limits

These are focused live integration checks, not a complete game playthrough or a replacement for player feel testing. They initiate attacks through reflection to control windup timing. The invulnerability check exercises Health's damage gate, not keyboard dodge timing. Input bindings, navigation around obstacles, scene restart, authored animation clips, weapon contact sweeps, and a production build are not covered by this fixture. The existing sword movement remains prototype presentation; full-body animation/contact synchronization is the next milestone.
