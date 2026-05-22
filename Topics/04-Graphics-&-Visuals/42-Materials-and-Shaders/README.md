# 🎨 Materials and Shaders in Unity: Standard Shader, Metallic/Smoothness, Textures

> [!Note]
> This material explains how to create and configure the visual appearance of objects in Unity using Materials and Shaders.
> We'll cover the Standard Shader (the main shader for realistic graphics), the Metallic and Smoothness parameters, and the use of Textures.

---

## 1. What are Materials and Shaders? 🤔
- Shader — a program that tells the GPU how to draw an object's surface: how it reacts to light, what colors to display, whether it should be shiny, transparent, etc.
- Material — a "wrapper" around a shader. You create a material, assign a shader to it, adjust its parameters (colors, textures, values), and then apply that material to one or many objects in the scene.

### How to create a material:
In the `Project` window → right-click → `Create` → `Material`. Name it, e.g., `Metal_Grey`.

---

## 2. Standard Shader 🌟
Standard Shader is Unity's primary shader for Physically Based Rendering (PBR). 
It is designed for realistic materials (metal, plastic, wood, rubber, etc.) and works correctly under any lighting.

### Where to find it:
By default, any new material uses the `Standard Shader`. 
You can change the shader at the top of the `Inspector` → `Shader` button → choose `Standard` or `Standard (Specular setup)`.

### Two variants of Standard Shader:
- Standard (Metallic workflow) — uses `Metallic` and `Smoothness` parameters. This is the main choice for most cases.
- Standard (Specular setup) — uses a specular color (`Specular Color`) and `Smoothness`. Suitable for older or specific pipelines.

---

## 3. Metallic and Smoothness Parameters 🪞🧼
These two parameters control how light reflects from a surface.

### 🔩 Metallic
- Value from 0 to 1.
  - `0` — dielectric (non-metal): wood, plastic, stone, rubber. Diffuse reflections, weak or no specular reflections.
  - `1` — pure metal: gold, steel, copper. Reflects light like a mirror, no diffuse color (metal "tints" the reflections).
  - Intermediate values (0.2–0.8) — dirty or oxidized metal, mixed materials.
 
### ✨ Smoothness
- Value from 0 to 1.
  - `0` — very rough surface (sand, matte paint) → wide, dull highlights.
  - `1` — perfectly smooth surface (mirror, polished glass) → sharp, bright reflections.
 
### Combination:
- High Metallic + High Smoothness → shiny metal (chrome, silver).
- Low Metallic + Low Smoothness → matte plastic or raw wood.
- Low Metallic + High Smoothness → glossy ceramic or varnished non-metal.

---

## 4. Textures 🖼️
A texture is simply an image (PNG, JPG, TGA, etc.) that is mapped onto an object's surface to add detail without increasing model complexity.

### Common texture maps in Standard Shader:
| Texture Slot | Purpose |
| --- | --- |
| Albedo (Main Texture) | Defines the base color of the object. For metals, this should be black (or very dark), because metals reflect their surroundings. |
| Metallic / Smoothness (packed map) | Red channel → Metallic. Alpha channel (or another specified channel) → Smoothness. Saves memory. | 
| Normal Map | Creates the illusion of relief (dents, bumps) without changing geometry. |
| Height Map | Actual vertex displacement (requires tessellation). |
| Occlusion Map | Darkens crevices (gaps, joints) for realism. |
| Emission Map | Makes the object glow (e.g., monitor screen, lava). |

### How to apply a texture to a material:
1. Import the texture into Unity (drag it into the `Project` window).
2. Select your material in `Project`.
3. In the `Inspector`, find the `Albedo` slot and drag your texture there. Or click the circle to the right of the slot and choose a texture from the list.
4. For a `Metallic/Smoothness` map: in the `Metallic Map` slot, assign your texture.
   Then below the slot, set `Metallic` and `Smoothness` to `0` (so the map fully controls the values).
   Ensure the texture is imported with `sRGB = false` (Color Space not required).

---

## 5. Example: Creating a realistic material 🧪
Goal: Create a rusty metal sheet.

1. Create a material `RustedMetal`.
2. In the `Albedo` slot, place a rust texture (brown-orange-grey).
3. In the `Metallic Map` slot, place a custom texture where:
   - Clean metal areas → white (Metallic ≈ 1)
   - Rusty areas → black (Metallic ≈ 0)
  
4. Set the `Smoothness` parameter (below the map) to `0.2` — rust will be even rougher if the map controls Smoothness.
5. Add a `Normal Map` to simulate corrosion bumps.
6. Drag the material onto an object in the scene — now it looks like old rusty metal.

---

## 6. Where to see the result 👀
- In the Scene View (with lighting enabled) or Game View.
- In the material's Inspector, there is a "Preview" button — sphere, cube, or cylinder for testing.
- Use the Lighting window (`Window → Rendering → Lighting`) to set up global illumination so that PBR works correctly.

---

### ⭐ If this project was useful, put a star on GitHub!
