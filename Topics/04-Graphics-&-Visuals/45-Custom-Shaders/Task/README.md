# 🧪 Practical Task: Animated Distortion Shader
Goal: Write a custom shader that:
1. Takes a texture, distortion strength, and animation speed parameters.
2. Animates UV coordinates over time using sine/cosine.
3. Distorts pixels based on their screen position (e.g., a "ripple" or "heat haze" effect).

---

## 📋 Task Description
Create a shader named `Shader "Custom/HeatWave"` that works on Opaque objects and produces a "heat wave" or "water ripple" effect. The shader must include:

### Properties (Inspector parameters):
- `_MainTex` (Texture) — the base texture.
- `_Strength` (Float, Range 0…0.1) — distortion strength.
- `_Speed` (Float, Range 0…5) — animation speed.
- `_ColorTint` (Color) — tint color mixed with the texture.

### HLSL code requirements:
- Vertex shader should transform position and pass UVs normally.
- Fragment shader must:
  - Get the time `_Time.y`.
  - Compute UV offset: `offset.x = sin(uv.y * 20 + _Time.y * _Speed) * _Strength` and similarly for `offset.y` using cosine.
  - Offset the original UV and sample the texture.
  - Blend the texture color with `_ColorTint`.
  - Return the final color.

---

## ✅ Expected Result
- Create a new material in Unity and assign your shader to it.
- Apply the material to a simple plane or sphere.
- In Play mode, you should see the texture smoothly "breathing" and distorting in waves.

---

## 💡 Hints
- Use `sin()` and `cos()` of scaled UVs + `_Time.y`.
- Keep the `_Strength` value small (0.01–0.05) to avoid excessive distortion.
- For a nicer effect, distort both U and V channels differently.

---

### ⭐ If this project was useful, put a star on GitHub!
