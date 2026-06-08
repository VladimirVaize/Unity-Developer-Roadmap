# 🎯 Задача: «Тестирование здоровья игрока (PlayerHealth)»
Вы разрабатываете RPG игру. У вас есть класс `PlayerHealth`, который управляет здоровьем персонажа. 
Вам нужно написать юнит-тесты для этого класса, используя Unity Test Framework.

## 📝 Класс PlayerHealth (тестируемый код):
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

## 📋 Задачи для тестирования:
1. Тест 1 (Edit Mode): Проверить, что при создании объекта здоровье равно максимальному (100)
2. Тест 2 (Edit Mode): Проверить, что `TakeDamage(30)` уменьшает здоровье до 70
3. Тест 3 (Edit Mode): Проверить, что `TakeDamage` не может опустить здоровье ниже 0
4. Тест 4 (Edit Mode): Проверить, что `IsDead` становится true при здоровье ≤ 0
5. Тест 5 (Edit Mode): Проверить, что `Heal` не может превысить максимальное здоровье
6. Тест 6 (Edit Mode): Проверить, что при смерти персонажа метод `Heal` ничего не делает
7. Тест 7 (Play Mode): Используя `[UnityTest]`, проверить, что `ResetHealth()` восстанавливает здоровье через 0.5 секунды после смерти

---

## 🧰 Требования к реализации:
- Создайте отдельную тестовую сборку (.asmdef) для Edit Mode тестов
- Используйте `[SetUp]` для создания экземпляра `PlayerHealth` перед каждым тестом
- Используйте `[TearDown]` для очистки после каждого теста
- Для Play Mode теста используйте `[UnityTest]` и `yield return new WaitForSeconds(0.5f)`
- Проверьте, что события `OnDamageTaken` и `OnDeath` вызываются корректно

---

## 💡 Подсказки:
- Для проверки событий используйте флаги или счётчики внутри теста
- В Edit Mode тестах создавайте `PlayerHealth` через `new GameObject().AddComponent<PlayerHealth>()`
- Не забывайте вызывать `ResetHealth()` между тестами при необходимости

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
