# 🌐 WebGL in Unity: Build, Limitations and JavaScript Interaction
Unity WebGL is a platform that allows exporting games to a format that runs in the browser via WebGL technology (OpenGL ES 2.0/3.0). 
However, due to browser environment limitations and C# to JavaScript compilation (via IL2CPP), there are serious constraints and specific development approaches.

---

## 1. WebGL Build Process
### 🔧 Build Requirements:
| Component | Requirement |
| --- | --- |
| Unity Version | 2018.4+ (LTS recommended) |
| Browser | Chrome, Firefox, Safari, Edge (latest versions) |
| WebGL | WebGL 1.0 (default), WebGL 2.0 (optional) |
| Compression | Gzip, Brotli (for load optimization) |
| Hosting | Requires HTTPS (except localhost) |

### 📦 Build Settings:
1. File → Build Settings → Platform → WebGL → Switch Platform
2. Player Settings → WebGL:

```csharp
// Key settings
PlayerSettings.WebGL.template = "PROJECT:Default";
PlayerSettings.WebGL.debugSymbols = true;
PlayerSettings.WebGL.memorySize = 256;
PlayerSettings.WebGL.threadsSupport = false;
PlayerSettings.WebGL.dataCaching = true;
```

### 🏗️ After Build:
```text
Build/
├── index.html
├── Build.webgl.data
├── Build.webgl.framework.js
├── Build.webgl.wasm
└── Build.webgl.loader.js
```

---

## 2. Key WebGL Limitations
### 🚫 Limitation 1: No Multithreading
WebGL 1.0/2.0 doesn't support shared memory (SharedArrayBuffer), so:

| What DOESN'T work | What to use instead |
| --- | --- |
| `System.Threading.Thread` | Main thread only |
| `async/await` (fully) | Coroutines (`StartCoroutine`) |
| `Parallel.ForEach` | `yield return null` |
| `Task.Run()` | `InvokeRepeating` |

```csharp
// ❌ DOESN'T WORK in WebGL
using System.Threading;
void Start() {
    Thread thread = new Thread(() => {
        Debug.Log("This won't execute!");
    });
    thread.Start();
}

// ✅ WORKS (coroutines)
void Start() {
    StartCoroutine(DelayedAction());
}
IEnumerator DelayedAction() {
    yield return new WaitForSeconds(1f);
    Debug.Log("This executes after one second");
}

// ✅ WORKS (static timer)
void Start() {
    InvokeRepeating("DoWork", 0f, 0.5f);
}
void DoWork() {
    Debug.Log("Executes every 0.5 seconds");
}
```

### 🚫 Limitation 2: Limited File System Access
WebGL has no direct access to the user's local file system. 
All work goes through a virtual file system in browser memory 
(IDBFS - IndexedDB File System).

```csharp
// ❌ DOESN'T WORK
string filePath = "C:\\Users\\User\\save.txt";
File.WriteAllText(filePath, "data");
File.OpenRead("config.ini");

// ✅ WORKS (only PersistentDataPath)
string savePath = Application.persistentDataPath + "/save.json";
File.WriteAllText(savePath, JsonUtility.ToJson(data));

// For resources only through Resources or StreamingAssets
TextAsset config = Resources.Load<TextAsset>("config");
```

### 🚫 Limitation 3: Network Restrictions (CORS, WebSocket)
```csharp
// ✅ WORKS - HTTP requests (UnityWebRequest)
using UnityEngine.Networking;
IEnumerator LoadData() {
    using UnityWebRequest request = UnityWebRequest.Get("https://api.example.com/data");
    yield return request.SendWebRequest();
    
    if (request.result == UnityWebRequest.Result.Success) {
        Debug.Log(request.downloadHandler.text);
    }
}

// ✅ WORKS - WebSocket (via plugin)
// Use Native WebSocket or third-party libraries

// ❌ DOESN'T WORK - TCP/UDP sockets
// System.Net.Sockets is completely unavailable in WebGL
```

### 🚫 Limitation 4: System Calls
Many familiar APIs are unavailable:

| API | Status | Reason |
| --- | --- | --- |
| `System.Diagnostics.Process` | ❌ | No OS process access |
| `System.IO.Ports` | ❌ | No COM ports in browser |
| `System.Net.Sockets` | ❌ | No low-level sockets |
| `System.Management` | ❌ | WMI doesn't exist in browser |
| `System.Drawing` | ❌ | Use Unity Texture2D | 
| `Registry` | ❌ | No Windows registry | 
| `Environment.GetFolderPath` | ⚠️ | Works only for special folders |

```csharp
// ❌ DOESN'T WORK
string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
int processors = Environment.ProcessorCount;  // Always returns 1

// ✅ Alternatives
string deviceModel = SystemInfo.deviceModel;  // "WebGL"
string graphicsAPI = SystemInfo.graphicsDeviceVersion;  // "WebGL 1.0"
int memoryMB = SystemInfo.systemMemorySize;  // Browser RAM
```

