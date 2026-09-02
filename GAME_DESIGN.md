# Tiny Necromancer — Game Design (Living Document)

> Working title. The game may be renamed later as the world and identity become broader than necromancy alone.

## Core Vision
A stylized 3D fantasy action roguelite/RPG built around descending deeper and deeper through a vast underground dungeon that begins as a natural cave and becomes stranger, older, and more dangerous the farther the player goes.

The player should not win only by having bigger numbers. Knowledge, timing, perception, environmental awareness, enemy behavior, body positioning, equipment, and build choices should matter.

Core fantasy:
- Begin as a relatively normal adventurer.
- Descend into a cave that seems ordinary at first.
- Discover powers, relics, strange classes, souls, and ancient systems.
- Fight increasingly intelligent and mechanically distinct enemies.
- Grow from survivor into something extraordinary.
- Always feel tempted to go one level deeper.

## Camera & Presentation
The game should support both:
- Third-person combat/exploration.
- First-person combat/exploration.

The player can switch perspectives with a button.

### Third Person
Best for:
- Spatial awareness.
- Melee combat.
- Multiple enemies.
- Summons/companions.
- Reading body positioning and dodges.

### First Person
Best for:
- Immersion.
- Precise aiming.
- Inspecting environments.
- Targeting body parts.
- Using perception/eye abilities.

Final character architecture must support both modes without changing the core gameplay logic.

## World Structure
### Surface Camp
The game begins at a small camp near the entrance to the cave.

The camp can grow as NPCs are rescued, recruited, or discovered.
Possible services:
- Merchant
- Blacksmith
- Alchemist
- Trainer
- Relic / magic specialist
- Class or title specialist

The camp starts intentionally small and becomes a visible record of player progress.

### The Descent
The dungeon is not a separate magical tower placed somewhere in the world. It begins as a cave.

The player enters the cave and keeps descending:

Surface
↓
Natural caves
↓
Older tunnels
↓
Buried structures
↓
Ancient ruins
↓
Strange underground biomes
↓
Civilizations / sealed regions / impossible spaces
↓
Deep bosses
↓
The final depths
↓
The final boss

The deeper the player travels, the more the environment should raise questions about who created these places, what was buried below the world, and why powerful creatures remain there.

General loop:
1. Prepare at camp.
2. Enter the cave.
3. Explore and fight.
4. Learn how enemies and environments behave.
5. Gain XP, items, titles, abilities, souls, equipment, and knowledge.
6. Descend deeper.
7. Defeat floor/region bosses and unlock new depths.
8. Return, die, resurrect, or reach a progression checkpoint.
9. Improve the character and attempt a deeper descent.

## Combat Philosophy
Combat should reward observation and intentional decisions rather than repeated basic attacks.

The player should ask:
- What kind of enemy is this?
- How does it attack?
- What can it see or hear?
- What part of its body is vulnerable?
- Should I block, parry, dodge, crouch, clash weapons, use magic, or manipulate the environment?
- Can I disable the enemy instead of simply reducing HP to zero?

## Core Defensive Mechanics
### Dodge
Dodge can avoid physical and magical attacks when timed and positioned correctly.

Different evasive movements may counter different attack heights and directions.

Example:
- Horizontal attack at chest/head height → crouch + backward dodge/lean can avoid it.
- Vertical strike → sidestep may be better.
- Low sweep → jump or another evasive option may be required.

### Shield Block
A normal physical shield should primarily block physical/melee attacks coming from a valid direction.

Rules:
- Blocking is directional.
- Attacks from behind should normally bypass the shield.
- Blocking can consume stamina/posture.
- Heavy impacts can cause recoil or guard break.
- Ordinary shields do not automatically block magic.
- Special enchanted shields may later have magical defensive properties.

### Sword Parry
A correctly timed sword parry can completely negate a compatible melee attack.

Perfect parry concept:
- 0 damage.
- Prevents certain enemy techniques such as disarm.
- Staggers or opens the opponent.
- Creates a counterattack window.

Magic is normally not parried by an ordinary sword unless a specific ability/equipment rule says otherwise.

### Berserker Weapon Clash
Some classes should defend differently.

A Berserker may replace traditional parry with an offensive weapon clash:
- Enemy attacks.
- Berserker attacks the incoming weapon during the timing window.
- Weapons collide.
- Attack can be cancelled or converted into a strength/posture contest.

Class identity should change how the player solves combat, not only change damage percentages.

## Physical Combat & Hit Information
The combat architecture should eventually carry more information than simple HP damage.

