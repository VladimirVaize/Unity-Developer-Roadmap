# 📁 StreamingAssets in Unity: StreamingAssets Folder, File Reading, Big Data Streaming, Video
`StreamingAssets` is a special folder in Unity whose contents are copied "as is" 
into the final application build without any processing (compression, conversion, obfuscation). 
This is the ideal place for storing files that need to be readable at runtime 
(JSON, XML, audio, video, custom configs, DLLs, etc.).

---

## 1. What is StreamingAssets and Where is it Located?
### StreamingAssets Folder Features:
| Feature | Description |
| --- | --- |
| Path in Editor | `Assets/StreamingAssets/` |
| Path in Build | Platform-dependent (see table below) |
| Unity Processing | ❌ No compression, renaming, or conversion |
| Read Access | ✅ Through standard file operations (`System.IO`) |
| Write Access | ⚠️ Only on some platforms (Android — no) |
| Folder Nesting | ✅ Full support |

### 📍 StreamingAssets Paths on Different Platforms:
| Platform | Path | Example |
| --- | --- | --- |
| Unity Editor | `Application.dataPath + "/StreamingAssets"` | `C:/MyGame/Assets/StreamingAssets` |
| Windows/Mac/Linux | `Application.streamingAssetsPath` | `C:/MyGame/MyGame_Data/StreamingAssets` |
| Android | `jar:file:// + Application.dataPath + !/assets/` | (files inside APK) |
| iOS | `Application.streamingAssetsPath` | `/var/.../Raw/` |
| WebGL | `Application.streamingAssetsPath` | `/StreamingAssets` (on server) |

> [!Important]
> On Android, files inside `StreamingAssets` are packed into the APK and are not directly accessible via `System.IO.File.ReadAllText`. Use `UnityWebRequest` or `WWW`.

---

## 2. How to Add Files to StreamingAssets?
### Method 1: Through File System
1. Create folder `Assets/StreamingAssets`
2. Copy files into it (via Explorer or Unity Project Window)

### Method 2: Through Unity Editor (automatic copying)
```csharp
using UnityEditor;
using System.IO;

public class StreamingAssetsHelper
{
    [MenuItem("Tools/Copy Config to StreamingAssets")]
    public static void CopyConfigFile()
    {
        string source = Application.dataPath + "/Configs/gameData.json";
        string dest = Application.streamingAssetsPath + "/gameData.json";
        
        if (!Directory.Exists(Application.streamingAssetsPath))
            Directory.CreateDirectory(Application.streamingAssetsPath);
        
        File.Copy(source, dest, true);
        AssetDatabase.Refresh();
        Debug.Log($"File copied to StreamingAssets: {dest}");
    }
}
```

### Folder Structure in StreamingAssets:
```text
Assets/StreamingAssets/
├── configs/
│   ├── settings.json
│   └── localization/
│       ├── en.json
│       └── ru.json
├── videos/
│   └── intro.mp4
├── levels/
│   ├── level1.bin
│   └── level2.bin
└── readme.txt
```

---

## 3. Reading Files from StreamingAssets
### 📝 Example 1: Simple Text File Reading (all platforms except Android)
```csharp
using System.IO;
using UnityEngine;

public class StreamingAssetReader : MonoBehaviour
{
    void Start()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
            ReadFromStreamingAssetsAndroid();
        #else
            ReadFromStreamingAssetsStandard();
        #endif
    }
    
    void ReadFromStreamingAssetsStandard()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "configs/settings.json");
        
        if (File.Exists(path))
        {
            string jsonContent = File.ReadAllText(path);
            Debug.Log($"Content loaded: {jsonContent}");
            ParseConfig(jsonContent);
        }
        else
        {
            Debug.LogError($"File not found: {path}");
        }
    }
    
    void ReadFromStreamingAssetsAndroid()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "configs/settings.json");
        
        using (UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Get(path))
        {
            request.SendWebRequest();
            
            while (!request.isDone) { }
            
            if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                string jsonContent = request.downloadHandler.text;
                Debug.Log($"Android content: {jsonContent}");
                ParseConfig(jsonContent);
            }
            else
            {
                Debug.LogError($"Failed to load: {request.error}");
            }
        }
    }
    
    void ParseConfig(string json) { /* ... */ }
}
```

