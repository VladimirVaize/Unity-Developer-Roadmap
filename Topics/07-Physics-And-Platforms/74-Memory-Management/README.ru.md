# 🧠 Управление памятью на мобильных устройствах: Texture Compression, ASTC, Resolutions, Sprite Atlases
На мобильных устройствах память графического процессора (VRAM) и оперативная память (RAM) являются критическими ресурсами. 
Текстуры — одни из главных потребителей памяти. Неправильное управление текстурами может привести к вылетам (OutOfMemory), 
фризам и высокому энергопотреблению. В этом руководстве мы разберём ключевые техники оптимизации.

---

## 1. Как текстуры потребляют память?
Размер текстуры в памяти вычисляется по формуле:
```text
Память (байты) = Ширина × Высота × Битность / 8
```

Пример для текстуры 2048×2048 в формате RGBA32 (32 бита на пиксель):
```text
2048 × 2048 × 32 / 8 = 16 777 216 байт ≈ 16 МБ
```

Если у вас 100 таких текстур → 1.6 ГБ памяти, что убьёт любое мобильное устройство.
> [!Important]
> Это только видеопамять. Каждая текстура также занимает место в оперативной памяти при загрузке.

### 🎯 Основные способы сокращения памяти текстур:
| Метод | Экономия | Потеря качества |
| --- | --- | --- |
| Уменьшение разрешения | Высокая | Высокая |
| Сжатие текстур (ASTC/ETC2) | Очень высокая | Средняя |
| Спрайт-атласы | Умеренная (меньше вызовов рендера) | Нет |
| Mipmaps | Дополнительная память (по желанию) | Улучшает сглаживание |

---

## 2. Сжатие текстур (Texture Compression)
Сжатие работает как JPEG, но для GPU: текстура хранится в сжатом виде и распаковывается аппаратно при рендеринге.

### 📊 Сравнение форматов сжатия для мобильных устройств:
| Формат | Платформы | Бит на пиксель | Размер (1024x1024) | Качество |
| --- | --- | --- | --- | --- |
| ASTC 4x4 | Android (современный), iOS | 8 bpp | 1 МБ | Отличное |
| ASTC 6x6 | Android, iOS | 3.56 bpp | 456 КБ | Хорошее |
| ASTC 8x8 | Android, iOS | 2 bpp | 256 КБ | Среднее |
| ETC2 RGB | Android (все современные) | 4 bpp | 512 КБ | Хорошее |
| ETC2 RGBA | Android | 8 bpp | 1 МБ | Хорошее |
| PVRTC | iOS (старые устройства) | 2-4 bpp | 256-512 КБ | Среднее |
| RGBA32 (несжатый) | Все | 32 bpp | 4 МБ | Идеальное |

> 🏆 ASTC (Adaptive Scalable Texture Compression) — лучший выбор для современных мобильных устройств.
> Он поддерживает произвольные размеры блоков от 4x4 до 12x12.

### 🛠️ Пример настройки в Unity:
1. Выберите текстуру в Project Window
2. В инспекторе найдите Platform Settings
3. Переопределите настройки для Android и iOS:

```csharp
// Пример кода для проверки формата текстуры в рантайме
public class TextureFormatChecker : MonoBehaviour
{
    void Start()
    {
        Texture2D tex = GetComponent<Renderer>().material.mainTexture as Texture2D;
        Debug.Log($"Формат текстуры: {tex.format}");
        
        switch (tex.format)
        {
            case TextureFormat.ASTC_4x4:
            case TextureFormat.ASTC_6x6:
            case TextureFormat.ASTC_8x8:
                Debug.Log("✓ Используется ASTC сжатие (оптимально)");
                break;
            case TextureFormat.ETC2_RGB:
            case TextureFormat.ETC2_RGBA8:
                Debug.Log("✓ Используется ETC2 сжатие (хорошо)");
                break;
            case TextureFormat.RGBA32:
                Debug.Log("⚠ ВНИМАНИЕ: Несжатая текстура! Огромное потребление памяти!");
                break;
        }
    }
}
```

### 📝 ASTC — подробно:
Выбор блоков ASTC:
| Блок | Бит/пиксель | Когда использовать |
| --- | --- | --- |
| 4x4 | 8 bpp | Иконки, UI, тексты (максимальное качество) |
| 5x4 | 6.4 bpp | Основные текстуры окружения |
| 5x5 | 5.12 bpp | Персонажи, пропсы среднего размера |
| 6x6 | 3.56 bpp | Фоновые текстуры, ландшафт |
| 8x8 | 2 bpp | Очень большие текстуры (небо, дальний план) |

### Настройка ASTC в Unity:
```text
Текстура → Inspector → Android → Override
Format: ASTC (6x6 block size)
Compression Quality: High (медленнее билд, лучше качество)
```

---

