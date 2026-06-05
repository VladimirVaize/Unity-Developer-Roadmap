# 📦 Asset Bundles: Упаковка контента для патчей и DLC

Этот материал посвящён AssetBundle — технологии упаковки ресурсов в Unity для их динамической загрузки и обновления. 
Хотя этот подход считается более старым и во многом вытесняется Addressable Assets, он остаётся актуальным для крупных проектов, 
где требуется полный контроль над процессом управления контентом, патчами и DLC.

---

## 🎯 Что такое AssetBundle и зачем он нужен?
AssetBundle — это архивный файл, содержащий ресурсы (модели, текстуры, звуки, префабы, сцены), 
которые можно загружать по требованию или скачивать с сервера после выхода приложения.

### Основные задачи AssetBundle:
- 📉 Уменьшение начального размера приложения — ресурсы загружаются по мере необходимости
- 🔄 Горячие обновления (патчи) — исправление багов и замена контента без переупаковки всего приложения
- 🛍️ Распространение DLC (Downloadable Content) — добавление новых уровней, персонажей, предметов после релиза
- 💾 Оптимизация памяти — выгрузка неиспользуемых ресурсов

---

## 🏗️ Архитектура AssetBundle: как это работает
### Основные компоненты системы:
1. Сборки (Bundles) — файлы с расширением `.bundle` или `.assetbundle`, содержащие упакованные ресурсы
2. Манифесты (Manifests) — служебные файлы, описывающие содержимое каждого бандла и зависимости между ними
3. Кэш (Cache) — локальное хранилище скачанных бандлов на устройстве пользователя
4. Сервер (CDN) — удалённое хранилище для распространения бандлов

### Два основных API для загрузки:

| API | Назначение | Особенность |
| --- | --- | --- |
| `AssetBundle.LoadFromFile` | Локальная загрузка из StreamingAssets или PersistentDataPath | Синхронная/асинхронная, эффективна для LZ4 сжатия |
| `UnityWebRequest` | Сетевая загрузка с сервера + кэширование | Поддержка версионирования и кэша через `Caching` |

---

## 🔨 Создание AssetBundle (Процесс билда)
### Шаг 1: Назначение имени бандла в редакторе
В окне Project выберите любой ресурс (префаб, текстуру, модель). В инспекторе найдите секцию AssetBundle в самом низу:
- Выберите `New...` чтобы создать новое имя бандла
- Опционально укажите Variant (например, `hd`, `mobile`, `low`) для разных платформ/качеств

### Шаг 2: Скрипт для сборки
Создайте скрипт в папке `Editor`:
```csharp
using UnityEditor;
using System.IO;

public class AssetBundleBuilder
{
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        string outputPath = "AssetBundles";
        
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);
        
        // BuildAssetBundleOptions.None — LZMA сжатие (минимальный размер, но медленная загрузка)
        // BuildAssetBundleOptions.ChunkBasedCompression — LZ4 (быстрая загрузка, больше размер)
        BuildPipeline.BuildAssetBundles(
            outputPath,
            BuildAssetBundleOptions.ChunkBasedCompression,
            BuildTarget.StandaloneWindows64  // Измените под вашу платформу
        );
        
        Debug.Log("AssetBundles built successfully!");
    }
}
```

> [!Important]
> Результат сборки должен находиться вне папки `Assets/` на время билда, чтобы AssetDatabase не мешал процессу.
> После сборки файлы можно скопировать в `StreamingAssets` для локальной упаковки или выгрузить на сервер.

---

## 📥 Загрузка AssetBundle
### Способ 1: Локальная загрузка из StreamingAssets
```csharp
using System.Collections;
using UnityEngine;

public class LocalBundleLoader : MonoBehaviour
{
    IEnumerator Start()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "AssetBundles/characters");
        
        // Асинхронная загрузка (рекомендуется)
        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(path);
        yield return request;
        
        AssetBundle bundle = request.assetBundle;
        if (bundle == null)
        {
            Debug.LogError("Failed to load AssetBundle!");
            yield break;
        }
        
        // Загрузка ассета по имени
        GameObject prefab = bundle.LoadAsset<GameObject>("Player");
        Instantiate(prefab);
        
        // Очистка: false — оставляет созданные объекты, удаляет только связь с бандлом
        bundle.Unload(false);
    }
}
```

### Способ 2: Сетевая загрузка с кэшированием (для DLC и патчей)
```csharp
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class WebBundleLoader : MonoBehaviour
{
    IEnumerator LoadBundleFromWeb()
    {
        string url = "https://yourserver.com/bundles/characters";
        uint version = 2;  // Версия бандла — при изменении скачается заново
        
        using (UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(url, version))
        {
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
                GameObject prefab = bundle.LoadAsset<GameObject>("Player");
                Instantiate(prefab);
                bundle.Unload(false);
            }
            else
            {
                Debug.LogError($"Download failed: {request.error}");
            }
        }
    }
}
```

### Способы загрузки в сравнении:

| Метод | Для чего | Особенности |
| --- | --- | --- |
| `LoadFromFile(Async)` | Локальные бандлы | Самый быстрый, без копирования в память  |
| `LoadFromMemory(Async)` | Шифрованные/кастомные загрузки | Требует `byte[]`, двойная память |
| `UnityWebRequest` | Сетевые загрузки | Поддержка кэша, версионирование |
| `LoadFromCacheOrDownload` | Устаревший | Используйте `UnityWebRequest` вместо `WWW` |

---

