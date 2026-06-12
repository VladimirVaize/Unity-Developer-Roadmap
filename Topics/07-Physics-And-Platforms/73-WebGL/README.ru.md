# 🌐 WebGL в Unity: сборка, ограничения и взаимодействие с JavaScript

Unity WebGL — это платформа, позволяющая экспортировать игру в формат, 
работающий в браузере через технологию WebGL (OpenGL ES 2.0/3.0). 
Однако из-за особенностей окружения браузера и компиляции C# в JavaScript (через IL2CPP), 
существуют серьёзные ограничения и специфические подходы к разработке.

---

## 1. Процесс сборки под WebGL
### 🔧 Требования к сборке:

| Компонент | Требование |
| --- | --- |
| Unity версия | 2018.4+ (рекомендуется LTS) |
| Браузер | Chrome, Firefox, Safari, Edge (последние версии) |
| WebGL | WebGL 1.0 (по умолчанию), WebGL 2.0 (опционально) |
| Сжатие | Gzip, Brotli (для оптимизации загрузки) |
| Хостинг | Требует HTTPS (кроме localhost) |

### 📦 Настройки сборки:
1. File → Build Settings → Platform → WebGL → Switch Platform
2. Player Settings → WebGL:
```csharp
// Ключевые настройки
PlayerSettings.WebGL.template = "PROJECT:Default";  // HTML шаблон
PlayerSettings.WebGL.debugSymbols = true;          // Символы для отладки
PlayerSettings.WebGL.memorySize = 256;              // Размер памяти (MB)
PlayerSettings.WebGL.threadsSupport = false;        // Многопоточность (обычно OFF)
PlayerSettings.WebGL.dataCaching = true;            // Кэширование данных
```

### 🏗️ После сборки получаем:
```text
Build/
├── index.html           # Точка входа
├── Build.webgl.data     # Ресурсы (текстуры, звуки, сцены)
├── Build.webgl.framework.js  # JS-фреймворк Unity
├── Build.webgl.wasm      # WebAssembly (основной код)
└── Build.webgl.loader.js # Загрузчик
```

---

## 2. Ключевые ограничения WebGL
### 🚫 Ограничение 1: Отсутствие многопоточности
WebGL 1.0/2.0 не поддерживает разделяемую память (SharedArrayBuffer), поэтому:
| Что НЕ работает | Что использовать вместо |
| --- | --- |
| `System.Threading.Thread` | Основной поток только |
| `async/await` (полноценно) | Корутины (`StartCoroutine`) |
| `Parallel.ForEach` | `yield return null` |
| `Task.Run()` | `InvokeRepeating` |

```csharp
// ❌ НЕ РАБОТАЕТ в WebGL
using System.Threading;
void Start() {
    Thread thread = new Thread(() => {
        Debug.Log("Это не выполнится!");
    });
    thread.Start();
}

// ✅ РАБОТАЕТ (корутины)
void Start() {
    StartCoroutine(DelayedAction());
}
IEnumerator DelayedAction() {
    yield return new WaitForSeconds(1f);
    Debug.Log("Это выполнится через секунду");
}

// ✅ РАБОТАЕТ (статический таймер)
void Start() {
    InvokeRepeating("DoWork", 0f, 0.5f);
}
void DoWork() {
    Debug.Log("Выполняется каждые 0.5 секунды");
}
```

### 🚫 Ограничение 2: Ограниченный доступ к файловой системе
В WebGL нет прямого доступа к локальной файловой системе пользователя. 
Вся работа идёт через виртуальную файловую систему в памяти браузера (IDBFS - IndexedDB File System).

```csharp
// ❌ НЕ РАБОТАЕТ
string filePath = "C:\\Users\\User\\save.txt";
File.WriteAllText(filePath, "data");
File.OpenRead("config.ini");

// ✅ РАБОТАЕТ (только PersistentDataPath)
string savePath = Application.persistentDataPath + "/save.json";
File.WriteAllText(savePath, JsonUtility.ToJson(data));

// Для чтения ресурсов только через Resources или StreamingAssets
TextAsset config = Resources.Load<TextAsset>("config");
```

