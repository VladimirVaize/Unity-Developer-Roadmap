# 🎨 Post-Processing Stack в Unity: Bloom, Depth of Field, Color Grading, Anti-aliasing (Volume Framework)
Post-Processing — это финальный этап рендеринга, когда к уже отрисованному изображению применяются визуальные эффекты. 
Unity предоставляет мощный Volume Framework для настройки пост-эффектов как глобально, так и локально (в зависимости от положения камеры).

---

## 1. Основы Post-Processing в Unity
### 📦 Установка Post-Processing:
1. Window → Package Manager
2. Найти Post Processing
3. Установить пакет (версия 3.x или новее)

### 🏗️ Настройка базовой системы:
```csharp
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingSetup : MonoBehaviour
{
    public PostProcessVolume volume;
    public PostProcessProfile profile;
    
    void Start()
    {
        // Создаём Volume, если его нет
        if (volume == null)
        {
            GameObject volumeObj = new GameObject("PostProcessVolume");
            volume = volumeObj.AddComponent<PostProcessVolume>();
            volume.isGlobal = true;
        }
        
        // Создаём профиль, если его нет
        if (profile == null)
        {
            profile = ScriptableObject.CreateInstance<PostProcessProfile>();
            volume.sharedProfile = profile;
        }
    }
}
```

### 🎯 Компоненты Post-Processing:
| Компонент | Описание |
| --- | --- |
| PostProcessLayer | Добавляется на камеру, активирует пост-обработку |
| PostProcessVolume | Контейнер для настроек эффектов |
| PostProcessProfile | Хранит настройки всех эффектов (Asset) |
| PostProcessEffect | Конкретный эффект (Bloom, DOF и т.д.) |

---

## 2. Bloom — эффект свечения
Bloom создаёт эффект "свечения" вокруг ярких объектов. Имитирует рассеивание света в глазу или объективе камеры.

### 🎛️ Параметры Bloom:
| Параметр | Описание | Значение |
| --- | --- | --- |
| Intensity | Интенсивность свечения | 0-10 (обычно 0.5-2) |
| Threshold | Порог яркости для свечения | 0-1 (0.8-1.0) |
| Scatter | Рассеивание (ширина свечения) | 0-1 |
| Tint | Оттенок свечения | Color (белый/жёлтый) |
| Dirt Texture | Текстура грязи на линзе | Texture2D |

### 🖥️ Пример настройки Bloom:
```csharp
using UnityEngine.Rendering.PostProcessing;

public class BloomController : MonoBehaviour
{
    public PostProcessVolume volume;
    private Bloom bloom;
    
    void Start()
    {
        // Получаем или создаём эффект Bloom
        if (!volume.profile.TryGetSettings(out bloom))
        {
            bloom = volume.profile.AddSettings<Bloom>();
        }
        
        // Настройка параметров
        bloom.intensity.value = 1.5f;      // Интенсивность
        bloom.threshold.value = 0.9f;      // Порог (только очень яркие объекты)
        bloom.scatter.value = 0.7f;        // Широкое рассеивание
        bloom.tint.value = Color.yellow;   // Тёплое свечение
        bloom.dirtTexture.value = Resources.Load<Texture2D>("LensDirt");
        bloom.dirtIntensity.value = 0.3f;  // Интенсивность грязи
    }
    
    public void SetBloomIntensity(float intensity)
    {
        if (bloom != null)
            bloom.intensity.value = intensity;
    }
}
```

### 🌟 Особенности использования Bloom:
```csharp
// Динамическое изменение Bloom (например, при взрыве)
public class ExplosionBloom : MonoBehaviour
{
    public PostProcessVolume volume;
    private Bloom bloom;
    private float originalIntensity;
    
    void Start()
    {
        volume.profile.TryGetSettings(out bloom);
        originalIntensity = bloom.intensity.value;
    }
    
    public void TriggerExplosion()
    {
        StartCoroutine(FlashBloom(3f, 2f));
    }
    
    System.Collections.IEnumerator FlashBloom(float peak, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float intensity = Mathf.Lerp(originalIntensity, peak, Mathf.Sin(t * Mathf.PI));
            bloom.intensity.value = intensity;
            elapsed += Time.deltaTime;
            yield return null;
        }
        bloom.intensity.value = originalIntensity;
    }
}
```

---

## 3. Depth of Field (DOF) — глубина резкости
Depth of Field имитирует фокус камеры: объекты в фокусе чёткие, а передний и задний план размыты.

