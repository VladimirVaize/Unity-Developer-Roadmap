# 🗺️ Управление сценами в Unity: LoadScene, Additive режим, DontDestroyOnLoad

Этот материал посвящён трём ключевым механизмам управления сценами в Unity: асинхронная и синхронная загрузка сцен (LoadScene), 
аддитивная загрузка (LoadSceneMode.Additive) и сохранение объектов между сценами (DontDestroyOnLoad). 
Вы узнаете, как строить многоуровневые игры, бесшовные переходы и глобальные менеджеры.

---

## 1. 📂 LoadScene – загрузка сцены
### Для чего нужно:
Метод `SceneManager.LoadScene()` позволяет загрузить новую сцену, полностью заменяя текущую. 
Это основа для переходов между меню, уровнями, экранами Game Over и т.д.

### Основные способы вызова:
```csharp
using UnityEngine.SceneManagement;

// По имени сцены (регистр важен!)
SceneManager.LoadScene("Level02");

// По индексу сцены (порядок в Build Settings)
SceneManager.LoadScene(1);

// С дополнительными параметрами (см. следующий раздел)
SceneManager.LoadScene("Level02", LoadSceneMode.Single); // режим по умолчанию
```

### 🎮 Как использовать:
1. Добавьте все нужные сцены в Build Settings (`File` → `Build Settings` → `Add Open Scenes`).
2. Вызовите загрузку в любом скрипте:
   - При нажатии кнопки "Start Game".
   - При смерти игрока перезагрузить текущий уровень.
   - При завершении уровня загрузить следующий.
  
### Пример – перезагрузка текущего уровня по нажатию `R`:
```csharp
void Update()
{
    if (Input.GetKeyDown(KeyCode.R))
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
```

### ⚠️ Важные нюансы:
- Синхронная загрузка (`LoadScene`) может вызвать "заморозку" игры на долю секунды (особенно на тяжёлых сценах). Для больших уровней используйте асинхронную загрузку.
- После загрузки новой сцены все объекты из предыдущей сцены уничтожаются (если не помечены `DontDestroyOnLoad`).

---

## 2. ➕ LoadSceneMode.Additive – аддитивная загрузка
### Для чего нужно:
Аддитивный режим загружает новую сцену поверх текущей, не удаляя существующие объекты. Это позволяет:
- Делать персистентные миры (подгрузка частей уровня по мере движения игрока).
- Загружать менеджеров и системы (аудио, сохранения) из отдельной сцены.
- Строить бесшовные открытые миры.

### Как использовать:
```csharp
// Загрузить сцену аддитивно
SceneManager.LoadScene("UIScene", LoadSceneMode.Additive);

// Асинхронная аддитивная загрузка (рекомендуется)
StartCoroutine(LoadAdditiveSceneAsync("EnvironmentScene"));
```

### 📌 Реальный пример – загрузка UI поверх игровой сцены:
1. У вас есть сцена `Gameplay` с уровнем и персонажем.
2. Есть сцена `UI` с панелью инвентаря, полоской здоровья и меню паузы.
3. При запуске уровня вы загружаете `UI` аддитивно:
```csharp
SceneManager.LoadScene("UI", LoadSceneMode.Additive);
```

4. Объекты из `UI` живут поверх геймплея. При этом если вы перезагрузите `Gameplay` (аддитивно не нужно), UI останется на месте.

### 🔁 Как выгрузить аддитивную сцену:
```csharp
SceneManager.UnloadSceneAsync("UIScene");
```

Пример – динамическая подгрузка комнат в подземелье:

Игрок подходит к двери → вы асинхронно загружаете сцену `Room_Corridor` аддитивно → когда игрок заходит в коридор, выгружаете предыдущую комнату (`UnloadSceneAsync`). Так работает бесшовный открытый мир.

---

## 3. 🛡️ DontDestroyOnLoad – сохранение объектов между сценами
### Для чего нужно:
Обычно при загрузке новой сцены Unity уничтожает все объекты из старой сцены. Метод `DontDestroyOnLoad()` помечает объект как "неуничтожаемый" – он переживёт загрузку любой новой сцены.

### Типичные применения:
- Глобальный менеджер игры (GameManager) – хранит счёт, жизни, прогресс, настройки.
- Музыкальный плеер (AudioManager) – музыка не прерывается при смене уровней.
- Сетевое соединение – не терять связь с сервером при переходе между экранами.

### Как использовать:
```csharp
void Awake()
{
    // Сделать этот объект неуничтожаемым
    DontDestroyOnLoad(gameObject);
}
```

### ⚠️ Проблема дубликатов (и её решение):
Если вы перезагрузите сцену, где уже есть `GameManager` с `DontDestroyOnLoad`, а в новой сцене тоже есть `GameManager`, то у вас появятся два одинаковых менеджера. Это приводит к ошибкам.

Правильный паттерн – синглтон с проверкой дубликатов:
```csharp
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Уничтожаем дубликат
        }
    }
}
```

### 🧹 Как "убить" объект DontDestroyOnLoad, когда он больше не нужен:
Если вам нужно удалить глобальный объект (например, при выходе в главное меню), просто вызовите `Destroy(gameObject)` вручную. Unity позволит это сделать, несмотря на `DontDestroyOnLoad`.

---

## 4. 🔄 Асинхронная загрузка (Async) – для больших сцен
Синхронная `LoadScene` может вызвать фриз на тяжёлых уровнях. Используйте асинхронный вариант, чтобы показывать прогресс-бар.
```csharp
IEnumerator LoadYourSceneAsync(string sceneName)
{
    AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
    
    // Пока сцена не загружена
    while (!asyncLoad.isDone)
    {
        float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
        Debug.Log($"Загрузка: {progress * 100}%");
        // Обновляйте UI прогресс-бар
        yield return null;
    }
}
```

---

## 5. 🧩 Взаимодействие: полный пример
### Сценарий игры:
1. Главное меню (MainMenu).
2. Игровая сцена (Gameplay).
3. UI-сцена со здоровьем и инвентарём (UI).
4. Глобальный менеджер игры (GameManager) должен жить всегда.

### Реализация:
```csharp
// В MainMenu по кнопке "Start Game"
public void OnStartGame()
{
    // Загружаем GameManager из отдельной сцены (если ещё не загружен)
    SceneManager.LoadScene("GameManagerScene", LoadSceneMode.Additive);
    
    // Загружаем игровую сцену
    SceneManager.LoadScene("Gameplay");
    
    // Загружаем UI поверх
    SceneManager.LoadScene("UI", LoadSceneMode.Additive);
}

// В GameManager (синглтон, DontDestroyOnLoad)
void Awake()
{
    if (Instance == null) 
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    else Destroy(gameObject);
}
```

---

## 📊 Шпаргалка

| Задача | Код |
| --- | --- |
| Загрузить сцену с заменой | `SceneManager.LoadScene("SceneName")` |
| Загрузить аддитивно | `SceneManager.LoadScene("SceneName", LoadSceneMode.Additive)` |
| Выгрузить аддитивную | `SceneManager.UnloadSceneAsync("SceneName")` |
| Сделать объект вечным | `DontDestroyOnLoad(gameObject)` |
| Асинхронная загрузка | `SceneManager.LoadSceneAsync("SceneName")` |

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