### 🚫 Limitation 5: Display Features
```csharp
// WebGL cannot work in background
void OnApplicationFocus(bool hasFocus) {
    if (!hasFocus) {
        Time.timeScale = 0;
        AudioListener.pause = true;
    }
}

// WebGL doesn't support true fullscreen (only via browser API)
public void ToggleFullscreen() {
    Screen.fullScreen = !Screen.fullScreen;
}
```

---

## 3. Interaction with JavaScript
This is the most powerful feature of WebGL — you can call JavaScript from C# and vice versa.

### 📞 Calling JavaScript from C# (DllImport)
Step 1: Create a `.jslib` file (e.g., `Assets/Plugins/WebGL/MyPlugin.jslib`)
```javascript
// MyPlugin.jslib
mergeInto(LibraryManager.library, {
    ShowAlert: function(message) {
        window.alert(UTF8ToString(message));
    },
    
    GetCurrentURL: function() {
        var url = window.location.href;
        var buffer = _malloc(url.length + 1);
        writeStringToMemory(url, buffer);
        return buffer;
    },
    
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
    
    GetBrowserLanguage: function() {
        var lang = navigator.language || navigator.userLanguage;
        var buffer = _malloc(lang.length + 1);
        writeStringToMemory(lang, buffer);
        return buffer;
    }
});
```

Step 2: Declare methods in C#
```csharp
using System.Runtime.InteropServices;
using UnityEngine;

public class JSCommunicator : MonoBehaviour
{
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

### 📥 Calling C# from JavaScript
Step 1: Register C# method for JavaScript
```csharp
using UnityEngine;
using System.Runtime.InteropServices;

public class JSCallbackReceiver : MonoBehaviour
{
    void Start()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        WebGLInput.captureAllKeyboardInput = true;
#endif
    }
    
    public void OnJSCall(string message)
    {
        Debug.Log("Called from JS: " + message);
        ShowNativeDialog(message);
    }
    
    public static void StaticJSReceiver(string data)
    {
        Debug.Log("Static JS Call: " + data);
        Instance?.OnJSCall(data);
    }
    
    private static JSCallbackReceiver _instance;
    void Awake() { _instance = this; }
}
```

Step 2: Call Unity from JavaScript
```javascript
function CallUnityMethod(methodName, value) {
    var unityInstance = document.querySelector('canvas').unityInstance;
    unityInstance.SendMessage('GameObjectName', methodName, value);
}

CallUnityMethod('OnJSCall', 'Hello from browser button!');
```

### 🎮 Complete Example: JSBridge (Bidirectional Communication)
```csharp
public class WebGLBridge : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void JS_RequestUserData();
    
    [DllImport("__Internal")]
    private static extern void JS_SaveData(string key, string value);
    
    [DllImport("__Internal")]
    private static extern string JS_GetPlatform();
    
    public void OnUserDataReceived(string jsonData)
    {
        Debug.Log("User data from JS: " + jsonData);
        var user = JsonUtility.FromJson<UserData>(jsonData);
        ProcessUserData(user);
    }
    
    public void OnJSError(string error)
    {
        Debug.LogError("JS Error: " + error);
    }
    
    public void RequestUserData()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        JS_RequestUserData();
#endif
    }
}
```

---

## 4. WebGL Build Optimization
### 📏 Build Size
| Component | Typical Size | Optimization |
| --- | --- | --- |
| .wasm file | 5-15 MB | IL2CPP Code Generation -> Faster (smaller) |
| .data file | 1-100 MB | Split into Addressables |
| .framework.js | 200-500 KB | Gzip/Brotli compression |

### ⚡ Optimization Tips:
```csharp
// 1. Use Asset Bundles for content loading
IEnumerator LoadLevelAssetBundle() {
    var request = UnityWebRequestAssetBundle.GetBundle("https://cdn.example.com/level1");
    yield return request.SendWebRequest();
    AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
}

// 2. Reduce Instantiate/Destroy calls
// Use Object Pooling instead of frequent object creation

// 3. WebGL specific settings
PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Brotli;
PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.None;
PlayerSettings.WebGL.dataCaching = true;
```

### 🐛 Debugging WebGL
```csharp
// Enable debug mode
#if UNITY_WEBGL && DEVELOPMENT_BUILD
Debug.Log("WebGL Development Mode");
Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
#endif

// Using browser console
[DllImport("__Internal")]
private static extern void ConsoleLog(string message);
```

---

## 5. Useful Patterns for WebGL
### 🎯 "Polyfill for Unsupported APIs" Pattern
```csharp
public class WebGLPolyfill : MonoBehaviour
{
    public static bool IsWebGL => Application.platform == RuntimePlatform.WebGLPlayer;
    
    public static void SafeInvoke(System.Action action, float delaySeconds)
    {
        if (IsWebGL)
        {
            Instance.StartCoroutine(DelayedInvoke(action, delaySeconds));
        }
        else
        {
            Task.Delay((int)(delaySeconds * 1000)).ContinueWith(_ => action());
        }
    }
}
```

---

### ⭐ If this project was useful, put a star on GitHub!
