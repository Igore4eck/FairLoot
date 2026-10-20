# Fair Loot

*The spoils go to those who earned them, and death to those who asked for it.*

A small single-player mod for **Mount & Blade II: Bannerlord 1.4.8**. The base game leaves two things to chance, and Fair Loot ties both to what actually happened on the field: **what you take home** after a battle, and **who dies** in it.

By **Igore4eck** (Ihor Prokofiev). Download: [Nexus Mods](https://www.nexusmods.com/mountandblade2bannerlord/mods/14144) · Source: [github.com/Igore4eck/FairLoot](https://github.com/Igore4eck/FairLoot). Українська версія: [README_UK.md](README_UK.md).

---

## The idea

In the base game the loot screen is a lottery. You can hide behind your shield wall all battle and still walk away with a noble's lamellar, while the man you unhorsed and cut down yourself leaves you nothing. Whether a fallen soldier dies or is only knocked out does not depend on the blow at all: a dagger scratch and a couched lance at full gallop get the same roll of the dice.

Fair Loot changes both:

- **Your kills are yours.** Everything worn by an enemy you killed with your own hand (weapons, armor, the horse and its harness) goes to you after a victory. If you want plate, take it off its owner yourself.
- **The baggage train is not bottomless.** The ordinary pile of post-battle loot is cut, by default in half: your soldiers grab their share, things get broken, lost or left behind.
- **The blow decides.** A weak final blow usually leaves the victim alive. A heavy one kills more often. A truly crushing one (100 damage by default) kills anybody, lords included.

Nothing is added to the world: no new items, no new troops, no save data. The mod only changes how the game hands out what is already there.

---

## Requirements

- Bannerlord **1.4.8** (single player, campaign).
- **[Mod Configuration Menu v5](https://www.nexusmods.com/mountandblade2bannerlord/mods/612)** (MCM) and the libraries it needs: Harmony, ButterLib, UIExtenderEx. Fair Loot itself does not use Harmony.

No other mods are needed. Fair Loot does not depend on RBM or any overhaul, but works with them (see *Compatibility*).

## Installation

1. Extract the `FairLoot` folder into `...\Mount & Blade II Bannerlord\Modules\`.
2. Enable **Fair Loot** in the launcher, below MCM and its libraries.
3. Settings: in game, **Options → Mod Options → Fair Loot**.

You **can add the mod to a running campaign or remove it**: it keeps nothing in the save.

---

## How it works

### 1. Ordinary loot is reduced

When your party collects loot after a victory, Fair Loot removes a share of the items (by count, not by value).

- Default: **50% kept**. At 100% the feature is off; at 0% only your personal loot remains.
- Every item has the same chance to be removed, so cheap and expensive items are thinned out evenly. Fractions are rounded at random, so on average exactly the chosen share stays.
- Only **your party's** item loot is affected. Gold, prisoners, AI parties and battles you are not in are not touched.

### 2. Personal loot: the gear of those you killed

During a battle Fair Loot records every enemy **killed by your hero personally**. After a **victory** the full equipment each of them went into battle with is added to your loot, **on top of** the reduced ordinary loot:

- all weapon slots, helmet, body armor, gloves, boots, cape, **horse and harness**;
- with their item modifiers: a rusty sword stays rusty, a masterwork one stays masterwork;
- banners, quest items and items that cannot be traded are skipped.

What does **not** count:

- **Knockouts.** Only a kill counts, so the death curve below also decides whether you get the gear.
- **Kills by allies, your troops or companions.** Other people's feats are not yours.
- **Siege engines and ballistae** (naval ones included), even when you are the one aiming.
- **Killing only the horse.** The rider has to fall by your hand.

A message after the battle shows how many personal items were added.

Within one encounter (for example a battle fought in several waves) the kills add up. **If you retreat**, your personal loot is not lost: it stays tied to that battle, and you get it if your side still wins (by *Send troops* or by the troops who stayed on the field). **A defeat loses it.**

### 3. The final blow decides death

The game first calculates the usual chance that a fallen character dies instead of being knocked out (medicine, perks, level and the campaign's death settings). Fair Loot then adjusts that chance by the damage of the **final blow**: the HP damage after armor, the number you see in the hit report.

The adjustment is a continuous piecewise-linear curve with these anchor points:

| Final blow | Soldiers | Heroes (lords, companions, you) |
|---|---|---|
| up to 8 | chance to **survive** ×5 | chance to survive ×3 |
| 25 | the game's base chance, unchanged | (no anchor) |
| 40 | chance to **die** ×2 | chance to die ×1.5 |
| 65 | chance to die ×3 | chance to die ×2 |
| **100+** (setting) | **certain death** | **certain death** |

Between the anchors the chance changes linearly, and every value is kept within 0–100%. Heroes have softer factors and no anchor at 25: between 8 and 40 their curve is a straight line. At the neutral point the result equals the game's own chance: exactly 25 damage for soldiers and about 29 damage for heroes, whatever the base chance is.

Examples (chance of death by final-blow damage):

| Base chance of death | 5 | 15 | 25 | 30 | 40 | 50 | 65 | 80 | 100 |
|---|---|---|---|---|---|---|---|---|---|
| Soldier, 20% | 0% | 8% | 20% | 27% | 40% | 48% | 60% | 77% | 100% |
| Soldier, 50% | 0% | 21% | 50% | 67% | 100% | 100% | 100% | 100% | 100% |
| Hero, 10% | 0% | 3% | 8% | 10% | 15% | 17% | 20% | 54% | 100% |
| Hero, 0% | 0% | 0% | 0% | 0% | 0% | 0% | 0% | 43% | 100% |

This applies to **every human in the battles you fight**: enemies, your soldiers, companions, lords and (unless you turn it off) your own hero. Autoresolved battles (*Send troops*) are not affected.

**Characters who cannot die.** The game or a quest protects some characters from death. Fair Loot keeps that protection for any blow below the certain-death damage and lifts it for a blow at or above it.

### 4. Leaving the field without leaving the battle

In the base game, staying past the red line at the edge of the map ends the battle for you and sends you back to the campaign map as a retreat. With Fair Loot, in a **campaign field battle**:

- stay outside the red line for about **9.5 seconds** (just before the game's own 10-second retreat) **while someone on your side is still fighting**;
- your hero **leaves the field alive and unhurt**, like a retreating soldier, and does not come back to this battle;
- the battle goes on as after a knockout: the AI takes over your formations, the camera switches to the spectator camera, and **Tab** opens the scoreboard with fast forward and the button to end the battle.

If nobody on your side is still fighting, the game's usual retreat happens instead. Sieges, hideouts and naval battles keep the base-game behavior.

---

## Settings (MCM)

**Options → Mod Options → Fair Loot.** Changes apply at once, no restart needed.

| Group | Setting | Default | Range | What it does |
|---|---|---|---|---|
| Loot | **Ordinary loot kept** | 50% | 0–100% | Share of the ordinary post-battle loot that stays. 100% = no reduction. |
| Loot | **Personal loot** | on | on/off | After a victory, adds the full gear of enemies you killed yourself. |
| Death in battle | **Final blow decides death** | on | on/off | Turns the death curve on or off. Off = the game's standard rules for everyone. |
| Death in battle | **Certain-death damage** | 100 | 70–300 | A final blow of this much damage or more always kills, lords included. Moves the last anchor of the curve; the 8/25/40/65 anchors stay. |
| Death in battle | **Apply to your hero** | on | on/off | Off = your own hero keeps the game's standard death rules. |
| Misc | **Leave the field and watch** | on | on/off | The red-line behavior described above. Off = the base game's retreat. |
| Misc | **Welcome message** | on | on/off | A short overview of the mod when a new campaign starts. |

---

## What you give up (read before you play)

Fair Loot adds a few rules the base game does not have. Each of them can be turned off in the menu.

- **Less loot overall.** By default half of the ordinary loot disappears. Personal loot makes up for it only if you fight yourself.
- **No loot from knockouts.** If your victim survives (weak final blow), you get nothing from them.
- **Lords and your hero can die from heavy blows, even if the campaign's death setting forbids it.** Fair Loot works on top of the campaign's *Battle death* option: when that option sets the base chance to 0%, blows of 65 damage and more can still kill (43% at 80 damage, certain at 100). The same goes for **blunt** weapons, which never kill in the base game. To keep your hero safe, turn off *Apply to your hero*; to raise the bar for everyone, raise *Certain-death damage*.
- **Leaving the field is final.** A hero who left through the red line cannot return to that battle. If your side then loses, it is your defeat, and your personal loot is lost.
- **Custom Battle is not affected.** The mod works only in the campaign.

---

## Compatibility

- **RBM (Realistic Battle Mod):** works with RBM 4.5.x (Fair Loot touches nothing RBM changes). RBM's heavier blows mean more fatal hits; if that is too much, raise *Certain-death damage*.
- **Other loot mods:** Fair Loot reduces the loot at the moment the game collects it. Loot that another mod adds *after* that moment is not cut.
- **Mods that replace the "killed or knocked out" model:** Fair Loot wraps the model registered before it. A mod loaded later that replaces the same model can switch the death curve off. Load Fair Loot after such mods.
- **Saves:** no save data. Safe to add or remove mid-campaign.

## Languages

English, Brazilian Portuguese, Chinese (Simplified and Traditional), French, German, Italian, Japanese, Korean, Polish, Russian, Spanish, Turkish.

---

## Building from source

Requires the .NET SDK (any recent version; the project targets .NET Framework 4.7.2).

```
dotnet build Source/FairLoot/FairLoot.csproj -c Release -p:GameFolder="<Bannerlord folder>" -p:McmFolder="<MCM module folder>"
```

The DLL goes straight to `bin/Win64_Shipping_Client`. `Source/Verification` holds a small test of the death curve: `dotnet run --project Source/Verification`.

## License

[MIT](LICENSE): use the code freely, keep the copyright notice.
