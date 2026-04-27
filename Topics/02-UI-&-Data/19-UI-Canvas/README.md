# 🎨 UI Canvas in Unity: Render Modes

Canvas is the component that holds all user interface (UI) elements: buttons, texts, panels, images. 
How the UI is displayed on the screen and how it interacts with the 3D world depends on the Canvas render mode.

Unity has three render modes for Canvas:

1. **Screen Space – Overlay**
2. **Screen Space – Camera**
3. **World Space**

---

## 🟢 1. Screen Space – Overlay

### How it works:
The Canvas is placed directly on top of the screen, above the entire 3D scene. 
The UI is always visible regardless of camera position. Elements are positioned in pixel coordinates (e.g., x=100, y=200).

### When to use:
- Main menus, inventory panels, health bars (HUD)
- Dialog windows that must always stay on top
- Any interface that should not be hidden behind 3D objects

### Example:
Create a Canvas (by default it is already in this mode). Add a button in the center. 
Run the scene — the button will always be visible, even if the camera rotates or 3D objects overlap it.

### How to configure:
In the Canvas Inspector → `Render Mode = Screen Space – Overlay`.

#### Additional:
- `Pixel Perfect` — sharp pixels (for retro games)
- `Sort Order` — rendering order if multiple Canvases exist

---

## 🔵 2. Screen Space – Camera

### How it works:
The Canvas is "attached" to the screen but rendered through a specified camera. 
This allows the UI to interact with the 3D world — for example, elements can be partially occluded by 3D objects if those objects are between the camera and the Canvas.

### When to use:
- Effect where a 3D model passes in front of a dialog window
- 3D shadows or particles over UI
- UI that should respect camera perspective (e.g., "holographic" screens)

### Example:
1. Create a Canvas, set `Render Mode = Screen Space – Camera`.
2. Drag your Main Camera into the `Render Camera` field.
3. Add a 3D cube between the camera and the Canvas (e.g., 2 units from the camera).
4. Run the game — the cube will occlude part of the UI.

### How to configure:
- `Plane Distance` — distance from the camera to the Canvas plane. Smaller distance = UI appears closer to the camera.
- If the distance is larger than that to a 3D object, the object will occlude the UI.

---

## 🟣 3. World Space

### How it works:
The Canvas becomes a 3D object in the scene. It has size, rotation, and position. 
The UI appears as part of the world — for example, a sign above a door, a monitor screen in the game, or floating damage numbers above a character.

### When to use:
- UI attached to a 3D object (name above an NPC, health bar above an enemy)
- Interactive screens inside the game (kiosk, terminal)
- Speech bubbles in 3D space

### Example:
1. Create a Canvas, set `Render Mode = World Space`.
2. Set its size (e.g., `Width=3, Height=2`).
3. Add a button and text to the Canvas.
4. Place the Canvas as a 3D object above a door.
5. Run the game — now you must approach the door to interact with the UI (e.g., Raycast from the camera).

### How to configure:
- `Event Camera` — the camera that will handle clicks on this Canvas (usually Main Camera).
- Scale the Canvas via Transform like any other 3D object.
- For text readability, use `Dynamic Pixels Per Unit`.

---

## 📊 Quick Comparison Table

| Mode | Display Location | Camera Dependent | 3D Occlusion | Typical Use |
|-----------|----------|------------|----------------|-------------|
| Overlay | Above entire screen | No | No | Menus, HUD, cursor |
| Camera | Attached to screen via camera | Yes | Partial | UI with 3D effects |
| World Space | As an object in the scene | Yes (for Raycast) | Full | Signs, NPC names, screens |

---

## 🧠 Summary
- **Overlay** — simplest and fastest for static UI.
- **Camera** — hybrid mode for depth effects.
- **World Space** — for interface that lives inside the game world.

Choosing a mode depends on whether the UI should be part of the world or always in front of the player's eyes.

---

### ⭐ If this project was useful, put a star on GitHub!
