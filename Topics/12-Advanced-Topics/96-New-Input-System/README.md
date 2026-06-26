# 🎮 New Input System in Unity: Action Maps, Processors, Interactions — Modern Replacement for Old Input Manager
Unity Input System is a completely redesigned input system that replaces the legacy Input Manager. 
It solves fundamental problems of the old approach: hardcoded key bindings, 
spaghetti code with `Input.GetKey` in `Update()`, and inability to easily switch between different input devices.

---

## 🔥 Why Input System is Better Than the Old Approach?

| Aspect | Old Input Manager | New Input System |
| --- | --- | --- |
| Key binding | Hardcoded (`KeyCode.W`) | Abstract (Action "Move") |
| Device switching | Manual, via `#if` | Automatic, via Control Schemes |
| Gamepad support | Limited | Full, with vibration |
| Mobile devices | Separate logic | Unified system |
| Testing | Only on real device | Emulation in Editor |
| Code | `Update()` with many checks | Event-driven |

---

## 1. Core Concepts
The new system is built on three key abstractions:

| Concept | Description | Example |
| --- | --- | --- |
| Action | Abstract player action | "Move", "Jump", "Fire" |
| Binding | Maps Action to specific keys/devices | Key `W` → Move |
| Action Map | Group of Actions for one context | "Gameplay", "UI", "Menu" |

---

## 2. Action Maps
Action Maps group actions by usage context, avoiding input conflicts.

### 🧱 Typical Action Map Structure:
```text
PlayerControls
├── Gameplay 🎮
│   ├── Move (WASD / Left Stick)
│   ├── Jump (Space / Button South)
│   ├── Fire (Left Mouse / Right Trigger)
│   └── Look (Mouse Delta / Right Stick)
├── UI 📋
│   ├── Navigate (Arrows / D-Pad)
│   ├── Submit (Enter / Button South)
│   └── Cancel (Escape / Button East)
└── Menu 📱
    ├── Pause (Escape / Start)
    └── Select (Mouse Click / Button South)
```

---

## 3. Processors
Processors transform raw device signals before they reach your Action. They can be applied at three levels:
1. On Binding — for a specific binding
2. On Action — for all bindings of an Action
3. On Control — at the device level

### 📋 Popular Processors:

| Processor | Description | Use Case |
| --- | --- | --- |
| `Invert` | Inverts value (Y-axis) | Camera inversion |
| `AxisDeadzone` | Removes small values | Stick drift elimination |
| `StickDeadzone` | Dead zone for sticks | Gamepad: small movements |
| `Scale` | Scales value | Camera sensitivity reduction |
| `Clamp` | Clamps value | Value limiting |
| `Normalize` | Normalizes vector | Vector of length 1 |

---

## 4. Interactions
Interactions define how the system interprets signals from a device. 
Instead of checking GetKeyDown in code, you describe behavior at the Asset level.

### 📋 Popular Interactions:

| Interaction | Description | Signal |
| --- | --- | --- |
| `Tap` | Single tap (quick) | `performed` on release |
| `Hold` | Long press (hold) | `performed` after duration |
| `Press` | Press (any) | `started` / `performed` / `canceled` |
| `SlowTap` | Slow tap | `performed` on release |
| `MultiTap` | Multi-tap | Double/triple click |

---

## 5. Control Schemes
Control Schemes allow configuring different binding sets for different controller types.

### 🎮 Example: Two Device Setup
```yaml
Control Scheme 1: "Keyboard&Mouse"
  - Move: WASD
  - Jump: Space
  - Fire: Left Mouse

Control Scheme 2: "Gamepad"
  - Move: Left Stick
  - Jump: Button South (A)
  - Fire: Right Trigger
```

---

## 6. Best Practices
### ✅ Recommendations:
1. Use Action Maps — separate Gameplay, UI, Menu
2. Generate C# class — convenient and safe
3. Don't mix old and new Input System — choose one
4. Use Events, not polling in Update — event-driven is better
5. Configure Processors in Asset — keeps code clean
6. Test in Editor with different devices — use Input Debugger

---

### ⭐ If this project was useful, put a star on GitHub!
