# 🔬 Profiler Markers in Unity: Custom Markers for Accurate Profiling
Profiler is Unity's primary tool for performance analysis. However, built-in markers don't always cover your specific code. 
Custom markers (ProfilerMarkers) allow you to add your own labels to the Profiler to accurately measure the execution time of critical code sections.

---

## 1. What Are Profiler Markers?
ProfilerMarker is a tool for measuring the execution time of specific code sections. 
It works like a "stopwatch": you mark the start and end, and Unity records this in the Profiler.

### 🎯 Why Use Custom Markers?
| Task | How Markers Help |
| --- | --- |
| Code optimization | Find the slowest methods |
| Algorithm comparison | Measure which is faster |
| Performance debugging | See where FPS drops occur |
| Team collaboration | Understand who owns which code |
| CI/CD | Automated performance testing |

### 📊 Comparison with Other Methods:
| Method | Accuracy | Performance Impact | In Profiler |
| --- | --- | --- | --- |
| `Stopwatch` | ⭐⭐⭐⭐⭐ | High (slows code) | ❌ No |
| `Debug.Log` | ⭐ | Very High | ❌ No |
| ProfilerMarker | ⭐⭐⭐⭐⭐ | Minimal | ✅ Yes |
| Built-in Profiler | ⭐⭐⭐⭐ | None | ✅ Yes |

---

## 2. Old Approach: Begin/End (Deprecated)
Previously, `Profiler.BeginSample` and `Profiler.EndSample` were used:
```csharp
// ❌ DEPRECATED APPROACH (not recommended)
using UnityEngine.Profiling;

void Update()
{
    Profiler.BeginSample("My Method");
    // ... your code ...
    Profiler.EndSample();
}
```

Issues:
- String parameters cause allocations
- Must close properly (`EndSample`), otherwise errors
- Slower than the new approach

---

## 3. New Approach: ProfilerMarker (Recommended)
```csharp
using Unity.Profiling; // IMPORTANT: New namespace!

public class PerformanceTester : MonoBehaviour
{
    // Create marker once (static or instance)
    private static readonly ProfilerMarker s_UpdateMarker = 
        new ProfilerMarker("Player.Update");
    
    private static readonly ProfilerMarker s_AIUpdateMarker = 
        new ProfilerMarker("AI.Update");
    
    private static readonly ProfilerMarker s_RenderMarker = 
        new ProfilerMarker("Render.Prepare");

    void Update()
    {
        // Using marker
        using (s_UpdateMarker.Auto())
        {
            // All code inside this block will be profiled
            UpdatePlayer();
            UpdateCamera();
        }
        
        // Or manually
        s_AIUpdateMarker.Begin();
        UpdateAI();
        s_AIUpdateMarker.End();
        
        // Nested markers
        using (s_RenderMarker.Auto())
        {
            PrepareRenderData();
        }
    }
}
```

---

## 4. Marker Types and Creation
### 🏗️ Creating a Marker:
```csharp
using Unity.Profiling;

public class MarkerExamples : MonoBehaviour
{
    // 1. Static marker (one per project)
    private static readonly ProfilerMarker StaticMarker = 
        new ProfilerMarker("Static.Marker");
    
    // 2. Instance marker (one per object)
    private readonly ProfilerMarker InstanceMarker = 
        new ProfilerMarker("Instance.Marker");
    
    // 3. Marker with category (for filtering)
    private static readonly ProfilerMarker CategoryMarker = 
        new ProfilerMarker(ProfilerCategory.Scripts, "Category.Marker");
    
    // 4. Marker with dynamic name (via interpolation)
    public void DynamicMarker(string objectName)
    {
        using (new ProfilerMarker($"Dynamic.Object.{objectName}").Auto())
        {
            // Code for specific object
        }
    }
}
```

### 📂 Available Categories:
```csharp
// ProfilerCategory — built-in categories
ProfilerCategory.Scripts      // Scripts
ProfilerCategory.Render       // Rendering
ProfilerCategory.Physics      // Physics
ProfilerCategory.Animation    // Animation
ProfilerCategory.Audio        // Audio
ProfilerCategory.Memory       // Memory
ProfilerCategory.Network      // Network
ProfilerCategory.UI           // UI
ProfilerCategory.Video        // Video
ProfilerCategory.Lighting     // Lighting
ProfilerCategory.Gui          // Old GUI
ProfilerCategory.System       // System calls
```

---

