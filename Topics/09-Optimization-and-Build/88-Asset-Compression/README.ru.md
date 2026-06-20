# 🗜️ Сжатие и оптимизация ассетов в Unity: Texture Importer, Audio Importer, Model Importer, Presets
Оптимизация ассетов — один из ключевых этапов разработки игр. 
Правильные настройки импорта текстур, аудио и моделей могут сократить размер финальной сборки в 2-5 раз и значительно повысить производительность.

---

## 1. Texture Importer — Настройка импорта текстур
Texture Importer — это окно настроек для каждого текстурного ассета. 
Здесь задаются параметры сжатия, размера, формата и платформо-зависимые настройки.

### 📂 Доступ к Texture Importer:
1. Выберите текстуру в Project Window
2. В Inspector откроется Texture Importer

### 🎯 Основные параметры Texture Importer:
| Параметр | Описание | Рекомендация |
| --- | --- | --- |
| Texture Type | Тип текстуры (Sprite, Normal Map, UI, etc.) | Выбирайте по назначению |
| Sprite Mode | Single / Multiple / Polygon | Для спрайтов — Multiple |
| Pixels Per Unit | Количество пикселей на юнит | 100 (стандарт) |
| Mesh Type | Tight / Rectangular | Для спрайтов — Tight |
| Wrap Mode | Repeat / Clamp / Mirror | Зависит от текстуры |
| Filter Mode | Point / Bilinear / Trilinear | Для пиксель-арта — Point |
| Compression | None / Low / Normal / High | High для большинства текстур |
| Format | RGBA Compressed, DXT, ASTC и др. | Выбирайте под платформу |

### 🖼️ Настройка сжатия для разных платформ:
```csharp
// Пример скрипта для автоматической настройки сжатия текстур
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class TextureOptimizer : MonoBehaviour
{
    [MenuItem("Tools/Optimize Textures")]
    public static void OptimizeTextures()
    {
        // Находим все текстуры в папке Assets/Textures/
        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Textures" });
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            
            if (importer == null) continue;
            
            // Настройка для Android
            TextureImporterPlatformSettings androidSettings = importer.GetPlatformTextureSettings("Android");
            androidSettings.overridden = true;
            androidSettings.format = TextureImporterFormat.ASTC_6x6;
            androidSettings.compressionQuality = 50;
            importer.SetPlatformTextureSettings(androidSettings);
            
            // Настройка для iOS
            TextureImporterPlatformSettings iosSettings = importer.GetPlatformTextureSettings("iPhone");
            iosSettings.overridden = true;
            iosSettings.format = TextureImporterFormat.ASTC_6x6;
            iosSettings.compressionQuality = 50;
            importer.SetPlatformTextureSettings(iosSettings);
            
            // Для Standalone используем DXT
            TextureImporterPlatformSettings standaloneSettings = importer.GetPlatformTextureSettings("Standalone");
            standaloneSettings.overridden = true;
            standaloneSettings.format = TextureImporterFormat.DXT5;
            importer.SetPlatformTextureSettings(standaloneSettings);
            
            // Сжатие
            importer.textureCompression = TextureImporterCompression.Compressed;
            
            // Переимпорт
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            Debug.Log($"Оптимизирована текстура: {path}");
        }
    }
}
#endif
```

### 📊 Таблица форматов сжатия:
| Формат | Платформа | Качество | Размер | Когда использовать |
| --- | --- | --- | --- | --- |
| ASTC 4x4 | Android, iOS | Отличное | Средний | Игры с высоким качеством |
| ASTC 6x6 | Android, iOS | Хорошее | Маленький | Мобильные игры (рекоменд.) |
| ASTC 8x8 | Android, iOS | Среднее | Очень маленький | Для текстур высокого разрешения |
| DXT1 | Standalone | Хорошее | Маленький | Без альфа-канала |
| DXT5 | Standalone | Отличное | Средний | С альфа-каналом |
| ETC2 | Android | Хорошее | Средний | Старые устройства |
| PVRTC | iOS | Среднее | Маленький | Старые iOS устройства |
| RGBA 32-bit | Все | Идеальное | Огромный | Только для UI и критичных текстур |

### 🎨 Настройка спрайтов (Sprite):
```csharp
// Настройка спрайтов для 2D игры
public class SpriteSetupExample : MonoBehaviour
{
    void ConfigureSprite(Texture2D texture)
    {
        string path = AssetDatabase.GetAssetPath(texture);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        
        // Настройка как спрайта
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.spritePixelsPerUnit = 100;
        importer.spriteMeshType = SpriteMeshType.Tight;
        
        // Автоматическая нарезка спрайтов
        importer.spriteIsPolygon = false;
        importer.spriteExtrude = 1;
        importer.spriteGenerateFallbackPhysicsShape = true;
        
        // Сжатие для пиксель-арта
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.npotScale = TextureImporterNPOTScale.None;
        
        AssetDatabase.ImportAsset(path);
    }
}
```

