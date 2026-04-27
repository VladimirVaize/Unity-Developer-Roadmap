# 🧪 Practical Task: UI Canvas Render Modes

## 📌 Task: Create Three Types of Health Display
You are working on an RPG game. You need to implement three different health bars for the same character, using all three Canvas Render Modes:
1. **Screen Space – Overlay** — health bar must always be in the top-left corner of the screen (global HUD).
2. **Screen Space – Camera** — health bar hovers above the character but is occluded by a 3D object if that object comes closer to the camera than the character.
3. **World Space** — health bar is a 3D sign above the character's head, rotates with the character, and is only visible from a certain distance.

---

## 🧱 What to do:

1. Create a scene with:
   - A player (cube or capsule)
   - A camera (Main Camera)
   - A 3D cube that can move (e.g., with WASD or simply placed between the camera and the player)
2. Create three separate Canvases (or three separate indicators) on the same screen:

### ✅ Canvas 1 – Overlay HUD
- Render Mode = `Screen Space – Overlay`
- Place a health bar (Image + Text) in the top-left corner
- Independent of camera and 3D objects

### ✅ Canvas 2 – Screen Space – Camera (with occlusion effect)
- Render Mode = `Screen Space – Camera`
- `Render Camera` = Main Camera
- `Plane Distance` = 1.5
- The health bar hovers above the player (positioned in screen coordinates attached to the player — use a script with `Camera.WorldToScreenPoint`)
- Move the 3D cube between the camera and the player — the cube should occlude the health bar if it is closer to the camera

### ✅ Canvas 3 – World Space
- Render Mode = `World Space`
- Canvas size: width 2, height 1
- Make this Canvas a child of the player object (so it moves with the player)
- Rotate the Canvas to face the camera (or add a billboard script)
- Add a background, health bar, and text

3. Write a simple script `HealthBarUpdater` that:
   - Manages health value (with +/- key presses)
   - Updates all three indicators simultaneously (fill amount and text)

4. (Optional but useful) Add an effect where when the player rotates — the World Space Canvas rotates with them, while the other two remain static.

---

## 🔍 Expected result after running the scene:

| Action | Expectation |
|-----------------------|-------------------------|
| Change health (+/-) | All three bars update at the same time |
| Rotate the camera | Overlay and Camera-UI stay on screen, World Space rotates |
| Move the 3D cube between camera and player | Only the Camera-mode bar is partially occluded by the cube |
| Zoom in/out the camera | World Space changes visible size, Overlay and Camera do not |

---

## 🧠 Reflection questions:
- Which render mode is best for UI that must always be readable?
- Which mode gives the feeling that UI is part of the game world?
- Why is the `Plane Distance` parameter needed in Camera mode?

---

### ⭐ If this project was useful, put a star on GitHub!
