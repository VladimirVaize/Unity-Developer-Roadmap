# 🎯 Physics2D in Unity: Rigidbody2D, Collider2D, Layers and Differences from 3D Physics
Unity provides two independent physics systems: Physics (3D) and Physics2D (2D). 
They use different engines, components, and settings. 
Physics2D is optimized for a flat world (X and Y axes) 
and has no third dimension (Z is just render depth).

---

## 1. Main Differences Between 2D and 3D Physics

| Aspect | Physics (3D) | Physics2D (2D) |
| --- | --- | --- |
| Movement axes | X, Y, Z (free movement in all three dimensions) | X, Y (plane only, Z used only for sorting) |
| Collider types | BoxCollider, SphereCollider, CapsuleCollider, MeshCollider | BoxCollider2D, CircleCollider2D, CapsuleCollider2D, PolygonCollider2D, EdgeCollider2D |
| Rigidbody component | Rigidbody (3D) | Rigidbody2D |
| Gravity | Vector (default (0, -9.81, 0)) | Usually only Y-axis (default -9.81 on Y) |
| Rotation | Around an arbitrary axis in 3D | Only around Z-axis (rotation angle) |
| Physics materials | PhysicMaterial | PhysicsMaterial2D |
| Collision layer system | Layers + Physics Matrix | Layers + Physics2D Matrix (configured separately) |
| Collision detection | Discrete, Continuous, Continuous Dynamic | Discrete, Continuous (also available but simpler) |
| Performance | Heavier (3D computations) | Lighter (2D computations, less data) |

Main rule: Never mix 2D and 3D physics on the same object — they don't interact with each other. 2D objects only collide with 2D, 3D only with 3D.

---

## 2. Rigidbody2D — The Foundation of Physics Behavior
`Rigidbody2D` is the component that enables physics control for an object: gravity, velocity, rotation, forces.

### 🔧 Key Rigidbody2D Properties:

| Property | Description | Usage Example |
| --- | --- | --- |
| `bodyType` | Body type (Static, Kinematic, Dynamic) | Dynamic = full simulation |
| `gravityScale` | Global gravity multiplier | `1` = normal gravity, `0` = weightlessness |
| `linearVelocity` | Linear velocity (Vector2) | `rb.linearVelocity = new Vector2(5, 0)` |
| `angularVelocity` | Angular velocity (rotation) | `rb.angularVelocity = 180` (deg/sec) |
| `mass` | Mass (affects collisions) | `1` (default) |
| `drag` | Resistance to linear movement | `0.5` (slows down) |
| `angularDrag` | Resistance to rotation | `0.05` |
| `freezeRotation` | Prevents rotation (locks Z axis) | `true` — won't rotate |

### 🎮 Code Examples:
```csharp
using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    private Rigidbody2D rb;
    public float moveSpeed = 5f;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Rigidbody2D setup
        rb.bodyType = RigidbodyType2D.Dynamic; // full physics
        rb.gravityScale = 1f;                  // normal gravity
        rb.freezeRotation = true;               // don't rotate from collisions
        rb.drag = 1f;                          // slight air resistance
    }
    
    void Update()
    {
        // Movement via velocity (recommended for physics)
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        
        rb.linearVelocity = new Vector2(moveX * moveSpeed, rb.linearVelocity.y);
    }
    
    public void Jump(float force)
    {
        // Force impulse for jump
        rb.AddForce(new Vector2(0, force), ForceMode2D.Impulse);
    }
}
```

### 📌 Body Types:

| Type | Description | Example |
| --- | --- | --- |
| `Static` | Doesn't move, doesn't react to forces (ideal for ground and walls) | Ground, walls, platforms |
| `Kinematic` | Moves only via code (`MovePosition`), doesn't react to forces | Elevators, moving platforms |
| `Dynamic` | Full physics simulation, reacts to gravity and collisions | Player, enemies, items |

```csharp
// Kinematic movement example
void MovePlatform(Vector2 newPosition)
{
    rb.MovePosition(newPosition); // teleportation with interpolation
}
```

---

## 3. Collider2D — Shape for Collisions
`Collider2D` defines the object's shape for physical collisions. Without a collider, the object doesn't interact with physics.
### 🧩 Collider2D Types:

| Type | Shape | Best Use Case |
| --- | --- | --- |
| `BoxCollider2D` | Rectangle | Solid platforms, walls, crates |
| `CircleCollider2D` | Circle | Balls, round enemies, items |
| `CapsuleCollider2D` | Capsule (rectangle with semicircles) | Characters, enemies with rounded shapes |
| `PolygonCollider2D` | Custom polygon | Complex shapes, sprites (can generate from outline) |
| `EdgeCollider2D` | Open line (by points) | Line platforms, trampolines |

### 📝 Collider Setup Example:
```csharp
using UnityEngine;

public class ColliderSetup : MonoBehaviour
{
    void Start()
    {
        // Add and configure BoxCollider2D
        BoxCollider2D boxCollider = gameObject.AddComponent<BoxCollider2D>();
        boxCollider.size = new Vector2(1.5f, 1.5f);
        boxCollider.offset = new Vector2(0, 0.5f); // collider offset
        
        // Make collider a trigger (doesn't block movement but generates events)
        boxCollider.isTrigger = true;
    }
}
```