### ⚡ Оптимизация максимального размера:
```csharp
// Уменьшение максимального размера для мобильных платформ
public static void ResizeTexturesForMobile()
{
    TextureImporter importer = GetTextureImporter();
    
    // Устанавливаем максимальный размер 2048 для мобильных
    TextureImporterPlatformSettings settings = importer.GetDefaultPlatformTextureSettings();
    settings.maxTextureSize = 2048;
    importer.SetPlatformTextureSettings(settings);
    
    // Для Android отдельно
    TextureImporterPlatformSettings androidSettings = importer.GetPlatformTextureSettings("Android");
    androidSettings.overridden = true;
    androidSettings.maxTextureSize = 1024; // Ещё меньше для старых устройств
    importer.SetPlatformTextureSettings(androidSettings);
}
```

---

## 2. Audio Importer — Настройка импорта аудио
Audio Importer управляет параметрами сжатия и качества звуковых файлов.

### 🔉 Основные параметры Audio Importer:
| Параметр | Описание | Рекомендация |
| --- | --- | --- |
| Force To Mono | Преобразование в моно | Да для большинства звуков |
| Normalize | Нормализация громкости | Да для SFX, Нет для музыки |
| Load In Background | Загрузка в фоновом режиме | Да для больших файлов |
| Ambisonic | 3D-звук для VR | Только для VR |
| Compression Format | Формат сжатия | Зависит от типа звука |

### 📊 Форматы сжатия аудио:
| Формат | Качество | Размер | Применение |
| --- | --- | --- | --- |
| PCM | Идеальное | Огромный | Только для критичных звуков |
| ADPCM | Хорошее | Маленький | SFX, короткие звуки |
| Vorbis | Хорошее | Маленький | Музыка, длинные звуки |
| MP3 | Среднее | Очень маленький | Фоновая музыка |
| HEVAG | Хорошее | Маленький | Японские консоли |

### 🎵 Пример настройки Audio Importer:
```csharp
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class AudioOptimizer : MonoBehaviour
{
    [MenuItem("Tools/Optimize Audio")]
    public static void OptimizeAudio()
    {
        string[] guids = AssetDatabase.FindAssets("t:AudioClip", new[] { "Assets/Audio" });
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            AudioImporter importer = AssetImporter.GetAtPath(path) as AudioImporter;
            
            if (importer == null) continue;
            
            // Настройка SFX
            if (path.Contains("SFX"))
            {
                importer.forceToMono = true;
                importer.normalize = true;
                importer.loadInBackground = false;
                
                // ADPCM для коротких SFX
                importer.defaultSampleSettings = new AudioImporterSampleSettings
                {
                    compressionFormat = AudioCompressionFormat.ADPCM,
                    sampleRateSetting = AudioSampleRateSetting.OverrideSampleRate,
                    sampleRateOverride = 22050,
                    quality = 0.5f
                };
            }
            
            // Настройка музыки
            if (path.Contains("Music"))
            {
                importer.forceToMono = false;
                importer.normalize = false;
                importer.loadInBackground = true;
                
                // Vorbis для музыки
                importer.defaultSampleSettings = new AudioImporterSampleSettings
                {
                    compressionFormat = AudioCompressionFormat.Vorbis,
                    sampleRateSetting = AudioSampleRateSetting.PreserveSampleRate,
                    quality = 0.7f // 70% качества
                };
            }
            
            // Настройка для мобильных платформ
            AudioImporterSampleSettings androidSettings = importer.GetOverrideSampleSettings("Android");
            androidSettings.compressionFormat = AudioCompressionFormat.Vorbis;
            androidSettings.quality = 0.5f; // 50% для экономии места
            importer.SetOverrideSampleSettings("Android", androidSettings);
            
            AudioImporterSampleSettings iosSettings = importer.GetOverrideSampleSettings("iPhone");
            iosSettings.compressionFormat = AudioCompressionFormat.MP3;
            iosSettings.quality = 0.6f;
            importer.SetOverrideSampleSettings("iPhone", iosSettings);
            
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            Debug.Log($"Оптимизирован звук: {path}");
        }
    }
}
#endif
```

