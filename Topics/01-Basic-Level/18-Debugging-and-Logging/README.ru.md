# 🐛 Дебаг и логирование в Unity: От Debug.Log до файла на диске

> [!Note]
> Логирование — это «чёрный ящик» вашей игры. Оно позволяет заглянуть внутрь работающего кода, не останавливая выполнение. 
> В этой статье разберём четыре уровня логирования: от простейшего вывода в консоль до записи ошибок в файл в готовом билде.

---

## 📢 1. Debug.Log (и его родственники)
Самый простой и быстрый способ узнать, что происходит в коде.

### Методы семейства `Debug`:
- `Debug.Log("Сообщение")` — обычное информационное сообщение (белый текст).
- `Debug.LogWarning("Предупреждение")` — жёлтое предупреждение (не критично, но стоит обратить внимание).
- `Debug.LogError("Ошибка")` — красная ошибка (обычно прерывает логику, но не останавливает игру).

### Как использовать прямо в коде:
```csharp
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health = 100;

    public void TakeDamage(int amount)
    {
        health -= amount;
        Debug.Log($"Игрок получил {amount} урона. Осталось здоровья: {health}");
        
        if (health <= 0)
        {
            Debug.LogError("ИГРОК МЁРТВ! Триггерим анимацию смерти.");
        }
        
        if (health < 20)
        {
            Debug.LogWarning("Здоровье игрока критически низкое!");
        }
    }
}
```

### Где смотреть результат:
В редакторе Unity — окно Console (`Window → General → Console`).

### Плюсы:
- Мгновенно, просто, не требует настройки.
- В консоли можно кликнуть по сообщению — Unity выделит объект, отправивший лог.

### Минусы:
- Остаются в билде (если не убрать) и снижают производительность.
- Захламляют консоль в финальной версии игры.

---

## 🚦 2. Conditional: Как убрать логи из билда без удаления кода
Атрибут `[Conditional]` позволяет полностью исключить вызов метода при сборке, если не определён символ условной компиляции.

### Пример без Conditional (логи останутся в билде):
```csharp
public static class MyLogger
{
    public static void Log(string message)
    {
        Debug.Log($"[LOG] {message}");
    }
}
```

### Пример с Conditional (логи уйдут из RELEASE билда):
```csharp
using UnityEngine;
using System.Diagnostics;

public static class MyLogger
{
    [Conditional("UNITY_EDITOR")]          // Работает только в редакторе
    public static void LogEditor(string message)
    {
        Debug.Log($"[EDITOR] {message}");
    }
    
    [Conditional("DEVELOPMENT_BUILD")]     // Работает в DEVELOPMENT билдах
    public static void LogDev(string message)
    {
        Debug.Log($"[DEV] {message}");
    }
    
    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void LogEditorOrDev(string message)
    {
        Debug.Log($"[EDITOR OR DEV] {message}");
    }
}
```

### Как это работает:
1. В `Player Settings` → `Other Settings` → `Script Compilation` → `Scripting Define Symbols` добавляете `DEVELOPMENT_BUILD` (или создаёте свой символ, например `MY_GAME_LOGS`).
2. Если символ НЕ определён — все вызовы методов с `[Conditional("СИМВОЛ")]` полностью вырезаются компилятором.
3. Важно: Сам метод и его тело физически есть в коде, но вызовы исчезают — это не ускоряет пустой метод, а удаляет сам вызов.

### Типичные символы в Unity:
- `UNITY_EDITOR` — определён всегда, когда код запущен внутри редактора.
- `DEVELOPMENT_BUILD` — определён для Development билдов (галочка `Development Build` при сборке).
- `UNITY_ANDROID` / `UNITY_IOS` — определяются автоматически при сборке под платформу.

---

## 📦 3. Логирование в билдах: Development vs Release

### Development Build (галочка в Build Settings):
- Включает профилировщик (Profiler).
- Позволяет удалённо отлаживать игру.
- Сохраняет все `Debug.Log` в билде (их можно увидеть через лог-файл).
- Добавляет оверхед по производительности — не для релиза!

### Release Build (без галочки Development Build):
- `Debug.Log` технически вызывается, но редактор Unity в билде не пишет их в консоль. Однако они всё равно выполняются и создают нагрузку.
- Чтобы убрать нагрузку — используйте `[Conditional]` или директивы препроцессора `#if ... #endif`.

### Директивы препроцессора (альтернатива Conditional):
```csharp
public static void MyExpensiveLog(string message)
{
    #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log(message);
    #endif
}
```
Разница с `[Conditional]`: директива убирает весь код внутри блока, а `Conditional` убирает только вызовы методов, оставляя тело метода в сборке. Для большинства случаев `Conditional` — чище и удобнее.

---

## 💾 4. Вывод логов в файл на диске
В готовом билде консоли нет. Чтобы узнать, что пошло не у игрока, нужно писать логи в файл.

### Где лежат логи Unity:
- Windows: `%USERPROFILE%\AppData\LocalLow\<CompanyName>\<ProductName>\Player.log`
- Mac: `~/Library/Logs/<CompanyName>/<ProductName>/Player.log`
- Linux: `~/.config/unity3d/<CompanyName>/<ProductName>/Player.log`

`CompanyName` и `ProductName` берутся из `Edit → Project Settings → Player`.

### Как дописать свой лог в этот файл:
```csharp
using UnityEngine;
using System.IO;

public class FileLogger : MonoBehaviour
{
    private string logPath;
    
    void Awake()
    {
        // Application.persistentDataPath — ещё один вариант (кросс-платформенная папка)
        logPath = Path.Combine(Application.persistentDataPath, "my_game_log.txt");
        Debug.Log($"Лог-файл будет здесь: {logPath}");
        
        // Перехватываем все сообщения Debug.Log
        Application.logMessageReceived += HandleLog;
    }
    
    void HandleLog(string logString, string stackTrace, LogType type)
    {
        string entry = $"[{System.DateTime.Now:HH:mm:ss}] [{type}] {logString}\n";
        if (type == LogType.Error || type == LogType.Exception)
        {
            entry += $"СТЕК: {stackTrace}\n";
        }
        
        File.AppendAllText(logPath, entry);
    }
    
    void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }
    
    // Ручная запись
    public static void WriteToFile(string message)
    {
        string path = Path.Combine(Application.persistentDataPath, "my_game_log.txt");
        File.AppendAllText(path, $"[{System.DateTime.Now:HH:mm:ss}] {message}\n");
    }
}
```

### Подписка на `Application.logMessageReceived`:
- Ловит все вызовы `Debug.Log`, `Debug.LogWarning`, `Debug.LogError`, даже из чужих плагинов.
- Срабатывает в билдах, включая Release.
- Позволяет продублировать всё в файл, не переписывая каждое лог-сообщение.

---

## 🧠 Вывод: Стратегия логирования в проекте

| Этап | Что использовать |
|----------------|---------------------------------------------|
| Ранняя разработка | `Debug.Log` + окно Console |
| Функциональные тесты | `[Conditional("DEVELOPMENT_BUILD")]` + Development Build |
| Релизная сборка | Отключить все логи через `[Conditional]` + оставить только критичные ошибки в файл через `Application.logMessageReceived` |
| Поддержка (LiveOps) | Писать избранные логи в `Application.persistentDataPath` и давать игроку кнопку «Отправить лог» |

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
