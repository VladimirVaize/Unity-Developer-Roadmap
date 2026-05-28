# High Definition Render Pipeline (HDRP) 🌟
## What is HDRP?
High Definition Render Pipeline (HDRP) is a graphics rendering pipeline in Unity designed for creating photorealistic, 
high-quality graphics on powerful platforms: high-end PC, PlayStation 5, Xbox Series X/S, and modern consoles.

HDRP gives developers full control over lighting, shadows, materials, and post-processing, allowing them to achieve AAA-quality games and architectural visualization.

> [!Important]
> HDRP is not suitable for mobile devices, old consoles, or low-end PCs. For those, use Built-in Render Pipeline or URP (Universal Render Pipeline).

---

## Which platforms is HDRP for?
### ✅ High-performance platforms:
- Windows / macOS (gaming PCs with discrete GPU)
- PlayStation 5, PlayStation 4 Pro
- Xbox Series X/S, Xbox One X
- Some high-end VR devices

### ❌ Not recommended for:
- Mobile devices (iOS, Android)
- Nintendo Switch
- Web platforms (WebGL)
- Weak integrated GPUs

---

## Key Features of HDRP 📸
### 1. Physically Based Lighting (PBR)
HDRP uses an advanced lighting model based on real physics. 
Materials react to light just like in the real world: 
metal shines, wood scatters light, glass refracts rays.

### 2. Advanced Light Sources 💡
- HDRI Sky — high dynamic range sky
- Volumetric Fog — volumetric fog that interacts with light
- Area Lights — rectangular and disc light sources (realistic soft lighting)
- IES Profiles — real light profiles from lamp manufacturers

### 3. Advanced Shadow Features
- Contact Shadows — shadows from small details even under general lighting
- Screen Space Shadows — high-definition shadows in the foreground
- Cascaded Shadow Maps — detailed shadows at any distance

### 4. Advanced Materials 🧪
- Lit Shader — main shader with support for all texture maps (Albedo, Normal, Metallic, Smoothness, Ambient Occlusion, Emissive)
- Decal Projector — decals on walls (graffiti, cracks, puddles) without modifying the model
- Fabric Shader — realistic cloth (velvet, silk, denim)
- Hair Shader — hair with anisotropic reflection

### 5. Post-Processing (Volume System) 🎨
HDRP uses the Volume system, where camera settings can change depending on the player's position in space.

#### Main effects:
- Bloom — glow from bright areas
- Depth of Field — depth of field (background blur)
- Motion Blur — blur during movement
- Color Grading — color correction (LUT)
- Exposure — automatic or manual exposure (HDR)
- Ambient Occlusion — darkening corners and crevices
- Screen Space Reflections (SSR) — realistic reflections

### 6. Physically Based Sky ☀️
Physically Based Sky — dynamic sky that automatically adjusts to the time of day (sun, moon, stars, clouds). 
You can set latitude, longitude, date, and time — and the sun will rise exactly where it should.

### 7. Ray Tracing 🚀
If your platform supports DirectX 12 Ultimate and RTX GPUs, HDRP includes:
- Raytraced Reflections — perfect reflections (not just SSR)
- Raytraced Shadows — soft and accurate shadows from any object
- Raytraced Ambient Occlusion — maximally realistic darkening
- Raytraced Global Illumination — light bouncing between surfaces

---

## How to start working with HDRP? 🛠️
1. Installation:
   - `Window` → `Package Manager` → `Unity Registry`
   - Install the `High Definition RP` package
  
2. Creating an HDRP project:
   - When creating a new project, select the `High Definition 3D` template (recommended)
   - Or create an `HDRP Asset` and assign it in project settings (`Graphics Settings`)
  
3. Setting up the camera:
   - Delete the old camera in your scene, add a `Camera` from `GameObject` → `Rendering` → `HDRP Camera`
  
4. Volume (post-processing):
   - Add an empty object → add a `Volume` component
   - Create a new `Volume Profile` and add effects (Bloom, Depth of Field, etc.)
  
---

## Simple Example 🎯
Scenario: You're making a night alley scene with wet asphalt and a neon sign.

What HDRP provides:
1. Volumetric fog — light from the neon scatters in the air.
2. Emissive Material — the sign glows and creates bloom.
3. Decal Projector — a puddle on the asphalt reflecting the neon.
4. Screen Space Reflection — the neon's reflection in the puddle.
5. Contact Shadows — shadows from trash bins even under dim lighting.

Without HDRP you would get a flat image without glow, volumetric fog, or realistic reflections.

---

## When to choose HDRP?

| Your project | Is HDRP suitable? |
| --- | --- |
| AAA first-person shooter | ✅ Yes |
| Architectural visualization | ✅ Yes |
| Simulation (flight, racing) | ✅ Yes |
| Mobile puzzle game | ❌ No (URP) |
| 2D platformer | ❌ No (Built-in or URP) |
| Nintendo Switch game | ❌ No |

---

### ⭐ If this project was useful, put a star on GitHub!
