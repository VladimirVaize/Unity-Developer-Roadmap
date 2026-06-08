# 🔗 Integration Testing in Unity: Testing Game Scenarios, Mocking

Integration testing verifies how multiple components work together. 
Unlike unit tests (which test isolated pieces of code), integration tests check interactions between objects, 
scenarios, and systems — exactly as they occur in a real game.

---

## 1. What Is Integration Testing in Unity?
An integration test checks a chain of interactions between multiple components: 
for example, how the health system reacts to damage from an enemy, or how the level manager handles level completion.

### 📌 Differences from Unit Testing:
| Aspect | Unit Tests | Integration Tests | 
| --- | --- | --- |
| Test subject | Single method or class | Interaction of multiple systems |
| Isolation | Complete (mock all dependencies) | Partial (real objects + mocks) |
| Execution speed | Very fast | Slower (physics, animation, scenes) |
| Typical environment	| Edit Mode | Play Mode |
| Stability | High | May depend on execution order |

---

## 2. Testing Game Scenarios
Game scenarios are sequences of events that occur during gameplay: fighting a boss, opening a chest, completing a level.

### 🎮 Example: Testing "Player Collects a Coin" Scenario
```csharp
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CoinCollectionIntegrationTest
{
    private GameObject _player;
    private GameObject _coin;
    private PlayerInventory _inventory;
    private Coin _coinComponent;

    [UnityTest]
    public IEnumerator Scenario_PlayerCollectsCoin_ScoreIncreases()
    {
        // Arrange — create the scene
        _player = new GameObject("Player");
        _inventory = _player.AddComponent<PlayerInventory>();
        _inventory.initialScore = 0;
        
        _coin = new GameObject("Coin");
        _coinComponent = _coin.AddComponent<Coin>();
        _coinComponent.value = 10;
        
        // Add trigger to coin
        var collider = _coin.AddComponent<SphereCollider>();
        collider.isTrigger = true;
        
        // Add Rigidbody to player for physics
        var rb = _player.AddComponent<Rigidbody>();
        rb.useGravity = false;
        
        // Act — player touches the coin
        _player.transform.position = _coin.transform.position;
        
        // Wait one frame for OnTriggerEnter to process
        yield return null;
        
        // Assert — verify result
        Assert.AreEqual(10, _inventory.Score);
        Assert.IsTrue(_coinComponent.IsCollected);
    }
}
```

### 🧩 Classes for the Test (Code Under Test):
```csharp
// PlayerInventory.cs
public class PlayerInventory : MonoBehaviour
{
    public int initialScore = 0;
    public int Score { get; private set; }
    
    void Start()
    {
        Score = initialScore;
    }
    
    public void AddScore(int value)
    {
        Score += value;
    }
}

// Coin.cs
public class Coin : MonoBehaviour
{
    public int value = 10;
    public bool IsCollected { get; private set; }
    
    void OnTriggerEnter(Collider other)
    {
        if (IsCollected) return;
        
        var inventory = other.GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            inventory.AddScore(value);
            IsCollected = true;
            gameObject.SetActive(false);
        }
    }
}
```

---

## 3. Mocking in Unity
A mock object (or "mock") is a substitute object that mimics the behavior of a real component but with controlled responses. 
Mocks allow isolating the system under test from its dependencies.

### 🎭 Why Use Mocks?
| Problem | Solution with Mock |
| --- | --- |
| Dependency on network or database | Mock that returns fake data |
| Slow operations (physics, animations) | Mock with instant response |
| Hard-to-reproduce states (boss victory) | Mock controlled by the test |
| Third-party SDKs (ads, analytics) | Mock with stub methods |

### 🔧 Simple Hand-Written Mock Example:
```csharp
// Interface for the dependency
public interface IDamageCalculator
{
    int CalculateDamage(int baseDamage, float defense);
}

// Real class (not used in this integration test)
public class RealDamageCalculator : IDamageCalculator
{
    public int CalculateDamage(int baseDamage, float defense)
    {
        return Mathf.RoundToInt(baseDamage * (1 - defense / 100f));
    }
}

// Mock class for tests
public class MockDamageCalculator : IDamageCalculator
{
    public int StubbedDamage { get; set; } = 50;
    
    public int CalculateDamage(int baseDamage, float defense)
    {
        return StubbedDamage;
    }
}

// Class under test
public class CombatSystem : MonoBehaviour
{
    private IDamageCalculator _damageCalculator;
    
    public void SetDamageCalculator(IDamageCalculator calculator)
    {
        _damageCalculator = calculator;
    }
    
    public int ProcessAttack(int baseDamage, float defense)
    {
        return _damageCalculator.CalculateDamage(baseDamage, defense);
    }
}

// Integration test with a mock
[Test]
public void Combat_WithMockCalculator_ReturnsStubbedValue()
{
    var combat = new CombatSystem();
    var mock = new MockDamageCalculator { StubbedDamage = 999 };
    combat.SetDamageCalculator(mock);
    
    int result = combat.ProcessAttack(100, 50);
    
    Assert.AreEqual(999, result);
}
```

---

## 4. Advanced Mocking with Libraries (NSubstitute, Moq)
Unity does not have a built-in mocking library, but you can use NSubstitute (recommended) or Moq in the Editor.

