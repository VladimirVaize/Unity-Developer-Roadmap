# 🔊 AudioSource и AudioListener: Проигрывание звуков, 2D vs 3D звук, позиционирование в пространстве
Звук в Unity — неотъемлемая часть погружения игрока в игровой мир. 
Два основных компонента отвечают за воспроизведение звука: AudioListener (как «уши» игрока) и AudioSource (как «источник звука»). 
В этом руководстве мы разберём их настройку, различия между 2D и 3D звуком, а также пространственное позиционирование.

---

## 1. AudioListener — «Уши» в игровом мире
AudioListener — это компонент, который представляет слушателя в сцене. 
Обычно он прикреплён к Main Camera, так как камера — это «глаза игрока», а слушатель — его «уши». 
В сцене может быть только один активный AudioListener.

### 🎛️ Основные свойства AudioListener:
| Свойство | Описание |
| --- | --- |
| `Volume` | Глобальная громкость (через `AudioListener.volume`) |
| `Pause` | Пауза для всех звуков (`AudioListener.pause`) |
| `VelocityUpdateMode` | Режим обновления скорости для эффекта Доплера |

### 📝 Пример работы с AudioListener:
```csharp
using UnityEngine;

public class GlobalAudioManager : MonoBehaviour
{
    void Start()
    {
        // Устанавливаем глобальную громкость (0 - тихо, 1 - норма, 2 - вдвое громче)
        AudioListener.volume = 1f;
        
        // Проверяем, есть ли активный слушатель
        AudioListener currentListener = FindObjectOfType<AudioListener>();
        if (currentListener != null)
        {
            Debug.Log("AudioListener найден на объекте: " + currentListener.gameObject.name);
        }
    }
    
    void Update()
    {
        // Кнопка M - выключить/включить звук
        if (Input.GetKeyDown(KeyCode.M))
        {
            AudioListener.volume = AudioListener.volume > 0 ? 0f : 1f;
            Debug.Log($"Глобальная громкость: {AudioListener.volume}");
        }
        
        // Кнопка P - пауза всех звуков
        if (Input.GetKeyDown(KeyCode.P))
        {
            AudioListener.pause = !AudioListener.pause;
            Debug.Log(AudioListener.pause ? "Звук на паузе" : "Звук продолжен");
        }
    }
}
```

> [!Important]
> Если в сцене больше одного `AudioListener`, Unity выдаст предупреждение. Отключайте лишние.

---

## 2. AudioSource — Источник звука
AudioSource — компонент, который проигрывает звуковые клипы (`AudioClip`). 
Он может воспроизводить как простые 2D-звуки (фоновая музыка), так и 3D-звуки с учётом позиции в пространстве.

### 🎛️ Основные свойства AudioSource:
| Свойство | Описание |
| --- | --- |
| `AudioClip` | Ссылка на звуковой файл |
| `Output` | Аудиомикшер (AudioMixerGroup) для эффектов |
| `Mute` | Отключение звука |
| `Loop` | Зацикливание воспроизведения |
| `Play On Awake` | Автоматический старт при создании |
| `Volume` | Громкость источника (0-1) |
| `Pitch` | Высота тона (1 = норма, >1 выше, <1 ниже) |
| `Pan` | Стереопанорама (2D звук, от -1 до 1) |
| `Spatial Blend` | Баланс между 2D и 3D (0 = полный 2D, 1 = полный 3D) |

### 📝 Пример базового проигрывания:
```csharp
using UnityEngine;

public class SimpleSoundPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip backgroundMusic;
    
    private AudioSource audioSource;
    
    void Start()
    {
        // Добавляем компонент AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        
        // Настройка для фоновой музыки
        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        audioSource.volume = 0.5f;
        audioSource.Play();
    }
    
    void Update()
    {
        // Прыжок с воспроизведением звука
        if (Input.GetButtonDown("Jump"))
        {
            PlayJumpSound();
        }
    }
    
    private void PlayJumpSound()
    {
        // Простой способ - PlayOneShot (не прерывает другие звуки)
        audioSource.PlayOneShot(jumpSound, 0.8f);
        
        // Альтернатива - PlayClipAtPoint (одноразовый звук в мире)
        // AudioSource.PlayClipAtPoint(jumpSound, transform.position, 1f);
    }
}
```

### 🎵 Режимы воспроизведения:
| Метод | Описание |
| --- | --- |
| `Play()` | Обычное воспроизведение (начинает с начала) |
| `PlayOneShot(AudioClip clip, float volumeScale)` | Наложение звука поверх текущего |
| `PlayClipAtPoint(AudioClip clip, Vector3 position, float volume)` | Статический метод для одноразового звука в точке |
| `Stop()` | Остановка воспроизведения | 
| `Pause()` | Пауза с возможностью продолжить |
| `UnPause()` | Снятие паузы |