### 🎛️ Параметры Depth of Field:
| Параметр | Описание | Значение |
| --- | --- | --- |
| Focus Distance | Расстояние до объекта в фокусе | 0-100 (метров) |
| Aperture | Диафрагма (чем меньше, тем больше размытие) | 1-32 |
| Focal Length | Фокусное расстояние | 1-300 мм |
| Kernel Size | Качество размытия | Small/Medium/Large |
| Max Blur Size | Максимальное размытие | 0-10 |

### 📸 Пример настройки DOF:
```csharp
using UnityEngine.Rendering.PostProcessing;

public class DepthOfFieldController : MonoBehaviour
{
    public PostProcessVolume volume;
    private DepthOfField dof;
    
    void Start()
    {
        if (!volume.profile.TryGetSettings(out dof))
        {
            dof = volume.profile.AddSettings<DepthOfField>();
        }
        
        // Настройка портретного режима
        dof.focusDistance.value = 5f;        // Фокус на 5 метров
        dof.aperture.value = 2.8f;           // Малая диафрагма = сильное размытие
        dof.focalLength.value = 85f;         // Портретный объектив
        dof.kernelSize.value = KernelSize.Medium;
        dof.maxBlurSize.value = 5f;
    }
    
    public void SetFocusOnObject(Transform target)
    {
        if (dof != null && Camera.main != null)
        {
            float distance = Vector3.Distance(Camera.main.transform.position, target.position);
            dof.focusDistance.value = distance;
        }
    }
    
    public void EnableCinematicMode()
    {
        dof.aperture.value = 1.4f;           // Очень сильное размытие
        dof.focalLength.value = 50f;         // Кинематографический объектив
    }
}
```

### 🎮 Динамический DOF для RPG:
```csharp
public class DynamicDOF : MonoBehaviour
{
    public PostProcessVolume volume;
    public Transform player;
    public Transform currentTarget;
    private DepthOfField dof;
    private float smoothVelocity = 0f;
    
    void Start()
    {
        volume.profile.TryGetSettings(out dof);
    }
    
    void Update()
    {
        if (currentTarget != null)
        {
            // Плавный перефокус на цель
            float targetDistance = Vector3.Distance(player.position, currentTarget.position);
            dof.focusDistance.value = Mathf.SmoothDamp(
                dof.focusDistance.value,
                targetDistance,
                ref smoothVelocity,
                0.3f
            );
        }
    }
    
    public void FocusOnTarget(Transform target)
    {
        currentTarget = target;
    }
    
    public void ResetFocus()
    {
        currentTarget = null;
        dof.focusDistance.value = 10f;  // Возврат в бесконечность
    }
}
```

---

## 4. Color Grading — цветокоррекция
Color Grading позволяет менять цветовую гамму изображения, создавая определённое настроение.

### 🎛️ Параметры Color Grading:
| Параметр | Описание | Значение |
| --- | --- | --- |
| Post-exposure | Общая экспозиция | -10 до +10 EV |
| Contrast | Контрастность | -100 до +100 |
| Saturation | Насыщенность | -100 до +100 |
| Color Filter | Цветовой фильтр | Color |
| Hue Shift | Сдвиг оттенка | -180 до +180 |
| Temperature | Температура (синий/жёлтый) | -100 до +100 |
| Tint | Оттенок (зелёный/пурпурный) | -100 до +100 |
| Lift | Тени (цвет) | Color |
| Gamma | Средние тона (цвет) | Color |
| Gain | Света (цвет) | Color |

