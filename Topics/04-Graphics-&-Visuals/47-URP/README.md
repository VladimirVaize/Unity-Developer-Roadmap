# 🎨 Universal Render Pipeline (URP): Setup, Render Features, Advantages

URP is a ready-to-use render pipeline from Unity designed for high-performance graphics on mobile devices, 
consoles, PC, and the web. It replaces the old Built-in Render Pipeline and is more flexible and modern.

In this guide, we'll cover: how to set up URP in your project, what Render Features are, and why URP is better than the old approach.

---

## 1️⃣ Setting up URP in a project
To start using URP, you need to create and install a URP Asset – a file that stores all rendering settings (shadow quality, anti-aliasing, resolution, etc.).

### 🔧 Step-by-step setup:
1. Create a URP Asset:
   - In the `Project` window, right-click → `Create` → `Rendering` → `Universal Render Pipeline` → `Pipeline Asset` (and optionally `Pipeline Asset (Forward Renderer)`).
   - Name it, e.g., `URP_Settings`.
  
2. Assign the URP Asset in Project Settings:
   - `Edit` → `Project Settings` → `Graphics`.
   - Drag your `URP_Settings` into the `Scriptable Render Pipeline Settings` field.
  
3. Upgrade materials (if converting an old project):
   - `Edit` → `Render Pipeline` → `Universal Render Pipeline` → `Upgrade Project Materials to URP Materials`.
  
✅ Done! Your project now uses URP. All new materials will automatically be created with URP support (Lit Shader instead of Standard).

---

## 2️⃣ Render Features
Render Feature is a way to add an extra rendering pass to the pipeline without writing complex code. For example: outline effect, fog, motion blur, screen rain effect, etc.

### 🧩 How to add and configure a Render Feature:
1. Select your Forward Renderer Asset (usually inside the URP Asset or separate).
2. In the Inspector, find the `Render Features` section.
3. Click `Add Render Feature` → choose a built-in one (e.g., `Render Objects` or `Screen Space Ambient Occlusion`).
4. Configure parameters:
   - `Event` — when to execute (before/after opaque geometry, after post-processing, etc.).
   - `Filters` — which objects to affect (by layer, tag, shader).
   - `Material` — which material to apply (e.g., semi-transparent highlight).
  
### 🌟 Example usage:
You want enemies to turn red for a second when damaged, without changing their main material.
- Add a `Render Objects` Feature.
- Set `Layer Mask = Enemies`.
- Specify a `Material` with red color and `Blending = Additive`.
- Choose `Event = AfterRenderingOpaques`.

By enabling this feature via script, you get a temporary effect.

---

## 3️⃣ Advantages of URP over Built-in Render Pipeline

| Feature | Built-in RP | URP |
| --- | --- | --- |
| 🚀 Mobile performance | Medium | High (optimized) | 
| 🎨 Shaders | Standard shader | Lit / Unlit / Simple Lit (lighter) |
| 🌅 Post-processing | Via separate package | Built-in via Volume |
| ✨ Render Features | ❌ No | ✅ Yes (easy to extend) |
| 🔆 Lighting | Several soft shadow types | Core types + adaptive |
| 📱 2D Rendering | Limited | Excellent built-in 2D Renderer |
| 🧪 Future support | Supported but not evolving | Unity's main pipeline |

---

## 💡 Additional benefits:
- Easy migration from Built-in (auto-upgrade materials).
- Universal shaders — work the same on all platforms.
- URP Shader Graph support — create visual effects without code.

---

## 🧠 Summary
- URP — modern, fast, and flexible render pipeline.
- Setup takes 2 minutes: create an Asset and assign in Project Settings.
- Render Features let you add effects without programming.
- Advantages — performance, built-in post-processing, Unity's future.

---

### ⭐ If this project was useful, put a star on GitHub!
