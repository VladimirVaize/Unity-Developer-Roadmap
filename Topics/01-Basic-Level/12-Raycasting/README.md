# 🔦 Raycasting in Unity: Camera/Point Ray, LayerMask, RaycastHit

>[!NOTE]
>Raycasting is a technique where an imaginary ray is cast from a point in a given direction. Unity determines which objects intersect this ray and returns information about them.
>This is an essential tool for: aiming, shooting, mouse clicks on objects, distance measurement, AI (can the enemy see the player), and much more.

---

## 🧠 Core Components of Raycasting

### 1. Ray
`Ray` is a structure containing two parts:
- `origin` — the starting point in space from which the ray is cast.
- `direction` — the vector indicating where the ray travels.

#### Creating a ray:
```csharp
Ray ray = new Ray(originPoint, directionVector);
```

### 2. RaycastHit (Hit Information)

If the ray hits an object, Unity fills a `RaycastHit` structure. It contains:
- `point` — the world-space impact point.
- `normal` — the surface normal at the hit point.
- `collider` — the collider of the object that was hit.
- `distance` — the distance from the ray origin to the hit point.
- `transform` — the transform of the hit object.

#### Usage example:
```csharp
RaycastHit hit;
if (Physics.Raycast(ray, out hit, maxDistance))
{
    Debug.Log("Hit: " + hit.collider.name);
    Debug.Log("Distance: " + hit.distance);
}
```

### 3. LayerMask
`LayerMask` allows you to restrict the ray — it will only interact with objects belonging to the specified layers. 
This is critical for performance and logic (e.g., a shooting ray should not hit the shooter itself).

#### How to use LayerMask:
1. Assign a layer to objects in the Inspector (e.g., `Enemy`, `Player`, `Wall`).
2. In your script, create a mask:
```csharp
public LayerMask enemyLayer; // Assign the Enemy layer in the Inspector
```
3. Pass the mask to the Raycast method:
```csharp
if (Physics.Raycast(ray, out hit, 100f, enemyLayer))
{
    // Hit only enemies
}
```

#### Bitwise operations with LayerMask:
```csharp
// Ignore the "Player" layer
int layerToIgnore = LayerMask.NameToLayer("Player");
LayerMask mask = ~(1 << layerToIgnore); // Invert the bit

// Check only "Enemy" and "Wall" layers
LayerMask mask = LayerMask.GetMask("Enemy", "Wall");
```

---

## 🎯 Types of Raycasts

### 1. From Camera to Screen Point (Mouse Click)
The most common scenario: detect which object the player clicked on.
```csharp
void Update()
{
    if (Input.GetMouseButtonDown(0))
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f))
        {
            Debug.Log("You clicked on: " + hit.collider.name);
        }
    }
}
```
How it works: `ScreenPointToRay` converts 2D mouse coordinates into a ray that goes from the camera through that screen point into the 3D world.

### 2. From an Arbitrary Point in a Direction
For example, a ray from the enemy's eyes toward the player (visibility check).
```csharp
Vector3 enemyEyes = enemy.transform.position + Vector3.up * 1.5f;
Vector3 directionToPlayer = (player.transform.position - enemyEyes).normalized;
Ray ray = new Ray(enemyEyes, directionToPlayer);
RaycastHit hit;
float maxDistance = Vector3.Distance(enemyEyes, player.transform.position);

if (Physics.Raycast(ray, out hit, maxDistance))
{
    if (hit.collider.CompareTag("Player"))
        Debug.Log("Enemy sees the player!");
    else
        Debug.Log("Enemy cannot see the player (blocked by a wall)");
}
```

### 3. Physics.Raycast with Multiple Hits (RaycastAll)
Use when you need all objects along the ray (e.g., a piercing bullet).
```csharp
RaycastHit[] hits = Physics.RaycastAll(ray, 100f);
foreach (RaycastHit hit in hits)
{
    Debug.Log("Hit: " + hit.collider.name);
}
```

### 4. Physics.Raycast with Debugging (Debug.DrawRay)
Very useful for visualizing rays in the Scene View.
```csharp
Debug.DrawRay(origin, direction * distance, Color.red, 1f); // red ray for 1 second
```

---

## ⚙️ Physics.Raycast Overloads (Key Parameters)
```csharp
// The most complete version:
Physics.Raycast(Ray ray, out RaycastHit hit, float maxDistance, int layerMask, QueryTriggerInteraction triggerInteraction);

// maxDistance = 0 means "infinite"
// layerMask = ~0 means "all layers"
// triggerInteraction: Collider, Ignore, Respect
```

---

## 🧪 Example: Aiming and Shooting System
```csharp
public class Gun : MonoBehaviour
{
    public Camera playerCamera;
    public LayerMask shootableLayers; // Only enemies and destructible objects
    public float range = 100f;

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range, shootableLayers))
        {
            Debug.Log($"Hit {hit.collider.name} at distance {hit.distance}");
            
            // Apply damage
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null) enemy.TakeDamage(10);
            
            // Visualize the hit
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green, 0.5f);
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * range, Color.red, 0.5f);
        }
    }
}
```

---

### ⭐ If this project was useful, put a star on GitHub!
