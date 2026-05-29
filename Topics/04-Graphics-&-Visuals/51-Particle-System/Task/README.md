# 📘 Assignment: Create a particle system "Magic explosion with sparks and smoke"

You are a 2D platformer developer. Implement the magic bomb explosion effect that includes:
- Main burst of sparks (one‑time)
- Particles moving in different directions
- Collision with the ground (2D colliders) and a small smoke effect on impact
- Smoke (another system) that remains after the explosion
- Optimization for mobile devices (max 200 particles)

---

## 🧱 Basic requirements:
1. System A – "Sparks"
   - Emission: 1 Burst with 60 particles at t=0
   - Shape: Circle (radius 0.5)
   - Speed: 2 to 6 in random directions
   - Lifetime: 1 – 1.5 sec
   - Color: yellow → orange → red → transparent
   - Collision: World with 2D Colliders, Dampen = 0.7. Kill on first hit? No – only at end of life.
  
2. Subsystem B – "Small smoke" (Sub‑Emitter)
   - Attached to `A` on the Collision event
   - Each particle of A spawns 1 particle of B on impact
   - B is a slow‑floating grey smoke puff
   - Lifetime = 0.8 sec, small quantity (to avoid thousands of particles)
  
3. System C – "Post‑explosion smoke cloud"
   - Triggered manually from a script 0.2 seconds after the explosion
   - Loop = true, Rate over Time = 10
   - Shape: Circle, radius 2, particles grow and rise slowly
   - Max Particles = 30 (limit)
  
4. Optimization
   - Ensure `Max Particles` in A = 80, in B = 40, in C = 30
   - Use a simple additive material
   - Disable collisions for system C (it doesn't collide)
  
---

## 📦 Bonus (self‑improvement):
- Add particle rotation (`Rotation over Lifetime`)
- Use `Texture Sheet Animation` with a 2x2 spark atlas
- Make particles of B have random size 0.3–0.7

---

## 🧠 Evaluation criteria:
- When the scene runs, the bomb explodes on key press (or timer)
- Sparks fly, collide with platforms, leave smoke traces
- Smoke appears after and lingers for 2–3 seconds, then fades
- FPS does not drop below 58 on an average device

---

### ⭐ If this project was useful, put a star on GitHub!