### ⚡ Collision and Trigger Events:
| Type | Methods | Purpose |
| --- | --- | --- |
| Trigger (`isTrigger = true`) | `OnTriggerEnter2D(Collider2D other)` | Damage zones, item collection, door activation |
| Collision (`isTrigger = false`) | `OnCollisionEnter2D(Collision2D collision)` | Physical hits, jumping on platforms |

```csharp
// Trigger — coin collection
void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Player"))
    {
        Destroy(gameObject);
        ScoreManager.AddPoints(10);
    }
}

// Collision — jump on enemy
void OnCollisionEnter2D(Collision2D collision)
{
    if (collision.gameObject.CompareTag("Enemy"))
    {
        // Check if player is on top
        float contactY = collision.contacts[0].point.y;
        float playerBottom = transform.position.y - GetComponent<Collider2D>().bounds.extents.y;
        
        if (contactY > playerBottom)
        {
            // Kill enemy
            Destroy(collision.gameObject);
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.AddForce(Vector2.up * 10f, ForceMode2D.Impulse);
        }
    }
}
```

---

## 4. Layers and Sorting Layers — Collision & Render Organization
Unity has two different layer systems for 2D physics:
### 🎨 Sorting Layers (Render Order)
Determines the render order of sprites (who is on top of whom).

| Sorting Layer | Order (lower = further back) |
| --- | --- |
| Background | -1 (drawn first) |
| Default | 0 (default) |
| Foreground | 1 (above Default) |
| UI | 2 (above everything) |

```csharp
// Sorting Layer setup via code
SpriteRenderer sr = GetComponent<SpriteRenderer>();
sr.sortingLayerName = "Foreground";
sr.sortingOrder = 5; // order within layer (higher = on top)
```

### 🔒 Layers (For Physics)
Layers are used for collision configuration (who collides with whom).

Configuring Physics2D Matrix:
1. `Edit > Project Settings > Physics 2D > Layer Collision Matrix`
2. Check boxes — layers collide, uncheck — they ignore.

```csharp
// Setting Layer via code
gameObject.layer = LayerMask.NameToLayer("Player");

// Ignoring collisions between two layers
Physics2D.IgnoreLayerCollision(
    LayerMask.NameToLayer("Player"),
    LayerMask.NameToLayer("Enemy"),
    ignore: true
);
```

### 📋 Example Collision Matrix:
| Layer | Player | Enemy | Ground | Collectible |
| --- | --- | --- | --- | --- |
| Player | ✅ | ✅ | ✅ | ✅ |
| Enemy | ✅ | ✅ | ✅ | ❌ |
| Ground | ✅ | ✅ | ✅ | ❌ |
| Collectible | ✅ | ❌ | ❌ | ❌ |

---

## 5. Physics Materials (PhysicsMaterial2D)
`PhysicsMaterial2D` configures friction and bounciness of objects.

| Property | Description |
| --- | --- |
| `friction` | Friction force (0 = ice, 1 = rubber) |
| `bounciness` | Elasticity (0 = no bounce, 1 = perfect bounce) |

```csharp
// Creating material in code (or via Asset)
PhysicsMaterial2D bouncyMat = new PhysicsMaterial2D("Bouncy");
bouncyMat.bounciness = 0.8f;
bouncyMat.friction = 0.2f;

// Apply to collider
BoxCollider2D collider = GetComponent<BoxCollider2D>();
collider.sharedMaterial = bouncyMat;
```

---

## 6. Complete Example: 2D Platformer with Physics
```csharp
using UnityEngine;

public class Platformer2D : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D groundCheck;
    
    public float moveSpeed = 10f;
    public float jumpForce = 15f;
    public LayerMask groundLayer;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.freezeRotation = true;
        
        groundCheck = GetComponent<BoxCollider2D>();
        groundCheck.isTrigger = true; // trigger for ground check
    }
    
    void Update()
    {
        // Horizontal movement
        float move = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);
        
        // Jump
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
    
    private bool IsGrounded()
    {
        // Check via OverlapCircle
        Vector2 feetPos = transform.position - Vector3.up * 0.5f;
        return Physics2D.OverlapCircle(feetPos, 0.3f, groundLayer);
    }
    
    void OnDrawGizmos()
    {
        // Visualize ground check
        Gizmos.color = IsGrounded() ? Color.green : Color.red;
        Vector2 feetPos = transform.position - Vector3.up * 0.5f;
        Gizmos.DrawWireSphere(feetPos, 0.3f);
    }
}
```

---

## 7. Useful Physics2D Methods

| Method | Purpose |
| --- | --- |
| `Physics2D.Raycast` | Casts a ray (line-of-sight check) |
| `Physics2D.OverlapCircle` | Checks an area around a point |
| `Physics2D.OverlapBox` | Checks a rectangular area |
| `Physics2D.Linecast` | Checks a line segment between points |
| `Physics2D.IgnoreCollision(collider1, collider2)` | Temporarily ignores collisions |

```csharp
// Ray from player position to the right
RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, 10f);
if (hit.collider != null)
{
    Debug.Log("Hit: " + hit.collider.name);
}
```

---

### ⭐ If this project was useful, put a star on GitHub!