### 📦 Installing NSubstitute:
1. Add a `nuget.config` file to your project
2. Use NuGetForUnity (free asset from Asset Store)
3. Or manually download the DLL and place it in `Assets/Plugins`

### 📝 Example with NSubstitute:
```csharp
using NSubstitute;
using NUnit.Framework;

public class AdvancedMockExample
{
    [Test]
    public void AudioService_WhenMusicVolumeChanged_InvokesEvent()
    {
        // Arrange — create mocks
        var audioSettings = Substitute.For<IAudioSettings>();
        var uiService = Substitute.For<IUIService>();
        
        var settingsManager = new SettingsManager(audioSettings, uiService);
        
        // Configure mock behavior
        audioSettings.MaxVolume.Returns(10);
        
        // Act
        settingsManager.SetMusicVolume(7);
        
        // Assert — verify method calls
        audioSettings.Received(1).SetMusicVolume(7);
        uiService.Received(1).ShowToast("Volume changed to 7");
    }
}
```

---

## 5. Creating Integration Tests for Scenes
Integration tests can load entire scenes and verify object interactions.

### 🏗️ Example: Testing the "Enemy Battle" Scene
```csharp
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class CombatSceneIntegrationTest
{
    [UnityTest]
    public IEnumerator CombatScene_PlayerDefeatsEnemy_ShowsVictoryScreen()
    {
        // Load the test scene
        SceneManager.LoadScene("CombatScene");
        yield return null; // Wait one frame
        
        // Find objects
        var player = GameObject.FindWithTag("Player").GetComponent<PlayerHealth>();
        var enemy = GameObject.FindWithTag("Enemy").GetComponent<EnemyAI>();
        var gameUI = GameObject.Find("GameUI").GetComponent<GameUIManager>();
        
        // Set enemy health to 1 for a quick victory
        enemy.SetHealth(1);
        
        // Attack the enemy
        player.Attack(enemy);
        yield return new WaitForSeconds(0.5f);
        
        // Check victory
        Assert.IsTrue(enemy.IsDead);
        Assert.IsTrue(gameUI.VictoryScreen.activeSelf);
    }
}
```

---

## 6. Testing Timed Events and Delays
Game scenarios often include delays (weapon reload, animations). 
Use `yield return new WaitForSeconds()` or time mocks to test them.

### ⏱️ Example: Testing Weapon Reload
```csharp
[UnityTest]
public IEnumerator Weapon_Reload_AfterDelay_CanFireAgain()
{
    // Arrange
    var weapon = new GameObject("Weapon").AddComponent<Weapon>();
    weapon.reloadTime = 1f;
    weapon.ammo = 0;
    
    // Act — start reloading
    weapon.StartReload();
    Assert.IsFalse(weapon.CanFire); // Cannot fire during reload
    
    // Wait 0.9 seconds (reload not yet complete)
    yield return new WaitForSeconds(0.9f);
    Assert.IsFalse(weapon.CanFire);
    
    // Wait another 0.2 seconds
    yield return new WaitForSeconds(0.2f);
    
    // Assert — reload completed
    Assert.IsTrue(weapon.CanFire);
    Assert.AreEqual(weapon.maxAmmo, weapon.ammo);
}
```

---

## 7. Test Doubles and Their Varieties

| Type | Description | Example |
| --- | --- | --- |
| Dummy | Passed but never used | `DummyLogger logger` |
| Stub | Returns predefined answers | Always returns `true` for `IsConnected()` |
| Spy | Records calls for later verification | Records every `Save()` call in a list |
| Mock | Stub + Spy + expectation verification | `mock.Received(1).Save()` |
| Fake | Simplified working implementation | InMemoryRepository instead of database |

### 📝 Example of All Types:
```csharp
// Dummy
public class DummyLogger : ILogger
{
    public void Log(string message) { /* does nothing */ }
}

// Stub
public class StubNetwork : INetworkService
{
    public bool IsConnected() => true; // always true
}

// Spy
public class SpySaveSystem : ISaveSystem
{
    public List<string> SavedData = new List<string>();
    public void Save(string data) => SavedData.Add(data);
}

// Fake
public class FakeTimeService : ITimeService
{
    public float CurrentTime { get; set; } = 0;
    public void Update(float deltaTime) => CurrentTime += deltaTime;
}
```

---

## 8. Best Practices for Integration Testing
1. Start with unit tests — verify each component in isolation first
2. Don't mock everything — integration tests should check real interactions
3. Use tagged scenes — create a dedicated `TestScenes/PlayerCombat` scene for tests
4. Clean up state — use `[SetUp]` and `[TearDown]` to destroy test objects
5. Set timeouts — tests with coroutines can hang
6. Test edge cases — what happens if an enemy dies during an attack?

### ⏲️ Timeout for Coroutines:
```csharp
[UnityTest]
public IEnumerator LongOperation_TimeoutAfter5Seconds()
{
    bool completed = false;
    // Start long operation...
    
    float timeout = 5f;
    float startTime = Time.time;
    
    while (!completed && Time.time - startTime < timeout)
    {
        yield return null;
    }
    
    Assert.IsTrue(completed, "Operation did not complete within 5 seconds");
}
```

---

### ⭐ If this project was useful, put a star on GitHub!
