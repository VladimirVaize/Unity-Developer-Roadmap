# 🎯 Task: «Dynamic Audio Environment for a Dungeon»
You are developing a dungeon in an RPG game. 
You need to create a dynamic sound system that responds to:
1. Different dungeon zones (cave, echoing hall, water room)
2. Player states (normal mode, combat, stealth)
3. Events (opening a chest, activating a trap)

## 📝 What to Implement:
### Part 1: Creating AudioMixer and Groups
1. Create an AudioMixer named `DungeonMixer`
2. Create the following group hierarchy:
   - `Master`
     - `Music`
     - `SFX`
       - `Footsteps`
       - `UI`
       - `Combat`
      
     - `Ambience`
       - `Drip`
       - `Wind`
      
     - `Voice`

### Part 2: Effects and Snapshots
3. Add effects:
   - Add Lowpass Filter to the `Master` group
   - Add Echo to the `Voice` group
   - Add Distortion to the `SFX/Combat` group (for explosions)
  
4. Create snapshots:
   - `Normal` — default settings
   - `Underwater` — Lowpass: 600 Hz, Ambience volume: -5 dB
   - `Cave` — Reverb: 2 sec decay, Echo on Voice
   - `CombatIntense` — Distortion on SFX/Combat: 0.3, music: +3 dB
   - `StealthMode` — Music: -15 dB, Footsteps: -8 dB
  
### Part 3: Control Scripts
5. Write script `DungeonAudioZone.cs`:
   - On trigger zones, switch snapshots
   - Enter water zone → `Underwater`
   - Enter cave zone → `Cave`
   - Exit zone → `Normal`
  
6. Write script `CombatAudioHandler.cs`:
   - Method `StartCombat()` → transition to `CombatIntense` in 0.3 sec
   - Method `EndCombat()` → return to `Normal` in 1 sec
   - On taking damage → play sound through `SFX/Combat` group with distortion
  
7. Write script `TreasureChest.cs`:
   - On chest open:
     - Temporarily (for 0.5 sec) switch to a snapshot (e.g., `TreasureGlow`)
     - Play sound with Reverb effect (spatial echo)
     - Return to previous snapshot
    
### Part 4: Exposed Parameters
8. Expose the following parameters for code control:
   - `MasterVolume`
   - `MusicVolume`
   - `SFXVolume`
   - `LowpassCutoff`
   - `EchoWetMix`
  
9. Implement UI sliders to adjust music and SFX volume in real time.

---

## 🧰 Implementation Requirements:
- Use at least 3 different snapshots
- Use at least 3 effects (Lowpass, Echo, Reverb/Distortion)
- All snapshot transitions must be smooth (> 0.2 sec)
- Add comments to each public method
- In `DEVELOPMENT_BUILD`, log snapshot switching info to console

---

## 🔍 Verification:
1. Verify that in the water zone, sound becomes "muffled" (Lowpass)
2. Verify that in the cave, voice has echo
3. Verify that when combat starts, music becomes louder and hit sounds are distorted
4. Verify that when opening a chest, the sound "spreads" with reverb
5. Verify that volume sliders work and save values between scenes

---

## 💡 Expected Console Output (DEVELOPMENT_BUILD):
```text
[Audio] Player entered Water zone → Switching to Underwater snapshot (0.5s)
[Audio] Lowpass cutoff set to 600 Hz
[Audio] Player entered Cave zone → Switching to Cave snapshot (0.8s)
[Audio] Echo enabled on Voice group
[Audio] Combat started → CombatIntense snapshot (0.3s)
[Audio] Chest opened → TreasureGlow snapshot (0.2s) then revert
[Audio] Chest sound played with Reverb effect
[Audio] Combat ended → Normal snapshot (1s)
```

---

## 🏆 Bonus Task (Optional):
Implement Sidechain Compression (Ducking):
- When a dialogue starts, music automatically becomes quieter by 6 dB
- Use the Send effect from the `Music` group to a compressor
- Control via `DialogueTrigger` script

---

### ⭐ If this project was useful, put a star on GitHub!
