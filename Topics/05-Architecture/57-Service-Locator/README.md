# 🧭 Service Locator: Global Access to Services Without Singletons

This material covers the Service Locator pattern — an alternative to singletons for providing global access to services (audio, saves, analytics, scene management, etc.). 
You'll learn why singletons can be problematic, how Service Locator works, and how to implement it in Unity.

---

## 1. ⚠️ The Singleton Problem
### What is a Singleton?
A Singleton is a class with a static `Instance` field and a private constructor, guaranteeing that only one instance exists.

### Why are Singletons often considered an anti-pattern?

| Problem | Description |
| --- | --- |
| 🔗 Hidden dependencies | Any code can call `AudioManager.Instance.PlaySound()`. It's hard to understand where sounds are actually triggered. |
| 🧪 Testing difficulty | You cannot replace a real service with a mock in unit tests. |
| 🌍 Global state | Application state is scattered across many singletons, leading to unexpected side effects. |
| 🔄 Tight coupling | A class using a singleton is tightly bound to a concrete implementation. Cannot easily swap services. |
| ⏱️ Initialization order | If `AudioManager.Instance` is called before the singleton is created → `NullReferenceException`. |

### Example of problematic code:
```csharp
public class Player : MonoBehaviour
{
    void Die()
    {
        // Hard dependency on a concrete class
        AudioManager.Instance.PlaySound("death");
        SaveManager.Instance.SaveGame();
    }
}
```

---

## 2. 🧩 What is Service Locator?
### Definition:
Service Locator is a design pattern that provides a centralized registry where you can register and retrieve any globally accessible service. 
Instead of calling `AudioManager.Instance`, you write `ServiceLocator.Get<IAudioService>().PlaySound()`.

### Key principles:
- 🎯 Work through interfaces (e.g., `IAudioService`, `ISaveService`).
- 📦 Register services in one place (usually in a root scene or bootstrapper).
- 🔍 Dynamically retrieve a service by its interface type.
- 🔄 Ability to swap implementations without changing dependent code.

### Advantages over Singletons:
| Advantage | Description |
| --- | --- |
| 🔌 Loose coupling | Code depends on an interface, not a concrete class. |
| 🧪 Testability | You can register a mock instead of a real service in tests. |
| 🎮 Different contexts | You can have different service implementations for different scenes/platforms. |
| 📍 Lifecycle control | Services can be created and destroyed when needed. |

---

## 3. 🏗️ Service Locator Implementation in Unity
### Base Service Locator Class
```csharp
using System;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator
{
    private static readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

    /// <summary>
    /// Register a service
    /// </summary>
    public static void Register<T>(T service) where T : class
    {
        Type type = typeof(T);
        if (_services.ContainsKey(type))
        {
            Debug.LogWarning($"Service {type.Name} already registered. Overwriting.");
            _services[type] = service;
        }
        else
        {
            _services.Add(type, service);
        }
    }

    /// <summary>
    /// Get a service
    /// </summary>
    public static T Get<T>() where T : class
    {
        Type type = typeof(T);
        if (_services.TryGetValue(type, out object service))
        {
            return service as T;
        }
        
        throw new Exception($"Service {type.Name} not registered!");
    }

    /// <summary>
    /// Check if a service is registered
    /// </summary>
    public static bool IsRegistered<T>() where T : class
    {
        return _services.ContainsKey(typeof(T));
    }

    /// <summary>
    /// Unregister a service
    /// </summary>
    public static void Unregister<T>() where T : class
    {
        _services.Remove(typeof(T));
    }

    /// <summary>
    /// Clear all services (used when loading a new scene)
    /// </summary>
    public static void Clear()
    {
        _services.Clear();
    }
}
```

### Example Interface and Audio Service Implementation
```csharp
// Service interface
public interface IAudioService
{
    void PlaySound(string soundName);
    void PlayMusic(string musicName);
    void SetVolume(float volume);
}

// Real implementation
public class AudioService : IAudioService
{
    private AudioSource _soundSource;
    private AudioSource _musicSource;
    private Dictionary<string, AudioClip> _clips;

    public AudioService(AudioSource soundSource, AudioSource musicSource, Dictionary<string, AudioClip> clips)
    {
        _soundSource = soundSource;
        _musicSource = musicSource;
        _clips = clips;
    }

    public void PlaySound(string soundName)
    {
        if (_clips.TryGetValue(soundName, out AudioClip clip))
            _soundSource.PlayOneShot(clip);
    }

    public void PlayMusic(string musicName)
    {
        if (_clips.TryGetValue(musicName, out AudioClip clip))
        {
            _musicSource.clip = clip;
            _musicSource.loop = true;
            _musicSource.Play();
        }
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }
}
```

### Service Initialization (Bootstrapper)
```csharp
public class Bootstrapper : MonoBehaviour
{
    [SerializeField] private AudioSource _soundSource;
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioClip[] _audioClips;

    void Awake()
    {
        // Create a clip dictionary
        var clipsDict = new Dictionary<string, AudioClip>();
        foreach (var clip in _audioClips)
            clipsDict[clip.name] = clip;

        // Create the service
        IAudioService audioService = new AudioService(_soundSource, _musicSource, clipsDict);
        
        // Register with Service Locator
        ServiceLocator.Register<IAudioService>(audioService);
        
        DontDestroyOnLoad(this);
    }
}
```

### Using a Service Anywhere
```csharp
public class Player : MonoBehaviour
{
    private IAudioService _audioService;

    void Start()
    {
        // Get the service via Service Locator
        _audioService = ServiceLocator.Get<IAudioService>();
    }

    void Die()
    {
        _audioService.PlaySound("death");
    }
}
```

---

## 4. 🎮 Advanced: Scene-Context Service Locator
In complex projects, it's useful to have different contexts for different scenes (a locator that clears when loading a new scene).
```csharp
public class SceneServiceLocator
{
    private static SceneServiceLocator _current;
    public static SceneServiceLocator Current => _current ?? (_current = new SceneServiceLocator());

    private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

    public void Register<T>(T service) { /* ... */ }
    public T Get<T>() { /* ... */ }
    public void Clear() { _services.Clear(); }

    // Call this when loading a new scene
    public static void Reset()
    {
        _current = new SceneServiceLocator();
    }
}
```

---

## 5. ⚖️ Service Locator vs Dependency Injection (DI)
| Aspect | Service Locator | Dependency Injection (DI) |
| --- | --- | --- |
| Injection complexity | Low — service obtained on demand | Medium/High — requires DI container |
| Dependency visibility | Hidden (call inside method) | Explicit (via constructor or parameters) |
| Testability | Good (can swap registration) | Excellent (dependencies passed explicitly) | 
| Simplicity for small projects | ✅ Great fit | ❌ Overkill |
| Simplicity for large projects | ⚠️ Can lead to chaos | ✅ Recommended |

### When to use Service Locator:
- Small to medium projects.
- Prototyping and game jams.
- When you don't want to pass the same services through 5 levels of nesting.

### When to use DI:
- Large projects with a team of developers.
- High testing requirements.
- Clean architecture and modularity are critical.

---

### ⭐ If this project was useful, put a star on GitHub!