### 🚫 Ограничение 3: Сетевые ограничения (CORS, WebSocket)
```csharp
// ✅ РАБОТАЕТ - HTTP запросы (UnityWebRequest)
using UnityEngine.Networking;
IEnumerator LoadData() {
    using UnityWebRequest request = UnityWebRequest.Get("https://api.example.com/data");
    yield return request.SendWebRequest();
    
    if (request.result == UnityWebRequest.Result.Success) {
        Debug.Log(request.downloadHandler.text);
    }
}

// ✅ РАБОТАЕТ - WebSocket (через плагин)
// Используйте Native WebSocket или сторонние библиотеки

// ❌ НЕ РАБОТАЕТ - TCP/UDP сокеты
// System.Net.Sockets полностью отсутствует в WebGL
```

### 🚫 Ограничение 4: Системные вызовы
Многие привычные API недоступны:

| API | Статус | Причина |
| --- | --- | --- |
| `System.Diagnostics.Process` | ❌ | Нет доступа к процессам ОС |
| `System.IO.Ports` | ❌ | Нет COM-портов в браузере |
| `System.Net.Sockets` | ❌ | Нет низкоуровневых сокетов |
| `System.Management` | ❌ | WMI не существует в браузере | 
| `System.Drawing` | ❌ | Используйте Unity Texture2D |
| `Registry` | ❌ | Нет реестра Windows |
| `Environment.GetFolderPath` | ⚠️ | Работает только для спец. папок |

```csharp
// ❌ НЕ РАБОТАЕТ
string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
int processors = Environment.ProcessorCount;  // Всегда возвращает 1

// ✅ Альтернативы
string deviceModel = SystemInfo.deviceModel;  // "WebGL"
string graphicsAPI = SystemInfo.graphicsDeviceVersion;  // "WebGL 1.0"
int memoryMB = SystemInfo.systemMemorySize;  // Оперативная память браузера
```

### 🚫 Ограничение 5: Особенности отображения
```csharp
// WebGL не может работать в фоне
void OnApplicationFocus(bool hasFocus) {
    if (!hasFocus) {
        Time.timeScale = 0;  // Ставим игру на паузу вручную
        AudioListener.pause = true;
    }
}

// WebGL не поддерживает полноценный полноэкранный режим (только через браузер)
public void ToggleFullscreen() {
    Screen.fullScreen = !Screen.fullScreen;  // Работает через браузерный API
}
```

---

## 3. Взаимодействие с JavaScript
Это самая мощная особенность WebGL — вы можете вызывать JavaScript из C# и наоборот.

### 📞 Вызов JavaScript из C# (DllImport)
Шаг 1: Создайте `.jslib` файл (например, `Assets/Plugins/WebGL/MyPlugin.jslib`)
```javascript
// MyPlugin.jslib
mergeInto(LibraryManager.library, {
    // Функция для браузера
    ShowAlert: function(message) {
        window.alert(UTF8ToString(message));
    },
    
    // Получение данных из браузера
    GetCurrentURL: function() {
        var url = window.location.href;
        var buffer = _malloc(url.length + 1);
        writeStringToMemory(url, buffer);
        return buffer;
    },
    
    // Работа с localStorage
    SaveToLocalStorage: function(key, value) {
        var k = UTF8ToString(key);
        var v = UTF8ToString(value);
        localStorage.setItem(k, v);
    },
    
    LoadFromLocalStorage: function(key) {
        var k = UTF8ToString(key);
        var value = localStorage.getItem(k);
        if (!value) return 0;
        var buffer = _malloc(value.length + 1);
        writeStringToMemory(value, buffer);
        return buffer;
    },
    
    // Запрос к API браузера
    GetBrowserLanguage: function() {
        var lang = navigator.language || navigator.userLanguage;
        var buffer = _malloc(lang.length + 1);
        writeStringToMemory(lang, buffer);
        return buffer;
    }
});
```

