# 🛠️ Practical Task: Creating a material for a rusty metal barrel

## 📝 Task Objective
Learn how to create a PBR material using the Standard Shader, configure Metallic and Smoothness, and apply textures (Albedo, Metallic/Smoothness map, Normal Map). 
Finally, you will apply the material to a barrel model and evaluate the result under different lighting conditions.

---

## 🧩 Steps to follow
### Step 1. Prepare textures (download or create)
You will need 3 textures (you can find free ones on sites like Poly Haven, AmbientCG, or create your own in Photoshop/GIMP):
- `Barrel_Albedo.png` — base: wood (brown) + metal hoops (grey-steel) + rust spots (orange-red).
- `Barrel_MetallicSmoothness.png` — Metallic/Smoothness map:
  - Red channel (Metallic):
    - Wood → 0 (black)
    - Clean metal hoops → 1 (white)
    - Rusty areas → 0.2–0.4 (dark grey)
   
  - Alpha channel (Smoothness):
    - Wood → 0.1 (very rough)
    - Metal → 0.8 (smooth)
    - Rust → 0.3
   
- `Barrel_Normal.png` — normal map with fine scratches, dents, and wood grain.

### Step 2. Import textures into Unity
- Drag all three textures into `Assets/Textures/Barrel`.
- Click on `Barrel_MetallicSmoothness.png` → in the `Inspector`, uncheck `sRGB` (because this is not a color image). Press `Apply`.

### Step 3. Create the material
- In the `Project` window → right-click → `Create` → `Material`. Name it `Barrel_RustyMetal`.
- In the `Inspector`, make sure `Shader` is set to `Standard`.

### Step 4. Assign textures to the material
- Drag `Barrel_Albedo.png` into the **Albedo slot**.
- Drag `Barrel_MetallicSmoothness.png` into the **Metallic Map** slot.<br>Immediately below, the `Metallic` and `Smoothness` parameters will appear — set both to `0` (so the map fully controls the values).
- Drag `Barrel_Normal.png` into the **Normal Map** slot.<br>Ensure the map type is set to `Default` or `Normal Map` (Unity will auto-detect).

### Step 5. Fine-tuning (optional)
- `Smoothness Source` → choose `Alpha` (if your MetallicSmoothness map uses the alpha channel for smoothness). Or `Metallic` if you use another channel.
- `Tiling` → usually (`1,1`) for a standard barrel. If the texture repeats, increase the values.
- Leave the `Metallic` and `Smoothness` sliders (below the map) at `0`, because the map is driving the values.

### Step 6. Apply to a model
- In the scene, create a simple cylinder (`CameObject → 3D Object → Cylinder`) or import a barrel model (FBX).
- Drag the `Barrel_RustyMetal` material from `Project` directly onto the model in the Scene View or in the model's `Inspector` → `Mesh Renderer → Materials` slot.

### Step 7. Testing
- Make sure there is at least one directional light (`Directional Light`) in the scene.
- Enter Play Mode or simply look in the Scene View.
- Rotate the camera around the barrel. Change the light angle and observe how the reflections on the metal hoops differ from the matte rust and wood.
- (Optional) Add a `Point Light` near the barrel to see bright reflections.

---

## ✅ Success Criteria
- Metal parts of the barrel reflect light (shine), while wood and rust are matte.
- Smoothness varies: metal is smooth, wood is rough.
- The normal map creates the illusion of scratches and dents (without changing geometry).
- When lighting changes, the material's behavior looks realistic (metal does not "eat" the environment color, as expected in PBR).

---

## 🔍 Tips
- If the metal looks too dark, check that the `Albedo` for metal areas uses black (or very dark grey), not brown. Metal reflects the environment, not its albedo.
- To preview a normal map in Unity: select the texture → `Inspector` → `Texture Type` = `Normal Map`.
- Use the Material Preview (sphere at the bottom of the material's inspector) for quick checks without leaving the editor.

---

### ⭐ If this project was useful, put a star on GitHub!