### 🎚️ Продвинутая настройка звука:
```csharp
// Настройка 3D-звука
public class Audio3DSetup : MonoBehaviour
{
    void Configure3DSound(AudioSource source)
    {
        // Параметры 3D-звука
        source.spatialBlend = 1f;        // Полностью 3D
        source.dopplerLevel = 1f;         // Эффект Доплера
        source.spread = 0f;              // Разброс звука
        
        // Роллоф (затухание с расстоянием)
        source.rolloffMode = AudioRolloffMode.Logarithmic;
        source.minDistance = 1f;
        source.maxDistance = 50f;
        
        // Для диалогов используем меньшее затухание
        source.rolloffMode = AudioRolloffMode.Custom;
    }
}
```

---

## 3. Model Importer — Настройка импорта 3D-моделей
Model Importer управляет импортом 3D-моделей (FBX, OBJ, DAE и др.).

### 📐 Основные параметры Model Importer:
| Вкладка | Параметр | Описание |
| --- | --- | --- |
| Model | Scale Factor | Масштаб модели |
| | Use File Scale | Использовать масштаб из файла |
| | Bake Axis Conversion | Конвертация осей |
| Rig | Animation Type | None / Humanoid / Generic |
| | Avatar Definition | Create From This Model / Copy From Other |
| Animation | Import Animation | Импортировать анимацию |
| | Anim. Compression | Off / Keyframe Reduction / Optimal |
| Materials | Import Materials | Импортировать материалы |
| | Material Naming | By Base Textures / From Model |

### 🎮 Пример настройки Model Importer:
```csharp
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class ModelOptimizer : MonoBehaviour
{
    [MenuItem("Tools/Optimize Models")]
    public static void OptimizeModels()
    {
        string[] guids = AssetDatabase.FindAssets("t:Model", new[] { "Assets/Models" });
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;
            
            if (importer == null) continue;
            
            // Общие настройки
            importer.importMaterials = true;
            importer.materialName = ModelImporterMaterialName.BasedOnTextureName;
            importer.materialSearch = ModelImporterMaterialSearch.Local;
            
            // Настройка Rig
            importer.animationType = ModelImporterAnimationType.Generic;
            importer.avatarSetup = ModelImporterAvatarSetup.NoAvatar;
            
            // Оптимизация анимации
            importer.importAnimation = true;
            importer.animationCompression = ModelImporterAnimationCompression.Optimal;
            
            // Удаление неиспользуемых костей
            importer.optimizeGameObjects = true;
            importer.extraUserProperties = false;
            
            // Настройка LOD (если есть)
            importer.importLOD = true;
            
            // Сжатие мешей
            importer.meshCompression = ModelImporterMeshCompression.Medium;
            
            // Удаление неиспользуемых данных
            importer.keepQuads = false;
            importer.weldVertices = true;
            
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            Debug.Log($"Оптимизирована модель: {path}");
        }
    }
    
    [MenuItem("Tools/Optimize Models/Characters")]
    public static void OptimizeCharacters()
    {
        // Для персонажей используем Humanoid
        string[] guids = AssetDatabase.FindAssets("t:Model", new[] { "Assets/Models/Characters" });
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;
            
            if (importer == null) continue;
            
            importer.animationType = ModelImporterAnimationType.Humanoid;
            importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
            importer.animationCompression = ModelImporterAnimationCompression.KeyframeReduction;
            
            AssetDatabase.ImportAsset(path);
        }
    }
    
    [MenuItem("Tools/Optimize Models/Environment")]
    public static void OptimizeEnvironment()
    {
        // Для окружений используем Generic (без анимации)
        string[] guids = AssetDatabase.FindAssets("t:Model", new[] { "Assets/Models/Environment" });
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;
            
            if (importer == null) continue;
            
            importer.animationType = ModelImporterAnimationType.None;
            importer.importAnimation = false;
            importer.meshCompression = ModelImporterMeshCompression.High;
            importer.optimizeGameObjects = true;
            
            AssetDatabase.ImportAsset(path);
        }
    }
}
#endif
```

### 🏗️ Оптимизация мешей для мобильных устройств:
```csharp
// Настройка импорта для мобильных платформ
public static void ConfigureMobileModels()
{
    string[] guids = AssetDatabase.FindAssets("t:Model");
    
    foreach (string guid in guids)
    {
        string path = AssetDatabase.GUIDToAssetPath(guid);
        ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;
        
        if (importer == null) continue;
        
        // Мобильные оптимизации
        importer.meshCompression = ModelImporterMeshCompression.High;
        importer.optimizeGameObjects = true;
        importer.weldVertices = true;
        
        // Уменьшаем количество полигонов для мобильных
        importer.importNormals = ModelImporterNormals.Calculate;
        importer.importTangents = ModelImporterTangents.Calculate;
        
        // Ограничиваем количество вершин
        importer.importBlendShapes = false; // Отключаем BlendShapes на мобильных
        
        AssetDatabase.ImportAsset(path);
    }
}
```

