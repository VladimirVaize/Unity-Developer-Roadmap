# 🧪 Practical Task: Magical Particle Trail

## 📖 Task Description
You need to create a magical particle trail effect that follows a moving object (e.g., a player or a flying crystal). 
Effect requirements:
- Particles spawn only when the object is moving (not while idle)
- Particles fade over time (opacity decreases)
- Particles have a random color in the range (blue → purple → pink)
- Particles shrink in size over their lifetime
- Particles do not fly sideways but gently fall downward (gravity)
- Use VFX Graph (not the classic particle system)

---

## 🛠️ Step-by-Step Instructions
1. Create a VFX Graph named `MagicTrail`
2. Configure Initialize:
   - Spawn mode: `Rate over Distance` (particles per distance traveled)
   - Position: `Set Position (Plane)` or `Set Position (Mesh)` attached to the object
   - Lifetime: `Set Lifetime (Random)` — 1.0–2.5 seconds
   - Color: `Set Color (Gradient)` — blue → purple → pink (random gradient sampling)
   - Size: `Set Size (Random)` — 0.1–0.4
  
3. Configure Update:
   - `Gravity (weak)` — particles slowly fall down
   - `Drag` — slight air resistance
   - `Fade` — opacity decreases toward the end of lifetime
  
4. Configure Output:
   - Use `Quad Output` with a material that supports transparency (alpha)
   - Enable `Alpha Blending`
  
5. Attach the effect to an object:
   - Add a `Visual Effect` component to your moving object
   - Assign the `MagicTrail` VFX Graph
   - Ensure `Rate over Distance` is used (not time-based spawn)
  
6. (Bonus, ✨) Add a `TrailColor` parameter to the Blackboard and control the trail's color from a script based on speed changes

---

## ✅ Success Criteria
- Effect does NOT spawn particles when the object stands still
- Particles smoothly fade out and shrink
- Trail color varies within the specified gradient range
- Effect maintains 60+ FPS with ≥1000 particles (check via `Window → Analysis → Profiler`)
- You can explain why VFX Graph is better suited for this task than the classic particle system

---

## 💡 Hint
Use the `Set Spawn Rate` block → `Rate over Distance` instead of `Constant`. 
It automatically calculates the distance traveled between frames and spawns particles proportionally to the path length. 
This is the key to a proper trail effect!

---

### ⭐ If this project was useful, put a star on GitHub!