A hit should be capable of describing:
- Damage amount.
- Damage type (slash, blunt, pierce, fire, etc.).
- Attacker/source.
- Hit position.
- Body part hit.
- Attack direction.
- Force/stagger.
- Whether the attack can cause special injury effects.

This allows combat systems to react to what actually happened.

## Body-Part Damage & Injuries
Humanoid and compatible creatures may use localized hit zones such as:
- Head
- Torso
- Left arm
- Right arm
- Left leg
- Right leg

Possible consequences:
- Leg/tendon damage → reduced mobility, kneeling, falling, instability.
- Arm damage → weaker/slower attacks, inability to use certain weapons.
- Eye damage → reduced perception, sight range, accuracy, or field of view.
- Throat wounds on compatible living creatures → bleeding over time.
- Head attacks → high-risk/high-reward critical opportunities.

Not every creature uses the same anatomy. Undead, constructs, slimes, insects, monsters, and bosses can have different body systems.

### Dismemberment
Dismemberment is a possible later mechanic, not yet guaranteed.

Before committing to it, the prototype should prove that localized damage and disabled limbs improve gameplay. If implemented, losing a limb should affect combat behavior and abilities rather than being purely cosmetic.

## Target Lock / Body Targeting
The player should be able to fight freely or use a targeting system.

Possible lock-on behavior:
- Lock onto an enemy.
- Optionally select a body region such as head, torso, arm, or leg.
- Selected region influences attack orientation/aim.
- It does not guarantee a hit.

Enemies can still:
- Move.
- Block.
- Dodge.
- Parry.
- Protect injured body parts.

## Humanoid Combat Prototype
Current development uses a simple technical humanoid made from Unity primitives.

Purpose:
- Test body proportions.
- Establish hand attachment points.
- Equip sword and shield.
- Test block/parry direction.
- Add body hitboxes later.
- Prepare architecture for the final rigged character.

The prototype visual will eventually be replaced by the final 3D model while the Player root, gameplay scripts, CharacterController, health, movement, combat, and other logic remain.

## Character Progression
### Level
- Gain levels through combat, exploration, bosses, and discoveries.
- Level affects stats and unlocks stronger systems.
- Soul Capacity grows with progression instead of allowing an unlimited army immediately.

### Core Resources
Planned resources:
- Health
- Mana
- Stamina / posture-related resource
- XP

Additional resources should only exist if they add meaningful decisions.

## Innate Class / Birth Gift
People in this world can be born with innate classes, gifts, affinities, or supernatural traits.

Possible archetypes:
- Arcane Swordsman
- Elemental Mage
- Martial Artist
- Summoner
- Soul Binder / Necromancer
- Healer / Support
- Berserker
- Rare ocular/perception user

A starting class should strongly influence early gameplay but should not prevent experimentation with other systems.

## Rare Eye / Perception Powers
Three current eye concepts:

### Eye of Clairvoyance
Possible functions:
- Detect presence or movement through obstacles under limited conditions.
- Reveal hidden threats or paths.
- Perceive traces left by creatures.

Should have limitations such as mana drain, range, duration, overload, or incomplete information.

### Mana Detection Eye
Possible functions:
- See mana signatures.
- Detect enchanted objects.
- Reveal magical traps or active spells.
- Estimate magical intensity.

### Analytical Eye
Possible functions that unlock progressively:
- Identify creature type.
- Estimate level/threat.
- Read approximate health.
- Reveal injured body parts.
- Discover resistances or weaknesses.
- Identify known abilities after sufficient observation.

The Analytical Eye should not reveal every secret instantly. Information can improve through level, observation time, knowledge, and experience.

## Titles
The player can earn titles through accomplishments and discoveries.

Example:
**Wolf Hunter**
- +5% damage against wolves/related monsters.

The player can change the currently equipped title with a button/menu.

Current direction:
- Only one or a small number of titles can be active at once.
- Titles can provide combat bonuses, social effects, reputation effects, or special interactions.
- Titles should represent accomplishments and choices rather than becoming an unlimited pile of passive buffs.

## Magic System
Mana-based abilities are planned.

Possible categories:
- Elemental magic
- Arcane magic
- Defensive magic
- Soul magic
- Summoning
- Utility / movement magic

Spells should have mechanical identities, not simply different-colored projectiles.

Magic should interact with the physical combat rules. For example, an ordinary shield or sword parry does not automatically stop a magical attack.

## Environmental Interaction
The environment should be a system, not only decoration.

### Fire, Smoke & Enclosed Spaces
Example concept:
- The player starts a fire inside a poorly ventilated small cave.
- Smoke/gases accumulate.
- Visibility decreases.
- Creatures and the player can suffer breathing/poison/asphyxiation effects.
- A careless environmental solution can kill everyone in the room, including the player.