---

## 4. Presets — Шаблоны настроек
Presets позволяют сохранить настройки импорта и применять их к множеству ассетов.

### 📁 Создание Preset:
1. Настройте Texture Importer для одного ассета
2. В верхнем правом углу Inspector → Preset → Save Preset
3. Дайте имя, например `MobileTexture.preset`
4. Preset будет сохранён в папке Assets/Presets/

### 🔄 Применение Preset к ассетам:
```csharp
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEditor.Presets;

public class PresetManager : MonoBehaviour
{
    [MenuItem("Tools/Apply Presets")]
    public static void ApplyPresets()
    {
        // Загрузка Preset
        Preset texturePreset = AssetDatabase.LoadAssetAtPath<Preset>("Assets/Presets/MobileTexture.preset");
        Preset audioPreset = AssetDatabase.LoadAssetAtPath<Preset>("Assets/Presets/MobileAudio.preset");
        Preset modelPreset = AssetDatabase.LoadAssetAtPath<Preset>("Assets/Presets/MobileModel.preset");
        
        // Применение к текстурам
        string[] textureGuids = AssetDatabase.FindAssets("t:Texture2D");
        foreach (string guid in textureGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null && texturePreset.CanBeAppliedTo(importer))
            {
                texturePreset.ApplyTo(importer);
                Debug.Log($"Применён Preset к: {path}");
            }
        }
        
        // Применение к звукам
        string[] audioGuids = AssetDatabase.FindAssets("t:AudioClip");
        foreach (string guid in audioGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            AudioImporter importer = AssetImporter.GetAtPath(path) as AudioImporter;
            if (importer != null && audioPreset.CanBeAppliedTo(importer))
            {
                audioPreset.ApplyTo(importer);
            }
        }
        
        // Применение к моделям
        string[] modelGuids = AssetDatabase.FindAssets("t:Model");
        foreach (string guid in modelGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;
            if (importer != null && modelPreset.CanBeAppliedTo(importer))
            {
                modelPreset.ApplyTo(importer);
            }
        }
        
        AssetDatabase.Refresh();
        Debug.Log("Все Preset применены!");
    }
}
#endif
```

### 🏷️ Автоматическое применение Preset по папкам:
```csharp
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;

public class AutoPresetApplier : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        // Автоматическая настройка текстур в папке UI
        if (assetPath.Contains("/UI/"))
        {
            Preset uiPreset = AssetDatabase.LoadAssetAtPath<Preset>("Assets/Presets/UITexture.preset");
            if (uiPreset != null && uiPreset.CanBeAppliedTo(assetImporter))
            {
                uiPreset.ApplyTo(assetImporter);
                Debug.Log($"Auto-applied UI Preset to: {assetPath}");
            }
        }
    }
    
    void OnPreprocessAudio()
    {
        // Автоматическая настройка звуков в папке SFX
        if (assetPath.Contains("/SFX/"))
        {
            Preset sfxPreset = AssetDatabase.LoadAssetAtPath<Preset>("Assets/Presets/SFXAudio.preset");
            if (sfxPreset != null && sfxPreset.CanBeAppliedTo(assetImporter))
            {
                sfxPreset.ApplyTo(assetImporter);
                Debug.Log($"Auto-applied SFX Preset to: {assetPath}");
            }
        }
    }
    
    void OnPreprocessModel()
    {
        // Автоматическая настройка моделей в папке Characters
        if (assetPath.Contains("/Characters/"))
        {
            Preset characterPreset = AssetDatabase.LoadAssetAtPath<Preset>("Assets/Presets/CharacterModel.preset");
            if (characterPreset != null && characterPreset.CanBeAppliedTo(assetImporter))
            {
                characterPreset.ApplyTo(assetImporter);
                Debug.Log($"Auto-applied Character Preset to: {assetPath}");
            }
        }
    }
}
#endif
```

---

