# 🎯 Code-Based Animation in Unity: DoTween, LeanTween, Interpolations, and Easing Functions

In Unity, there are two main animation approaches: through the Animation Window (creating clips in the editor) and code-based animation. 
The latter provides flexibility, dynamic control, and allows creating animations "on the fly" without creating hundreds of animation clips.

### 📌 When to use code-based animation?
- Popup windows, tooltips, notifications
- Item pickup effects, experience points
- UI animations (buttons, panels, health bars)
- Procedural object swaying (trees, idle enemies)
- Animations that depend on game parameters (stronger hit = stronger bounce)

---

## 📚 1. What are Tween Libraries?
Tween (short for in-between) is an animation method where you specify start and end values, and the library automatically calculates all intermediate states.

Instead of manually writing in `Update()`:
```csharp
float t = 0;
void Update() {
    t += Time.deltaTime / duration;
    transform.position = Vector3.Lerp(startPos, endPos, t);
}
```

You write just one line:
```csharp
transform.DOMove(endPos, duration);  // DOTween
LeanTween.move(gameObject, endPos, duration);  // LeanTween
```

---

## 🚀 2. Popular Tween Libraries
### 🟣 DOTween (Demigiant) — Recommended Choice
Installation: Window → Package Manager → [+ ] → "Add package from git URL" → `https://github.com/Demigiant/dotween`

| Feature | Description |
| --- | --- |
| Functionality | Very rich (almost anything can be animated) |
| UI Compatibility | Excellent (Text, CanvasGroup, Image) |
| Performance | High, low memory allocation |
| Community | Huge, many examples |

Basic Syntax:
```csharp
using DG.Tweening;

// Simple movement
transform.DOMove(new Vector3(5, 0, 0), 1f);

// With additional parameters
transform.DOMove(new Vector3(5, 0, 0), 1f)
    .SetEase(Ease.OutBounce)  // easing type
    .SetLoops(2, LoopType.Yoyo)  // loop with return
    .SetDelay(0.5f);  // delay before start
```

