# 🧪 Practical Task: Creating a "Scanning Effect" with Shader Graph

## 🎯 Objective
Create an Unlit shader that applies a moving scan line effect to an object (like in radar or sci-fi visuals). The shader should work on any 3D model and include:
- A main texture (e.g., metal or panel surface).
- A scan line that moves along the object's local Y-axis.
- Glow (emission) in the scan line area.
- Ability to adjust color, speed, and width via parameters in the material inspector.

---

## 📝 Requirements
1. Shader type: Unlit Shader Graph (URP).
2. Blackboard parameters:
   - `MainTexture` (Texture 2D) — the object's base texture.
   - `ScanColor` (Color) — color of the scan line.
   - `Speed` (Float) — movement speed of the scan line (range -2 to 2).
   - `Width` (Float) — width of the scan line (range 0.1 to 0.5).
   - `EmissionIntensity` (Float) — glow intensity (range 1–5).
  
3. Graph logic:
   - Use world or local coordinates (via `Position` node in Object space).
   - Isolate the Y coordinate (G channel or vector component 1).
   - Create the scan line using `Sine` or `Sawtooth` wave (`Time` + `Multiply` + `Fraction` or `Sawtooth Wave`).
   - Compare the object's Y coordinate with the current scan line position (nodes: `Subtract`, `Abs`, `Step` or `Compare`).
   - Blend (`Lerp`) the main texture with `ScanColor` and add emission.
  
4. Bonus (optional):
   - Add a `GlowFalloff` parameter to make the scan line edges softer.
   - Make the scan line work in UV space (instead of world coordinates) for a decal-like effect.
  
---

## 🔧 Expected Result
- In Scene View, a bright scan line moves top-to-bottom (or bottom-to-top) on an object (e.g., a sphere or custom model).
- The scan line has the specified color and width.
- In the scan line area, the texture becomes brighter (glow effect).
- Changing `Speed`, `Width`, or `ScanColor` in the material inspector updates the effect in real time.

---

## 🧩 Node Hints

| What you need to do | Nodes |
| --- | --- |
| Get object's Y coordinate | `Position` (Object Space) → `Split` (G output) |
| Create moving scan line position | `Time` → `Multiply` (with Speed) → `Fraction` (cycle 0-1) |
| Scan line width | `Subtract` (Y_obj - Y_line) → `Abs` → `Step` (threshold = Width) |
| Blend textures | `Sample Texture 2D` (MainTexture) → `Lerp` with ScanColor |
| Add glow | `Multiply` (ScanColor × EmissionIntensity) → `Add` to the result |

---

## ✅ Success Criteria
- Shader compiles without errors.
- A material with this shader can be assigned to any object.
- The scan line visibly moves.
- Parameters affect speed, width, and color.
- The effect works both in the editor and in Play Mode (build).

---

### ⭐ If this project was useful, put a star on GitHub!
