# 🎬 Creating Simple Property Animations (Animation Window)

> [!Note]
> Animation in Unity is the process of changing an object's properties over time.
> The Animation Window allows you to create frame-by-frame animations without writing code — just by recording changes to parameters
> (position, rotation, scale, color, transparency, and even your script variables).

---

## 🧠 Key Concepts

| Concept | Description |
|-------------------|-----------------------------------|
| Animation Clip | A file containing an animation (e.g., "door opening"). Stores keyframes. |
| Keyframe | A moment in time where a specific property value is set. |
| Dopesheet | A view that displays keyframes as horizontal bars. |
| Curves | A view showing the smoothness of value changes between keyframes. |

---

## ⚙️ How to Open the Animation Window
`Window → Animation → Animation` (or `Ctrl + 6` on Windows, `Cmd + 6` on Mac)

---

## 🛠️ Creating Your First Animation (Step by Step)

### 1️⃣ Select an object
In the Hierarchy, select a GameObject (e.g., a `Cube`).

### 2️⃣ Open Animation Window and create a clip
Click `Create` in the Animation window. Choose a name (e.g., `RotateAnimation`) and save. Unity will automatically add an `Animator` component to the object.

### 3️⃣ Add a property to animate
Click Add Property → select a property:
- `Transform` → `Position`, `Rotation`, `Scale`
- `Sprite Renderer` → `Color`
- `Light` → `Intensity`
- or any public field from your script (`public float speed`)

### 4️⃣ Create keyframes
- Move the red playback head to a time (e.g., `0:00`)
- Click the red record button 🔴 (or modify the property value — the keyframe will auto-save)
- Move to another time (e.g., `1:00`)
- Change the property value (the cube will rotate, move, or change color)

### 5️⃣ Play the animation
- In the Animation window, click ▶️ Play (preview)
- In the editor, click the main Play button — the animation will loop if `Loop Time` is enabled in the clip settings.

---

## 📐 Property Animation Examples

### Example 1: Bouncing Cube (changing Y position)
- Property: `Transform.Position.y`
- Frame `0:00` → `y = 0`
- Frame `0:30` → `y = 2`
- Frame `1:00` → `y = 0`

Result: the cube smoothly bounces up and down.

### Example 2: Sprite Fade Out
- Property: `Sprite Renderer.Color.a` (alpha channel)
- Frame `0:00` → `a = 1` (fully visible)
- Frame `2:00` → `a = 0` (fully transparent)

Result: the object fades out over 2 seconds.

### Example 3: Rotation around Z‑axis
- Property: `Transform.Rotation.z` (in degrees)
- Frame `0:00` → `z = 0`
- Frame `2:00` → `z = 360`

Result: one full rotation in 2 seconds.

---

## 🔁 How to Loop an Animation
`Ctrl + D` on the clip in the Project Window → select the copy → in the Inspector, check **Loop Time**. 
Alternatively, select the clip in **Project Window** and at the bottom of the Inspector set `Loop Time: true`.

---

## 🧩 Important Note
Animations override property values during runtime. 
If you try to move an object via script while it's being animated — the animation will win 
(this can be solved with `AnimatorController` layers and masks, but that's an advanced topic).

---

### ⭐ If this project was useful, put a star on GitHub!