## 3. Разрешения текстур (Texture Resolutions)
Золотое правило: используйте минимально необходимое разрешение. Мобильные экраны имеют ограниченную чёткость.

### 📱 Рекомендуемые разрешения для разных типов текстур:
| Тип текстуры | Макс. разрешение | Почему |
| --- | --- | --- |
| UI иконки | 128×128 - 256×256 | На экране они маленькие |
| UI фоны | 1024×1024 | Растягиваются на весь экран |
| Персонаж (третье лицо) | 1024×1024 | Занимает 30-50% экрана |
| Персонаж (первое лицо - руки/оружие) | 2048×2048 | Близко к камере |
| Окружение | 512×512 - 1024×1024 | Много объектов |
| Небо (Skybox) | 2048×2048 | Растянуто на весь экран |
| Спрайты (2D игра) | 512×512 - 1024×1024 | Зависит от размера на экране |

### 🛠️ Автоматическое управление разрешением:
```csharp
using UnityEngine;

public class AdaptiveTextureManager : MonoBehaviour
{
    [System.Serializable]
    public class TextureProfile
    {
        public Texture2D texture;
        public int maxResolution = 1024;
    }

    public TextureProfile[] textures;
    
    void Start()
    {
        // Определяем уровень производительности устройства
        int qualityLevel = GetDeviceQualityLevel();
        
        foreach (var profile in textures)
        {
            int targetResolution = Mathf.Min(profile.maxResolution, GetResolutionByQuality(qualityLevel));
            
            // Изменяем разрешение текстуры в рантайме (требуется AssetBundle-подход)
            Debug.Log($"Текстура {profile.texture.name} будет ограничена {targetResolution}x{targetResolution}");
        }
    }
    
    private int GetDeviceQualityLevel()
    {
        // 0: low, 1: medium, 2: high
        if (SystemInfo.systemMemorySize < 2048) return 0;
        if (SystemInfo.systemMemorySize < 4096) return 1;
        return 2;
    }
    
    private int GetResolutionByQuality(int quality)
    {
        switch (quality)
        {
            case 0: return 512;   // Низкий
            case 1: return 1024;  // Средний
            default: return 2048; // Высокий
        }
    }
}
```

> [!Tip]
> Для мобильных игр редко нужны текстуры больше 2048x2048. Исключение — кинематографические кат-сцены.

---

## 4. Спрайт-атласы (Sprite Atlases)
Спрайт-атлас — это одна большая текстура, содержащая множество мелких спрайтов. 
Это радикально снижает количество вызовов рендеринга (draw calls).

### 📊 Преимущества атласов:
| Показатель | Без атласа | С атласом |
| --- | --- | --- |
| Draw calls (100 спрайтов) | 100 | 1-2 |
| Память GPU | Сумма всех текстур | Одна текстура |
| Загрузка времени | 100 отдельных файлов | 1 файл |

### 🛠️ Создание атласа в Unity (Sprite Atlas V2):
```csharp
// Пример конфигурации атласа через скрипт
using UnityEngine.U2D;
using UnityEditor.U2D;
using UnityEngine;

#if UNITY_EDITOR
public class SpriteAtlasBuilder : MonoBehaviour
{
    [MenuItem("Tools/Build UI Atlas")]
    public static void BuildUIAtlas()
    {
        // Создаём новый атлас
        SpriteAtlas atlas = ScriptableObject.CreateInstance<SpriteAtlas>();
        
        // Настройки сжатия
        atlas.SetPlatformSettings(new SpriteAtlasPlatformSettings
        {
            name = "Android",
            format = TextureImporterFormat.ASTC_6x6,
            maxTextureSize = 2048
        });
        
        // Добавляем спрайты из папки
        string folderPath = "Assets/Sprites/UI";
        Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(folderPath);
        
        atlas.Add(sprites);
        
        // Сохраняем
        AssetDatabase.CreateAsset(atlas, "Assets/Atlas/UIAtlas.spriteatlasv2");
        AssetDatabase.SaveAssets();
    }
}
#endif
```

### 📝 Ручная настройка атласа:
1. Window → 2D → Sprite Atlas
2. Нажмите Create New Sprite Atlas
3. Добавьте спрайты в поле Objects for Packing
4. Настройки:
   - Allow Rotation: Off (лучше сжатие)
   - Tight Packing: Off (если не нужны точные границы)
   - Padding: 2-4 пикселя (предотвращает артефакты соседей)
  
