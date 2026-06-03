# 🧭 Service Locator: Глобальный доступ к сервисам без синглтонов

Этот материал посвящён паттерну Service Locator — альтернативе синглтонам для предоставления глобального доступа к сервисам 
(аудио, сохранения, аналитика, управление сценами и т.д.). 
Вы узнаете, почему синглтоны могут быть проблемой, как работает Service Locator, и как реализовать его в Unity.

---

## 1. ⚠️ Проблема синглтонов (Singleton)
### Что такое синглтон?
Синглтон — это класс, который имеет статическое поле `Instance` и приватный конструктор, гарантируя, что существует только один экземпляр.

### Почему синглтоны часто считаются анти-паттерном?

| Проблема | Описание |
| --- | --- |
| 🔗 Скрытые зависимости | Любой код может вызвать `AudioManager.Instance.PlaySound()`. Трудно понять, откуда на самом деле вызывается звук. |
| 🧪 Сложность тестирования | Нельзя подменить реальный сервис на заглушку (mock) в юнит-тестах. |
| 🌍 Глобальное состояние | Состояние приложения размазано по множеству синглтонов, что приводит к неожиданным побочным эффектам. |
| 🔄 Жёсткая связанность (tight coupling) | Класс, использующий синглтон, жёстко привязан к конкретной реализации. Нельзя легко заменить один сервис на другой. |
| ⏱️ Порядок инициализации | Если `AudioManager.Instance` вызывается до того, как синглтон создан — возникает `NullReferenceException`. |

### Пример проблемного кода:
```csharp
public class Player : MonoBehaviour
{
    void Die()
    {
        // Жёсткая зависимость от конкретного класса
        AudioManager.Instance.PlaySound("death");
        SaveManager.Instance.SaveGame();
    }
}
```

--- 

## 2. 🧩 Что такое Service Locator?
### Определение:
Service Locator — это паттерн проектирования, который предоставляет централизованный реестр (реестр сервисов), 
где можно зарегистрировать и получить любой глобально доступный сервис. 
Вместо вызова `AudioManager.Instance` вы пишете `ServiceLocator.Get<IAudioService>().PlaySound()`.

### Ключевые принципы:
- 🎯 Работа через интерфейсы (например, `IAudioService`, `SaveService`).
- 📦 Регистрация сервисов в одном месте (обычно в корневой сцене или bootstrapper).
- 🔍 Динамическое получение сервиса по типу интерфейса.
- 🔄 Возможность замены реализации без изменения зависимого кода.

### Преимущества перед синглтонами:
| Преимущество | Описание |
| --- | --- |
| 🔌 Слабая связанность | Код зависит от интерфейса, а не от конкретного класса. |
| 🧪 Тестируемость | В тестах можно зарегистрировать заглушку вместо реального сервиса. |
| 🎮 Разные контексты | Можно иметь разные реализации сервисов для разных сцен / платформ. |
| 📍 Контроль жизненного цикла | Сервисы можно создавать и уничтожать в нужный момент. |

---

## 3. 🏗️ Реализация Service Locator в Unity
### Базовый класс Service Locator
```csharp
using System;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator
{
    private static readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

    /// <summary>
    /// Регистрация сервиса
    /// </summary>
    public static void Register<T>(T service) where T : class
    {
        Type type = typeof(T);
        if (_services.ContainsKey(type))
        {
            Debug.LogWarning($"Сервис {type.Name} уже зарегистрирован. Перезаписываем.");
            _services[type] = service;
        }
        else
        {
            _services.Add(type, service);
        }
    }

    /// <summary>
    /// Получение сервиса
    /// </summary>
    public static T Get<T>() where T : class
    {
        Type type = typeof(T);
        if (_services.TryGetValue(type, out object service))
        {
            return service as T;
        }
        
        throw new Exception($"Сервис {type.Name} не зарегистрирован!");
    }

    /// <summary>
    /// Проверка, зарегистрирован ли сервис
    /// </summary>
    public static bool IsRegistered<T>() where T : class
    {
        return _services.ContainsKey(typeof(T));
    }

    /// <summary>
    /// Удаление сервиса
    /// </summary>
    public static void Unregister<T>() where T : class
    {
        _services.Remove(typeof(T));
    }

    /// <summary>
    /// Очистка всех сервисов (используется при загрузке новой сцены)
    /// </summary>
    public static void Clear()
    {
        _services.Clear();
    }
}
```

### Пример интерфейса и реализации сервиса аудио
```csharp
// Интерфейс сервиса
public interface IAudioService
{
    void PlaySound(string soundName);
    void PlayMusic(string musicName);
    void SetVolume(float volume);
}

// Реальная реализация
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

### Инициализация сервисов (Bootstrapper)
```csharp
public class Bootstrapper : MonoBehaviour
{
    [SerializeField] private AudioSource _soundSource;
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioClip[] _audioClips;

    void Awake()
    {
        // Создаём словарь клипов
        var clipsDict = new Dictionary<string, AudioClip>();
        foreach (var clip in _audioClips)
            clipsDict[clip.name] = clip;

        // Создаём сервис
        IAudioService audioService = new AudioService(_soundSource, _musicSource, clipsDict);
        
        // Регистрируем в Service Locator
        ServiceLocator.Register<IAudioService>(audioService);
        
        DontDestroyOnLoad(this);
    }
}
```

### Использование сервиса в любом месте
```csharp
public class Player : MonoBehaviour
{
    private IAudioService _audioService;

    void Start()
    {
        // Получаем сервис через Service Locator
        _audioService = ServiceLocator.Get<IAudioService>();
    }

    void Die()
    {
        _audioService.PlaySound("death");
    }
}
```

---

## 4. 🎮 Расширенная версия: Service Locator с контекстами сцен
В сложных проектах полезно иметь разные контексты для разных сцен (локатор, который очищается при загрузке новой сцены).
```csharp
public class SceneServiceLocator
{
    private static SceneServiceLocator _current;
    public static SceneServiceLocator Current => _current ?? (_current = new SceneServiceLocator());

    private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

    public void Register<T>(T service) { /* ... */ }
    public T Get<T>() { /* ... */ }
    public void Clear() { _services.Clear(); }

    // Вызывать при загрузке новой сцены
    public static void Reset()
    {
        _current = new SceneServiceLocator();
    }
}
```

---

## 5. ⚖️ Service Locator vs Dependency Injection (DI)

| Аспект | Service Locator | Dependency Injection (DI) |
| Сложность внедрения | Низкая — сервис получается по требованию | Средняя/высокая — требуется контейнер DI |
| Явность зависимостей | Скрытые (вызов внутри метода) | Явные (через конструктор или параметры) |
| Тестируемость | Хорошая (можно подменить регистрацию) | Отличная (зависимости передаются явно) |
| Простота для небольших проектов | ✅ Отлично подходит | ❌ Избыточно |
| Простота для больших проектов | ⚠️ Может привести к хаосу | ✅ Рекомендуется |

### Когда использовать Service Locator:
- Небольшие и средние проекты.
- Прототипирование и game jams.
- Когда вы не хотите передавать одни и те же сервисы через 5 уровней вложенности.

### Когда использовать DI:
- Крупные проекты с командой разработчиков.
- Высокие требования к тестированию.
- Чистая архитектура и модульность критичны.

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