Шаг 2: Объявите методы в C#
```csharp
using System.Runtime.InteropServices;
using UnityEngine;

public class JSCommunicator : MonoBehaviour
{
    // Импорт функций из .jslib
    [DllImport("__Internal")]
    private static extern void ShowAlert(string message);
    
    [DllImport("__Internal")]
    private static extern string GetCurrentURL();
    
    [DllImport("__Internal")]
    private static extern void SaveToLocalStorage(string key, string value);
    
    [DllImport("__Internal")]
    private static extern string LoadFromLocalStorage(string key);
    
    [DllImport("__Internal")]
    private static extern string GetBrowserLanguage();
    
    public void Test()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        // Код выполнится только в WebGL сборке
        ShowAlert("Hello from Unity!");
        string url = GetCurrentURL();
        Debug.Log("Current URL: " + url);
        
        SaveToLocalStorage("score", "1000");
        string savedScore = LoadFromLocalStorage("score");
        Debug.Log("Saved score: " + savedScore);
        
        string lang = GetBrowserLanguage();
        Debug.Log("Browser language: " + lang);
#else
        Debug.Log("Running in Editor - JS calls disabled");
#endif
    }
}
```

### 📥 Вызов C# из JavaScript
Шаг 1: Зарегистрируйте C# метод для JavaScript
```csharp
using UnityEngine;
using System.Runtime.InteropServices;

public class JSCallbackReceiver : MonoBehaviour
{
    void Start()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        // Регистрируем callback в глобальной области
        WebGLInput.captureAllKeyboardInput = true;
#endif
    }
    
    // Метод будет вызван из JavaScript
    public void OnJSCall(string message)
    {
        Debug.Log("Called from JS: " + message);
        ShowNativeDialog(message);
    }
    
    // Статический метод для вызова из JS
    public static void StaticJSReceiver(string data)
    {
        Debug.Log("Static JS Call: " + data);
        Instance?.OnJSCall(data);  // Передаём в экземпляр
    }
    
    private static JSCallbackReceiver _instance;
    void Awake() { _instance = this; }
}
```

Шаг 2: В JavaScript обратитесь к Unity
```javascript
// Вставить в HTML или в .jslib
function CallUnityMethod(methodName, value) {
    // Получаем экземпляр Unity
    var unityInstance = document.querySelector('canvas').unityInstance;
    
    // Вызов метода на GameObject
    unityInstance.SendMessage('GameObjectName', methodName, value);
    
    // Или прямой вызов статического метода
    unityInstance.Module.SendMessage('GameObjectName', methodName, value);
}

// Пример использования
CallUnityMethod('OnJSCall', 'Hello from browser button!');
```

### 🎮 Полный пример: JSBridge (двусторонняя связь)
```csharp
// JS Bridge для WebGL
public class WebGLBridge : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void JS_RequestUserData();
    
    [DllImport("__Internal")]
    private static extern void JS_SaveData(string key, string value);
    
    [DllImport("__Internal")]
    private static extern string JS_GetPlatform();
    
    // Вызывается из JavaScript
    public void OnUserDataReceived(string jsonData)
    {
        Debug.Log("User data from JS: " + jsonData);
        var user = JsonUtility.FromJson<UserData>(jsonData);
        ProcessUserData(user);
    }
    
    // Вызывается из JavaScript при ошибках
    public void OnJSError(string error)
    {
        Debug.LogError("JS Error: " + error);
    }
    
    // Вызвать JS из Unity
    public void RequestUserData()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        JS_RequestUserData();
#endif
    }
    
    public void SaveGameData(string key, int score)
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        JS_SaveData(key, score.ToString());
#endif
    }
}

[System.Serializable]
public class UserData
{
    public string name;
    public int level;
    public int gold;
}
```

