# 🔗 Интеграционное тестирование в Unity: Тестирование игровых сценариев, мок-объекты (mocking)

Интеграционное тестирование проверяет, как различные компоненты системы работают вместе. 
В отличие от юнит-тестов (которые тестируют изолированные куски кода), интеграционные 
тесты проверяют взаимодействие между объектами, сценариями и системами — точно так, 
как это происходит в реальной игре.

---

## 1. Что такое интеграционное тестирование в Unity?
Интеграционный тест проверяет цепочку взаимодействий между несколькими компонентами: 
например, как система здоровья реагирует на урон от врага, или как менеджер уровней обрабатывает завершение уровня.

### 📌 Отличия от юнит-тестирования:

| Аспект | Юнит-тесты | Интеграционные тесты |
| --- | --- | --- |
| Объект тестирования | Отдельный метод или класс | Взаимодействие нескольких систем |
| Изоляция | Полная (моки всех зависимостей) | Частичная (реальные объекты + моки) |
| Скорость выполнения | Очень быстрые | Медленнее (физика, анимация, сцены) |
| Типичное окружение | Edit Mode | Play Mode |
| Стабильность | Высокая | Может зависеть от порядка выполнения |

---

## 2. Тестирование игровых сценариев
Игровые сценарии — это последовательности событий, которые происходят 
во время игры: сражение с боссом, открытие сундука, прохождение уровня.

### 🎮 Пример: Тестирование сценария «Игрок собирает монету»
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
        // Arrange — создаём сцену
        _player = new GameObject("Player");
        _inventory = _player.AddComponent<PlayerInventory>();
        _inventory.initialScore = 0;
        
        _coin = new GameObject("Coin");
        _coinComponent = _coin.AddComponent<Coin>();
        _coinComponent.value = 10;
        
        // Добавляем триггер на монету
        var collider = _coin.AddComponent<SphereCollider>();
        collider.isTrigger = true;
        
        // Добавляем Rigidbody игроку для физики
        var rb = _player.AddComponent<Rigidbody>();
        rb.useGravity = false;
        
        // Act — игрок касается монеты
        _player.transform.position = _coin.transform.position;
        
        // Ждём один кадр для обработки OnTriggerEnter
        yield return null;
        
        // Assert — проверяем результат
        Assert.AreEqual(10, _inventory.Score);
        Assert.IsTrue(_coinComponent.IsCollected);
    }
}
```

### 🧩 Классы для теста (тестируемый код):
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

## 3. Мок-объекты (Mocking) в Unity
Mock-объект (или просто "мок") — это объект-заменитель, который имитирует поведение реального компонента, 
но с контролируемыми реакциями. Моки позволяют изолировать тестируемую систему от её зависимостей.

### 🎭 Зачем нужны моки?

| Проблема | Решение с моком |
| --- | --- |
| Зависимость от сети или базы данных | Мок, возвращающий фиктивные данные |
| Медленные операции (физика, анимация) | Мок с мгновенным ответом |
| Трудно воспроизводимые состояния (победа босса) | Мок, управляемый из теста |
| Сторонние SDK (реклама, аналитика) | Мок с заглушками методов |

### 🔧 Простой пример мока (ручной):
```csharp
// Интерфейс для зависимости
public interface IDamageCalculator
{
    int CalculateDamage(int baseDamage, float defense);
}

// Реальный класс (не тестируемый в интеграционном тесте)
public class RealDamageCalculator : IDamageCalculator
{
    public int CalculateDamage(int baseDamage, float defense)
    {
        // Сложные вычисления с сетевыми вызовами
        return Mathf.RoundToInt(baseDamage * (1 - defense / 100f));
    }
}

// Мок-класс для тестов
public class MockDamageCalculator : IDamageCalculator
{
    public int StubbedDamage { get; set; } = 50;
    
    public int CalculateDamage(int baseDamage, float defense)
    {
        // Просто возвращаем заданное значение
        return StubbedDamage;
    }
}

// Тестируемый класс
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

// Интеграционный тест с моком
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

## 4. Продвинутое мокирование с библиотеками (NSubstitute, Moq)
Unity не имеет встроенной библиотеки для мокирования, но вы можете использовать NSubstitute (рекомендуется) или Moq в редакторе.

### 📦 Установка NSubstitute:
1. Добавьте в проект файл `nuget.config`
2. Используйте NuGetForUnity (бесплатный ассет из Asset Store)
3. Или скачайте DLL вручную и поместите в `Assets/Plugins`

