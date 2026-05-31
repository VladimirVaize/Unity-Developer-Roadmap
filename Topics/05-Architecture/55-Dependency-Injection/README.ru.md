# 🧩 Внедрение зависимостей (DI) в Unity: Контейнер Zenject / Extenject

Этот материал посвящён принципу Внедрения зависимостей (Dependency Injection, DI) и его реализации в Unity с помощью фреймворка Zenject (активно поддерживаемый форк — Extenject). 
Вы узнаете, какие проблемы решает DI, что такое контейнер, как настраивать привязки (bindings) и использовать инъекции в коде.

---

## 1. 🤔 Что такое Dependency Injection и зачем он нужен?
### Проблема: жёсткая связанность (tight coupling)

В типичном Unity-проекте классы часто сами создают свои зависимости:
```csharp
public class PlayerController : MonoBehaviour
{
    private WeaponService _weaponService;

    private void Start()
    {
        _weaponService = new WeaponService(); // жёсткая привязка
    }
}
```

Недостатки:
- Нельзя легко заменить `WeaponService` на другую реализацию (например, для тестов или другого режима игры).
- Код сложно тестировать изолированно.
- Нарушается принцип инверсии зависимостей (DIP) из SOLID: класс зависит от конкретной реализации, а не от абстракции.

### Решение: Dependency Injection
Внедрение зависимостей — это подход, при котором класс не создаёт свои зависимости сам, а получает их извне (кто-то другой «внедряет» их)
```csharp
public class PlayerController : MonoBehaviour
{
    private IWeaponService _weaponService;

    // Зависимость приходит извне (через конструктор, метод или поле)
    public PlayerController(IWeaponService weaponService)
    {
        _weaponService = weaponService;
    }
}
```

Преимущества:
- Класс становится независимым от конкретных реализаций.
- Легко подменять зависимости (например, для юнит-тестов использовать Mock-объекты).
- Код становится более гибким, переиспользуемым и тестируемым.

---

## 2. 📦 DI-контейнер: что это и зачем он нужен?
DI-контейнер — это инструмент, который автоматически управляет созданием объектов и внедрением их зависимостей. 
Вы регистрируете в контейнере, какой интерфейс какой реализацией должен быть обеспечен, 
а затем просто запрашиваете нужный объект — контейнер сам построит всю цепочку зависимостей.

Zenject / Extenject — это мощный DI-фреймворк, созданный специально для Unity. Он поддерживает:
- Инъекции в конструкторы, поля, свойства и методы.
- Привязки для обычных C# классов и для `MonoBehaviour`.
- Жизненные циклы: синглтон, временный объект, кэшированный.
- Валидацию зависимостей прямо в редакторе.

---

## 3. 🔧 Основные понятия Zenject
### 3.1 Installer (Установщик)
Класс, в котором описывается, какие зависимости и как нужно зарегистрировать в контейнере.
```csharp
using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        // Здесь будут привязки
    }
}
```

### 3.2 Bind (Привязка)
Метод, связывающий тип (интерфейс или класс) с конкретной реализацией и её жизненным циклом.

| Тип привязки | Описание |
| --- | --- |
| `.To<>()` | Указывает, какая реализация будет предоставлена для интерфейса. |
| `.AsSingle()` | Один экземпляр на весь контейнер (синглтон). |
| `.AsTransient()` | Новый экземпляр при каждом запросе. |
| `.AsCached()` | Один экземпляр, но не синглтон в глобальном смысле. |
| `.FromInstance()` | Использовать готовый экземпляр. |
| `.FromComponentInHierarchy()` | Найти компонент в иерархии сцены. |
| `.NonLazy()` | Создать объект сразу, даже если его никто не запрашивает. |

### 3.3 Типы инъекций

| Тип | Пример | Примечание |
| --- | --- | --- |
| Конструктор | `public Player(IService s) { }` | Предпочтительный способ |
| Поле | `[Inject] private IService _service;` | Удобно для MonoBehaviour |
| Свойство | `[Inject] public IService Service { get; set; }` | Редко используется |
| Метод | `[Inject] public void Construct(IService s) { }` | Для пост-инициализации |

---

## 4. 🎮 Пример использования Zenject в проекте
### Сценарий: управление звуком через сервис
Шаг 1: Создайте интерфейс и реализацию
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
        // Здесь реальная логика воспроизведения
    }
}
```

Шаг 2: Создайте Installer
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

Шаг 3: Используйте инъекцию в любом классе
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

Шаг 4: Добавьте Installer в сцену
- Создайте пустой GameObject → добавьте компонент `SceneContext`.
- В поле `Installers` добавьте ваш `AudioInstaller`.
- Либо создайте `ProjectContext` для глобальных зависимостей.

---

## 5. 🧠 Продвинутые сценарии
### 5.1 Привязка через ScriptableObjectInstaller
Позволяет настраивать параметры игры прямо в редакторе, а изменения в режиме Play сохраняются после остановки.

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

### 5.2 Привязка с условиями (Conditional Binding)
```csharp
Container.Bind<IDamageCalculator>().To<MeleeDamage>().WhenInjectedInto<Warrior>();
Container.Bind<IDamageCalculator>().To<RangedDamage>().WhenInjectedInto<Archer>();
```

### 5.3 Фабрики (Factories)
Когда нужно создавать объекты динамически (враги, пули):
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

## 6. ⚠️ Частые ошибки и как их избежать

| Ошибка | Решение |
| --- | --- |
| Вызов `Container.Resolve<>()` в коде по всему проекту | Используйте инъекции через конструктор или поле. `Resolve` — это анти-паттерн Service Locator. |
| Забыли добавить Installer в контекст | Проверьте `SceneContext` или `ProjectContext`. Используйте `ZenjectValidateScene` для проверки. |
| Циклическая зависимость | Используйте `LazyInject<T>` или фабрику. |
| Синглтон ссылается на Scoped-объект | Синглтон не должен напрямую зависеть от объекта с ограниченным временем жизни. Используйте фабрику или `LazyInject`. |
| Утечки памяти при асинхронных операциях | Всегда используйте `CancellationToken` и корректно вызывайте `Dispose` у скоупов. |

---

## 7. 🔄 Альтернативы Zenject

| Название | Особенности |
| --- | --- |
| VContainer | Лёгкий, быстрый, поддерживает скоупы и асинхронные операции |
| StrangeIoC | MVCS-фреймворк со встроенным DI |
| Reflex | Минималистичный DI |

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
