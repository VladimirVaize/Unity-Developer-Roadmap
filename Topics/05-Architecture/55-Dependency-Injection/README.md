# 🧩 Dependency Injection (DI) in Unity: Zenject / Extenject Container

This material covers the Dependency Injection (DI) principle and its implementation in Unity using the Zenject framework (actively maintained fork — Extenject). 
You will learn what problems DI solves, what a container is, how to configure bindings, and how to use injections in code.

---

## 1. 🤔 What is Dependency Injection and why do you need it?
### The problem: tight coupling
In a typical Unity project, classes often create their own dependencies:
```csharp
public class PlayerController : MonoBehaviour
{
    private WeaponService _weaponService;

    private void Start()
    {
        _weaponService = new WeaponService(); // tight coupling
    }
}
```

Drawbacks:
- Cannot easily replace `WeaponService` with another implementation (e.g., for testing or different game modes).
- Hard to test code in isolation.
- Violates the Dependency Inversion Principle (DIP) from SOLID: the class depends on a concrete implementation, not an abstraction.

### Solution: Dependency Injection
Dependency Injection is an approach where a class does not create its own dependencies but receives them from the outside (someone else "injects" them).
```csharp
public class PlayerController : MonoBehaviour
{
    private IWeaponService _weaponService;

    // Dependency comes from outside (via constructor, method, or field)
    public PlayerController(IWeaponService weaponService)
    {
        _weaponService = weaponService;
    }
}
```

Advantages:
- The class becomes independent of concrete implementations.
- Easy to substitute dependencies (e.g., use Mock objects for unit tests).
- Code becomes more flexible, reusable, and testable.

---

## 2. 📦 DI Container: what it is and why you need it
A DI container is a tool that automatically manages object creation and dependency injection. 
You register in the container which interface should be provided by which implementation, 
then simply request the desired object — the container builds the entire dependency chain for you.

Zenject / Extenject is a powerful DI framework built specifically for Unity. It supports:
- Constructor, field, property, and method injection.
- Bindings for regular C# classes and `MonoBehaviour`.
- Lifecycles: singleton, transient, cached.
- Editor-time validation of dependencies.

---

## 3. 🔧 Zenject Core Concepts
### 3.1 Installer
A class where you describe which dependencies and how to register them in the container.
```csharp
using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        // Bindings go here
    }
}
```

### 3.2 Bind
A method that links a type (interface or class) to a specific implementation and its lifecycle.

| Binding Type | Description |
| --- | --- |
| `.To<>()` | Specifies which implementation will be provided for the interface. |
| `.AsSingle()` | One instance per container (singleton). |
| `.AsTransient()` | New instance on each request. |
| `.AsCached()` | One instance, but not a global singleton. |
| `.FromInstance()` | Use an existing instance. |
| `.FromComponentInHierarchy()` | Find a component in the scene hierarchy. |
| `.NonLazy()` | Create the object immediately, even if no one requests it. |

### 3.3 Injection Types

| Type | Example | Note |
| --- | --- | --- |
| Constructor | `public Player(IService s) { }` | Preferred approach |
| Field | `[Inject] private IService _service;` | Convenient for MonoBehaviour |
| Property | `[Inject] public IService Service { get; set; }` | Rarely used |
| Method | `[Inject] public void Construct(IService s) { }` | For post-initialization |

---

## 4. 🎮 Example: Using Zenject in a Project
### Scenario: Sound management via a service
Step 1: Create an interface and implementation
```csharp
public interface IAudioService
{
    void PlaySound(string soundId);
}

public class AudioService : IAudioService
{
    public void PlaySound(string soundId)
    {
        Debug.Log($"Playing sound: {soundId}");
        // Actual playback logic here
    }
}
```

Step 2: Create an Installer
```csharp
using Zenject;

public class AudioInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IAudioService>()
            .To<AudioService>()
            .AsSingle();
    }
}
```

Step 3: Use injection in any class
```csharp
public class UIManager : MonoBehaviour
{
    [Inject] private IAudioService _audioService;

    public void OnButtonClick()
    {
        _audioService.PlaySound("click");
    }
}
```

Step 4: Add the Installer to your scene
- Create an empty GameObject → add a `SceneContext` component.
- In the `Installers` field, add your `AudioInstaller`.
- Or create a `ProjectContext` for global dependencies.

---

## 5. 🧠 Advanced Scenarios
### 5.1 Binding via ScriptableObjectInstaller
Allows you to configure game parameters directly in the editor — changes made in Play mode persist after stopping.
```csharp
public class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
{
    public float playerSpeed = 5f;
    public int maxHealth = 100;

    public override void InstallBindings()
    {
        Container.BindInstance(playerSpeed).WhenInjectedInto<PlayerMovement>();
        Container.BindInstance(maxHealth).WhenInjectedInto<PlayerHealth>();
    }
}
```

### 5.2 Conditional Binding
```csharp
Container.Bind<IDamageCalculator>().To<MeleeDamage>().WhenInjectedInto<Warrior>();
Container.Bind<IDamageCalculator>().To<RangedDamage>().WhenInjectedInto<Archer>();
```

### 5.3 Factories
When you need to create objects dynamically (enemies, bullets):
```csharp
Container.BindFactory<Enemy, EnemyFactory>();

public class EnemySpawner : ITickable
{
    private readonly EnemyFactory _enemyFactory;

    public EnemySpawner(EnemyFactory enemyFactory)
    {
        _enemyFactory = enemyFactory;
    }

    public void Tick()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var enemy = _enemyFactory.Create();
        }
    }
}
```

---

## 6. ⚠️ Common Mistakes and How to Avoid Them

| Mistake | Solution |
| --- | --- |
| Calling `Container.Resolve<>()` all over the codebase | Use constructor or field injection. `Resolve` is an anti-pattern (Service Locator). |
| Forgot to add Installer to Context | Check `SceneContext` or `ProjectContext`. Use `ZenjectValidateScene` to verify. |
| Circular dependency | Use `LazyInject<T>` or a factory. |
| Singleton references a Scoped object | Singleton should not directly depend on a limited-lifetime object. Use a factory or `LazyInject`. |
| Memory leaks with async operations | Always use `CancellationToken` and properly call `Dispose` on scopes. |

---

## 7. 🔄 Alternatives to Zenject

| Name | Features |
| --- | --- |
| VContainer | Lightweight, fast, supports scopes and async operations |
| StrangeIoC | MVCS framework with built-in DI |
| Reflex | Minimalistic DI |

---

### ⭐ If this project was useful, put a star on GitHub!
