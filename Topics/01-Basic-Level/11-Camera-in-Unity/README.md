# 🎥 Camera in Unity: Projections, Clear Flags, Culling Mask & Render Texture

> [!NOTE]
> The camera is the "eyes" of your game. It defines what and how the player sees. This material covers four key settings: projection types, Clear Flags, Culling Mask, and Render Texture.

---

## 🧭 1. Projection Types: Perspective vs Orthographic

### 🎭 Perspective – mimics the real world. Objects appear smaller at a distance. Used in 3D games (shooters, RPGs, racing).

### 📐 Orthographic – no perspective distortion. Objects are displayed at the same scale regardless of distance. Ideal for 2D games, UI, top‑down strategy games.

### ⚙️ How to use:
Select a camera → in Inspector → Camera component → Projection property → choose `Perspective` or `Orthographic`. For orthographic cameras, also adjust Size (the vertical visible area).

### 📌 Example:
In a 3D platformer, use `Perspective` for depth. In a top‑down chess game, use `Orthographic` so pieces don't get distorted at the edges.

---

## 🧹 2. Clear Flags
Determines what to fill empty areas of the screen with after rendering each frame. Options:

- **Skybox** 🌌 – sky (default). Empty areas are filled with a sky image.
- **Solid Color** 🎨 – fill with a single color (e.g., black).
- **Depth Only** 📏 – does not clear color, only the depth buffer. Used for multiple cameras (e.g., UI camera).
- **Don’t Clear** 🚫 – clears nothing (rare, for special effects).

### ⚙️ How to use:
Camera → Inspector → Camera → Clear Flags → choose the desired option.

### 📌 Example:
Main camera – `Skybox`. A second camera for a minimap – `Depth Only`, so it draws over the world without erasing the sky.

---

## 🎭 3. Culling Mask
Determines which object layers the camera will see. Allows you to hide certain objects from a specific camera (e.g., hide UI from the main camera).

### ⚙️ How to use:
1. Assign layers to objects in the Inspector (e.g., `UI`, `Player`, `Environment`).
2. Select a camera → Culling Mask → uncheck the layers you do NOT want this camera to see.

### 📌 Example:
The player's camera sees only the `Environment` layer. The UI camera sees only the `UI` layer. Thus UI elements do not interfere with the main image but are displayed on top.

---

## 🖼️ 4. Render Texture
Allows you to output a camera's image to a texture that you can use inside the game (e.g., for a monitor screen, mirror, CCTV camera, minimap).

### ⚙️ How to use:
1. Right-click in the Project window → `Create` → `Render Texture`. Name it, e.g., `MirrorTexture`.
2. Select a camera → in the Camera component → Target Texture field → drag `MirrorTexture` into it.
3. Create a material or UI Image that displays this texture:
   - For a 3D object: drag `MirrorTexture` onto the `Main Texture` property of a material.
   - For UI: add a `Raw Image` and assign `MirrorTexture` to its `Texture` field.
  
### 📌 Example:
You want to make a CCTV camera screen in your game. 
Create a separate camera pointed at the door. 
Its `Target Texture` = `CCTVTexture`. 
Place that texture onto a plane (monitor) in the scene. 
The player will see a live image of the door on the monitor.

---

### ⭐ If this project was useful, put a star on GitHub!
