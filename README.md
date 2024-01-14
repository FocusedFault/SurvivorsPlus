# SurvivorsPlus (WIP)

Adds acceleration to Artificer and MUL-T to match other survivors. Can configure off survivor changes and acceleration change.

## Commando

<details>

### Roll

- No longer stops sprinting
- Invincible on roll

### Suppressive Fire (Vortex Rounds)

- Damage increase 100% -> 200%
- Proc Coefficient increase 1 -> 1.5
- Reduces cooldown 9s -> 6s

### Grenade (Sticky Grenade)

- now with more stick
</details>

## Huntress

<details>

### Flurry

- Increased proc coefficient 0.7 -> 0.8
- Reduced delay duration 1.3s -> 1s

### Blink

- Reduced cooldown 7s -> 6s

### Phase Blink

- Reduced charges 3 -> 1
- Increased cooldown 2s -> 3s

### Arrow Rain (Electric Volley)

- Replaced slow with shock
- Increased damage 225% -> 450%
- Increased proc coefficient 0.2 -> 0.6
- Can now target flying enemies
</details>

## Bandit

<details>

### Hemorrhage

- Reduced length 15s -> 7.5s
- Reduced damage 2000% -> 1000%

### Blast (Hyperion Sharpshooter)

- Reduced recoil/bloom
- Highlights Weak Points that guarantee critical damage

### Serrated Dagger

- Applies 2 stacks of Hemorrhage instead of 1

### Smoke Bomb

- Increases invisibility duration 3s -> 5s
- Increases cooldown 6s -> 8s

### Lights Out (Open Wound)

- No longer resets cooldowns on kill
- Doubles Hemorrhage stacks (on crit)
- Applies Hemorrhage (on crit)
</details>

## Engineer

<details>

### Bouncing Grenades

- Removes charging mechanic, immediately fires 3 grenades

### Pressure Mines

- Removes arming mechanic, immediately explodes for 300% damage
- Reduced cooldown 7s -> 5s

### Bubble Shield

- Actually impenetrable (can go out but nothing can go in)
- Reduced time active 15s -> 10s

### TR58 Carbonizer Turret

- Increases range 25 -> 50
- Can now fire shurikens
- Makes firing smoother
</details>

## Mercenary

<details>

- Increases base and level regen to match other melee survivors (2.5/0.5)

### Blinding Assault

- Increases damage 300% -> 400%

### Focused Assault

- Increases damage 700% -> 800%

### Eviscerate

- Increases damage 110% -> 260%
</details>

## REX

<details>

### DIRECTIVE: Inject

- Increases proc coefficient 0.5 -> 0.8
</details>

## Acrid

<details>

### Blight

- Reduces base damage per tick 60% -> 30%
- Increases duration 5s -> 7.5s
- Stacks exponentially (Higher stacks deal more damage per tick)
</details>

## Captain

<details>

### Defensive Microbots

- No longer deletes puddles/pools

### Power Tazer

- Bounces to nearby enemies in a 15m radius
</details>

## Railgunner

<details>

- Crit damage now applies properly instead of being halved
- Reduces weakpoint size by 25% (mainly for Bandit)

### Smart Rounds

- Removed

### HH44 Marksman (HH44 Rounds)

- Is now the primary instead of smart rounds
- Reduces damage 400% -> 300%
- Cannot crit or hit weakspots with this ability
- Has an active reload system (uses secondary charges)
- Has an active reload bonus of +50% total damage

### M99 Sniper

- Reduces damage 1000% -> 600%

### Polar Field Device

- Reverses enemy projectiles instead of slowing them (does deal damage to enemies)

### Supercharge

- Reduces damage 4000% -> 2400%
- Reduces downtime 15s -> 10s

### Cryocharge

- Reduces damage 2000% -> 1200%
</details>

## Void Fiend

<details>

- Removes Crit mechanic
- Removes Heal mechanic
- Corruption only increases while in combat
- Corruption increase is reduced by 66%
- Corruption increases on kill
- Corrupted Mode no longer has +100 armor
- Corrupted Mode decreases slower
- Reduces Corrupted Mode transition time by 25%

</details>

## Credits

- Moffein [RiskyMod](https://thunderstore.io/package/Risky_Lives/RiskyMod/) for Captain code

## Changelog

**0.9.2**

- Decreases Supercharge downtime 15s -> 10s
- Increases Supercharge damage 2000% -> 2400%
- Reduces Weak Point size by 25%
- Changes Blast to Hyperion Sharpshooter (ty Maddie for the idea)

**0.9.1**

- Changes Railgunner's M1 to be closer to RoR1/RoRR Sniper
- Decreases Blight duration 10s -> 7.5s

**0.9.0**

- Fixes Railgunner Laser Scope multiplication being applied to all survivors

**0.0.8**

- Fixes crit items only applying half damage to Railgunner
- Swaps Railgunners M1 with HH44
- Reduced M99 damage
- Reduced HH44 damage
- Reduced Supercharge damage
- Fixes Acrid Blight description text
- Adds Captain changes
- Adds Void Fiend changes

**0.0.7**

- Fixes Railgunner M2 text being on the M1
- Reduces Supercharge and Cryocharge damage
- Adds Acrid change

**0.0.6**

- Decrease Evis damage 330% -> 260%
- Adds REX change
- Adds Railgunner changes

**0.0.5**

- Readds Arrow Rain damage buff
- Changes engi mobile turrets to fire shurikens
- Fixes Suppressive Fire to not force a delay after firing
- Increases duration/damage on Hemorrhage
- Increases Smoke Bomb duration and cooldown duration by 2s
- Reduces Proximity Mine cooldown 7s -> 5s
- Increases Blinding/Focused Assault damage
- Decreases Flurry duration between shots 1.3s -> 1s
- Decreases Lights Out (Open Wound) cooldown back to vanilla 6s -> 4s

**0.0.4**

- Adds config for enabling/disabling survivor changes
- Adds Merc changes
- Adds Artificer/MULT accel change
- Removes Arrow Rain damage buff
- Fixes engi bubble shield not flashing before deactivating

**0.0.3**

- Actually does the things in the patch notes this time
- Reduces Hemorrhage duration and damage 7.5s -> 5s 1000% -> 666%
- Reduces Arrow Rain damage 500% -> 350%
- Fixes additional Hemmorhage stacks not applying

**0.0.2**

- Reduces Hemorrhage damage
- Reduces Arrow Rain damage 500 -> 400
- Adds an extra Hemorrhage stack for Serrated Dagger
- Adds Engi changes

**0.0.1**

- Release