## 5. Полный пример: Единый скрипт оптимизации
```csharp
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;
using System.IO;

public class AssetOptimizerFull : AssetPostprocessor
{
    [MenuItem("Tools/Full Asset Optimization")]
    public static void OptimizeAllAssets()
    {
        OptimizeTextures();
        OptimizeAudio();
        OptimizeModels();
        AssetDatabase.Refresh();
        Debug.Log("=== Оптимизация всех ассетов завершена! ===");
    }
    
    static void OptimizeTextures()
    {
        string[] guids = AssetDatabase.FindAssets("t:Texture2D");
        int count = 0;
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) continue;
            
            // Пропускаем UI-текстуры (их не сжимаем)
            if (path.Contains("/UI/"))
            {
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                continue;
            }
            
            // Настройка для мобильных
            TextureImporterPlatformSettings androidSettings = importer.GetPlatformTextureSettings("Android");
            androidSettings.overridden = true;
            androidSettings.format = TextureImporterFormat.ASTC_6x6;
            androidSettings.maxTextureSize = 2048;
            importer.SetPlatformTextureSettings(androidSettings);
            
            TextureImporterPlatformSettings iosSettings = importer.GetPlatformTextureSettings("iPhone");
            iosSettings.overridden = true;
            iosSettings.format = TextureImporterFormat.ASTC_6x6;
            iosSettings.maxTextureSize = 2048;
            importer.SetPlatformTextureSettings(iosSettings);
            
            // Общие настройки
            importer.mipmapEnabled = true;
            importer.textureCompression = TextureImporterCompression.Compressed;
            importer.maxTextureSize = 4096;
            importer.npotScale = TextureImporterNPOTScale.ToNearest;
            
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            count++;
        }
        
        Debug.Log($"Оптимизировано текстур: {count}");
    }
    
    static void OptimizeAudio()
    {
        string[] guids = AssetDatabase.FindAssets("t:AudioClip");
        int count = 0;
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            AudioImporter importer = AssetImporter.GetAtPath(path) as AudioImporter;
            if (importer == null) continue;
            
            // SFX
            if (path.Contains("/SFX/"))
            {
                importer.forceToMono = true;
                importer.defaultSampleSettings = new AudioImporterSampleSettings
                {
                    compressionFormat = AudioCompressionFormat.ADPCM,
                    sampleRateSetting = AudioSampleRateSetting.OverrideSampleRate,
                    sampleRateOverride = 22050
                };
            }
            // Music
            else if (path.Contains("/Music/"))
            {
                importer.defaultSampleSettings = new AudioImporterSampleSettings
                {
                    compressionFormat = AudioCompressionFormat.Vorbis,
                    sampleRateSetting = AudioSampleRateSetting.PreserveSampleRate,
                    quality = 0.6f
                };
            }
            
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            count++;
        }
        
        Debug.Log($"Оптимизировано аудио: {count}");
    }
    
    static void OptimizeModels()
    {
        string[] guids = AssetDatabase.FindAssets("t:Model");
        int count = 0;
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;
            if (importer == null) continue;
            
            // Настройка для мобильных
            if (path.Contains("/Characters/"))
            {
                importer.animationType = ModelImporterAnimationType.Humanoid;
                importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
            }
            else
            {
                importer.animationType = ModelImporterAnimationType.None;
                importer.importAnimation = false;
            }
            
            importer.meshCompression = ModelImporterMeshCompression.Medium;
            importer.optimizeGameObjects = true;
            importer.weldVertices = true;
            
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            count++;
        }
        
        Debug.Log($"Оптимизировано моделей: {count}");
    }
}
#endif
```

---

## 6. Лучшие практики
### ✅ Рекомендации:
| Ассет | Рекомендация |
| --- | --- |
| Текстуры | Используйте ASTC на мобильных, DXT на десктопе |
| | Максимальный размер 2048 или 1024 для мобильных |
| | Включайте Mipmaps для 3D-объектов |
| | Для пиксель-арта используйте Point фильтр |
| Аудио | SFX сжимайте в ADPCM, музыку — в Vorbis/MP3 |
| | Используйте моно для SFX, стерео для музыки |
| | Понижайте частоту для SFX (22050 Гц) |
| Модели | Используйте сжатие мешей (Medium) |
| | Оптимизируйте скелет (удаляйте неиспользуемые кости) |
| | Для окружений отключайте анимацию |

### ❌ Частые ошибки:
```csharp
// ❌ ОШИБКА: Использование RGBA 32-bit на всех текстурах
// Это увеличивает размер в 4-6 раз

// ❌ ОШИБКА: Отключение Mipmaps для 3D объектов
// Это вызывает мерцание текстур вдалеке

// ❌ ОШИБКА: Импорт музыки в PCM
// Музыка занимает огромный объём

// ❌ ОШИБКА: Импорт моделей без сжатия мешей
// Увеличивает размер и время загрузки

// ❌ ОШИБКА: Импорт анимаций для статичных объектов
// Используйте None для окружений
```

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
