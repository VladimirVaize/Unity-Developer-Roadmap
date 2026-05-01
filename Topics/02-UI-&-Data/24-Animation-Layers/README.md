# 🎭 Animation Layers and Masks: Blending Animations in Unity

## 📌 Why do you need this?

In real games, characters rarely perform only one action at a time. 
For example, a soldier must run and shoot, a character — walk and wave, and a boss — idle and attack. 
Without special mechanisms, animations would conflict: either the run animation would fully play, or the shooting animation.

Animation Layers and Avatar Masks allow you to overlay animations on top of each other, separating the body into independent parts.

---

## 🧠 Animation Layers

### What are they?
They are like layers in Photoshop, but for animation. Each layer controls a specific set of movements, and Unity blends them according to rules you define.

### Where to find them?
1. Select an Animator Controller (in the Project window).
2. Open the Animator Window (`Window → Animation → Animator`).
3. On the left, you'll see the Layers tab (by default, there's a `Base Layer`).

### Key Layer Parameters

| Parameter | What it does |
|--------------|---------------------------|
| Weight | How strongly this layer influences the final animation (0 = none, 1 = full). |
| Mask | Which body parts this layer controls (e.g., only arms). |
| Blending | `Override` — replaces animation from lower layers. `Additive` — adds motion on top (e.g., flinching). |
| Sync | Automatically mirrors parameters and states from another layer (useful for left/right hand). |
| IK Pass | Enables Inverse Kinematics for this layer. |

### Example of using layers
- Layer 0 (Base Layer): walking, running, jumping — controls the whole body.
- Layer 1 (UpperBody): shooting, waving, attacking — weight 1, mask only on the upper body.
- Layer 2 (IdleSway): gentle swaying when idle — Additive, weight 0.3.

---

## 🎭 Avatar Masks

### What is a Mask?
A Mask is an asset that tells Unity: "Which bones (body parts) this layer is allowed to animate, and which it should leave alone."

### How to create a Mask
1. In the Project window: right-click → `Create → Avatar Mask`.
2. Name it, e.g., `UpperBodyMask`.
3. In the Inspector, a silhouette of the character appears:
   - ✅ Green bones — enabled (the layer can animate them)
   - 🔴 Red bones — disabled (the layer ignores them)
  
4. For blending (run + shoot), you typically create:
   - Upper body mask: spine, shoulders, arms, head enabled; legs and pelvis disabled.
   - Lower body mask: the opposite.
  
### Applying the Mask
1. In the Animator Window, select the layer.
2. In the Mask field, drag your created mask (e.g., `UpperBodyMask`).
3. Now this layer will only affect the upper body.

---

## 🔄 Blending Animations in Practice

### Classic example: running + shooting
Goal: the character runs (`Base Layer`), while the upper body turns and shoots (`UpperBodyLayer`).

#### Step-by-step setup:
1. Create two layers:
   - `Base Layer` — run, walk, jump animations.
   - `UpperBody` — shoot, aim, reload animations.
  
2. Create an `UpperBodyMask`, disabling all leg and pelvis bones (leave spine, arms, head).
3. Assign the mask to the `UpperBody` layer (Mask field).
4. Set Blending = Override (default). Weight = 1.
5. In script, control parameters:

```csharp
animator.SetFloat("Speed", speed);         // for Base Layer
animator.SetBool("IsShooting", isShooting); // for UpperBody
```

6. In the Animator Controller, make a transition to the shooting animation when `IsShooting == true` inside the `UpperBody` layer.

#### Result:
- When `IsShooting = false` — only running is visible.
- When `IsShooting = true` — the legs continue running (from Base Layer), while the arms and torso take the shooting animation (from UpperBody Layer).

---

## ⚙️ Blending Types: Override vs Additive

| Type | Behavior | Use Case |
|-------|--------------|----------------------|
| Override | Replaces the animation of lower layers on masked bones | Running + shooting (arms are completely replaced) |
| Additive | Adds motion on top of existing animations | Gentle breathing sway, hit reaction when damaged |

---

## 🧪 Pro Tips
- 💡 Sync layers — use when you have two similar layers (e.g., left and right hand animations). Changes in one layer automatically apply to the other.
- 💡 Weight can be changed in code — smoothly fade between layers.
- 💡 Masks save time — you don't need to create separate animations for every action combination.
- 💡 Debugging: In the Animator Window, enable Layers → Debug to see the current weight of each layer.

---

### ⭐ If this project was useful, put a star on GitHub!
