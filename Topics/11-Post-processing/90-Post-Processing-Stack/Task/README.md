# 🎯 Task: «Post-Effects System for Cinematic RPG»
You are developing an RPG game with a cinematic style. You need to create a post-processing system that:
1. Adapts to different locations (forest, dungeon, city)
2. Responds to player states (combat, stealth, dialogue)
3. Creates cinematic effects (during cutscenes)

## 📝 What to Implement:
### Part 1: Profile Configuration
1. Create 4 PostProcessProfile assets:
   - `Default` — standard settings (Bloom 0.5, slight contrast)
   - `Forest` — warm, green tint, soft Bloom
   - `Dungeon` — cold, high contrast, strong Bloom from torches
   - `Cinematic` — strong DOF, cinematic color grading
  
2. For each profile, configure:
   - Bloom (Intensity, Threshold, Tint)
   - Color Grading (Temperature, Contrast, Saturation)
   - Vignette (for Dungeon and Cinematic)
  
### Part 2: Dynamic Switching
3. Create `ZonePostProcessor.cs`:
   - Switches profiles in trigger zones
   - Enter forest → `Forest`
   - Enter dungeon → `Dungeon`
   - Exit → `Default`
  
4. Create `StatePostProcessor.cs`:
   - On combat start → increase contrast and saturation
   - On stealth → decrease brightness, add cold tint
   - On dialogue → enable DOF focusing on speaker
  
### Part 3: Anti-aliasing
5. Configure TAA on camera:
   - Jitter Spread: 0.75
   - Stationary Blending: 0.95
   - Motion Blending: 0.85
   - Add ability to switch to FXAA in settings
  
### Part 4: Depth of Field for Cutscenes
6. Create `CinematicDOF.cs`:
   - Smoothly enables DOF when cutscene starts
   - Focuses on active character
   - Uses Aperture: 2.8, Focal Length: 85mm
  
### Part 5: Dynamic Bloom
7. Create `DynamicBloom.cs`:
- On damage taken → Bloom flash (intensity 2.0 for 0.5 sec)
- On item pickup → soft flash (intensity 1.0 for 0.3 sec)

### Part 6: Local Volumes
8. Create local Volumes for:
   - Treasure rooms — warm golden light, enhanced Bloom
   - Dark corridors — Vignette, cold tint
   - Magic portals — purple tint, strong glow
  
---

## 🧰 Implementation Requirements:
- Use at least 4 different effects (Bloom, DOF, Color Grading, Vignette)
- All transitions must be smooth (> 0.5 sec)
- Add comments to each public method
- In `DEVELOPMENT_BUILD`, log profile switching info

---

## 🔍 Verification:
1. Walk through forest and dungeon zones — verify color grading changes
2. Start combat — verify contrast and saturation changes
3. Activate cutscene — verify DOF
4. Take damage — verify Bloom flash
5. Enter treasure room — verify local Volume

---

## 🏆 Bonus Task (Optional):
Implement settings saving system:
- User can adjust intensity of each effect
- Settings saved to `PlayerPrefs`
- Saved settings loaded on startup

---

### ⭐ If this project was useful, put a star on GitHub!
