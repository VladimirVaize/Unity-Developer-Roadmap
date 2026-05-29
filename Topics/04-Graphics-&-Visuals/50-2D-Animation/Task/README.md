# 🧪 Practical Task: 2D Animation of a Hero Character
Goal: Reinforce skills in working with Sprite Editor, Sprite Atlas, and skeletal animation (Anima2D) in Unity. 
You will create an animated hero character with running, jumping, and weapon switching.

---

## 📥 Provided Assets
1. `HeroSpritesheet.png` — spritesheet 1024×512, contains 12 run frames (85×85 px) and 6 jump frames (85×85 px).
2. `HeroParts.png` — a deconstructed character (head, torso, arms, legs, sword).
3. `WeaponIcons` — 5 weapon icons (32×32 px each).

---

## 🎯 Tasks
### Step 1: Slicing Sprites (Sprite Editor)
1. Import `HeroSpritesheet.png` into Unity.
2. In the Inspector, change `Sprite Mode` to `Multiple`.
3. Open Sprite Editor → slice sprites automatically (`Slice` → `Type: Automatic`).
4. Verify you have 18 sprites (12 run + 6 jump).
5. For each jump sprite, set `Pivot` to `Bottom` (base of feet).
6. Click `Apply`.

### Step 2: Creating Frame-by-Frame Animation
1. Select the first 12 run sprites in the `Project` window.
2. Drag them into the scene — Unity automatically creates a `GameObject` with an `Animator` component and the `HeroRun` animation.
3. Similarly, create a jump animation from the 6 sprites (name it `HeroJump`).
4. In the Animator window, create a transition from `HeroRun` to `HeroJump` using a trigger parameter named `Jump`.
5. Write a simple script that calls `animator.SetTrigger("Jump")` when pressing `Space`.

### Step 3: Optimization with Sprite Atlas
1. Create a Sprite Atlas named `HeroAtlas`.
2. Add the folder containing all hero sprites to `Objects for Packing`.
3. Click `Pack Preview` — verify all sprites are packed into one texture.
4. Run the scene and open the Profiler window (`Window` → `Analysis` → `Profiler`) → check the reduction in draw calls (compared to having the atlas disabled).

### Step 4: Skeletal Animation (Anima2D)
1. Import `HeroParts.png` (deconstructed character) into Unity.
2. Install the 2D Animation package (Anima2D) via Package Manager.
3. Select the sprite → Sprite Editor → open the `Skinning Editor` tab.
4. Create bones:
   - Root at the pelvis.
   - Spine → neck → head.
   - Left shoulder → left elbow → left hand.
   - Right shoulder → right elbow → right hand (holds the sword).
  
5. Bind sprite parts to bones (use `Auto Weights` or manually).
6. Create an `Attack` animation in the Animation window (1 second duration):
   - Rotate the right hand with the sword (from -45° to +90°).
  
7. Call `animator.Play("Attack")` on mouse click.

### Step 5: Weapon Switching (Bonus ⭐)
1. Import the 5 weapon icons (`WeaponIcons`).
2. Combine them into a separate Sprite Atlas (`WeaponAtlas`).
3. In the hero script, create an array of weapon sprites.
4. On keys `1`–`5`, change the sprite in the hand (for Anima2D, this will be the `RightHand` slot).

---

## ✅ Success Criteria
- Spritesheet is sliced, run and jump animations work.
- Transition between animations via Space (Jump) is smooth.
- Sprite Atlas reduces draw calls (verified in Profiler).
- Skeletal attack animation with hand rotation is created.
- (⭐) Weapon switching works without recompiling animation.

---

### ⭐ If this project was useful, put a star on GitHub!