## 5. Practical Examples
### 📊 Example 1: Profiling Scene Loading
```csharp
using Unity.Profiling;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private static readonly ProfilerMarker LoadSceneMarker = 
        new ProfilerMarker("Scene.Load");
    
    private static readonly ProfilerMarker LoadResourcesMarker = 
        new ProfilerMarker("Scene.LoadResources");
    
    private static readonly ProfilerMarker InitObjectsMarker = 
        new ProfilerMarker("Scene.InitObjects");
    
    public void LoadScene(string sceneName)
    {
        using (LoadSceneMarker.Auto())
        {
            SceneManager.LoadScene(sceneName);
            
            using (LoadResourcesMarker.Auto())
            {
                LoadAllResources();
            }
            
            using (InitObjectsMarker.Auto())
            {
                InitializeGameObjects();
                SpawnEnemies();
            }
        }
    }
    
    private void LoadAllResources() { /* ... */ }
    private void InitializeGameObjects() { /* ... */ }
    private void SpawnEnemies() { /* ... */ }
}
```

### 🔄 Example 2: Profiling Algorithms
```csharp
using Unity.Profiling;

public class AlgorithmTester : MonoBehaviour
{
    private static readonly ProfilerMarker BubbleSortMarker = 
        new ProfilerMarker("Algorithms.BubbleSort");
    
    private static readonly ProfilerMarker QuickSortMarker = 
        new ProfilerMarker("Algorithms.QuickSort");
    
    private static readonly ProfilerMarker MergeSortMarker = 
        new ProfilerMarker("Algorithms.MergeSort");
    
    public void CompareSorts(int[] data)
    {
        int[] bubbleData = (int[])data.Clone();
        using (BubbleSortMarker.Auto())
        {
            BubbleSort(bubbleData);
        }
        
        int[] quickData = (int[])data.Clone();
        using (QuickSortMarker.Auto())
        {
            QuickSort(quickData, 0, quickData.Length - 1);
        }
        
        int[] mergeData = (int[])data.Clone();
        using (MergeSortMarker.Auto())
        {
            MergeSort(mergeData, 0, mergeData.Length - 1);
        }
    }
    
    private void BubbleSort(int[] arr) { /* ... */ }
    private void QuickSort(int[] arr, int left, int right) { /* ... */ }
    private void MergeSort(int[] arr, int left, int right) { /* ... */ }
}
```

### 🎮 Example 3: Profiling Game Logic
```csharp
using Unity.Profiling;
using UnityEngine.AI;

public class GameLoopProfiler : MonoBehaviour
{
    private static readonly ProfilerMarker UpdateEnemiesMarker = 
        new ProfilerMarker("GameLoop.UpdateEnemies");
    
    private static readonly ProfilerMarker UpdatePlayerMarker = 
        new ProfilerMarker("GameLoop.UpdatePlayer");
    
    private static readonly ProfilerMarker UpdateProjectilesMarker = 
        new ProfilerMarker("GameLoop.UpdateProjectiles");
    
    private static readonly ProfilerMarker PathfindingMarker = 
        new ProfilerMarker("GameLoop.Pathfinding");
    
    void Update()
    {
        using (UpdatePlayerMarker.Auto())
        {
            UpdatePlayer();
        }
        
        using (UpdateEnemiesMarker.Auto())
        {
            foreach (var enemy in enemies)
            {
                using (PathfindingMarker.Auto())
                {
                    enemy.NavAgent.SetDestination(playerPosition);
                }
                enemy.UpdateBehavior();
            }
        }
        
        using (UpdateProjectilesMarker.Auto())
        {
            UpdateProjectiles();
        }
    }
}
```

### 🎨 Example 4: Profiling Rendering
```csharp
using Unity.Profiling;
using UnityEngine.UI;

public class UIRenderProfiler : MonoBehaviour
{
    private static readonly ProfilerMarker UIMarker = 
        new ProfilerMarker(ProfilerCategory.UI, "UI.Render");
    
    private static readonly ProfilerMarker TextUpdateMarker = 
        new ProfilerMarker("UI.TextUpdate");
    
    private static readonly ProfilerMarker ImageUpdateMarker = 
        new ProfilerMarker("UI.ImageUpdate");
    
    void LateUpdate()
    {
        using (UIMarker.Auto())
        {
            using (TextUpdateMarker.Auto())
            {
                UpdateAllTexts();
            }
            
            using (ImageUpdateMarker.Auto())
            {
                UpdateAllImages();
            }
        }
    }
    
    private void UpdateAllTexts()
    {
        foreach (var text in FindObjectsOfType<Text>())
        {
            text.text = GetUpdatedText(text.name);
        }
    }
    
    private void UpdateAllImages()
    {
        foreach (var image in FindObjectsOfType<Image>())
        {
            image.sprite = GetUpdatedSprite(image.name);
        }
    }
}
```

---

## 6. Advanced Techniques
### 🔄 Nested Markers
```csharp
public class NestedMarkers : MonoBehaviour
{
    private static readonly ProfilerMarker OuterMarker = 
        new ProfilerMarker("Outer.Method");
    
    private static readonly ProfilerMarker InnerMarker = 
        new ProfilerMarker("Inner.Method");
    
    void ComplexMethod()
    {
        using (OuterMarker.Auto())
        {
            using (InnerMarker.Auto())
            {
                DoHeavyWork();
            }
        }
    }
}
```