Соответствующий JavaScript (`Assets/Plugins/WebGL/JSBridge.jslib`):
```javascript
mergeInto(LibraryManager.library, {
    JS_RequestUserData: function() {
        // Эмуляция запроса к серверу
        var userData = {
            name: "Player1",
            level: 5,
            gold: 1000
        };
        
        // Вызываем Unity метод
        var json = JSON.stringify(userData);
        SendMessage('WebGLBridge', 'OnUserDataReceived', json);
    },
    
    JS_SaveData: function(key, value) {
        var k = UTF8ToString(key);
        var v = UTF8ToString(value);
        localStorage.setItem(k, v);
        console.log("Saved to localStorage: " + k + " = " + v);
    },
    
    JS_GetPlatform: function() {
        var platform = navigator.platform;
        var buffer = _malloc(platform.length + 1);
        writeStringToMemory(platform, buffer);
        return buffer;
    }
});
```

---

## 4. Оптимизация WebGL сборки
### 📏 Размер сборки
| Компонент | Типичный размер | Оптимизация |
| --- | --- | --- |
| .wasm файл | 5-15 MB | IL2CPP Code Generation -> Faster (smaller) |
| .data файл | 1-100 MB | Разделить на Addressables |
| .framework.js | 200-500 KB | Сжатие Gzip/Brotli |

### ⚡ Советы по оптимизации:
```csharp
// 1. Используйте Asset Bundles для загрузки контента
IEnumerator LoadLevelAssetBundle() {
    var request = UnityWebRequestAssetBundle.GetBundle("https://cdn.example.com/level1");
    yield return request.SendWebRequest();
    AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
}

// 2. Уменьшайте количество вызовов Instantiate/Destroy
// Используйте Object Pooling вместо частого создания объектов

// 3. WebGL специфичные настройки
PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Brotli;
PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.None;
PlayerSettings.WebGL.dataCaching = true;  // Кэшировать данные в IndexedDB
```

### 🐛 Отладка WebGL
```csharp
// Включение отладочного режима
#if UNITY_WEBGL && DEVELOPMENT_BUILD
Debug.Log("WebGL Development Mode");
Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
#endif

// Использование браузерной консоли
[DllImport("__Internal")]
private static extern void ConsoleLog(string message);

void BrowserLog(string msg) {
#if !UNITY_EDITOR && UNITY_WEBGL
    ConsoleLog(msg);
#endif
}
```

---

## 5. Полезные паттерны для WebGL
### 🎯 Паттерн "Полифилл для неподдерживаемых API"
```csharp
public class WebGLPolyfill : MonoBehaviour
{
    public static bool IsWebGL => Application.platform == RuntimePlatform.WebGLPlayer;
    
    public static void SafeInvoke(System.Action action, float delaySeconds)
    {
        if (IsWebGL)
        {
            // Используем корутины вместо Task.Delay
            Instance.StartCoroutine(DelayedInvoke(action, delaySeconds));
        }
        else
        {
            // На других платформах можно использовать Task.Delay
            Task.Delay((int)(delaySeconds * 1000)).ContinueWith(_ => action());
        }
    }
    
    static IEnumerator DelayedInvoke(System.Action action, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        action?.Invoke();
    }
    
    private static WebGLPolyfill _instance;
    void Awake() { _instance = this; }
}
```

### 💾 Паттерн "Сохранение с синхронизацией"
```csharp
public class WebGLSaveSystem : MonoBehaviour
{
    public void SaveGame(GameData data)
    {
        string json = JsonUtility.ToJson(data);
        
#if UNITY_WEBGL && !UNITY_EDITOR
        // Сохраняем в localStorage через JS
        SaveToLocalStorage("saveData", json);
#else
        // Сохраняем в файл на других платформах
        string path = Application.persistentDataPath + "/save.json";
        File.WriteAllText(path, json);
#endif
    }
    
    [DllImport("__Internal")]
    private static extern void SaveToLocalStorage(string key, string value);
}
```

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