### 🎬 Примеры цветокоррекции:
```csharp
using UnityEngine.Rendering.PostProcessing;

public class ColorGradingController : MonoBehaviour
{
    public PostProcessVolume volume;
    private ColorGrading grading;
    
    void Start()
    {
        if (!volume.profile.TryGetSettings(out grading))
        {
            grading = volume.profile.AddSettings<ColorGrading>();
        }
        
        // Настройка "тёплого" кино-эффекта
        grading.temperature.value = 20f;      // Тёплый оттенок
        grading.tint.value = -5f;             // Лёгкий зелёный
        grading.saturation.value = 10f;       // Немного насыщеннее
        grading.contrast.value = 5f;          // Лёгкий контраст
        grading.postExposure.value = 0.5f;    // Ярче
    }
    
    public void ApplyHorrorMode()
    {
        // Холодный, мрачный эффект для хоррора
        grading.temperature.value = -30f;     // Холодный
        grading.tint.value = 20f;             // Пурпурный оттенок
        grading.saturation.value = -30f;      // Приглушённые цвета
        grading.contrast.value = 30f;         // Сильный контраст
        grading.postExposure.value = -1f;     // Темнее
        grading.gain.value = new Color(0.8f, 0.7f, 0.9f); // Холодные света
    }
    
    public void ApplyNostalgiaMode()
    {
        // Винтажный эффект
        grading.temperature.value = 15f;
        grading.saturation.value = -20f;
        grading.contrast.value = -10f;
        grading.gain.value = new Color(1.1f, 1.0f, 0.8f); // Жёлтые света
        grading.lift.value = new Color(0.1f, 0.05f, 0f);  // Тёплые тени
    }
}
```

### 📊 Использование LUT (Look-Up Table):
```csharp
// Загрузка внешней LUT для цветокоррекции
public class LUTController : MonoBehaviour
{
    public PostProcessVolume volume;
    public Texture2D customLUT;  // Перетащить .png LUT
    
    void Start()
    {
        ColorGrading grading;
        if (volume.profile.TryGetSettings(out grading))
        {
            grading.mode.value = ColorGradingMode.HighDefinitionRange;
            grading.lutTexture.value = customLUT;
            grading.lutContribution.value = 1f;  // Полная интенсивность
        }
    }
}
```

---

## 5. Anti-aliasing — сглаживание
Anti-aliasing устраняет "лестницу" на диагональных линиях и краях объектов.

### 🎛️ Типы Anti-aliasing:
| Тип | Описание | Производительность |
| --- | --- | --- |
| FXAA | Быстрое, но менее качественное | 🟢 Очень быстро |
| SMAA | Хорошее качество, средняя скорость | 🟡 Средняя |
| TAA | Лучшее качество, возможны артефакты | 🔴 Требовательно |

### 🖥️ Настройка Anti-aliasing:
```csharp
using UnityEngine.Rendering.PostProcessing;

public class AntiAliasingController : MonoBehaviour
{
    public PostProcessVolume volume;
    
    void Start()
    {
        // Настройка через PostProcessLayer (на камере)
        PostProcessLayer layer = Camera.main.GetComponent<PostProcessLayer>();
        if (layer != null)
        {
            // Выбор метода сглаживания
            layer.antialiasingMode = PostProcessLayer.Antialiasing.TemporalAntialiasing;
            
            // Настройка TAA
            TemporalAntialiasing taa = layer.temporalAntialiasing;
            taa.jitterSpread = 0.75f;      // Разброс сэмплов
            taa.stationaryBlending = 0.95f; // Блендинг для статичных объектов
            taa.motionBlending = 0.85f;     // Блендинг для движущихся объектов
        }
    }
    
    public void SetAntiAliasing(string mode)
    {
        PostProcessLayer layer = Camera.main.GetComponent<PostProcessLayer>();
        if (layer != null)
        {
            switch (mode)
            {
                case "FXAA":
                    layer.antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
                    break;
                case "SMAA":
                    layer.antialiasingMode = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;
                    break;
                case "TAA":
                    layer.antialiasingMode = PostProcessLayer.Antialiasing.TemporalAntialiasing;
                    break;
                case "Off":
                    layer.antialiasingMode = PostProcessLayer.Antialiasing.None;
                    break;
            }
        }
    }
}
```

---

## 6. Volume Framework — локальные и глобальные эффекты
Volume Framework позволяет применять эффекты не глобально, а в зависимости от положения камеры (зоны).

### 🎯 Создание локального Volume:
1. GameObject → Volume → Box/Sphere Volume
2. Настроить Trigger (триггер)
3. Добавить PostProcessVolume компонент
4. Установить Is Global = false
5. Настроить Blend Distance (плавность перехода)
```csharp
using UnityEngine.Rendering.PostProcessing;

public class VolumeZone : MonoBehaviour
{
    public PostProcessVolume volume;
    public Color zoneColor = Color.red;
    
    void Start()
    {
        // Настройка локального Volume
        volume.isGlobal = false;
        
        // Получаем компонент Collider и делаем его триггером
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;
        
        // Настраиваем эффекты для зоны
        ColorGrading grading;
        if (volume.profile.TryGetSettings(out grading))
        {
            grading.colorFilter.value = zoneColor;
            grading.colorFilter.overrideState = true;
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Вход в зону пост-эффектов");
            // Можно активировать дополнительные эффекты
            volume.weight = 1f;
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Выход из зоны пост-эффектов");
            volume.weight = 0f;
        }
    }
}
```

