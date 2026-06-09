# 🎯 Task: «Coin Collector with 2D Physics»
You are developing a 2D platformer. You need to implement coin collection using 2D physics.

## 📝 Classes to Implement:
1. Player Class `PlayerController2D`:
```csharp
using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private LayerMask groundLayer;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        rb.gravityScale = 2f;
    }
    
    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveX * moveSpeed, rb.linearVelocity.y);
        
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
    
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheckPoint.position, 0.2f, groundLayer);
    }
}
```

2. Coin Class `Coin.cs` (to be implemented):
   - Requirements:
     - Must have `CircleCollider2D` configured as a trigger
     - On collection, increments a counter (static variable or event)
     - Has a rotation animation (rotate around Z-axis via `Transform.Rotate`)
     - On collection, plays a sound and starts a fade-out animation (e.g., scale down)
    
---

## 📋 Specific Tasks to Implement:
1. Physics Setup:
   - Player — `Dynamic` Rigidbody2D with gravity scale 2
   - Ground — `Static` Rigidbody2D with `BoxCollider2D`
   - Coin — `Kinematic` Rigidbody2D (only for trigger)
  
2. Layer Setup:
   - Create layers: `Player`, `Ground`, `Coin`
   - Configure Physics2D Matrix so that:
     - Player collides with Ground
     - Player doesn't physically collide with Coins (only trigger)
     - Coins don't collide with each other
    
3. Sorting Layers Setup:
   - Create Sorting Layers: `Background`, `Gameplay`, `Foreground`, `UI`
   - Place background on `Background`, player and coins on `Gameplay`, UI on `UI`
  
4. Write `Coin` class with the following functionality:
   - `OnTriggerEnter2D` — coin collection
   - `coinValue` field (int) — how many points the coin gives
   - `Collect()` method — disables collider, plays animation, invokes event
   - Use `Destroy(gameObject, 0.5f)` for deletion after animation
  
5. Write `GameManager` class (singleton) for score counting:
   - Static method `AddCoins(int amount)`
   - Display score in console or on UI Text
  
---

## 🧰 Additional Requirements:
- Add a physics material to the ground with friction 0.4 and bounciness 0
- Use `Physics2D.Raycast` for ground checking (instead of `OverlapCircle`) in one of the methods
- Implement a "magnet coin" effect: if the player is nearby (distance < 2), the coin is attracted to the player using `AddForce`

---

## 🔍 Expected Result:
- Player can walk and jump on platforms
- When touching a coin, it disappears and the score increases
- Console outputs: `"Coin collected! Total: X"`
- Coins rotate and have a collection animation

---

## 💡 Scene Structure (example):
```text
Scene
├── Ground (Static Rigidbody2D + BoxCollider2D)
├── Player (PlayerController2D + Dynamic Rigidbody2D + BoxCollider2D + Sprite)
└── Coins (Kinematic Rigidbody2D + CircleCollider2D (isTrigger) + Coin.cs + Sprite)
```

---

### ⭐ If this project was useful, put a star on GitHub!
