# 🧪 Practical Assignment: Creating a Photorealistic Scene with HDRP
Goal: Learn to configure high-quality lighting, post-processing, and materials in the High Definition Render Pipeline.

Task: Create a small night city scene with a neon sign, wet road, and volumetric fog.

---

## 🎯 Step-by-Step Instructions
### Step 1: Create an HDRP Project
- Open Unity Hub → `New Project`
- Select the `High Definition 3D` template
- Name the project `HDRP_NightCity`

### Step 2: Basic Objects
1. Delete the default camera (`Main Camera`) and directional light (`Directional Light`) from the scene
2. Create:
   - `GameObject` → `Rendering` → `HDRP Camera` (new camera)
   - `GameObject` → `3D Object` → `Plane` (ground)
   - `GameObject` → `3D Object` → `Cube` (building)
  
### Step 3: Material Setup
1. Create a `NeonMaterial`:
   - In the Project window → right-click → `Create` → `Material`
   - In the Inspector, select the `HDRP/Lit` shader
   - Set Albedo color → bright pink (RGB: 255, 0, 150)
   - Raise Emissive Intensity to `5` (glow)
   - Check the `Emissive` checkbox
  
2. Create a `WetAsphalt` material:
   - Shader `HDRP/Lit`
   - Albedo color → dark gray
   - Smoothness → `0.9` (wet shine)
   - Metallic → `0.8`
  
### Step 4: Lighting 
1. Add an `Area Light`:
   - `GameObject` → `Light` → `Area Light`
   - Place it above the building as a neon sign
   - Color: pink, Intensity: `20`
  
2. Add Volumetric Fog:
   - `GameObject` → `Volume` → `Global Volume`
   - In the Inspector → `Add Override` → `Fog` → select `Volumetric Fog`
   - Set `Density` = `0.2`
  
### Step 5: Post-Processing (Volume)
1. In the same `Global Volume`, add:
   - `Bloom` → Threshold = `0.9`, Intensity = `1.5`
   - `Depth of Field` → enable, Focus Distance = `10`, Aperture = `8`
   - `Color Grading` → Post-exposure = `0.5`, Contrast = `15`
  
### Step 6: Neon Sign (Decal Projector)
1. `GameObject` → `Rendering` → `Decal Projector`
2. Place it on the building's wall
3. In the Inspector, assign the `NeonMaterial` to it
4. Scale it into a rectangular sign shape

### Step 7: Testing
- Press Play and evaluate the result:
  - Blurred background (Depth of Field)
  - Glow from the neon sign (Bloom)
  - Fog scattering light
  - Wet road with reflections
 
---
 
## ✅ Success Criteria

| What to check | How to verify |
| --- | --- |
| HDRP is installed | In project settings `Graphics`, `HDRP Asset` is selected | 
| Volumetric fog works | The scene looks smoky, neon light is visible in the air |
| Bloom works | The bright pink neon creates a halo around the sign |
| Road material is wet | Specular highlights and reflections are visible on the plane |
| Decal works | A pink neon sign is on the building's wall |

---

## 🧠 Additional Ideas for Improvement
- Add a Physically Based Sky and set the time to midnight.
- Enable Screen Space Reflections for reflections in puddles.
- Add Contact Shadows on a small object (e.g., a trash bin).
- Animate the neon color (via script, changing Emissive Intensity).

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
