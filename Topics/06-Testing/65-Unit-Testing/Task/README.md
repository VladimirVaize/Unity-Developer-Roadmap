# 🎯 Task: «Testing Player Health (PlayerHealth)»
You are developing an RPG game. You have a `PlayerHealth` class that manages character health. 
You need to write unit tests for this class using the Unity Test Framework.

## 📝 PlayerHealth Class (Code to Test):
```csharp
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 100;
    private int _currentHealth;
    
    public int CurrentHealth => _currentHealth;
    public bool IsDead => _currentHealth <= 0;
    
    public event System.Action OnDamageTaken;
    public event System.Action OnDeath;
    
    void Start()
    {
        _currentHealth = _maxHealth;
    }
    
    public void TakeDamage(int amount)
    {
        if (IsDead) return;
        if (amount <= 0) return;
        
        _currentHealth = Mathf.Max(0, _currentHealth - amount);
        OnDamageTaken?.Invoke();
        
        if (IsDead)
        {
            OnDeath?.Invoke();
            Debug.Log("Player died!");
        }
    }
    
    public void Heal(int amount)
    {
        if (IsDead) return;
        if (amount <= 0) return;
        
        _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);
    }
    
    public void ResetHealth()
    {
        _currentHealth = _maxHealth;
    }
}
```

---

## 📋 Testing Tasks:
1. Test 1 (Edit Mode): Verify that upon object creation, health equals maximum (100)
2. Test 2 (Edit Mode): Verify that `TakeDamage(30)` reduces health to 70
3. Test 3 (Edit Mode): Verify that `TakeDamage` cannot reduce health below 0
4. Test 4 (Edit Mode): Verify that `IsDead` becomes `true` when health ≤ 0
5. Test 5 (Edit Mode): Verify that `Heal` cannot exceed maximum health
6. Test 6 (Edit Mode): Verify that after death, the `Heal` method does nothing
7. Test 7 (Play Mode): Using `[UnityTest]`, verify that `ResetHealth()` restores health 0.5 seconds after death

---

## 🧰 Implementation Requirements:
- Create a separate test assembly (.asmdef) for Edit Mode tests
- Use `[SetUp]` to create a `PlayerHealth` instance before each test
- Use `[TearDown]` for cleanup after each test
- For the Play Mode test, use `[UnityTest]` and `yield return new WaitForSeconds(0.5f)`
- Verify that `OnDamageTaken` and `OnDeath` events are called correctly

---

## 💡 Hints:
- Use flags or counters inside the test to verify events
- In Edit Mode tests, create `PlayerHealth` via `new GameObject().AddComponent<PlayerHealth>()`
- Call `ResetHealth()` between tests when needed

---

### ⭐ If this project was useful, put a star on GitHub!
