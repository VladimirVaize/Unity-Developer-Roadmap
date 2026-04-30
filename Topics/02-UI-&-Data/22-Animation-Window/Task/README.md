# 🧪 Practical Task: Pulsing Button Animation

## 🎯 Goal
Create an animation for a UI button that smoothly grows, shrinks, and changes color to attract the player's attention. The animation must loop.

---

## 📦 What you need
- A Unity scene with a UI Canvas
- A `Button` object on the Canvas
- An `Image` component on the button (exists by default)

---

## 📝 Task (Step by Step)

### Step 1. Setup
1. Create a new scene.
2. Add a `Canvas` (if missing: `RMB → UI → Canvas`).
3. Add a `Button` (`RMB on Canvas → UI → Button — Legacy`).
4. Rename the button to `PulsingButton`.

### Step 2. Open Animation Window and create a clip
1. Select `PulsingButton` in the Hierarchy.
2. Open the Animation Window (`Ctrl + 6`).
3. Click `Create`, name it `PulseAnimation`, and save.

### Step 3. Add properties
Click `Add Property` and add:
- `Rect Transform` → `Scale` (to change size)
- `Image` → `Color` (to change color)

### Step 4. Create keyframes for Scale
| Time | Scale value (X and Y) |
|--------|----------|
| 0:00 | (1, 1) |
| 0:30 | (1.2, 1.2) |
| 1:00 | (1, 1) |

🔧 How to do it: move the red playback head, then change Scale in the Inspector.

### Step 5. Create keyframes for Color
| Time | Color (in Image → Color) |
|-------|---------------------|
| 0:00 | White (255,255,255) |
| 0:30 | Red (255,0,0) |
| 1:00 | White |

🔧 Tip: click the color picker in the Inspector while recording 🔴.

### Step 6. Enable looping
1. In the Project window, find the `PulseAnimation` clip.
2. Select it → in the Inspector, check `Loop Time`.
3. Click `Apply`.

### Step 7. Verification
Press Play in the Unity editor. The button should smoothly grow, turn red, and return — infinitely.

---

## ✅ Success criteria
- The button smoothly changes size (1 → 1.2 → 1)
- The button smoothly changes color (white → red → white)
- The animation repeats without stopping
- The animation works during Play mode

---

## 🧠 Self-check questions
1. What would happen if you set `Scale` to (0.5, 0.5) at 0:30?
2. How would you make the animation last 2 seconds instead of 1?
3. How would you add a third color (blue) in the middle of the animation?

---

### ⭐ If this project was useful, put a star on GitHub!
