# 🎯 Practical Task: Run + Shoot using Animation Layers

## 📖 Task Description
You need to implement a character that can run and shoot simultaneously using animation layers and a mask in Unity. 
The upper body should be controlled by a separate layer so that the shooting animation does not interrupt the leg movement from running.

---

## 🧱 Given (starter assets)
- One character with a Humanoid rig (e.g., `Soldier`).
- Animations: `Run`, `Shoot_Pistol`, `Idle`.
- A starter script `PlayerController.cs` that:
  - Moves the character along the X/Z axes (WASD).
  - While holding the left mouse button, sets `animator.SetBool("IsShooting", true)`.
 
- An Animator Controller `SoldierAnimator` with:
  - A `Base Layer` containing `Idle` and `Run` (transition based on parameter `Speed > 0.1`).
  - A parameter `IsShooting` (bool).
 
---

## ✅ Your task (step by step)

### Part 1 — Create the Mask (Upper Body Mask)
1. In the `Project` window, create an Avatar Mask named `UpperBodyMask`.
2. Open the mask and disable (red color) all leg and pelvis bones. Leave only the spine, shoulders, arms, and head.
3. Save the mask.

### Part 2 — Configure the Layer (Upper Body Layer)
1. In the Animator Controller `SoldierAnimator`, create a new layer named `UpperBody`.
2. Set Weight = 1, Blending = Override.
3. In the Mask field, drag in `UpperBodyMask`.
4. Inside the `UpperBody` layer:
   - Create an empty `Any State` node.
   - Add a `Shoot` state.
   - Create a transition from `Any State` → `Shoot` using the condition `IsShooting == true`.
   - In the transition settings, uncheck `Has Exit Time` (so shooting starts instantly).
   - Create a transition back to `Any State` (or the default empty state) when `IsShooting == false`.
  
### Part 3 — Testing
1. Run the scene.
2. Hold W (run forward).
3. While running, hold down the left mouse button.
4. ✅ Expected result: The character continues running with its legs, but the torso and arms perform the shooting animation. After releasing the shoot button — the arms return to the running animation.

---

## ⭐ Bonus task (for advanced users)
- Add an Additive layer for a weapon recoil effect (light camera or arm shake when shooting).
- Implement smooth layer weight blending via code for a gradual transition into shooting:
```csharp
animator.SetLayerWeight(1, Mathf.Lerp(currentWeight, targetWeight, Time.deltaTime * 5f));
```
- Add a `Reload` animation on the same `UpperBody` layer triggered by another condition (`IsReloading`).

---

## 🔍 Success criteria
- `UpperBodyMask` is created and applied.
- The `UpperBody` layer has the correct mask and settings.
- During run + shoot, the legs do not stop.
- The shooting animation plays only while the left mouse button is held.
- (Bonus) Smooth layer fading or Additive effect.

---

### ⭐ If this project was useful, put a star on GitHub!
