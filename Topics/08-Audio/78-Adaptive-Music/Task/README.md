# 🎯 Task: «Adaptive Music for a Horror Game»
You are developing a first-person horror game. Music must adapt to the fear level and player actions. 
You need to implement an adaptive music system using vertical layering, snapshots, and dynamic parameters.

## 📝 What to Implement:
### Part 1: Creating Music Structure
1. Create 4 stems (AudioSource) for vertical layering:
   - `AtmosphereLayer` — constant low background (hum, wind)
   - `TensionLayer` — high-pitched tense notes (violins)
   - `ThreatLayer` — low bass, distant drum (monster approaching)
   - `PanicLayer` — chaotic noise, heartbeat (panic mode)
  
2. Create AudioMixer `HorrorMixer` with snapshots:
   - `Calm` — normal state
   - `Tense` — light tension
   - `NearThreat` — threat is close
   - `Panic` — chase/panic
  
### Part 2: Fear Level System
3. Implement `FearManager.cs` that tracks:
   - Distance to nearest enemy (0-30 meters)
   - Whether player sees enemy (true/false)
   - Player health percentage (100% → 0%)
   - Number of investigated items (fear accumulation rate)
  
4. Method `CalculateFearLevel()` returns a value from 0 to 1:
```text
Fear Level = (1 - (distance / 30)) * 0.4 +
             (sees enemy ? 0.3 : 0) +
             (1 - health) * 0.2 +
             (items_investigated / max_items) * 0.1
```

### Part 3: Adapting Music to Fear Level
5. In script `AdaptiveHorrorMusic.cs`:
   - `AtmosphereLayer` always active (volume 0.3-0.5)
   - `TensionLayer` active when fear > 0.3 (volume = fear)
   - `ThreatLayer` active when fear > 0.6 (volume = (fear-0.6)/0.4)
   - `PanicLayer` active when fear > 0.8 (volume = (fear-0.8)/0.2)
  
6. Snapshot switching:
   - Fear < 0.3 → `Calm`
   - 0.3 ≤ fear < 0.6 → `Tense`
   - 0.6 ≤ fear < 0.8 → `NearThreat`
   - fear ≥ 0.8 → `Panic`
  
### Part 4: Game Events Affecting Music
7. Implement special events:
   - `OnPlayerSpotted()` — enemy spotted player → instant fear = 0.7
   - `OnItemCollected()` — adds fear +0.05 (max 0.4 total)
   - `OnEscape()` — fear drops rapidly to 0.1 over 2 seconds
   - `OnDeath()` — play special sound through `Death` snapshot
  
### Part 5: Additional Effects
8. When fear > 0.7, enable Lowpass Filter on master with cutoff = 5000 - (fear * 4000) Hz
9. When fear > 0.8, add random "screamers" (short sounds) via separate AudioSource

---

## 🧰 Implementation Requirements:
- Use minimum 4 layers and 4 snapshots
- All volume changes must be smooth (via `Mathf.Lerp` in Update)
- Switch snapshots with `TransitionTo` and 0.5-1.5 second fade
- Add debug mode (F1 key) to visualize fear level and layer volumes on UI

---

## 🔍 Verification:
1. When player is far from enemies → fear ~0.1 → only `AtmosphereLayer` plays
2. When enemy approaches to 10 meters → fear ~0.4 → `TensionLayer` + `Tense` snapshot
3. When enemy visible at 3 meters → fear ~0.7 → `ThreatLayer` + `NearThreat` snapshot
4. If health drops to 20% and enemy visible → fear > 0.8 → `PanicLayer` + `Panic` + Lowpass
5. On successful escape → fear drops to 0.1 over 2 seconds

---

## 💡 Expected Console Output (DEVELOPMENT_BUILD):
```text
[Fear] Distance: 25m, Visible: False → Fear: 0.12 → Snapshot: Calm
[Fear] Distance: 12m, Visible: False → Fear: 0.35 → Snapshot: Tense, TensionLayer volume: 0.35
[Fear] Distance: 5m, Visible: True, Health: 60% → Fear: 0.68 → Snapshot: NearThreat, ThreatLayer volume: 0.2
[Event] PlayerSpotted! Fear jumped to 0.72
[Fear] Distance: 2m, Visible: True, Health: 25% → Fear: 0.85 → Snapshot: Panic, PanicLayer active, Lowpass: 1600Hz
[Event] Escape! Fear fading to 0.1 over 2s
```

---

## 🏆 Bonus Task (Optional):
Implement crossfade between stems synchronized with musical beats (use `AudioSettings.dspTime` and `ClipSamples`).

---

### ⭐ If this project was useful, put a star on GitHub!
