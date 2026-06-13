# 📁 StreamingAssets в Unity: Папка StreamingAssets, чтение файлов, стриминг больших данных, видео
`StreamingAssets` — это специальная папка в Unity, содержимое которой копируется «как есть» в 
финальную сборку приложения без какой-либо обработки (сжатия, конвертации, обфускации). 
Это идеальное место для хранения файлов, которые должны быть доступны для чтения в рантайме 
(JSON, XML, аудио, видео, пользовательские конфиги, DLL и т.д.).

---

## 1. Что такое StreamingAssets и где она находится?
### Особенности папки `StreamingAssets`:
| Характеристика | Описание |
| --- | --- |
| Путь в редакторе | `Assets/StreamingAssets/` |
| Путь в сборке | Зависит от платформы (см. таблицу ниже) |
| Обработка Unity | ❌ Не сжимается, не переименовывается, не конвертируется |
| Доступ на чтение | ✅ Через стандартные файловые операции (`System.IO`) |
| Доступ на запись | ⚠️ Только в некоторых платформах (Android — нельзя) |
| Вложенность папок | ✅ Полная поддержка |

### 📍 Пути к StreamingAssets на разных платформах:
| Платформа | Путь | Пример |
| --- | --- | --- |
| Unity Editor | `Application.dataPath + "/StreamingAssets"` | `C:/MyGame/Assets/StreamingAssets` |
| Windows/Mac/Linux | `Application.streamingAssetsPath` | `C:/MyGame/MyGame_Data/StreamingAssets` |
| Android | `jar:file:// + Application.dataPath + !/assets/` | (файлы внутри APK) |
| iOS | `Application.streamingAssetsPath` | `/var/.../Raw/` |
| WebGL | `Application.streamingAssetsPath` | `/StreamingAssets` (на сервере) |

> [!Important]
> На Android файлы внутри `StreamingAssets` упаковываются в APK и не доступны напрямую через `System.IO.File.ReadAllText`. Нужно использовать `UnityWebRequest` или `WWW`.

---

## 2. Как добавить файлы в StreamingAssets?
### Способ 1: Через файловую систему
1. Создайте папку `Assets/StreamingAssets`
2. Скопируйте в неё файлы (через проводник или Unity Project Window)

### Способ 2: Через редактор Unity (автоматическое копирование)
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

### Структура папок в StreamingAssets:
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

## 3. Чтение файлов из StreamingAssets
### 📝 Пример 1: Простое чтение текстового файла (все платформы, кроме Android)
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
    
    // Стандартный способ (работает в редакторе, Windows, macOS, iOS)
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
    
    // Android-специфичное чтение
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

### 📝 Пример 2: Кроссплатформенный метод чтения (UnityWebRequest)
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

### 📝 Пример 3: Чтение бинарных файлов (изображения, модели)
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

## 4. Стриминг больших данных
При работе с большими файлами (сотни МБ) важно читать данные по частям (чанкам), а не загружать всё в память сразу.
### 🧩 Пример 1: Потоковое чтение большого JSON (построчная обработка)
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
            // Android требует UnityWebRequest, но он не поддерживает потоковое чтение
            // Альтернатива: скопировать файл в persistentDataPath и читать оттуда
            yield return StartCoroutine(CopyStreamingAssetToPersistentPath("large_dataset.json"));
            filePath = Path.Combine(Application.persistentDataPath, "large_dataset.json");
        #endif
        
        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;
            int lineCount = 0;
            
            // Читаем построчно, не загружая весь файл в память
            while ((line = reader.ReadLine()) != null)
            {
                ProcessLine(line);
                lineCount++;
                
                // Позволяем UI обновиться каждые 100 строк
                if (lineCount % 100 == 0)
                {
                    Debug.Log($"Processed {lineCount} lines...");
                    yield return null; // Не блокируем главный поток
                }
            }
            
            Debug.Log($"Finished processing {lineCount} lines");
        }
    }
    
    void ProcessLine(string line)
    {
        // Обработка одной строки
    }
    
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

