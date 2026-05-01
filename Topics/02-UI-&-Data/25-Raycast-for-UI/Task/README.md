# 🧩 Practical Task: Blocking Shooting Through the UI

## 📝 Condition
### You have a simple scene:
- A 3D cube (target) with a `Collider` component and a script that changes its color on click.
- A `Fire` button (UI) in the corner of the screen.
- A `Pause Panel` (UI) that appears when you press the `Space` key. The panel covers half the screen and contains a "Pause" text and a `Resume` button.

### Requirements:
1. When you click on the cube (via `Physics.Raycast`), it changes its color to a random one.
2. When you click the `Fire` button, a message "Shot from UI" is printed to the console, and the cube must not change color.
3. When the pause panel is open, clicks on the cube must not work (the cube does not change color).
4. The pause panel should intercept clicks even if the cursor is over an "empty" area of the panel (not over the `Resume` button).
5. Clicking the `Resume` button closes the pause panel.

---

## 🎯 Goal
Implement `Graphic Raycaster` and proper use of `Raycast Target` so that:
- The `Fire` button never activates a shot on the cube.
- The pause panel blocks all clicks on the cube while it is active.

---

## 📋 Step‑by‑step hint
1. Add the `Graphic Raycaster` component to your Canvas (if missing).
2. Make sure an `EventSystem` exists in the scene.
3. On the `Fire` button, leave `Raycast Target = true` (default).
4. On the pause panel:
   - The background panel (Image) must have `Raycast Target = true`.
   - The text and the `Resume` button — default settings.
  
5. In the cube's script, before performing `Physics.Raycast`, add a check:
```csharp
if (EventSystem.current.IsPointerOverGameObject())
    return;
```
6. When opening the pause (`Space`), the panel becomes active (`SetActive(true)`). When closing — inactive.

---

## 🔍 Expected result
- A direct click on the cube while the pause is closed → the cube changes color.
- A click on the `Fire` button → console message appears, cube color does not change.
- Pause is open → the cube does not react to clicks (even if you click directly on it).
- Click on `Resume` → the pause panel disappears, the cube starts reacting again.

---

### ⭐ If this project was useful, put a star on GitHub!