### 🌍 Глобальный Volume для сцены:
```csharp
// Глобальный профиль для всей сцены
public class GlobalPostProcessing : MonoBehaviour
{
    public PostProcessVolume globalVolume;
    
    void Awake()
    {
        // Создаём глобальный Volume
        if (globalVolume == null)
        {
            GameObject volumeObj = new GameObject("GlobalVolume");
            globalVolume = volumeObj.AddComponent<PostProcessVolume>();
            globalVolume.isGlobal = true;
            globalVolume.weight = 1f;
        }
        
        // Добавляем эффекты
        PostProcessProfile profile = ScriptableObject.CreateInstance<PostProcessProfile>();
        
        // Bloom
        Bloom bloom = profile.AddSettings<Bloom>();
        bloom.intensity.value = 0.3f;
        
        // Color Grading
        ColorGrading grading = profile.AddSettings<ColorGrading>();
        grading.saturation.value = 5f;
        grading.contrast.value = 3f;
        
        globalVolume.sharedProfile = profile;
    }
}
```

---

## 7. Полный пример: Переключение профилей
```csharp
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingManager : MonoBehaviour
{
    [Header("Volumes")]
    public PostProcessVolume globalVolume;
    public PostProcessVolume underwaterVolume;
    public PostProcessVolume cinematicVolume;
    public PostProcessVolume menuVolume;
    
    [Header("Settings")]
    public float transitionTime = 1f;
    
    private PostProcessVolume currentVolume;
    
    void Start()
    {
        // Начинаем с глобального
        SwitchToVolume(globalVolume);
    }
    
    public void SwitchToUnderwater()
    {
        StartCoroutine(SwitchWithBlend(underwaterVolume));
    }
    
    public void SwitchToCinematic()
    {
        StartCoroutine(SwitchWithBlend(cinematicVolume));
    }
    
    public void SwitchToMenu()
    {
        StartCoroutine(SwitchWithBlend(menuVolume));
    }
    
    public void SwitchToDefault()
    {
        StartCoroutine(SwitchWithBlend(globalVolume));
    }
    
    private IEnumerator SwitchWithBlend(PostProcessVolume target)
    {
        float elapsed = 0f;
        float startWeight = currentVolume != null ? currentVolume.weight : 0f;
        
        // Уменьшаем текущий Volume
        while (elapsed < transitionTime)
        {
            float t = elapsed / transitionTime;
            if (currentVolume != null)
                currentVolume.weight = Mathf.Lerp(startWeight, 0f, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        if (currentVolume != null)
            currentVolume.weight = 0f;
        
        // Активируем новый Volume
        currentVolume = target;
        target.weight = 0f;
        target.gameObject.SetActive(true);
        
        elapsed = 0f;
        while (elapsed < transitionTime)
        {
            float t = elapsed / transitionTime;
            target.weight = Mathf.Lerp(0f, 1f, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        target.weight = 1f;
    }
}
```

---

## 8. Лучшие практики
### ✅ Рекомендации:
1. Используйте профили (Profiles) — переиспользуйте настройки между сценами
2. Не применяйте все эффекты сразу — это дорого
3. TAA + Motion Blur — дают красивый кинематографичный вид
4. Тестируйте на целевых устройствах — пост-эффекты требовательны к производительности
5. Используйте LUT для цветокоррекции — быстрее, чем настройка параметров

### ❌ Частые ошибки:
```csharp
// ❌ ОШИБКА: Все эффекты включены одновременно
// Может упасть FPS на мобильных устройствах

// ✅ ПРАВИЛЬНО: Включать только нужные эффекты
// В Player Settings → Graphics → Built-in post-processing = отключить
// и использовать только Volume Framework

// ❌ ОШИБКА: Слишком сильный Bloom
bloom.intensity.value = 10f; // Ослепляет!

// ✅ ПРАВИЛЬНО: 0.5-2.0 для реалистичного эффекта

// ❌ ОШИБКА: Неправильное смешивание Volume
// Два Volume с weight = 1 дают двойной эффект

// ✅ ПРАВИЛЬНО: Использовать только один активный Volume
```

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