### ⏱️ Conditional Profiling
```csharp
public class ConditionalProfiling : MonoBehaviour
{
    private static readonly ProfilerMarker HeavyMarker = 
        new ProfilerMarker("Heavy.Operation");
    
    private static readonly ProfilerMarker LightMarker = 
        new ProfilerMarker("Light.Operation");
    
    public bool isDebugMode = false;
    
    void Update()
    {
        if (isDebugMode)
        {
            using (HeavyMarker.Auto())
            {
                HeavyOperation();
            }
        }
        else
        {
            using (LightMarker.Auto())
            {
                LightOperation();
            }
        }
    }
}
```

### 📈 Collecting Statistics via ProfilerRecorder
```csharp
using Unity.Profiling;
using Unity.Profiling.LowLevel;

public class ProfilerRecorderExample : MonoBehaviour
{
    private ProfilerRecorder _recorder;
    private float _averageTime;
    
    void Start()
    {
        _recorder = ProfilerRecorder.StartNew(
            ProfilerCategory.Scripts,
            "My.Custom.Marker",
            30
        );
    }
    
    void Update()
    {
        if (_recorder.Valid)
        {
            _averageTime = _recorder.LastValue / 1000f;
            Debug.Log($"Average time: {_averageTime:F2} ms");
        }
    }
    
    void OnDestroy()
    {
        _recorder.Dispose();
    }
}
```

### 🧵 Profiling Multithreaded Code
```csharp
using Unity.Profiling;
using System.Threading.Tasks;

public class MultithreadedProfiling : MonoBehaviour
{
    private static readonly ProfilerMarker ThreadMarker = 
        new ProfilerMarker("Thread.Work");
    
    void Start()
    {
        Parallel.For(0, 10, i =>
        {
            using (ThreadMarker.Auto())
            {
                DoWork(i);
            }
        });
    }
    
    void DoWork(int id)
    {
        System.Threading.Thread.Sleep(100);
    }
}
```

---

## 7. Debugging Markers
### 🖥️ Viewing Markers in Profiler
1. Open Window → Analysis → Profiler
2. Select CPU Usage tab
3. In Hierarchy, you'll see your markers:
   - `Player.Update`
   - `GameLoop.UpdateEnemies`
   - `Algorithms.QuickSort`
   - etc.
  
### 🔍 Filtering by Category
```csharp
// Markers with category are automatically grouped
private static readonly ProfilerMarker PhysicsMarker = 
    new ProfilerMarker(ProfilerCategory.Physics, "Physics.Custom");
```

### 📱 Viewing on Mobile Devices
```csharp
#if UNITY_ANDROID || UNITY_IOS
    // Markers work even in release builds
    // (need Development Build in Player Settings)
#endif
```

---

## 8. Best Practices
### ✅ Recommendations:
1. Create markers statically — avoid creating new markers in Update
2. Use `using` blocks — they automatically close the marker
3. Give meaningful names — hierarchy with dots: `Category.Subcategory.Method`
4. Use categories — for filtering in Profiler
5. Don't profile micro-operations — markers have overhead
6. Remove markers before release — or use conditional compilation

### ❌ Common Mistakes:
```csharp
// ❌ ERROR: Creating marker in Update (allocations)
void Update()
{
    var marker = new ProfilerMarker("Update"); // BAD!
    using (marker.Auto()) { /* ... */ }
}

// ✅ CORRECT: Static marker
private static readonly ProfilerMarker UpdateMarker = 
    new ProfilerMarker("Update");

// ❌ ERROR: Forgot to close marker
void BadMethod()
{
    Profiler.BeginSample("Bad"); // Deprecated
    // ... if exception here, marker won't close
    Profiler.EndSample();
}

// ✅ CORRECT: using block or try/finally
void GoodMethod()
{
    using (var marker = new ProfilerMarker("Good").Auto())
    {
        // Automatically closes
    }
}

// ❌ ERROR: Too granular profiling
void Update()
{
    using (new ProfilerMarker("EveryFrame").Auto())
    {
        // Hundreds of markers per frame slow down the game
    }
}

// ✅ CORRECT: Profile only important sections
void Update()
{
    if (someCondition)
    {
        using (ConditionalMarker.Auto()) { /* ... */ }
    }
}
```

### 🏷️ Naming Conventions:
| Pattern | Example | Use Case |
| --- | --- | --- |
| `Category.Method` | `AI.Pathfinding` | Simple separation |
| `Module/Subsystem.Method` | `Gameplay/Combat.CalculateDamage` | Module hierarchy |
| `ObjectName.Method` | `PlayerController.Update` | Specific objects |
| `System.Threading` | `Physics.Async` | Multithreading |

---

### ⭐ If this project was useful, put a star on GitHub!