### 🟢 LeanTween — Lightweight Alternative
Installation: Unity Asset Store or GitHub (https://github.com/dentedpixel/LeanTween)

| Feature | Description |
| --- | --- |
| Size | Very lightweight, minimal overhead |
| Speed | Fast animation initialization |
| Syntax | Method chaining, similar to DOTween |

Basic Syntax:
```csharp
// Simple movement
LeanTween.move(gameObject, new Vector3(5, 0, 0), 1f);

// With additional parameters
LeanTween.move(gameObject, new Vector3(5, 0, 0), 1f)
    .setEase(LeanTweenType.easeOutBounce)
    .setLoopPingPong(2)  // 2 back-and-forth cycles
    .setDelay(0.5f);
```

### 📊 Library Comparison (from benchmarks)

| Library | GC Allocation (animation creation) | Startup speed for 50k animations |
| --- | --- | --- |
| PrimeTween | 0 B | ~6.4 ms |
| LeanTween | ~292 B | ~19 ms |
| DOTween | ~732 B | ~33 ms |
| UnityTweens | ~887 B | ~33 ms |

*Data from public benchmarks*

### 💡 How to choose?
- DOTween — for most projects (best balance of functionality and performance)
- LeanTween — for mobile games where build size matters
- PrimeTween — for GC-critical projects (zero allocations)

---

## 🧮 3. Interpolations
Interpolation is a mathematical method for calculating intermediate values between two points.

### Unity's Built-in Methods
| Method | Description | Formula |
| --- | --- | --- |
| `Mathf.Lerp` | Linear interpolation | Uniform movement from A to B |
| `Mathf.LerpUnclamped` | Unclamped linear | Can exceed [0,1] range |
| `Mathf.SmoothStep` | Smooth start and end | Acceleration at start, deceleration at end |
| `Vector3.Lerp | Vector interpolation | For positions, rotations, scales |

```csharp
// SmoothStep example — smooth acceleration and deceleration
float t = (Time.time - startTime) / duration;
float smoothValue = Mathf.SmoothStep(0, 10, t);
transform.position.x = smoothValue;  // Movement with smooth start and finish
```

### Manual Interpolation (without libraries)
If you prefer not to use third-party libraries, you can implement simple animation manually:
```csharp
public class SimpleTween : MonoBehaviour {
    public Vector3 startPos;
    public Vector3 endPos;
    public float duration = 1f;
    private float timer;
    private bool isAnimating;

    public void StartAnimation() {
        startPos = transform.position;
        timer = 0;
        isAnimating = true;
    }

    void Update() {
        if (!isAnimating) return;
        
        timer += Time.deltaTime / duration;
        float t = Mathf.Clamp01(timer);
        
        // Apply easing manually
        float easedT = EaseOutCubic(t);
        
        transform.position = Vector3.Lerp(startPos, endPos, easedT);
        
        if (t >= 1f) isAnimating = false;
    }
    
    private float EaseOutCubic(float x) {
        return 1 - Mathf.Pow(1 - x, 3);
    }
}
```

---

## 📈 4. Easing Functions
Easing functions determine the character of movement: whether an object will start abruptly and stop smoothly, bounce, spring, etc.

### Popular Easing Types
| Type | Visual Description | Use Case |
| --- | --- | --- |
| Linear | Constant speed | Conveyor movement, scrolling |
| Ease.In | Slow start → acceleration | Falling object (gravity) |
| Ease.Out | Fast start → deceleration | Braking car |
| Ease.InOut | Slow → fast → slow | Ball thrown up and falling |
| Ease.OutBounce | With damped bounces | Ball hitting the floor |
| Ease.OutElastic | With overshoot and return | "Springy" button |
| Ease.OutBack | Overshoot past target and return | Sliding menu |

### Examples in DOTween
```csharp
// Ease as a chain parameter
transform.DOMoveX(5, 1f).SetEase(Ease.OutBounce);

// Preset curves
transform.DOScale(1.5f, 0.5f).SetEase(Ease.InElastic);

// Custom curve
AnimationCurve customCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
transform.DOMoveY(10, 2f).SetEase(customCurve);
```

### Examples in LeanTween
```csharp
LeanTween.move(gameObject, targetPos, 1f)
    .setEase(LeanTweenType.easeOutBounce);

LeanTween.scale(gameObject, Vector3.one * 1.5f, 0.5f)
    .setEase(LeanTweenType.easeInElastic);
```

### Mathematical Formulas for Popular Easing
```csharp
// Functions for manual implementation

// Quadratic (acceleration)
float EaseInQuad(float x) => x * x;

// Quadratic (deceleration)
float EaseOutQuad(float x) => 1 - (1 - x) * (1 - x);

// Cubic
float EaseInCubic(float x) => x * x * x;
float EaseOutCubic(float x) => 1 - Mathf.Pow(1 - x, 3);

// Bounce (simplified version)
float EaseOutBounce(float x) {
    if (x < 1 / 2.75) return 7.5625f * x * x;
    if (x < 2 / 2.75) return 7.5625f * (x - 1.5f / 2.75f) * (x - 1.5f / 2.75f) + 0.75f;
    if (x < 2.5 / 2.75) return 7.5625f * (x - 2.25f / 2.75f) * (x - 2.25f / 2.75f) + 0.9375f;
    return 7.5625f * (x - 2.625f / 2.75f) * (x - 2.625f / 2.75f) + 0.984375f;
}
```

---

## 🔄 5. Loop Types

| LoopType | Description | DOTween | LeanTween |
| --- | --- | --- | --- |
| Restart | Restart from beginning | `LoopType.Restart` | `setLoopCount()` (default) |
| Yoyo | Back and forth | `LoopType.Yoyo` | `setLoopPingPong()` |
| Incremental | Cumulative shift | `LoopType.Incremental` | No equivalent |

```csharp
// DOTween: size pulsation
transform.DOScale(1.2f, 0.3f)
    .SetLoops(-1, LoopType.Yoyo);  // Infinite pulsation

// LeanTween: size pulsation
LeanTween.scale(gameObject, Vector3.one * 1.2f, 0.3f)
    .setLoopPingPong();  // Infinite pulsation
```

---

## 🎮 6. Practical Game Examples
### Damage Flash
```csharp
using DG.Tweening;

public class DamageFlash : MonoBehaviour {
    public SpriteRenderer spriteRenderer;
    
    public void Flash() {
        // Color → transparent → back, repeat 3 times
        spriteRenderer.DOColor(Color.clear, 0.1f)
            .SetLoops(6, LoopType.Yoyo);
    }
}
```

### Camera Shake on Hit
```csharp
// DOTween
Camera.main.transform.DOShakePosition(0.5f, 0.3f, 10, 90);

// LeanTween
LeanTween.moveX(Camera.main.gameObject, 
    Camera.main.transform.position.x + 0.2f, 0.05f)
    .setLoopPingPong(5);
```

### Floating Damage Text
```csharp
public void ShowDamageText(int damage, Vector3 worldPosition) {
    GameObject textObj = Instantiate(damagePrefab, worldPosition, Quaternion.identity);
    TextMeshPro text = textObj.GetComponent<TextMeshPro>();
    text.text = damage.ToString();
    
    // Animation: float up and fade out
    Sequence sequence = DOTween.Sequence();
    sequence.Append(textObj.transform.DOMoveY(worldPosition.y + 1.5f, 0.8f));
    sequence.Join(textObj.transform.DOScale(1.5f, 0.3f));
    sequence.Append(textObj.transform.DOScale(0f, 0.2f));
    sequence.OnComplete(() => Destroy(textObj));
}
```

### Idle Object Sway
```csharp
// Infinite smooth swaying
transform.DORotate(new Vector3(0, 0, 15f), 1f)
    .SetLoops(-1, LoopType.Yoyo)
    .SetEase(Ease.InOutSine);
```

### Smooth Health Bar Update
```csharp
public void UpdateHealthBar(float currentHealth, float maxHealth) {
    float targetFill = currentHealth / maxHealth;
    slider.DOValue(targetFill, 0.3f)
        .SetEase(Ease.OutCubic);
}
```

---

## ⚡ 7. Optimization Tips

| Tip | Explanation |
| --- | --- |
| Use SetAutoKill | DOTween destroys completed animations automatically. Disable for animations you might need again (`SetAutoKill(false)`) |
| Don't create thousands of short animations | Better to use one timer with manual update than 1000 separate Tweens per frame |
| Use .Complete() before restarting | `tween.Complete(); tween.Restart();` — ensures clean state | 
| LeanTween.describe() | Debug active animations (LeanTween) |
| DOTween.KillAll() | On scene change — clear all animations to avoid errors |
| Freezing animations | `DOTween.TogglePauseAll()` / `LeanTween.pauseAll()` |

---

### ⭐ If this project was useful, put a star on GitHub!