### Creature-Specific Environmental Knowledge
Example concept:
- Burning a particular plant/branch produces smoke that spiders strongly avoid.
- Instead of fighting them directly, the player can force them to flee.

The game should reward learning these relationships.

Possible future systems:
- Fire
- Smoke
- Ventilation
- Water
- Poison/gas
- Light/darkness
- Creature fears
- Environmental resistances

These systems should be introduced carefully so they remain understandable and performant.

## Enemy Intelligence & Behavior
Enemies should not all behave like generic attackers.

Current AI foundation includes:
- Vision/FOV.
- Hearing.
- Last known position.
- Investigation/search behavior.

Future enemy intelligence can include:
- Memory.
- Personality.
- Honor/cowardice/aggression.
- Team coordination.
- Perception damage.
- Adaptation to repeated player strategies.

Enemies should reason from what they can perceive, not cheat using hidden player information.

## Enemy / Boss Concepts
### The Dancer
Working concept: extremely agile monster/enemy.

Identity should focus on:
- Movement.
- Difficult attack rhythms.
- Dodging/repositioning.
- Punishing predictable attacks.

Details TBD.

### Samurai
A skilled, respectful warrior whose behavior is governed by honor.

Core rules/concepts:
- Prefers 1v1 combat.
- Does not want allies interfering in an honorable duel.
- Uses disciplined sword techniques.
- Can disarm the player.
- If the player's weapon is knocked onto the floor, the player can attempt to retrieve it.
- The Samurai can position himself to prevent easy recovery.
- If both combatants become disarmed, the fight can transition into unarmed combat.
- A perfect parry can defeat/prevent certain disarm techniques.
- His behavior should express character, not only combat stats.

#### Dungeon 1 Final Boss — Samurai (Current Concept)
At the end of the first major dungeon region, the Samurai is a candidate final boss.

When defeated, instead of behaving like a generic enemy:
- He accepts defeat.
- He kneels.
- The encounter transitions into a deliberate finishing moment.
- The player ends the duel with a decapitation/finisher sequence.

The presentation should emphasize respect, consequence, and the Samurai's code rather than random gore.

### Minotaur
Possible major enemy/boss concept:
- Heavy charge attacks.
- Strength-based weapon clashes.
- Can force the player into a weapon lock/struggle.
- An ancestral strike is a signature high-threat attack.

Weapon-lock concept:
- Strength, stamina/posture, timing, equipment, and player decisions influence the contest.
- The Minotaur can progressively overpower the player.
- Losing completely may trigger a severe finisher/failure state.

Avoid reducing this to simple button mashing.

## Soul / Necromancy System
Necromancy is an important path, but not the entire game.

### Soul Capacity
The player has limited Soul Capacity that increases through progression.

Different bound creatures consume different capacity.

Example:
- Small creature: 1 slot
- Skilled humanoid: 1–2 slots
- Large monster: 2–3 slots
- Boss-tier soul: 3+ slots

### Soul Usage
Possible uses:
- Bind/summon a creature.
- Consume a soul for another benefit.
- Upgrade or transform a summon.
- Use souls in crafting/progression.

Exact rules remain experimental.

## Summons & Intelligent Companions
Summons should:
- Follow intelligently.
- Understand combat roles.
- Avoid excessive micromanagement.
- Remain readable on screen.

Long-term goal:
The player can give natural-language-style orders to advanced companions/souls, while the game converts those orders into bounded actions such as follow, protect, scout, attack, retreat, distract, or use an ability.

AI reasoning should occur on meaningful events/state changes, not every frame.

## Death & Resurrection
Working concept: **Soul Anchor**.

Possible rules:
- Certain places allow the player's soul to anchor.
- Death returns the player to an anchor.
- Some knowledge/progression can persist.
- The world may not reset perfectly.
- Repeated deaths may affect corruption, encounters, or world state.

This must have original lore and rules.

## Lore — Draft v0.1
This section is intentionally a starting point, not final canon.

### The World Above
Civilization has existed for centuries on top of something it does not fully understand.

People know magic exists. They know individuals are sometimes born with strange classes, affinities, or gifts. These powers are treated as part of life, religion, warfare, social status, and fear.

But nobody agrees on why these gifts exist.

Some believe they come from gods.
Some believe they are inherited fragments of ancient bloodlines.
Some scholars believe the powers are leaking upward from something beneath the earth.

### The Cave
Near the starting settlement is a cave known for monsters, missing travelers, and valuable materials.

