# 🏷️ Tags and Layers in Unity

> [!Note]
> Tags and Layers are powerful tools for organizing, filtering, and controlling object interactions in Unity.
> They allow you to efficiently solve tasks ranging from finding objects to advanced physics collision configuration.

---

## 🧷 What are Tags?

### Purpose:
A tag is a simple text label that can be assigned to a GameObject. 
Multiple objects can share the same tag. Tags are used for logical grouping (e.g., "Player", "Enemy", "Collectable") and for finding objects without storing direct references.

### How to use:
1. Select an object in the Hierarchy.
2. In the Inspector, find the Tag dropdown (at the top, below the object's name).
3. Choose an existing tag (e.g., `Player`, `Finish`, `EditorOnly`) or create a new one:
   - Click `Add Tag...` → the Tag Manager opens.
   - Enter the new tag name and click `Save`.
   - Your new tag is now available for selection.
4. In C# code, you can check an object's tag or find objects by tag:
```csharp
// Check tag
if (otherGameObject.CompareTag("Player"))
{
    // Do something with the player
}

// Find an object by tag
GameObject player = GameObject.FindWithTag("Player");

// Find all objects with a tag
GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
```

### Usage example:
In your game, there are coins (tag `"Coin"`). When the player touches a coin, you check: 
`if (other.CompareTag("Coin")) { Destroy(other.gameObject); Score += 10; }`. This way, coins don't need to be registered in separate lists.

---

## 🗂️ What are Layers?

### Purpose:
Layers are numeric categories (from 0 to 31) that let you control camera visibility, 
ignore specific objects in raycasts, and configure physics interactions between objects. 
Unlike tags, layers are typically used for mass exclusion or filtering.

### How to use:
1. Select an object → in the Inspector, find the Layer dropdown.
2. Choose an existing layer (`Default`, `TransparentFX`, `Ignore Raycast`, `Water`, `UI`) or create a new one:
   - Click `Add Layer...` → the Tag Manager opens.
   - In `User Layer 8` ... `User Layer 31`, enter your layer names (e.g., `Player`, `Enemy`, `Environment`, `Projectile`).
   - Go back to the object and assign the appropriate layer.
3. Configuring Physics Interactions (Physics Matrix):
   - `Edit` → `Project Settings` → `Physics` (or `Physics 2D` for 2D).
   - Find the Layer Collision Matrix section.
   - This is a table where rows and columns are layers. Check or uncheck boxes to enable/disable collisions between specific layers.
   > Example: You don't want projectiles (layer `Projectile`) to collide with other projectiles, but they should collide with enemies (layer `Enemy`).
   > In the matrix, uncheck `Projectile × Projectile`, keep `Projectile × Enemy` checked.
4. Using Layer Masks in Raycast: <br>
   A LayerMask allows a raycast to ignore all objects except the chosen layers.
```csharp
// Create a mask that includes only "Player" and "Enemy" layers
int layerMask = (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("Enemy"));

// Or: everything except the "Ignore Raycast" layer
layerMask = ~(1 << LayerMask.NameToLayer("Ignore Raycast"));

// Use in Raycast
RaycastHit hit;
if (Physics.Raycast(transform.position, transform.forward, out hit, 100f, layerMask))
{
    Debug.Log("Hit: " + hit.collider.gameObject.name);
}
```

### Usage example:
A camera follows the player. You shoot a ray from the camera toward the player to check if anything (like a wall) is blocking the view. 
Walls are on the `Environment` layer. The ray mask excludes the `Player` layer (so the ray passes through the player) and includes everything else. 
If the ray hits `Environment`, the view is blocked.

---

## ⚙️ Physics Matrix

### Purpose:
Defines which layer pairs can physically collide (trigger `OnCollisionEnter`, `OnTriggerEnter`) and affect movement (Rigidbody).

### How to configure:
  - Go to `Project Settings` → `Physics` (or `Physics 2D`).
  - Find `Layer Collision Matrix`.
  - By default, all layers can collide with all others. Uncheck the intersection of two layers — objects on those layers will pass through each other without physics response (triggers may still work if enabled).

### Practical example:
  - `Player` layer and `Enemy` layer — collide (checked).
  - `Player` layer and `Collectable` layer — collide (checked), but Collectable is a trigger.
  - `Projectile` layer and `Projectile` layer — do NOT collide (unchecked), so bullets don't hit each other.
  - `Projectile` layer and `Player` layer — collide (checked), so the player takes damage.

---

## 🆚 Tag vs Layer – When to Use What?

| Task | Tag | Layer |
|------------------------------|----------|---------|
| Find a specific object (e.g., player) | ✅ `FindWithTag` | ❌ Not intended | 
| Group check inside `OnTriggerEnter` | ✅ `CompareTag` | ✅ `gameObject.layer` |
| Ignore an entire group in Raycast | ❌ Inefficient | ✅ LayerMask |
| Configure physics collisions (who collides with whom) | ❌ Impossible | ✅ Physics Matrix |
| Filter camera rendering (Culling Mask) | ❌ | ✅ |

### Best practice:
  - Use tags for logical identification (who is this? – player, enemy, pickup).
  - Use layers for system-wide settings (physics, visibility, raycasting).

---

### ⭐ If this project was useful, put a star on GitHub!