### 📝 Example 2: Cross-platform Reading Method (UnityWebRequest)
```csharp
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class CrossPlatformReader : MonoBehaviour
{
    public string fileName = "data.json";
    
    IEnumerator Start()
    {
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);
        
        using (UnityWebRequest request = UnityWebRequest.Get(path))
        {
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                string content = request.downloadHandler.text;
                Debug.Log($"Successfully loaded {fileName}: {content}");
                ProcessData(content);
            }
            else
            {
                Debug.LogError($"Error loading {fileName}: {request.error}");
            }
        }
    }
    
    void ProcessData(string data) { /* ... */ }
}
```

### 📝 Example 3: Reading Binary Files (images, models)
```csharp
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class BinaryStreamingAssetLoader : MonoBehaviour
{
    IEnumerator LoadTextureFromStreamingAssets(string relativePath)
    {
        string fullPath = System.IO.Path.Combine(Application.streamingAssetsPath, relativePath);
        
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(fullPath))
        {
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                GetComponent<Renderer>().material.mainTexture = texture;
                Debug.Log($"Texture loaded from: {relativePath}");
            }
            else
            {
                Debug.LogError($"Failed to load texture: {request.error}");
            }
        }
    }
    
    IEnumerator LoadBinaryData(string relativePath)
    {
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, relativePath);
        
        using (UnityWebRequest request = UnityWebRequest.Get(path))
        {
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                byte[] binaryData = request.downloadHandler.data;
                Debug.Log($"Loaded {binaryData.Length} bytes from {relativePath}");
            }
        }
    }
}
```

---

## 4. Streaming Big Data
When working with large files (hundreds of MB), it's important to read data in chunks rather than loading everything into memory at once.

### 🧩 Example 1: Streaming Large JSON (line-by-line processing)
```csharp
using System.IO;
using System.Collections;
using UnityEngine;

public class LargeFileStreamReader : MonoBehaviour
{
    IEnumerator ProcessLargeFileCoroutine()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "large_dataset.json");
        
        #if UNITY_ANDROID && !UNITY_EDITOR
            yield return StartCoroutine(CopyStreamingAssetToPersistentPath("large_dataset.json"));
            filePath = Path.Combine(Application.persistentDataPath, "large_dataset.json");
        #endif
        
        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;
            int lineCount = 0;
            
            while ((line = reader.ReadLine()) != null)
            {
                ProcessLine(line);
                lineCount++;
                
                if (lineCount % 100 == 0)
                {
                    Debug.Log($"Processed {lineCount} lines...");
                    yield return null;
                }
            }
            
            Debug.Log($"Finished processing {lineCount} lines");
        }
    }
    
    void ProcessLine(string line) { }
    
    IEnumerator CopyStreamingAssetToPersistentPath(string fileName)
    {
        string source = Path.Combine(Application.streamingAssetsPath, fileName);
        string dest = Path.Combine(Application.persistentDataPath, fileName);
        
        using (UnityWebRequest request = UnityWebRequest.Get(source))
        {
            request.SendWebRequest();
            
            while (!request.isDone)
                yield return null;
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                File.WriteAllBytes(dest, request.downloadHandler.data);
                Debug.Log($"Copied {fileName} to persistentDataPath");
            }
        }
    }
}
```

### 🧩 Example 2: Buffered Reading with FileStream
```csharp
using System.IO;
using System.Text;

public class BufferedFileReader
{
    public static void ReadLargeFileInChunks(string filePath, int chunkSize = 4096)
    {
        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        using (BinaryReader reader = new BinaryReader(fs))
        {
            long totalBytes = fs.Length;
            long bytesRead = 0;
            
            while (bytesRead < totalBytes)
            {
                int bytesToRead = (int)Mathf.Min(chunkSize, totalBytes - bytesRead);
                byte[] chunk = reader.ReadBytes(bytesToRead);
                
                ProcessChunk(chunk);
                
                bytesRead += bytesToRead;
                float progress = (float)bytesRead / totalBytes;
                Debug.Log($"Read progress: {progress:P}");
            }
        }
    }
    
    static void ProcessChunk(byte[] chunk) { }
}
```

