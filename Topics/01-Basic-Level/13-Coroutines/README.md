# 🌀 Coroutines in Unity: IEnumerator, WaitForSeconds, Delayed Actions

> [!NOTE]
> Coroutines are a powerful Unity mechanism that allows you to execute code step by step, spreading its execution over time without blocking the main game thread.
> Unlike `Update()`, which runs every frame, a coroutine can "wait" for a specified time or event, then resume exactly where it left off.

---

## 🔍 What is IEnumerator?
`IEnumerator` is a special return type for coroutine functions. It allows the function to "pause" its execution using the `yield return` keyword.

### Syntax example:
```csharp
using System.Collections;
using UnityEngine;

public class CoroutineExample : MonoBehaviour
{
    void Start()
    {
        // Start the coroutine
        StartCoroutine(MyCoroutine());
    }

    IEnumerator MyCoroutine()
    {
        Debug.Log("Coroutine started");
        yield return null; // Wait 1 frame
        Debug.Log("One frame passed");
    }
}
```
Important: A coroutine must return `IEnumerator` and be started with `StartCoroutine()`.

---

## ⏱️ WaitForSeconds — time-based waiting
`WaitForSeconds` is the most popular way to pause a coroutine for a specified number of seconds (real-time, independent of frame rate).

### Syntax:
```csharp
yield return new WaitForSeconds(2.5f); // Wait 2.5 seconds
```

### Example: blinking object:
```csharp
IEnumerator Blink()
{
    while (true) // Infinite blinking
    {
        GetComponent<Renderer>().enabled = false; // Disable
        yield return new WaitForSeconds(0.5f);
        GetComponent<Renderer>().enabled = true; // Enable
        yield return new WaitForSeconds(0.5f);
    }
}
```

---

## ⏲️ Other types of yields

| Command | Description |
|-------------------------------------|---------------------------------------|
| `yield return null` | Wait 1 frame |
| `yield return new WaitForSeconds(1)` | Wait 1 real-time second |
| `yield return new WaitForSecondsRealtime(1)` | Wait 1 second, ignoring Time.timeScale |
| `yield return new WaitForFixedUpdate()` | Wait for the next physics update (usually 0.02 sec) |
| `yield return new WaitUntil(() => condition)` | Wait until condition becomes true |
| `yield return new WaitWhile(() => condition)` | Wait while condition is true, then continue |
| `yield return StartCoroutine(AnotherCoroutine())` | Wait for another coroutine to finish |

---

## 🎯 Delayed actions — practical scenarios

### 1. Delay before executing an action
```csharp
IEnumerator DestroyAfterDelay(GameObject obj, float delay)
{
    yield return new WaitForSeconds(delay);
    Destroy(obj);
}
// Usage: StartCoroutine(DestroyAfterDelay(enemy, 3f));
```

### 2. Gradual change (animation without Animator)
```csharp
IEnumerator FadeOut(SpriteRenderer renderer, float duration)
{
    float elapsed = 0;
    Color startColor = renderer.color;
    Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0);
    
    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        float t = elapsed / duration;
        renderer.color = Color.Lerp(startColor, endColor, t);
        yield return null; // Wait every frame
    }
}
```

### 3. Spawning enemy waves
```csharp
IEnumerator SpawnWaves()
{
    for (int wave = 1; wave <= 5; wave++)
    {
        Debug.Log($"Wave {wave} starts!");
        for (int i = 0; i < 10; i++)
        {
            Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            yield return new WaitForSeconds(0.5f); // Pause between enemies
        }
        yield return new WaitForSeconds(3f); // Pause between waves
    }
}
```

---

## 🛑 Stopping a coroutine
```csharp
// Start
Coroutine myRoutine = StartCoroutine(MyCoroutine());

// Stop a specific coroutine
StopCoroutine(myRoutine);

// Stop all coroutines on this object
StopAllCoroutines();
```

---

## ⚠️ Important notes
- Coroutines only work on active (`active = true`) GameObjects. If the object is disabled, the coroutine stops.
- `WaitForSeconds` depends on `Time.timeScale`. During a pause (`timeScale = 0`), the wait will never finish. Use `WaitForSecondsRealtime` for pause menus.
- Never use `while(true)` without a `yield return` inside — it will freeze the game.
- Coroutines cannot return values directly. Use `Action` callbacks or events to pass results.

---

### ⭐ If this project was useful, put a star on GitHub!
