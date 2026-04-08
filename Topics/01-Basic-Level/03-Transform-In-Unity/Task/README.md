# 🧪 Practical Task: Transform and Hierarchy

## 🎯 Goal
Learn to control position, rotation (Euler and Quaternion), scale, and object hierarchy through the Inspector and scripts in Unity.

## 🧱 Scene for the task
Create a new 3D scene. It will contain:
- A platform (Cube)
- A red cube (ChildCube)
- A blue cube (EnemyCube)
- An empty parent object (ParentHolder)

---

## 📝 Tasks (complete in order)

### 1️⃣ Hierarchy (Parent/Child)
- Create `ParentHolder` (Create Empty).
- Make `ChildCube` (red cube) a child of `ParentHolder` (drag in the Hierarchy).
- Set `ChildCube`'s local position relative to `ParentHolder`: `(1, 0, 0)`.
- Move `ParentHolder` to `(3, 2, 0)`. Where does `ChildCube` end up? (Answer: at world position `(4, 2, 0)`).

### 2️⃣ Position and rotation using Euler
- Place `EnemyCube` (blue cube) at position `(0, 1, 5)`.
- Rotate it around the Y axis by `45°` via the Inspector (Euler).
- Write a short script <a href="../Solution/RotateEnemy.cs"><code>RotateEnemy</code></a> that in `Update()` increases `transform.eulerAngles.y` by `30°` every second. Attach it to `EnemyCube`.

### 3️⃣ Rotation using Quaternion (LookRotation)
- Create an empty object `Target` at position `(2, 1, 0)`.
- Write a script <a href="../Solution/FaceTarget.cs"><code>FaceTarget</code></a> for `ChildCube`: use `Quaternion.LookRotation` so that the red cube always faces `Target`. Do not use Euler.
- Test: move `Target` with the mouse in Scene View — `ChildCube` should rotate instantly.

### 4️⃣ Scale and global coordinates
- Set `ParentHolder`'s scale to `(2, 1, 1)`.
- Print to the console (`Debug.Log`) the global scale (`transform.lossyScale`) for `ChildCube`. Explain why it is not `(1,1,1)`.
- Change `ChildCube`'s local scale to `(0.5, 1, 1)`. Calculate the final X size manually.

### 5️⃣ Bonus (optional)
- Write an <a href="../Solution/Orbit.cs"><code>Orbit</code></a> script that rotates `ChildCube` around `ParentHolder` in a circle using `transform.RotateAround`. Parameters: speed 30° per second, Y axis.

---

## ✅ What is being checked (for self-checking)

- Scripts <a href="../Solution/RotateEnemy.cs"><code>RotateEnemy</code></a>,
  <a href="../Solution/FaceTarget.cs"><code>FaceTarget</code></a>,
  <a href="../Solution/Orbit.cs"><code>Orbit</code></a> (if done).
- Console output values (task 4) and the answer to the `lossyScale` question.

---

### ⭐ If this project was useful, put a star on GitHub!