---

## 5. Working with Video from StreamingAssets
Unity's `VideoPlayer` can load video from StreamingAssets as well as from URLs.

### 🎬 Example 1: Playing Local Video from StreamingAssets
```csharp
using UnityEngine;
using UnityEngine.Video;

public class StreamingVideoPlayer : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private string videoFileName = "intro.mp4";
    
    void Start()
    {
        PlayVideoFromStreamingAssets(videoFileName);
    }
    
    void PlayVideoFromStreamingAssets(string fileName)
    {
        string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);
        
        #if UNITY_ANDROID && !UNITY_EDITOR
            videoPlayer.url = videoPath;
        #elif UNITY_IOS && !UNITY_EDITOR
            videoPlayer.url = videoPath;
        #elif UNITY_WEBGL && !UNITY_EDITOR
            videoPlayer.url = "StreamingAssets/" + fileName;
        #else
            videoPlayer.url = videoPath;
        #endif
        
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += OnVideoPrepared;
    }
    
    void OnVideoPrepared(VideoPlayer vp)
    {
        Debug.Log($"Video prepared: {vp.url}");
        vp.Play();
    }
}
```

### 🎬 Example 2: Streaming Large Video with Progress Bar
```csharp
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoStreamingManager : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private Text statusText;
    [SerializeField] private string videoURL = "https://example.com/big_video.mp4";
    
    IEnumerator Start()
    {
        videoPlayer.url = videoURL;
        
        videoPlayer.errorReceived += OnVideoError;
        videoPlayer.prepareCompleted += OnPrepareComplete;
        videoPlayer.started += OnVideoStarted;
        
        statusText.text = "Preparing video...";
        videoPlayer.Prepare();
        
        while (!videoPlayer.isPrepared)
        {
            progressSlider.value = videoPlayer.frameCount > 0 ? 
                (float)videoPlayer.frame / videoPlayer.frameCount : 0;
            yield return null;
        }
    }
    
    void OnPrepareComplete(VideoPlayer vp)
    {
        statusText.text = "Ready! Playing...";
        vp.Play();
    }
    
    void OnVideoStarted(VideoPlayer vp)
    {
        StartCoroutine(UpdatePlaybackProgress());
    }
    
    IEnumerator UpdatePlaybackProgress()
    {
        while (videoPlayer.isPlaying)
        {
            if (videoPlayer.frameCount > 0)
            {
                float progress = (float)videoPlayer.frame / videoPlayer.frameCount;
                progressSlider.value = progress;
                statusText.text = $"Playing: {progress:P0}";
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    void OnVideoError(VideoPlayer vp, string message)
    {
        Debug.LogError($"Video error: {message}");
        statusText.text = $"Error: {message}";
    }
}
```

### 🎬 Example 3: Chunk-based Dynamic Video Loading (Adaptive Streaming)
```csharp
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ChunkedVideoLoader : MonoBehaviour
{
    private const int CHUNK_SIZE = 1024 * 1024; // 1 MB chunks
    
    IEnumerator DownloadVideoInChunks(string videoUrl, System.Action<byte[]> onChunkReceived)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(videoUrl))
        {
            request.SendWebRequest();
            
            long totalBytes = 0;
            long downloadedBytes = 0;
            
            while (!request.isDone)
            {
                totalBytes = request.downloadedBytes;
                if (totalBytes - downloadedBytes >= CHUNK_SIZE)
                {
                    byte[] chunk = request.downloadHandler.data;
                    onChunkReceived?.Invoke(chunk);
                    downloadedBytes = totalBytes;
                    Debug.Log($"Downloaded {downloadedBytes} bytes");
                }
                yield return null;
            }
            
            byte[] finalChunk = request.downloadHandler.data;
            onChunkReceived?.Invoke(finalChunk);
        }
    }
}
```

---