---

## 3. 2D vs 3D звук
Ключевое различие между 2D и 3D звуком — учёт позиции источника относительно слушателя.

### 🖥️ 2D Sound (Плоский звук)
- Не зависит от позиции в пространстве
- Громкость и панорама не меняются при движении
- Используется для: UI-звуки, фоновая музыка, голос рассказчика, системные сигналы

Настройка:
- `Spatial Blend = 0`
- `Pan` настраивается вручную (или стерео)

### 🌍 3D Sound (Пространственный звук)
- Зависит от расстояния до AudioListener
- Громкость уменьшается с расстоянием (Attenuation / затухание)
- Поддерживает эффект Доплера
- Используется для: шаги, выстрелы, звуки окружения, звуки врагов

Настройка:
- `Spatial Blend = 1`
- Настройка кривой затухания в `AudioSource → 3D Sound Settings`

### 📊 Сравнение свойств:
| Характеристика | 2D Sound | 3D Sound |
| --- | --- | --- |
| `Spatial Blend` | 0 | 1 |
| Зависимость от расстояния | Нет | Да |
| Эффект Доплера | Нет | Да (опционально) |
| Панорамирование | Стерео/моно в микшере | Автоматическое на основе позиции |
| Производительность | Высокая (меньше расчётов) | Ниже (требует позиционирования) |

### 📝 Пример переключения режимов:
```csharp
public class SoundModeSwitcher : MonoBehaviour
{
    private AudioSource audioSource;
    
    [SerializeField] private AudioClip soundClip;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = soundClip;
        audioSource.loop = true;
        audioSource.Play();
    }
    
    void Update()
    {
        // Нажатие 1: 2D режим
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            audioSource.spatialBlend = 0f;
            Debug.Log("Режим: 2D Sound (позиция не влияет)");
        }
        
        // Нажатие 2: 3D режим
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            audioSource.spatialBlend = 1f;
            Debug.Log("Режим: 3D Sound (громкость зависит от расстояния)");
        }
        
        // Нажатие 3: Смешанный режим (полу-3D)
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            audioSource.spatialBlend = 0.5f;
            Debug.Log("Режим: Mixed (50% позиционирования)");
        }
    }
}
```

---

## 4. Позиционирование звука в 3D пространстве
Для 3D звука критически важно, где находится AudioSource относительно AudioListener. 
Unity автоматически вычисляет громкость, панораму и задержку на основе расстояния и направления.

### 📍 Настройки 3D звука (AudioSource → 3D Sound Settings):
| Параметр | Описание | Рекомендации |
| --- | --- | --- |
| `Volume Rolloff` | Форма кривой затухания громкости | `Logarithmic` (реалистично), `Linear` (просто) |
| `Min Distance` | Расстояние, на котором громкость максимальна | 1-5 единиц (для маленьких объектов) |
| `Max Distance` | Расстояние, на котором звук полностью затихает | 20-500 единиц (зависит от сцены) |
| `Spread` | Угол распространения звука (степени) | 0 = точечный, 360 = всенаправленный |
| `Doppler Level` | Интенсивность эффекта Доплера | 1 = норма, 0 = выкл |

### 📈 Типы кривых затухания (Rolloff):
1. Logarithmic Rolloff — наиболее реалистичный (звук затихает быстро вблизи, медленно вдали)
2. Linear Rolloff — линейное уменьшение громкости
3. Custom Rolloff — ручная настройка кривой через Animation Curve

### 📝 Пример динамического позиционирования:
```csharp
using UnityEngine;

public class MovingSoundSource : MonoBehaviour
{
    private AudioSource audioSource;
    private Transform playerTransform;
    
    [SerializeField] private float moveRadius = 10f;
    [SerializeField] private float moveSpeed = 2f;
    
    private Vector3 startPosition;
    private float angle = 0f;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        startPosition = transform.position;
        
        // Находим игрока (или объект с AudioListener)
        playerTransform = Camera.main.transform;
        
        // Настройка 3D звука
        audioSource.spatialBlend = 1f;
        audioSource.minDistance = 2f;
        audioSource.maxDistance = 30f;
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        audioSource.dopplerLevel = 1f;
        
        audioSource.Play();
    }
    
    void Update()
    {
        // Двигаем источник звука по кругу
        angle += moveSpeed * Time.deltaTime;
        float x = Mathf.Cos(angle) * moveRadius;
        float z = Mathf.Sin(angle) * moveRadius;
        transform.position = startPosition + new Vector3(x, 0, z);
        
        // Расчёт расстояния до слушателя
        float distanceToListener = Vector3.Distance(transform.position, playerTransform.position);
        Debug.Log($"Расстояние до слушателя: {distanceToListener:F2} | Громкость: {audioSource.volume:F2}");
    }
}
```

