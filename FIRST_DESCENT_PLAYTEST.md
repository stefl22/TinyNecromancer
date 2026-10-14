# First Descent — original visual prototype

Open `Assets/_Game/Scenes/FirstDescent_Cave.unity`, select **Game**, press **Play**, and click inside the Game view.

| Control | Action |
|---|---|
| WASD | Walk relative to the camera |
| Left Shift + WASD | Run |
| Left mouse | Sword attack |
| Hold right mouse | Frontal shield block |
| Space | Dodge with the existing i-frame timing |
| Hold middle mouse and drag | Orbit camera |
| Mouse wheel | Zoom |

The Warden sees/hears, approaches and attacks the player. Four landed player hits defeat him. Player death reloads this scene. Restart Play Mode to reset the encounter.

## What is implemented

- Original Wayfarer adventurer: faceted adult human proportions, teal tunic, leather harness/pouch/boots, light shoulder protection, travel shield and sword.
- Original Slate Warden: taller silhouette, rust-red clothing/mantle, dark steel helmet and shoulders, sword; a slower overhead attack distinct from the adventurer's diagonal cut.
- Both FBX files have valid Unity Humanoid Avatars, skinned meshes and character-specific Animator Controllers. Authored full-body clips: Idle, Walk, Run, Attack, Block, Dodge, Hit, Death and a reserved Parry pose. Parry gameplay is not implemented.
- Blender source and reproducible generation script: `ArtSource/FirstDescent`. No third-party models or animations were downloaded. Previous BaseHuman Blender files were not modified.
- An irregular faceted cave floor, walls and vault, boulders, stalagmites/stalactites, restrained mineral cluster, URP materials, warm/cool lighting, shadows and fog. The arena is provisional, not the final dungeon layout.
- The original gameplay roots/components remain. Their old `Visual` hierarchies are disabled backups in the new scene. `Prototype.unity` is unchanged.
- Animated actors use swept blade contact during a configurable portion of the attack clip, once per target per attack. They bypass the legacy proximity damage to prevent double hits. Legacy actors without CombatAnimator retain their old damage behavior. Attack cancellation also closes contact windows.
- Walk/run speed and gravity are enabled through settings in the new scene; defaults preserve the original scene. The camera avoids cave geometry and keeps right mouse available for blocking.

## Verification

The live Editor accepted both rigs as valid Humanoid Avatars and imported all nine clips per character. Eight FirstDescentChecks passed in Play Mode: locomotion/animation selection, dodge/i-frames, player blade contact, animated shield blocking, enemy blade contact, perception/chase, death cancelling weapon damage, and ground collision. The attack was adjusted in Blender after the first contact trace revealed a miss, then tested again against the real CharacterController collider.

Tests are under `AgentScripts`; run through Pipeline in Play Mode. They modify temporary runtime state, so exit/re-enter Play Mode after running them. `FirstDescentChecks.Main` exercises the real scene and uses synthetic keyboard/mouse input. `CombatLifecycleChecks.Main` covers the existing damage/cancellation rules separately.

The 11 existing lifecycle/defense checks also passed after integration (19 checks total). Final live Console counters: zero errors and zero warnings, with no compilation failure.

## Current limits

This is the first playable art pass, not final production art or motion capture. Hands have simplified geometry, there are no facial animations, and feet do not yet use terrain IK. Contact windows are normalized clip timing configured per combatant, not Animation Events yet. Dual wielding and additional enemy styles remain future work. The enemy uses the existing direct pursuit AI, so complex obstacle navigation is not introduced here. No final map/lore decisions or early soul powers were added.
