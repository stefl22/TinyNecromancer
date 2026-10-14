# Tiny Necromancer — Current Development Status

> Living checkpoint for the current prototype. GitHub/code takes priority over memory if any discrepancy appears later.

## Current Branch
- `feature/combat-v0.1`

## Current Prototype Scene
- `Assets/_Game/Scenes/Prototype.unity`

## Core Systems Working
### Player
- CharacterController movement
- Camera-relative movement
- Rotation toward movement
- Dodge
- Dodge cooldown
- Dodge i-frames
- Health / damage / death / scene restart
- Basic melee attack
- Attack feedback
- Noise emission
- Different noise intensity for movement, dodge, and attack

### Enemy
- Basic chase / melee attack
- Attack telegraph
- Health / damage / death
- Damage flash
- Hit reaction
- Field-of-view perception
- Raycast line-of-sight
- Last-known-position memory
- Investigation behavior
- 360-degree search
- Hearing / noise reaction
- `FacingMarker` retained for debugging where the enemy is looking

## Humanoid Combat Prototype
The player now has a technical humanoid visual under:

`Player > Visual > HumanoidPrototype`

Current visual parts:
- Torso
- Head
- LeftUpperArm
- LeftForeArm
- RightUpperArm
- RightForeArm
- LeftUpperLeg
- LeftLowerLeg
- RightUpperLeg
- RightLowerLeg
- RightHandPoint
- SwordPrototype
  - Handle
  - Blade
  - Guard
- LeftHandPoint
- ShieldPrototype

The old `CapsuleMesh` remains as a hidden/reference visual. The Player root and CharacterController remain the gameplay/collision root.

## Confirmed Current Transform Adjustments
These values were manually tuned in Unity and should be treated as the current prototype reference.

### LeftForeArm
- Position: `(-0.48, 0.78, 0)`
- Rotation: `(0, 0, 0)`
- Scale: `(0.14, 0.17, 0.14)`

### RightForeArm
- Position: `(0.48, 0.784, 0)`
- Rotation: `(0, 0, 0)`
- Scale: `(0.14, 0.17, 0.14)`

### SwordPrototype
- Position: `(0, 0.234, 0)`
- Rotation: `(0, 0, 0)`
- Scale: `(1, 1, 1)`

### SwordPrototype / Handle
- Position: `(0, -0.114, 0)`
- Rotation: `(0, 0, 0)`
- Scale: `(0.07, 0.10, 0.07)`

### SwordPrototype / Blade
- Position: `(0, -0.484, 0)`
- Rotation: `(0, 0, 0)`
- Scale: `(0.10, 0.50, 0.04)`

### SwordPrototype / Guard
- Position: `(0, -0.23, 0)`
- Rotation: `(0, 0, 0)`
- Scale: `(0.28, 0.04, 0.07)`

### ShieldPrototype
- Exact latest transform was not captured in the screenshots supplied for this checkpoint. Do not guess it; inspect the Unity scene before changing it.

## Blender BaseHuman v0.1 — Confirmed Measurements
These values were read directly from the Blender Transform panels in the user's screenshots on 2026-09-02. Treat them as the current source of truth for the BaseHuman blockout unless Blender itself shows a newer value.

### Base_Torso
- Location: `(0, 0, 1.6)` m
- Rotation: `(0, 0, 0)`
- Scale: `(1, 1, 1)`
- Dimensions: `X 1.03 m`, `Y 0.50 m`, `Z 1.50 m`

### Base_Pelvis
- Location: `(0, 0, 0.65)` m
- Rotation: `(0, 0, 0)`
- Scale: `(1, 1, 1)`
- Dimensions: `X 0.487 m`, `Y 0.0438 m`, `Z 0.61 m`

### Base_LeftThigh
- Location: `X -0.1771 m`, `Y -0.24999 m`, `Z -0.14844 m`
- Rotation: `(0, 0, 0)`
- Scale: `X 0.180`, `Y 0.048`, `Z 0.550`
- Dimensions: `X 0.36 m`, `Y 0.0968 m`, `Z 1.10 m`

### Axis Reminder for Blender
- `X` = left / right
- `Y` = front / back
- `Z` = up / down
- Use `Numpad 1` for Front Orthographic before judging left/right placement.
- Do not assume coordinates from visual placement; check the `N` panel Transform values first.

## Current Visual Assessment
- Forearms were intentionally reduced in size and now look more proportional for the technical prototype.
- Sword now follows the player correctly, clears the ground, and reads clearly as a sword.
- Shield follows the player correctly.
- Sword and shield are still prototype geometry; visual polish is not the goal yet.

## Next Planned Combat Steps
1. Confirm / tune shield neutral position.
2. Implement Player blocking state.
3. Shield should block eligible physical melee attacks from the front.
4. Normal shield should not automatically block magic.
5. Add parry foundation with weapon timing.
6. Add `DamageInfo` / attack metadata so damage can carry type, direction, source, hit point, etc.
7. Add body-part hitboxes to the humanoid prototype.
8. Add localized body damage / injury states.
9. Add target-lock / body-part targeting prototype.
10. Add crouch / specialized evasion interactions.

## Combat Design Rules Already Chosen
- Shield block and sword parry are distinct mechanics.
- Shield is primarily for physical/melee defense; ordinary shields do not automatically negate magic.
- Perfect sword parry should prevent damage and create a meaningful counter/stagger opportunity.
- Berserker-style defense may use a weapon clash instead of conventional parry.
- Body-part targeting should eventually support meaningful consequences such as leg injuries, blindness, bleeding, disarming, and possibly later dismemberment if it improves the game.
- Dismemberment is not committed yet; first prove localized damage and disabled-limb gameplay.

## Camera Direction
- Game is intended to support both third-person and first-person modes eventually.
- Third person is likely the primary combat/readability view.
- First person should support exploration, precision targeting, perception/eye abilities, and optionally combat.

## World / Story Direction
- The dungeon begins as a cave and descends deeper and deeper into increasingly unknown regions until the ultimate depths / final boss.
- The protagonist genuinely enjoys adventuring, combat, monsters, exploration, and descending with companions.
- In the early story, the original adventuring group discovers an ancient magic circle.
- The circle transports them to a very deep, unexplored section of the cave/dungeon.
- The companions die during the attempt to survive / escape.
- The protagonist survives and must find a way out of the deep dungeon alone.
- A time skip of roughly 2–3 years is planned.
- He later has new friends/companions and continues adventuring because he still loves it, but carries lasting guilt and grief over the original group.
- Soul perception/manipulation/binding is NOT an early power.
- Soul-related powers arrive much later in the game.
- Once he eventually gains those powers, returning to the place where his original companions died becomes an important future story possibility.
- The fate/state of the original companions' souls is intentionally undecided and should remain a major story mystery for now.

## Design Philosophy
- Prototype systems first, then validate them with a polished vertical slice.
- Do not wait until the very end to introduce real characters/environment art.
- Keep gameplay root and visual model separated so the technical humanoid can later be replaced by the final rigged character without rebuilding Player movement/combat systems.
- Ask the user before making major decisions about map layouts, camp layout, cave biome layouts, or world geography.