## 6. StreamingAssets vs Resources vs PersistentDataPath

| Feature | StreamingAssets | Resources | PersistentDataPath |
| --- | --- | --- | --- |
| Read Access | ✅ Yes (via paths) | ✅ Yes (via `Resources.Load`)	| ✅ Yes |
| Write Access | ⚠️ Desktop only | ❌ No | ✅ Yes |
| Compression in Build | ❌ No | ✅ Yes (LZ4) | ❌ No (outside build) |
| Internal Structure | Preserved | ❌ Flat | Preserved |
| Dynamic Loading | ✅ Yes (by filename) | ❌ Only resource path | ✅ Yes |
| Modifiable After Build | ❌ No | ❌ No | ✅ Yes |
| Streaming Support | ✅ Yes | ❌ No | ✅ Yes |
| Video Usage | ✅ Yes | ❌ No | ✅ Yes (copied) |

### Choosing the Right Location:
- StreamingAssets: Config files, video, large assets that need streaming reads
- Resources: Small assets loaded by name that rarely change
- PersistentDataPath: Player saves, downloaded content, logs

---

## 7. Cross-platform Nuances and Solutions
### 🔧 Android Specifics:
```csharp
// ✅ Correct: Always use UnityWebRequest on Android
IEnumerator LoadFromStreamingAssetsAndroid(string relativePath)
{
    string fullPath = Path.Combine(Application.streamingAssetsPath, relativePath);
    using (UnityWebRequest req = UnityWebRequest.Get(fullPath))
    {
        yield return req.SendWebRequest();
        if (req.result == UnityWebRequest.Result.Success)
        {
            // Success
        }
    }
}

// ❌ Incorrect: File.ReadAllText on Android will throw an exception
string content = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "config.json"));
```

### 🔧 iOS Specifics:
```csharp
// On iOS, StreamingAssets is available for standard file I/O
string path = Path.Combine(Application.streamingAssetsPath, "data.json");
if (File.Exists(path)) // Works on iOS
{
    string content = File.ReadAllText(path);
}
```

### 🔧 WebGL Specifics:
```csharp
// WebGL uses HTTP requests to access StreamingAssets
IEnumerator LoadFromStreamingAssetsWebGL(string fileName)
{
    string url = Application.streamingAssetsPath + "/" + fileName;
    using (UnityWebRequest req = UnityWebRequest.Get(url))
    {
        yield return req.SendWebRequest();
        // Process response
    }
}
```

### 🔧 Universal Loader:
```csharp
public static class UniversalStreamingAssetLoader
{
    public static IEnumerator LoadText(string relativePath, System.Action<string> onComplete, System.Action<string> onError = null)
    {
        string fullPath = Path.Combine(Application.streamingAssetsPath, relativePath);
        
        #if (UNITY_ANDROID || UNITY_WEBGL) && !UNITY_EDITOR
            using (UnityWebRequest request = UnityWebRequest.Get(fullPath))
            {
                yield return request.SendWebRequest();
                
                if (request.result == UnityWebRequest.Result.Success)
                {
                    onComplete?.Invoke(request.downloadHandler.text);
                }
                else
                {
                    onError?.Invoke(request.error);
                }
            }
        #else
            if (File.Exists(fullPath))
            {
                string content = File.ReadAllText(fullPath);
                onComplete?.Invoke(content);
            }
            else
            {
                onError?.Invoke($"File not found: {fullPath}");
            }
            yield return null;
        #endif
    }
}
```

---

## 8. Best Practices
1. Use `Path.Combine` instead of string concatenation for paths
2. Always check file existence before reading
3. For large files, use chunk-based streaming to avoid memory overload
4. On Android, always use `UnityWebRequest` — `File.ReadAllText` doesn't work
5. Cache files to `Application.persistentDataPath` for repeated access
6. Don't store secrets (passwords, API keys) in StreamingAssets — they're easily extractable
7. For video, use `VideoPlayer` with the correct format (H.264 for mobile)
8. Handle loading errors — files/networks may be unavailable

---

### ⭐ If this project was useful, put a star on GitHub!
