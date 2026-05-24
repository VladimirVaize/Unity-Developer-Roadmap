# 🛠️ Task: Lighting a Room with Static Objects and a Moving Character

### Goal:
Learn to distinguish light types and correctly use Baked and Realtime lighting.

---

## 📝 Task Description
You are making a small scene: an enclosed room (floor, walls, ceiling), with a statue on a pedestal in the center (static objects). There is also a moving character (sphere or capsule) that walks around the statue.

### You need to create lighting that:
1. Gives beautiful shadows on the walls and floor from the statue (without high performance cost).
2. Allows the character to cast shadows on static objects and vice versa.
3. Includes at least two types of light sources (Directional and Point OR Spot).

---

## 🧩 Steps to Complete
### 1️⃣ Prepare the scene
- Create a cube for the floor (Scale: X=10, Z=10, Y=0.2). Set it to `Static`.
- Create 4 walls (cubes) around the floor. Set each to `Static`.
- Create a ceiling (cube on top, same scale as floor). `Static`.
- Place a statue (e.g., Cylinder + Sphere on top) on a pedestal (small cube). Set statue and pedestal as `Static`.
- Create a character: `GameObject → 3D Object → Capsule`. It should NOT be Static (it moves).

### 2️⃣ Add a Directional Light
- Set it as sunlight angled 45° downward.
- For shadows, choose `Soft Shadows`.
- Mode: Mixed (to light both static objects and the character).

### 3️⃣ Add a Spot Light or Point Light
- For example, a Spot Light above the statue pointing down.
- Or a Point Light inside the statue for a "glowing crystal" effect.
- For this light source, set Mode = Baked (it doesn't move).

### 4️⃣ Set up Lightmap (Baked)
- Select all static objects (floor, walls, ceiling, statue, pedestal).
- In Inspector: check `Static` (or just `Contribute GI`).
- Open `Window → Rendering → Lighting`.
- In the `Scene` tab, disable Auto Generate (to control the process) or leave it on.
- Click `Generate Lighting` (at the bottom).

### 5️⃣ Set up Realtime for the character
- The character (Capsule) should not be Static.
- For Directional Light (Mixed) — shadows from the character will be real-time.
- Optionally, add a `Light Probe Group` to the character so it receives color information from baked lighting.

### 6️⃣ Check the result
- Run the scene (Play).
- Move the character around the statue.
- Verify:
  - Walls and statue have soft shadows (Lightmap).
  - The character casts real-time shadows and is lit by the Directional Light.
  - The Spot/Point Light above the statue looks beautiful, but its light is "baked" — no runtime cost.
 
---

## 🔍 Self-Check Questions
- Why are the statue and walls Static, but the character is not?
- What would happen if you switched the Directional Light from Mixed to Realtime?
- How would you change the settings if this were a mobile game?

---

## 🌟 Bonus Task (Optional)
Add a moving light source to the room (e.g., a flying firefly — a small sphere with a Point Light, Mode = Realtime). Write a simple script to make it fly in a circle.

---

### ⭐ If this project was useful, put a star on GitHub!
