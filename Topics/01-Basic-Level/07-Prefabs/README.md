# 📦 Prefabs in Unity: Creation, Applying Changes, Nested Prefabs

> [!NOTE]
> Prefabs are one of Unity's most powerful tools. They allow you to create reusable templates of GameObjects.
> Instead of configuring the same enemy 10 times, you build it once as a prefab and then simply "stamp" instances into your scene.
> When you change the original prefab — all of its copies update automatically.

---

## 🔧 1. Creating a Prefab

### How to make a prefab:
1. In the Hierarchy, create and configure any GameObject: add components (Rigidbody, script), assign a sprite/model, colors, parameters.
2. Drag this object from the Hierarchy window into the Project window (typically into a folder like `Assets/Prefabs`).
3. The object in the Hierarchy turns blue — this means it is now an instance of a prefab. The original prefab (blue icon) is stored in the Project window.

### Example: 🧠
You create a "Smart Enemy" with an `EnemyAI` script, 100 health, and an attack sound. Drag it into `Assets/Prefabs` → done. Now drag it into any scene 20 times, and every enemy will behave identically.

---

## ✏️ 2. Applying Changes
There are two ways to change all instances of a prefab:

### Method A — Edit the original prefab (direct)
- Double-click the prefab in the Project window. This opens Prefab Mode — a clean window showing only that object.
- Make changes (e.g., increase health from 100 to 150).
- Press `Save` (button in the top-left) or simply close Prefab Mode. All instances in the scene update.

### Method B — Through an instance in the scene (Overrides)
1. Select any prefab instance in the Hierarchy.
2. Change something in the Inspector (e.g., position, material color).
3. The `Overrides` button appears in the Inspector (highlighted in blue).
4. Click `Overrides` → choose:
   - `Apply All` — apply all changes to the original prefab (all instances will change).
   - `Revert All` — discard changes on this instance only.
   - `Apply Selected` — apply only a specific change.
  
### Example: 🎨
You place 5 copies of a tree prefab. 
You change the scale of one instance, making it larger. The others remain small. 
Then you click `Apply` on that scale change → now all trees become large. 
If you click `Revert`, that tree returns to its original small size.

---

## 🧩 3. Nested Prefabs
This is when one prefab contains another prefab inside it. For example, a `Room` prefab contains `Chair`, `Table`, `Lamp` prefabs. Each of these can be changed independently.

### How to create a nested prefab:
1. Create a `Chair` prefab (drag into Project).
2. Create a `Room` prefab (e.g., an empty GameObject).
3. Drag an instance of `Chair` from the Project directly onto `Room` in the Hierarchy (as a child object).
4. Now `Room` is a parent prefab that contains a child `Chair` prefab.

### What this gives you:
- If you change the original `Chair` prefab (color, size) — all chairs in all rooms update automatically (both inside `Room` and standalone).
- If you change the `Room` prefab (e.g., add a window) — it does not affect individual chairs.
- You can edit the nested prefab inside the parent: double-click `Room` → in Prefab Mode, you see the child `Chair` (it appears blue). Click on it → `Open` — this opens editing of `Chair` itself.

### Complex usage example: 🏰
A `Knight` prefab contains a `Sword` prefab. If you upgrade the `Sword` (increase damage from 10 to 20) — all knights in all scenes start hitting harder. 
However, each knight can still have their own cloak color (this change does not affect all unless you `Apply` it via Overrides).

---

## ⚠️ Important nuances

| Action | Result |
|------------|------------------|
| Change a property on an instance (position, color) | Affects ONLY that instance (Override) |
| `Apply` that change | Changes the original prefab → all instances |
| Remove a component from an instance | Component disappears only on that instance (Override) |
| `Revert All` on an instance | All overrides are discarded, becomes exact copy of original |
| Break the link (`Unpack Prefab` in context menu) | Turns the instance into a regular GameObject with no prefab connection |

---

## 🎯 When to use prefabs?
- ✅ Identical enemies, bullets, trees, coins, buildings.
- ✅ UI elements (buttons, panels, inventory windows).
- ✅ Whole levels or rooms (as nested prefabs).
- ✅ Any object that repeats more than 1-2 times in your project.