### 🧩 Пример 2: Буферизированное чтение с использованием FileStream
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
                
                // Обработка чанка
                ProcessChunk(chunk);
                
                bytesRead += bytesToRead;
                float progress = (float)bytesRead / totalBytes;
                Debug.Log($"Read progress: {progress:P}");
            }
        }
    }
    
    static void ProcessChunk(byte[] chunk)
    {
        // Обработка бинарных данных
    }
}
```

---

## 5. Работа с видео из StreamingAssets
Unity поддерживает видеоплеер (`VideoPlayer`), который может загружать видео как из StreamingAssets, так и из URL.
### 🎬 Пример 1: Воспроизведение локального видео из StreamingAssets
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
        
        // VideoPlayer понимает разные форматы путей на разных платформах
        #if UNITY_ANDROID && !UNITY_EDITOR
            // На Android используем UnityWebRequest
            videoPlayer.url = videoPath;
        #elif UNITY_IOS && !UNITY_EDITOR
            // На iOS путь к Raw папке
            videoPlayer.url = videoPath;
        #elif UNITY_WEBGL && !UNITY_EDITOR
            // WebGL требует относительный путь
            videoPlayer.url = "StreamingAssets/" + fileName;
        #else
            // Windows, Mac, Linux, Editor
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

### 🎬 Пример 2: Стриминг большого видео с прогресс-баром
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
    [SerializeField] private string videoURL = "https://example.com/big_video.mp4"; // Или локальный путь
    
    IEnumerator Start()
    {
        // Для удалённого стриминга
        videoPlayer.url = videoURL;
        
        // Подписываемся на события
        videoPlayer.errorReceived += OnVideoError;
        videoPlayer.prepareCompleted += OnPrepareComplete;
        videoPlayer.started += OnVideoStarted;
        
        statusText.text = "Preparing video...";
        videoPlayer.Prepare();
        
        // Отслеживаем прогресс подготовки
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

### 🎬 Пример 3: Динамическая загрузка видео по чанкам (адаптивный стриминг)
```csharp
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ChunkedVideoLoader : MonoBehaviour
{
    private const int CHUNK_SIZE = 1024 * 1024; // 1 MB чанки
    
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
            
            // Финальный чанк
            byte[] finalChunk = request.downloadHandler.data;
            onChunkReceived?.Invoke(finalChunk);
        }
    }
}
```

---

## 6. StreamingAssets vs Resources vs PersistentDataPath

| Характеристика | StreamingAssets | Resources | PersistentDataPath |
| --- | --- | --- | --- |
| Доступ на чтение | ✅ Да (через пути) | ✅ Да (через `Resources.Load`) | ✅ Да |
| Доступ на запись | ⚠️ Только десктоп | ❌ Нет | ✅ Да | 
| Сжатие в билде | ❌ Нет | ✅ Да (LZ4) | ❌ Нет (вне сборки) |
| Внутренняя структура | Сохраняется | ❌ Плоская | Сохраняется |
| Динамическая загрузка | ✅ Да (по имени файла) | ❌ Только по пути ресурсов | ✅ Да |
| Изменение после сборки | ❌ Нет | ❌ Нет | ✅ Да |
| Поддержка стриминга | ✅ Да | ❌ Нет | ✅ Да |
| Использование для видео | ✅ Да | ❌ Нет | ✅ Да (скопированное) |

### Выбор подходящего места:
- StreamingAssets: Конфигурационные файлы, видео, большие ассеты, которые нужно читать потоково
- Resources: Маленькие ассеты, которые загружаются по имени и редко меняются
- PersistentDataPath: Сохранения игрока, скачанный контент, логи

---

## 7. Кроссплатформенные нюансы и решения
### 🔧 Android особенности:
```csharp
// ✅ Правильно: Использовать UnityWebRequest всегда на Android
IEnumerator LoadFromStreamingAssetsAndroid(string relativePath)
{
    string fullPath = Path.Combine(Application.streamingAssetsPath, relativePath);
    using (UnityWebRequest req = UnityWebRequest.Get(fullPath))
    {
        yield return req.SendWebRequest();
        if (req.result == UnityWebRequest.Result.Success)
        {
            // Успешно
        }
    }
}

// ❌ Неправильно: File.ReadAllText на Android выбросит исключение
string content = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "config.json"));
```

### 🔧 iOS особенности:
```csharp
// На iOS StreamingAssets доступна для обычного файлового ввода/вывода
// Но нужно учитывать sandbox
string path = Path.Combine(Application.streamingAssetsPath, "data.json");
if (File.Exists(path)) // Работает на iOS
{
    string content = File.ReadAllText(path);
}
```

### 🔧 WebGL особенности:
```csharp
// WebGL использует HTTP-запросы для доступа к StreamingAssets
IEnumerator LoadFromStreamingAssetsWebGL(string fileName)
{
    string url = Application.streamingAssetsPath + "/" + fileName;
    using (UnityWebRequest req = UnityWebRequest.Get(url))
    {
        yield return req.SendWebRequest();
        // Обработка ответа
    }
}
```

### 🔧 Универсальный загрузчик:
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

## 8. Лучшие практики
1. Используйте `Path.Combine` вместо конкатенации строк для путей
2. Всегда проверяйте существование файла перед чтением
3. Для больших файлов используйте потоковую обработку (чанки), чтобы не перегружать память
4. На Android всегда используйте `UnityWebRequest` — `File.ReadAllText` не работает
5. Кэшируйте файлы в `Application.persistentDataPath` для многократного доступа
6. Не храните секреты (пароли, ключи API) в StreamingAssets — они легко извлекаются
7. Для видео используйте `VideoPlayer` с правильным форматом (H.264 для мобильных)
8. Обрабатывайте ошибки загрузки — сети/файлы могут быть недоступны

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
