# Tiny Necromancer — Game Design (Living Document)

> Working title. The game may be renamed later as the world and identity become broader than necromancy alone.

## Core Vision
A stylized 3D fantasy action roguelite/RPG where the player descends deeper and deeper into a dangerous underground world. People are born with an innate class or supernatural gift, but they can also learn additional combat disciplines, magic, summons, and rare powers.

The game should be easy to understand, satisfying to control, highly replayable, and built around the feeling of becoming dramatically more powerful over time.

## Camera & Presentation
- 3D stylized / low-poly art direction.
- Elevated third-person / 3-quarter camera.
- Player and companions remain clearly visible during combat.
- Visual progression should be obvious: stronger abilities, more impressive effects, rarer enemies, and changing environments as the player descends.

## World Structure
### Surface Camp
The player begins from a small camp near the entrance to the underground area.

The camp can grow over time as NPCs are rescued or unlocked.
Possible services later:
- Merchant
- Blacksmith
- Alchemist
- Trainer
- Relic / magic specialist

The camp is intentionally small at first so development remains manageable.

### Descent
The player enters the cave/dungeon and progresses through increasingly difficult floors or zones.

General loop:
1. Prepare at camp.
2. Enter the underground area.
3. Fight enemies and explore.
4. Gain XP, items, abilities, souls, or other rewards.
5. Descend deeper.
6. Fight bosses / special encounters.
7. Return to camp, die, or activate a progression checkpoint depending on the run.
8. Improve the character and attempt a deeper run.

The deeper the player goes, the stranger and more dangerous the world becomes.

## Character Progression
### Level
- The player gains levels through combat and exploration.
- Level affects stats and unlocks access to stronger systems.
- Soul capacity increases with progression rather than allowing unlimited summons from the beginning.

### Core Resources
Planned core resources:
- Health
- Mana
- Stamina
- XP

Additional resources may be introduced only if they serve a clear gameplay purpose.

## Innate Class / Birth Gift
Every person in the world can be born with an innate class, gift, affinity, or supernatural trait.

Examples of possible archetypes:
- Arcane Swordsman
- Elemental Mage
- Martial Artist
- Summoner
- Soul Binder / Necromancer
- Healer / Support
- Rare ocular / perception gift

The player's starting class should significantly influence early gameplay without permanently preventing experimentation with other systems.

## Magic System
The game will support mana-based abilities.

Possible categories:
- Elemental magic
- Arcane magic
- Defensive magic
- Soul magic
- Summoning
- Utility / movement magic

Spells should feel mechanically different, not just be recolored damage projectiles.

## Martial Arts / Physical Combat
Players should be able to build characters that rely heavily on physical combat rather than magic.

Planned features over time:
- Basic attacks
- Combos
- Dodge
- Stamina management
- Special techniques
- Weapon-based and unarmed styles

## Rare Eye / Perception Powers
Rare characters may possess special ocular or perception-based abilities.

These should create unique gameplay mechanics rather than simple stat buffs.

Example directions:
- Reveal enemy weak points.
- See hidden creatures, souls, or passages.
- Predict attack paths for a short duration.
- Perceive magical structures or vulnerabilities.

Designs must remain original and not reproduce protected characters, names, visuals, or abilities from existing anime or games.

## Soul / Necromancy System
Necromancy is an important power path, but it is not the entire game.

### Soul Capacity
The player has a limited Soul Capacity that increases through level/progression.

Different creatures can consume different amounts of capacity.
Example concept:
- Small creature: 1 slot
- Skilled humanoid: 1–2 slots
- Large monster: 2–3 slots
- Boss-tier soul: 3+ slots

This forces meaningful party/build decisions instead of allowing an unlimited army.

### Soul Usage
Possible uses for captured souls:
- Bind / summon the creature.
- Consume for another benefit.
- Upgrade or transform an existing summon.
- Use as a crafting or progression resource.

Exact rules will be prototyped before finalizing.

## Summoning
Summons should:
- Follow the player intelligently.
- Acquire enemies without excessive micromanagement.
- Have clear roles such as tank, melee damage, ranged damage, support, or control.
- Remain readable on screen even when several are active.

## Death & Resurrection
The game may include a resurrection mechanic inspired by the broader fantasy idea of returning after death, but it will have original rules, presentation, lore, and terminology.

Working concept: **Soul Anchor**.

Potential behavior:
- Certain locations allow the player's soul to anchor.
- Death can return the player to an anchor.
- Some information or progression may persist.
- The world may not always reset perfectly.
- Repeated deaths could influence world state, encounters, or corruption.

This system is a later milestone and will not be implemented until the core combat loop is fun.

## Art Direction
- Stylized 3D / low-poly.
- Strong silhouettes and readable combat effects.
- Main character begins as a relatively normal fantasy adventurer rather than an obvious overpowered dark lord.
- Character appearance can evolve visually as power grows.
- Soul/summon visuals will use an original visual language rather than copying recognizable anime aesthetics.

## Current Prototype Scope
The first playable prototype is intentionally small.

### Already working
- Basic player movement.
- Smooth acceleration/deceleration.
- 3-quarter camera follow.

### Next milestones
1. Camera-relative movement.
2. Basic health system.
3. Basic melee attack.
4. One enemy with simple AI.
5. Damage and death.
6. Mana resource.
7. One spell.
8. XP and level-up.
9. First innate class prototype.
10. Soul capture and one summon only after the core combat feels good.

## Development Rules
- Build one system at a time.
- Test immediately after implementation.
- Fix critical bugs before expanding scope.
- Keep `main` in a working state.
- Use GitHub commits as safe checkpoints.
- Use feature branches for larger systems once the project becomes more complex.
- Avoid adding systems only because they sound cool; every system must improve the core gameplay loop.

## Inspirations vs. Originality
Anime and games can inspire feelings such as progression, discovery, power growth, resurrection, summons, and rare abilities. The project must use original names, characters, lore, art direction, interfaces, animations, powers, dialogue, and specific mechanics so that it develops its own identity.

## Core Design Goal
The player should regularly think:

> "I want to go one floor deeper and see what power, enemy, item, or ability I find next."

That feeling of curiosity + progression + build experimentation is the heart of the game.
