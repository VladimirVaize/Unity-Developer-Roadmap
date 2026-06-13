# 🎯 Task: «Interactive Environmental Sound System»
You are developing a first-person 3D game. 
You need to create a sound system that responds to player movement and interaction with objects. 
The player walks around the level, picks up items, and interacts with sound sources.

## 📝 System Requirements:
1. Footstep Sounds (3D Sound)
   - Different surfaces: grass, concrete, wood
   - Footstep volume depends on player speed
   - Random sound selection from array for each surface
   - Correct `Min Distance` and `Max Distance` settings (footsteps audible within 10 meters)
  
2. Item Pickup Sound (2D Sound)
   - One-shot playback via `PlayOneShot`
   - Independent of player position (UI sound)
   - Random pitch variation for variety
  
3. Stationary Sound Sources (3D Sound)
   - 3 objects in scene: waterfall, campfire, wind chime
   - Each has a unique attenuation curve (Linear, Logarithmic, Custom)
   - Sound becomes louder as player approaches
  
4. Doppler Effect
   - Fast-moving object (flying saucer) flies past the player
   - Characteristic pitch shift should be audible during flyby
  
5. UI Sound Control
   - Volume sliders for: Master, Music, SFX
   - M key to mute all sounds
   - P key to pause all sounds
  
---

## 🧰 Technical Requirements:
- Create `PlayerSoundController` script for player sounds
- Create `WorldSoundSource` script for stationary sources
- Create `UIAudioManager` script for volume control
- Use `AudioMixer` with at least 3 groups (Master, Music, SFX)
- For footstep sounds, identify surfaces via tags or `TerrainData`

---

## 💡 Expected Result:
- Different footstep sounds depending on surface while moving
- Campfire sound gets louder as player approaches
- Short pleasant sound when picking up a coin
- Audible pitch shift when saucer flies past ("wooosh")

---

### ⭐ If this project was useful, put a star on GitHub!
