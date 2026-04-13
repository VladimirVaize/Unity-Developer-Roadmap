# 🧪 Practical Task: Prefabs in a Warehouse

## Objective
Learn how to create prefabs, apply changes via Overrides, and build nested prefabs.

## Context
You are making a warehouse simulator. The warehouse contains shelves, and on them — boxes. Boxes can hold different products, but all boxes share the same base (model, `Rigidbody` component, `BoxSound` script).

---

## Task (follow the steps)
### 🟢 Step 1. Create the base "Box" prefab
1. Create a `Cube` in the scene (GameObject → 3D Object → Cube).
2. Add a `Rigidbody` component.
3. Add an `Audio Source` component (sound when falling).
4. Attach a `BoxSound` script (imagine it exists — just add an empty dummy script).
5. Rename the object to `Box_Base`.
6. Drag `Box_Base` from the Hierarchy into the Project window into a `Prefabs` folder (create it if needed). It is now a prefab.

### 🟡 Step 2. Create a nested "Shelf" prefab
1. Create an empty GameObject → name it `Shelf_Base`.
2. Create 3 vertical cubes (shelf legs) and 2 horizontal cubes (shelves) — arrange them neatly.
3. Now the key part: drag 4 instances of `Box_Base` from the Project window onto the shelves (inside `Shelf_Base` as child objects).
4. Select the parent object `Shelf_Base` and drag it into the Project window → a `Shelf_Base` prefab is created, which contains the `Box_Base` prefab inside. This is a nested prefab.

### 🔵 Step 3. Modify through an instance (Override)
1. Drag 3 instances of `Shelf_Base` from the Project into the Hierarchy (three different shelves in the scene).
2. On the second shelf, select one of the boxes (the child `Box_Base` object).
3. In the Inspector, change its material color to red (or scale it to (1.5, 1.5, 1.5)).
4. This instance now has an Override (the `Overrides` button on the parent `Shelf_Base` becomes active).
5. Click `Overrides` → `Apply All`. What happened to the boxes on the other shelves? (They also became red/large).

### 🟠 Step 4. Edit the original prefab
1. Double-click `Box_Base` in the Project window → Prefab Mode opens.
2. Increase the box size from (1,1,1) to (1.2, 1.2, 1.2).
3. Click `Save` and close Prefab Mode.
4. Look at all the shelves in the scene. What changed? (All boxes became slightly larger — except those that had their scale overridden? Check!)

### 🔴 Step 5. Unpack and break the connection
1. Select the third shelf in the Hierarchy.
2. Right-click → `Prefab` → `Unpack Prefab` (fully detach).
3. Now delete one box from this shelf. Will boxes on other shelves be deleted? (No, the connection is gone).

---

## ❓ Control questions
- What happens if, after Step 3 (where we applied red color), we change the original `Box_Base` color to blue?
- What is the difference between `Unpack Prefab` and `Unpack Prefab Completely`?
- Why are nested prefabs useful in a real project?

---

## 🏆 Expected result
You should have a scene with multiple shelves where changing one prefab updates all linked instances, while overrides work locally until applied.

---

### ⭐ If this project was useful, put a star on GitHub!