### 🧪 Эффект Доплера (Doppler Effect):
Эффект Доплера изменяет высоту тона в зависимости от относительной скорости источника и слушателя.
```csharp
public class DopplerEffectDemo : MonoBehaviour
{
    private AudioSource audioSource;
    private Rigidbody rb;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        
        audioSource.spatialBlend = 1f;
        audioSource.dopplerLevel = 1f; // Включён эффект Доплера
        
        // Добавляем скорость объекту
        rb.velocity = new Vector3(20f, 0, 0); // Быстро движется вправо
    }
    
    void Update()
    {
        // Отображение текущего питча (высоты тона)
        Debug.Log($"Pitch: {audioSource.pitch:F2} (влияет эффект Доплера)");
    }
}
```

---

## 5. Пространственные эффекты и AudioMixer
Для более продвинутой работы со звуком используйте AudioMixer — он позволяет создавать группы, добавлять эффекты (реверберация, эхо, задержка) и управлять ими программно.

### 🎛️ Пример настройки AudioMixer:
```csharp
using UnityEngine;
using UnityEngine.Audio;

public class AudioMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer masterMixer;
    
    public void SetMasterVolume(float volume)
    {
        // volume от 0 до 1 преобразуем в децибелы (от -80 до 0)
        float dB = Mathf.Log10(Mathf.Max(0.0001f, volume)) * 20f;
        masterMixer.SetFloat("MasterVolume", dB);
    }
    
    public void SetSFXVolume(float volume)
    {
        float dB = Mathf.Log10(Mathf.Max(0.0001f, volume)) * 20f;
        masterMixer.SetFloat("SFXVolume", dB);
    }
    
    public void SetMusicVolume(float volume)
    {
        float dB = Mathf.Log10(Mathf.Max(0.0001f, volume)) * 20f;
        masterMixer.SetFloat("MusicVolume", dB);
    }
}
```

---

## 6. Практический пример: Система шагов с учётом поверхности
```csharp
using UnityEngine;

public class FootstepSystem : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource footstepSource;
    
    [Header("Footstep Sounds")]
    [SerializeField] private AudioClip[] concreteSteps;
    [SerializeField] private AudioClip[] grassSteps;
    [SerializeField] private AudioClip[] woodSteps;
    
    [Header("Settings")]
    [SerializeField] private float stepInterval = 0.5f;
    [SerializeField] private LayerMask groundLayer;
    
    private float stepTimer = 0f;
    private bool isMoving = false;
    private CharacterController characterController;
    
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        
        // Настройка 3D звука для шагов
        footstepSource.spatialBlend = 1f;
        footstepSource.minDistance = 1f;
        footstepSource.maxDistance = 15f;
        footstepSource.rolloffMode = AudioRolloffMode.Logarithmic;
        footstepSource.dopplerLevel = 0.1f; // Небольшой эффект для реализма
    }
    
    void Update()
    {
        // Проверяем, движется ли персонаж
        isMoving = characterController.velocity.magnitude > 0.1f;
        
        if (isMoving)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                PlayFootstep();
                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }
    
    private void PlayFootstep()
    {
        // Определяем поверхность под ногами
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.5f, groundLayer))
        {
            string surfaceTag = hit.collider.tag;
            AudioClip[] selectedClips = GetClipsForSurface(surfaceTag);
            
            if (selectedClips != null && selectedClips.Length > 0)
            {
                AudioClip clip = selectedClips[Random.Range(0, selectedClips.Length)];
                footstepSource.PlayOneShot(clip, Random.Range(0.8f, 1.2f)); // Случайная громкость
            }
        }
    }
    
    private AudioClip[] GetClipsForSurface(string tag)
    {
        switch (tag)
        {
            case "Concrete": return concreteSteps;
            case "Grass": return grassSteps;
            case "Wood": return woodSteps;
            default: return concreteSteps;
        }
    }
}
```

---

## 7. Частые ошибки и советы

### ❌ Частые ошибки:
1. Несколько AudioListener в сцене → предупреждение и возможные артефакты
2. Забыли включить `Play On Awake` → звук не играет при запуске
3. Spatial Blend = 0, но ожидали 3D эффектов → звук будет плоским
4. Слишком маленький `Max Distance` → звук резко обрывается
5. Слишком громкий 3D звук вблизи → искажения (клиппинг)

### ✅ Рекомендации:
1. Всегда проверяйте, что в сцене ровно один активный `AudioListener`
2. Используйте `AudioMixer` для глобального управления громкостью
3. Для одноразовых звуков применяйте `PlayOneShot` вместо создания новых `AudioSource`
4. Настраивайте `Min Distance` и `Max Distance` под размеры вашего мира
5. Предзагружайте `AudioClip` в `Start()` для избежания задержек при первом воспроизведении
6. Для фоновой музыки используйте отдельный `AudioSource` с `Spatial Blend = 0`

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
