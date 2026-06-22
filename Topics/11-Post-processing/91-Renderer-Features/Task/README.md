# 🎯 Task: «Dynamic Vignette Effect for Danger Zone»
You are developing an RPG game. 
You need to create a dynamic full-screen effect that activates when the player enters a danger zone (e.g., a lava biome). 
The effect should be a vignette with red glow and pulsing intensity.

## 📝 What to Implement:
### Part 1: Creating Fullscreen Shader Graph
1. Create a Fullscreen Shader Graph named `DangerVignette`.
2. Add parameters:
   - `_Intensity` (float, Range 0–1) — effect strength
   - `_VignetteSize` (float, Range 0–1) — vignette radius
   - `_VignetteColor` (Color) — glow color (red)
  
3. Implement vignette logic:
   - Calculate distance from center (`UV` → `Distance`)
   - Apply `smoothstep` for soft edge
   - Multiply by `_VignetteColor` and `_Intensity`
   - Blend with original color (BlitSource)
  
4. Create a Material with this shader.

### Part 2: Configuring Full Screen Pass Renderer Feature
5. Add Full Screen Pass Renderer Feature to the URP Renderer.
6. Set:
   - Pass Material → `DangerVignette` Material
   - Injection Point → `After Rendering Post Processing`
   - Requirements → `Color`
  
7. Verify the effect appears.

### Part 3: Control via C# Script
8. Create script `DangerZone.cs`:
   - On entering trigger (danger zone), smoothly increase `_Intensity` to 1
   - On exiting, smoothly decrease to 0
   - Add pulsing (using `Mathf.Sin(Time.time)`) when inside zone
  
9. Create script `VignetteController.cs`:
    - Accesses Material via `RendererFeature`
    - Contains methods: `SetIntensity(float)`, `SetVignetteSize(float)`
    - Uses `Material.SetFloat()` to change parameters at runtime
  
### Part 4: Additional Visual Effects
10. Add an audio effect when entering the zone (heartbeat).
11. Make vignette size shrink when approaching death (health < 20%).
12. Add a Blur post-effect through a separate Render Pass that activates only in the danger zone.

---

## 🧰 Implementation Requirements:
- Use Full Screen Pass Renderer Feature for the main effect.
- For blur, create a custom ScriptableRendererFeature.
- All transitions must be smooth (use `Mathf.Lerp`).
- Add comments to each public method.

---

### ⭐ If this project was useful, put a star on GitHub!