## 🔄 Патчинг и обновление контента
### Как работает система патчей:
1. Клиент хранит список локальных бандлов + их версии (или хэши)
2. Сервер предоставляет актуальный манифест (список бандлов и версий)
3. Пачер сравнивает списки и скачивает отсутствующие/изменившиеся бандлы

### Пример манифеста в формате JSON:
```json
{
    "bundles": [
        { "name": "characters", "hash": "a3f5c2e1d4b6", "size": 2048000 },
        { "name": "levels/level1", "hash": "b7d9f1a3c5e8", "size": 512000 }
    ]
}
```

### Простая логика пачера:
```csharp
public class PatchManager : MonoBehaviour
{
    [Serializable]
    public class BundleInfo
    {
        public string name;
        public string hash;
    }
    
    [Serializable]
    public class Manifest
    {
        public List<BundleInfo> bundles;
    }
    
    async Task CheckForUpdates()
    {
        // 1. Скачиваем манифест с сервера
        string manifestUrl = "https://yourserver.com/manifest.json";
        string json = await DownloadString(manifestUrl);
        Manifest serverManifest = JsonUtility.FromJson<Manifest>(json);
        
        // 2. Сравниваем с локальным кэшем (например, PlayerPrefs)
        foreach (var bundle in serverManifest.bundles)
        {
            string localHash = PlayerPrefs.GetString($"bundle_{bundle.name}");
            if (localHash != bundle.hash)
            {
                // 3. Скачиваем обновлённый бандл
                await DownloadBundle(bundle.name);
                PlayerPrefs.SetString($"bundle_{bundle.name}", bundle.hash);
            }
        }
    }
}
```

### ⚠️ Важные замечания о патчинге:
- Unity не предоставляет встроенного механизма дифференциальных патчей (скачивания только изменённых частей файла)
- Для дифференциальных обновлений требуется кастомный загрузчик
- При использовании `WWW.LoadFromCacheOrDownload` или `UnityWebRequest` достаточно изменить номер версии в параметре — API сам перекачает обновление

---

## 🧩 Управление зависимостями
Главная сложность AssetBundle — корректная обработка зависимостей между бандлами.

### Проблема:
Если у вас есть:
- `Player.prefab` (использует `Hero.mat`)
- `Hero.mat` (использует `Hero_Diffuse.png`)

И вы положили префаб в `characters.bundle`, а материал и текстуру оставили общими — Unity может:
1. Дублировать ресурсы в каждом бандле (увеличение размера)
2. Потерять ссылки при загрузке в неправильном порядке

### Решение:
```csharp
// 1. Загружаем сначала зависимости
IEnumerator LoadWithDependencies()
{
    // Сначала загружаем общие ресурсы
    AssetBundle sharedMat = AssetBundle.LoadFromFile(Path.Combine(bundlePath, "shared_materials"));
    AssetBundle sharedTex = AssetBundle.LoadFromFile(Path.Combine(bundlePath, "shared_textures"));
    
    // Затем загружаем бандл с персонажем
    AssetBundle characters = AssetBundle.LoadFromFile(Path.Combine(bundlePath, "characters"));
    
    // Только после загрузки всех зависимостей можно безопасно загружать ассеты
    GameObject player = characters.LoadAsset<GameObject>("Player");
    Instantiate(player);
}
```

### Манифест как помощник:
При сборке генерируется файл `AssetBundles.manifest`, который содержит все зависимости:
```csharp
AssetBundle manifestBundle = AssetBundle.LoadFromFile(bundlePath);
AssetBundleManifest manifest = manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

string[] dependencies = manifest.GetAllDependencies("characters");
foreach (string dep in dependencies)
{
    AssetBundle.LoadFromFile(Path.Combine(bundlePath, dep));
}
```

---

## 📊 Сравнение с Addressable Assets

| Характеристика | AssetBundle | Addressable Assets |
| --- | --- | --- |
| Уровень контроля | Полный, ручной | Автоматизированный |
| Сложность настройки | Высокая (ручное управление зависимостями) | Низкая (всё встроено) |
| Гибкость загрузки | Любая, всё контролируете | Ограничена фреймворком |
| Производительность | Потенциально выше (нет оверхеда) | Хорошая, но есть накладные расходы |
| Когда выбирать | Крупные проекты со сложными требованиями | Большинство средних и малых проектов |
| Поддержка патчей | Полный контроль над версионированием | Встроенная, простая |
| Скрипты и код | ❌ Нельзя обновить C# скрипты через Bundle | ❌ Тоже нельзя |

> [!Important]
> AssetBundle не позволяют обновлять C# скрипты — только ресурсы. Для обновления логики требуются другие решения (ILRuntime, HybridCLR).

---

## 🛠️ Практические советы
### 1. Сжатие:
- LZMA — минимальный размер, медленная распаковка (всё в память)
- LZ4 — чуть больше размер, но потоковая загрузка с диска
- Выбирайте LZ4 (`ChunkBasedCompression`) для локальных бандлов

### 2. Наименование:
```csharp
// Формат: [category]/[name]_[platform]_[quality]
"characters/heroes_windows_hd"
"levels/dungeon_android_mobile"
```

### 3. Память:
Всегда вызывайте `bundle.Unload(false)` когда бандл больше не нужен, чтобы освободить память

### 4. Платформы:
AssetBundle не совместимы между платформами — собирайте отдельно для каждой

### 5. iOS особенности:
- Устанавливайте флаг `No Backup` для скачанных бандлов через `iOS.Device.SetNoBackupFlag()`
- Используйте `Application.temporaryCachePath` для временных бандлов

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