### 📝 Пример с NSubstitute:
```csharp
using NSubstitute;
using NUnit.Framework;

public class AdvancedMockExample
{
    [Test]
    public void AudioService_WhenMusicVolumeChanged_InvokesEvent()
    {
        // Arrange — создаём мок
        var audioSettings = Substitute.For<IAudioSettings>();
        var uiService = Substitute.For<IUIService>();
        
        var settingsManager = new SettingsManager(audioSettings, uiService);
        
        // Настраиваем поведение мока
        audioSettings.MaxVolume.Returns(10);
        
        // Act
        settingsManager.SetMusicVolume(7);
        
        // Assert — проверяем вызовы методов
        audioSettings.Received(1).SetMusicVolume(7);
        uiService.Received(1).ShowToast("Volume changed to 7");
    }
}
```

---

## 5. Создание интеграционных тестов для сцен
Интеграционные тесты могут загружать целые сцены и проверять взаимодействие объектов.

### 🏗️ Пример: Тестирование сцены «Бой с врагом»
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
        // Загружаем тестовую сцену
        SceneManager.LoadScene("CombatScene");
        yield return null; // Ждём один кадр
        
        // Находим объекты
        var player = GameObject.FindWithTag("Player").GetComponent<PlayerHealth>();
        var enemy = GameObject.FindWithTag("Enemy").GetComponent<EnemyAI>();
        var gameUI = GameObject.Find("GameUI").GetComponent<GameUIManager>();
        
        // Подменяем здоровье врага на 1 для быстрой победы
        enemy.SetHealth(1);
        
        // Атакуем врага
        player.Attack(enemy);
        yield return new WaitForSeconds(0.5f);
        
        // Проверяем победу
        Assert.IsTrue(enemy.IsDead);
        Assert.IsTrue(gameUI.VictoryScreen.activeSelf);
    }
}
```

---

## 6. Тестирование временных событий и задержек
Игровые сценарии часто включают задержки (перезарядка оружия, анимации). 
Для их тестирования используйте `yield return new WaitForSeconds()` или моки таймеров.

### ⏱️ Пример: Тестирование перезарядки оружия
```csharp
[UnityTest]
public IEnumerator Weapon_Reload_AfterDelay_CanFireAgain()
{
    // Arrange
    var weapon = new GameObject("Weapon").AddComponent<Weapon>();
    weapon.reloadTime = 1f;
    weapon.ammo = 0;
    
    // Act — начинаем перезарядку
    weapon.StartReload();
    Assert.IsFalse(weapon.CanFire); // Не может стрелять во время перезарядки
    
    // Ждём 0.9 секунды (перезарядка ещё не завершена)
    yield return new WaitForSeconds(0.9f);
    Assert.IsFalse(weapon.CanFire);
    
    // Ждём ещё 0.2 секунды
    yield return new WaitForSeconds(0.2f);
    
    // Assert — перезарядка завершена
    Assert.IsTrue(weapon.CanFire);
    Assert.AreEqual(weapon.maxAmmo, weapon.ammo);
}
```

---

## 7. Тест-дабл (Test Double) и его разновидности

| Тип | Описание | Пример |
| --- | --- | --- |
| Dummy | Передаётся, но никогда не используется | `DummyLogger logger` |
| Stub | Возвращает предопределённые ответы | Всегда возвращает `true` для `IsConnected()` |
| Spy | Запоминает вызовы для последующей проверки | Запись в список каждого вызова `Save()` |
| Mock | Stub + Spy + проверка ожиданий | `mock.Received(1).Save()` |
| Fake | Упрощённая рабочая реализация | InMemoryRepository вместо базы данных |

### 📝 Пример всех типов:
```csharp
// Dummy
public class DummyLogger : ILogger
{
    public void Log(string message) { /* ничего не делает */ }
}

// Stub
public class StubNetwork : INetworkService
{
    public bool IsConnected() => true; // всегда true
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

## 8. Рекомендации по интеграционному тестированию
1. Начинайте с юнит-тестов — сначала проверьте каждый компонент изолированно
2. Не мокайте всё подряд — интеграционный тест должен проверять реальные взаимодействия
3. Используйте теговые сцены — создайте специальную сцену `TestScenes/PlayerCombat` для тестов
4. Очищайте состояние — используйте `[SetUp]` и `[TearDown]` для уничтожения тестовых объектов
5. Ставьте таймауты — тесты с корутинами могут зависнуть
6. Тестируйте граничные случаи — что произойдёт, если враг умирает во время атаки?

### ⏲️ Таймаут для корутин:
```csharp
[UnityTest]
public IEnumerator LongOperation_TimeoutAfter5Seconds()
{
    bool completed = false;
    // Запускаем долгую операцию...
    
    float timeout = 5f;
    float startTime = Time.time;
    
    while (!completed && Time.time - startTime < timeout)
    {
        yield return null;
    }
    
    Assert.IsTrue(completed, "Операция не завершилась за 5 секунд");
}
```

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