At first it appears to be only a dangerous natural cave.

But explorers who go deeper discover things that should not exist beneath an ordinary mountain:
- Stone corridors older than known kingdoms.
- Sealed doors.
- Weapons from cultures nobody recognizes.
- Underground ecosystems without sunlight.
- Creatures carrying unnatural amounts of mana.
- Human ruins far below any recorded civilization.

The cave is not simply a cave.
It is the entrance to layers of history buried beneath the world.

### The Central Mystery
Current lore direction:
The deeper layers contain evidence that the world's supernatural gifts, monsters, souls, and perhaps even resurrection are connected to an ancient force/system buried below civilization.

The player begins without knowing this truth.

Every major depth should answer one question while creating a larger one.

Example progression:
- Why are monsters appearing near the surface?
- Who built the first ruins?
- Why do some enemies behave like guardians rather than monsters?
- Why are human classes/gifts related to powers found underground?
- Why can certain souls be bound?
- What is the Soul Anchor actually connecting to?
- What lies at the bottom?

### Bosses as Lore
Major bosses should not feel randomly placed.

Each important boss should have a reason to exist where it is.

The Samurai, for example, may not simply be a monster guarding a door. Possible lore directions to explore later:
- An ancient warrior who chose to remain and guard a passage.
- A warrior resurrected by the dungeon but who preserved his code of honor.
- A descendant/member of a hidden underground order.
- A soul bound to a duty he refuses to abandon.

His respectful duel and acceptance of death should tell the player something about the dungeon: not everything below is mindless evil.

### The Final Depth
The final boss and final truth are intentionally undefined.

The final boss should connect directly to:
- Why the dungeon exists.
- Why power/class gifts exist.
- Why souls can be manipulated.
- Why the player can resurrect or return.
- What has been influencing the surface world.

We should not choose the final answer until the world's themes and major factions are better developed.

## Art Direction
- Stylized 3D / low-poly foundation, capable of becoming visually rich and atmospheric.
- Strong silhouettes.
- Readable melee hit reactions.
- Clear weapon and shield positioning.
- Distinct enemy identities.
- Environmental atmosphere becomes darker/stranger with depth.
- Main character starts as a relatively normal adventurer.
- Power growth becomes visible through equipment, effects, marks, relics, eyes, and abilities.

## Current Development Roadmap
### Foundation — Working
- Player movement.
- Camera-relative movement.
- Dodge.
- Invulnerability frames.
- Health/damage/death.
- Basic melee attack.
- Enemy attack/telegraph.
- Enemy vision/FOV.
- Enemy memory / last-known position.
- Enemy investigation/search.
- Enemy hearing.
- Different player noise intensities.
- Health UI.

### Current Focus
1. Finish technical Humanoid Combat Prototype.
2. Add RightHandPoint + sword.
3. Add LeftHandPoint + shield.
4. Build richer hit information (`DamageInfo`).
5. Body hitboxes.
6. Target lock/body targeting foundation.
7. Shield block.
8. Sword parry / perfect parry.
9. Crouch and attack-height evasion.
10. Localized injuries.

### Following Systems
- Mana.
- Ability framework.
- First spell.
- Stamina/posture.
- XP/leveling.
- Titles.
- Eye abilities.
- Soul Capacity.
- Soul capture.
- First summon.

### Vertical Slice
After the core combat architecture proves itself:
- First real player model.
- First real enemy model.
- Proper animations.
- Sword/shield assets.
- VFX.
- Sound.
- Improved UI.
- Small polished cave section.
- First strong example encounter.

## Development Rules
- Build one system at a time.
- Test immediately.
- Fix critical bugs before expanding scope.
- Keep `main` stable.
- Use feature branches and GitHub checkpoints.
- Code/gameplay state in GitHub takes priority over memory or assumptions.
- Prototype mechanics before investing in final art.
- Do not build beautiful maps before we understand the space required by combat.
- Before designing major maps, camp layout, cave structure, or biomes, decide the intended experience together.
- A cool idea becomes a committed feature only after it improves the core game and is feasible.

## Inspirations vs. Originality
Games and anime can inspire feelings such as progression, powerful abilities, honorable duels, summons, resurrection, tactical combat, and rare perception powers.

The project must create original:
- Names.
- Characters.
- Designs.
- Lore.
- Visual language.
- Dialogue.
- Music.
- UI.
- Specific powers/mechanics and their presentation.

## Core Design Goal
The player should regularly think:

> "I want to go one level deeper. I want to know what is down there, what it can do, and whether I am ready for it."

Curiosity + danger + knowledge + progression + expressive combat are the heart of the game.