### 🧪 Использование атласа в коде:
```csharp
using UnityEngine;
using UnityEngine.U2D;

public class SpriteFromAtlasLoader : MonoBehaviour
{
    [SerializeField] private SpriteAtlas uiAtlas;
    [SerializeField] private string spriteName = "button_play";
    
    void Start()
    {
        // Загружаем спрайт из атласа
        Sprite mySprite = uiAtlas.GetSprite(spriteName);
        
        if (mySprite != null)
        {
            GetComponent<Image>().sprite = mySprite;
            Debug.Log($"Спрайт {spriteName} загружен из атласа");
        }
        else
        {
            Debug.LogError($"Спрайт {spriteName} не найден в атласе");
        }
    }
    
    // Работа с несколькими спрайтами
    public void ShowIcons(string[] iconNames)
    {
        foreach (string name in iconNames)
        {
            Sprite icon = uiAtlas.GetSprite(name);
            if (icon != null)
                CreateIconObject(icon);
        }
    }
    
    private void CreateIconObject(Sprite sprite) { /* ... */ }
}
```

### ⚠️ Подводные камни атласов:
| Проблема | Решение |
| --- | --- |
| Артефакты на границах спрайтов | Увеличьте Padding до 4-8 |
| Слишком большой атлас (4096+) | Разбейте на несколько атласов по категориям (UI, персонажи, окружение) |
| Часто обновляемые спрайты | Выносите их в отдельный атлас (обновление одного спрайта требует пересборки всего атласа) |
| Спрайты с разным сжатием | Используйте отдельные атласы для разных настроек |

---

## 5. Mipmaps — управление качеством на расстоянии
Mipmaps — это серия уменьшенных копий текстуры. 
Они нужны для предотвращения мерцания на дальних объектах.

### 📊 Память для mipmaps:
```text
Общий размер с mipmaps = Исходный размер × (1 + 1/4 + 1/16 + 1/64 + ...) ≈ Исходный размер × 1.33
```

Пример: Текстура 1024×1024 (1 МБ) с mipmaps займёт ~1.33 МБ.
### 🛠️ Когда включать mipmaps:
| Тип объекта | Mipmaps | Причина |
| --- | --- | --- |
| Пол (земля) | ✅ Да | Далеко и близко одновременно |
| Стены зданий | ✅ Да | Видны с разных расстояний |
| UI элементы | ❌ Нет | Всегда на одном расстоянии |
| Персонаж (3D) | ✅ Да | Может отдаляться |
| 2D спрайты | ❌ Нет | Камера не меняет зум |

```csharp
// Проверка и управление mipmaps в коде
public class MipmapManager : MonoBehaviour
{
    void Start()
    {
        Texture texture = GetComponent<Renderer>().material.mainTexture;
        
        if (texture != null)
        {
            Debug.Log($"Mipmaps включены: {texture.mipmapCount > 1}");
            Debug.Log($"Количество уровней mipmap: {texture.mipmapCount}");
            
            // Ограничение максимального уровня mipmap (экономия памяти)
            texture.mipMapBias = -0.5f; // Сдвиг в сторону более чётких текстур
        }
    }
}
```

---

## 6. Практический чек-лист по памяти текстур
### ✅ Оптимизация для Android:
- Все текстуры сжаты в ASTC (или ETC2 для старых устройств)
- Размер текстур: степени двойки (2, 4, 8, 16, 32... 2048)
- UI текстуры: Generate Mipmaps = OFF
- 3D текстуры: Generate Mipmaps = ON
- Спрайты объединены в Sprite Atlases (макс. 2048x2048)
- Несжатые текстуры используются только где критично качество

### ✅ Оптимизация для iOS:
- Все текстуры сжаты в ASTC (на новых устройствах) или PVRTC (на старых)
- Используйте Override for iOS в настройках импорта
- Проверьте память через XCode Memory Debugger

### 🛠️ Мониторинг потребления памяти:
```csharp
using UnityEngine.Profiling;

public class TextureMemoryMonitor : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            long totalMemory = Profiler.GetTotalAllocatedMemory();
            long textureMemory = Profiler.GetAllocatedMemoryForGraphicsDriver();
            
            Debug.Log($"Общая память: {totalMemory / 1048576} MB");
            Debug.Log($"Память текстур: {textureMemory / 1048576} MB");
            
            if (textureMemory > 300 * 1048576) // >300 MB
                Debug.LogWarning("⚠ Высокое потребление видеопамяти!");
        }
    }
}
```

---

## 7. Типичные ошибки и решения
| Ошибка | Почему плохо | Решение |
| --- | --- | --- |
| RGBA32 текстура | 4 МБ на 1024x1024 вместо 0.5 МБ со сжатием | Переключить на ASTC |
| Атлас 4096×4096 | Вылеты на старых устройствах | Уменьшить до 2048×2048 |
| Mipmaps на UI | Трата памяти (33% лишних) | Выключить mipmaps |
| Текстура 300×300 | Не степень двойки → распаковка и лишняя память | Обрезать до 256×256 или 512×512 |
| Огромные спрайты | 2048×2048 для иконки 32×32 | Уменьшить до 128×128 + атлас |

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
