# 🧪 Practical Task: «Camera System for a Weapon Shop»

## 🎯 Goal
Create a scene that uses multiple cameras with different projection types, Clear Flags, Culling Mask, and Render Texture. As a result, you will implement a weapon display case and a monitor showing a CCTV feed.

---

## 🛠️ Tasks

### 1. Create a scene with three objects:
  - A floor (Plane).
  - A wall (Cube) with a brick texture.
  - Three simple weapon models (cubes with different colors).

### 2. Main camera (Perspective)
  - Projection type: `Perspective`.
  - Clear Flags: `Skybox`.
  - Culling Mask: sees everything except the `UI` layer (create a `UI` layer and assign it to the UI later).
  - The camera looks at the weapons from an angle.

### 3. Orthographic camera for a minimap
  - Projection type: `Orthographic`, Size = 5.
  - Clear Flags: `Solid Color` (dark gray).
  - Place the camera directly top‑down.
  - Culling Mask: sees only the `Weapons` layer (create this layer and assign it to the weapons).

### 4. CCTV camera
  - Create a new camera pointed at the shop entrance (e.g., a cube on the other side of the scene).
  - Clear Flags: `Depth Only`.
  - Culling Mask: sees everything except the `UI` layer.
  - Create a Render Texture (e.g., `CCTV_Texture`) and assign it to this camera's `Target Texture`.

### 5. Display the CCTV feed on a monitor
  - Add a plane (monitor) to the scene and create a material that uses `CCTV_Texture` as its `Main Texture`.
  - The monitor should show a live image from the CCTV camera.

### 6. Add simple UI
  - A button to switch the main camera's projection (Perspective ↔ Orthographic) via a script.
  - A text element showing the current camera mode.
  - Assign the UI to the `UI` layer and exclude that layer from the main camera's Culling Mask (so the UI does not interfere).

---

## ✅ Acceptance Criteria
- When the scene runs, the main camera shows the weapons in perspective.
- The minimap (orthographic camera) shows a top‑down view of only the weapons.
- The monitor displays the live feed from the CCTV camera.
- The button switches the main camera's projection, and the UI is always visible.
- All Clear Flags and Culling Mask settings are applied correctly.

---

## 📤 Deliverable

A ready‑to‑run Unity scene (`.unity` file) with the described cameras, 
a script for projection switching, and properly configured Render Textures. 
The task is considered complete when all items work without errors.

---

### ⭐ If this project was useful, put a star on GitHub!
