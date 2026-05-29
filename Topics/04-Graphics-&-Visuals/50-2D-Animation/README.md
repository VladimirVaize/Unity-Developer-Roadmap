# 🎞️ 2D Animation in Unity: Sprite Editor, Sprite Atlas, Skeletal Animation

This material covers three key Unity tools for creating 2D animation: Sprite Editor (slicing spritesheets), 
Sprite Atlas (memory optimization), and Skeletal Animation using Anima2D or third-party solutions (e.g., Spine). 
You'll learn what each tool is for and how to use it in practice.

---

## 1. 🖌️ Sprite Editor
### Purpose:
The Sprite Editor allows you to slice a single large spritesheet into multiple individual sprites. 
This is necessary when an artist gives you one PNG file containing all animation frames of a character 
(e.g., run, jump, attack) or tiles for a tilemap.

### How to use:
1. Import the spritesheet into the `Project` window.
2. Select it, then in the Inspector change `Sprite Mode` from `Single` to `Multiple`.
3. Click the Sprite Editor button (a separate window opens).
4. Use the Slice tool → `Type: Automatic` (or `Grid By Cell Size`). Unity automatically detects frame boundaries.
5. Click `Slice` → `Apply`. Each frame becomes a separate sprite (you'll see them by expanding the original file in `Project`).
6. Manually adjust rectangles and set the `Pivot` (e.g., at the character's feet).

### Example:
You have a `HeroRun.png` spritesheet sized 1024×256 containing 8 run frames of 128×128 pixels. 
You slice it in Sprite Editor, get 8 sprites: `HeroRun_0`, `HeroRun_1` … `HeroRun_7`. 
Then you create a run animation via the Animation window (drag these sprites onto the timeline).

---

## 2. 📦 Sprite Atlas
### Purpose:
A Sprite Atlas packs many small sprites into one large texture (atlas). 
This reduces draw calls and speeds up rendering. 
Especially important for mobile games and UI.

### How to use:
1. In the `Project` window → right-click → `Create` → `Sprite Atlas`.
2. Name it (e.g., `CharacterAtlas`).
3. In the atlas inspector, click `+` in the `Objects for Packing` section and add all required sprites or entire folders (Unity automatically includes all sprites inside).
4. Click Pack Preview — you'll see the sprites packed into one texture.
5. In code and prefabs, use the original sprites as usual — Unity automatically substitutes the correct coordinates from the atlas.
6. For control, enable `Allow Rotation` / `Tight Packing` for denser packing.

### Example:
You have 50 UI sprites (buttons, icons) each 32×32 pixels. 
Without an atlas, each icon is a separate draw call. You create `UIAtlas`, add all icons, 
click Pack Preview — Unity creates a single 1024×1024 texture. Draw calls drop from 50 to 1-2.

---

## 3. 🦴 Skeletal Animation – Anima2D and Third-Party Solutions
### Purpose:
Skeletal animation uses a hierarchy of bones that deform the sprite. 
Instead of hundreds of frames (as in a spritesheet), you animate bone positions and rotations. 
This gives smooth transitions, less memory usage, and the ability to change weapons/clothes without redrawing.

## 🔧 Anima2D (free package from Unity)
### How to use:
1. Install Anima2D via Package Manager (`Window` → `Package Manager` → search for `2D Animation`)
2. Import your character sprite (often split into parts: head, torso, arms, legs).
3. Select the sprite → in the `Sprite Editor` use the `Skinning Editor` tool (appears after installing the package).
4. Create bones: with the `Create Bone` tool, place points from the root (pelvis) to fingertips.
5. Bind sprite parts to bones (weight painting) — each part follows its bone.
6. Open the `Animation` window (`Window` → `Animation` → `Animation`), create a new animation (e.g., `Walk`).
7. Enable recording (red button) → rotate and move bones at different time markers — creates animation keys.
8. Done! The animation plays through the `Animator` component.

### Example:
A character consists of 10 parts (cut in Photoshop). In Anima2D, you create bones: pelvis, spine, neck, left shoulder, etc. 
Bind each part to its corresponding bone. Then create a "jump" animation: at second 0 legs are bent, at second 10 they're extended. 
Plays smoothly, without individual sprites.

## 🎭 Third-party solutions (Spine)
### Purpose:
Spine (by Esoteric Software) is a professional skeletal animation tool, more powerful than Anima2D. 
It allows animation blending, skin attachments, and frame events.

### How to use (integration with Unity):
1. Purchase a Spine license and export animation in `.json` / `.atlas` format.
2. Import the Spine Runtime for Unity from the Asset Store.
3. Place exported files into the `Assets` folder.
4. Drag the `.json` file into the scene — a GameObject appears with a `SkeletonAnimation` component.
5. Control animation via code: `skeletonAnimation.AnimationName = "run";`
6. Advantages: skin physics, curve-based animation, inverse kinematics (IK).

### Example:
In Spine, you create a character with `idle`, `walk`, and `attack` animations. 
In Unity, you simply call `skeletonAnimation.AnimationState.SetAnimation(0, "attack", false)`. 
The animation plays once, then returns to `idle`. Instant blending.

---

## 4. 🔄 How to choose a tool?

| Situation | Tool |
| --- | --- |
| Simple frame-by-frame animations (explosion, power-up) | Sprite Editor + regular Animation (frame by frame) |
| Many identical objects (zombies, bullets) | Sprite Atlas for memory optimization |
| Character with smooth movements and interchangeable parts | Anima2D (free) |
| Professional project with complex animation (IK, skins) | Spine (paid) |

---

### ⭐ If this project was useful, put a star on GitHub!
