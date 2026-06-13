# 🎧 AudioMixer в Unity: Группы каналов, эффекты (реверберация, эхо), снапшоты для динамического микширования
AudioMixer — это мощный инструмент Unity для профессионального управления звуком. 
Он позволяет маршрутизировать аудиопотоки, применять эффекты в реальном времени и 
динамически изменять параметры микширования через снапшоты (snapshots).

---

## 1. Основные понятия AudioMixer
| Понятие | Описание |
| --- | --- |
| Audio Mixer | Ассет, содержащий всю схему микширования |
| Группа (Group) | Канал, через который проходят звуки. Имеет громкость, панораму, эффекты |
| Эффект (Effect) | Обработка звука (реверберация, эхо, фильтры, компрессор и т.д.) |
| Отправка (Send / Receive) | Механизм маршрутизации звука между группами |
| Снапшот (Snapshot) | Сохранённое состояние всех параметров групп |
| Параметр (Exposed Parameter) | Параметр, доступный из скриптов |

### 📁 Создание AudioMixer
1. Project Window → Create → Audio Mixer
2. Назовите его, например, `MasterMixer`
3. Откройте через двойной клик → откроется окно Audio Mixer

---

## 2. Группы каналов (Audio Mixer Groups)
Группы организуются в иерархию. Корневая группа — `Master`, все остальные являются её дочерними.

### 🧱 Типовая структура:
```text
Master 🔊
├── Music 🎵
│   ├── Background
│   └── Combat
├── SFX 🔫
│   ├── Weapons
│   ├── Footsteps
│   └── UI
├── Voice 🗣️
│   ├── Dialogues
│   └── PlayerVoice
└── Ambience 🌲
```

### 🎮 Пример назначения группы в коде:
```csharp
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public AudioMixerGroup sfxGroup;  // Перетащить группу из Mixer
    public AudioMixerGroup musicGroup;
    
    private AudioSource sfxSource;
    private AudioSource musicSource;
    
    void Start()
    {
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.outputAudioMixerGroup = sfxGroup;
        
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.outputAudioMixerGroup = musicGroup;
        
        // Воспроизведение с указанием группы
        PlaySFX();
    }
    
    void PlaySFX()
    {
        sfxSource.clip = Resources.Load<AudioClip>("Sounds/explosion");
        sfxSource.Play();
    }
}
```

### 🎛️ Управление громкостью группы из кода:
```csharp
public class VolumeController : MonoBehaviour
{
    public AudioMixer masterMixer;
    
    public void SetMasterVolume(float volume)
    {
        // volume: от 0.0001 до 1 (логарифмическая шкала)
        masterMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }
    
    public void SetMusicVolume(float volume)
    {
        masterMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }
    
    public void SetSFXVolume(float volume)
    {
        masterMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }
}
```

> [!Important]
> В AudioMixer параметры громкости используют децибелы (dB).
> Преобразование: `dB = 20 * log10(linear)`.
> Линейное значение 1 → 0 dB, 0.5 → -6 dB, 0.1 → -20 dB.

### 🔓 Экспонирование параметра (Expose Parameter)
1. В окне AudioMixer кликнуть ПКМ на ползунке Volume группы
2. Выбрать Expose 'Volume' to script
3. В окне Exposed Parameters задать имя (например, `MasterVolume`)
4. Использовать `mixer.SetFloat("MasterVolume", value)`

---

## 3. Эффекты (Audio Mixer Effects)
Unity предоставляет готовые эффекты, которые можно добавлять в группы.

### 📋 Доступные эффекты:
| Эффект | Описание |
| --- | --- |
| Atmospheric Reverb | Реверберация для открытых пространств |
| Reverberation | Классическая реверберация (пещеры, залы) |
| Echo | Эхо с задержкой |
| Lowpass Filter | Пропускает низкие частоты (звук "под водой") |
| Highpass Filter | Пропускает высокие частоты |
| Chorus | Эффект хора (утолщение звука) |
| Flanger | "Взлётный" эффект |
| Distortion | Искажение (рок/металл гитара) |
| Compressor | Выравнивание громкости |
| ParamEQ | Эквалайзер |

### 🏗️ Добавление эффекта:
1. Выбрать группу в AudioMixer
2. В инспекторе группы нажать Add Effect
3. Выбрать нужный эффект
4. Настроить параметры в инспекторе

### 🌊 Пример 1: Реверберация (Reverb)
```csharp
// Создаём скрипт, который меняет реверберацию в зависимости от комнаты
public class ReverbZone : MonoBehaviour
{
    public AudioMixerGroup reverbGroup;
    public float reverbTime = 1.5f;   // Время затухания
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Получаем доступ к параметру ReverbTime эффекта
            reverbGroup.audioMixer.SetFloat("ReverbTime", reverbTime);
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Возвращаем стандартное значение
            reverbGroup.audioMixer.SetFloat("ReverbTime", 0.5f);
        }
    }
}
```

### 🗣️ Пример 2: Эхо (Echo) для радио-голоса
```csharp
public class RadioEffect : MonoBehaviour
{
    public AudioMixerGroup voiceGroup;
    
    public void EnableRadioEffect()
    {
        // Включаем эхо-эффект
        voiceGroup.audioMixer.SetFloat("EchoDelay", 200f);    // Задержка в мс
        voiceGroup.audioMixer.SetFloat("EchoDecay", 0.5f);    // Затухание
        voiceGroup.audioMixer.SetFloat("EchoWetMix", 0.8f);   // Громкость эффекта
    }
    
    public void DisableRadioEffect()
    {
        voiceGroup.audioMixer.SetFloat("EchoWetMix", 0f);     // Выключаем эффект
    }
}
```

### 🎛️ Пример 3: Динамический Lowpass Filter (погружение под воду)
```csharp
public class UnderwaterEffect : MonoBehaviour
{
    public AudioMixer masterMixer;
    private float originalLowpass;
    
    void Start()
    {
        // Сохраняем исходное значение
        masterMixer.GetFloat("LowpassCutoff", out originalLowpass);
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Устанавливаем частоту среза Lowpass (низкие частоты)
            masterMixer.SetFloat("LowpassCutoff", 800f);  // 800 Гц
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Возвращаем исходное значение (обычно 22000 Гц)
            masterMixer.SetFloat("LowpassCutoff", originalLowpass);
        }
    }
}
```

### 🔄 Пример 4: Send/Receive (маршрутизация)
Позволяет отправлять звук из одной группы в другую без копирования.

Настройка:
1. В группе-источнике добавить эффект Send
2. В целевой группе добавить эффект Receive
3. В Send выбрать Receive

---

## 4. Снапшоты (Snapshots) для динамического микширования
Snapshot — это снимок состояния ВСЕХ параметров всех групп в микшере. 
Позволяет плавно переключаться между разными звуковыми сценариями.

### 🎬 Типовые снапшоты:
| Snapshot | Описание |
| --- | --- |
| `Normal` | Обычное состояние игры |
| `Paused` | Звуки при паузе (приглушённая музыка) |
| `Combat` | Боевая музыка, акцент на SFX |
| `Menu` | Спокойная фоновая музыка |
| `Dialogue` | Приглушённый фон, громкий голос |

### 🛠️ Создание снапшота:
1. В окне AudioMixer нажать кнопку Snapshots (вверху слева)
2. Нажать `+` (Add Snapshot)
3. Дать имя, например, `Combat`
4. Изменить любые параметры в группах (громкость, эффекты)
5. Snapshot сохранит эти изменения как "разницу" от базового состояния

### 🎮 Переключение снапшотов из кода:
```csharp
public class GameAudioManager : MonoBehaviour
{
    public AudioMixer masterMixer;
    public AudioMixerSnapshot normalSnapshot;
    public AudioMixerSnapshot combatSnapshot;
    public AudioMixerSnapshot pausedSnapshot;
    public AudioMixerSnapshot menuSnapshot;
    
    public void EnterCombat()
    {
        // Плавный переход за 0.5 секунды
        combatSnapshot.TransitionTo(0.5f);
    }
    
    public void ExitCombat()
    {
        normalSnapshot.TransitionTo(1f);
    }
    
    public void OnPause()
    {
        pausedSnapshot.TransitionTo(0.3f);
    }
    
    public void OnResume()
    {
        normalSnapshot.TransitionTo(0.5f);
    }
}
```

### 🌊 Продвинутый пример: динамическое микширование с эффектами
```csharp
public class AdvancedAudioManager : MonoBehaviour
{
    public AudioMixer mixer;
    public AudioMixerSnapshot underwaterSnapshot;
    public AudioMixerSnapshot caveSnapshot;
    public AudioMixerSnapshot normalSnapshot;
    
    public float transitionTime = 1f;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("UnderwaterTrigger"))
        {
            // Применяем снапшот с подводными эффектами
            underwaterSnapshot.TransitionTo(transitionTime);
            // Дополнительно меняем Lowpass
            mixer.SetFloat("LowpassCutoff", 600f);
        }
        else if (other.CompareTag("CaveTrigger"))
        {
            caveSnapshot.TransitionTo(transitionTime);
            mixer.SetFloat("ReverbTime", 2f);
        }
    }
    
    public void FadeToSnapshot(AudioMixerSnapshot snapshot, float time)
    {
        snapshot.TransitionTo(time);
    }
}
```

### 🎚️ Переход с задержкой и обратным вызовом:
```csharp
using System.Collections;
using UnityEngine;

public class SnapshotWithDelay : MonoBehaviour
{
    public AudioMixerSnapshot combatSnapshot;
    public AudioMixerSnapshot normalSnapshot;
    
    public IEnumerator TransitionWithDelay(AudioMixerSnapshot target, float delay, float transitionTime)
    {
        yield return new WaitForSeconds(delay);
        target.TransitionTo(transitionTime);
    }
    
    public void EnterBossRoom()
    {
        StartCoroutine(TransitionWithDelay(combatSnapshot, 1f, 2f));
    }
}
```

---

## 5. Полный пример: Динамическое микширование для RPG
```csharp
public class RPGSoundManager : MonoBehaviour
{
    [Header("Mixer")]
    public AudioMixer masterMixer;
    
    [Header("Snapshots")]
    public AudioMixerSnapshot exploration;
    public AudioMixerSnapshot combat;
    public AudioMixerSnapshot stealth;
    public AudioMixerSnapshot dialogue;
    
    [Header("Parameters")]
    public float defaultTransition = 0.5f;
    
    private bool isInCombat = false;
    private bool isInStealth = false;
    
    void Update()
    {
        if (isInCombat)
            combat.TransitionTo(defaultTransition);
        else if (isInStealth)
            stealth.TransitionTo(defaultTransition);
        else
            exploration.TransitionTo(defaultTransition);
    }
    
    public void StartCombat()
    {
        isInCombat = true;
        isInStealth = false;
        masterMixer.SetFloat("MusicVolume", -3f);    // Громче музыку
        masterMixer.SetFloat("SFXVolume", 0f);       // SFX на максимум
    }
    
    public void EndCombat()
    {
        isInCombat = false;
        masterMixer.SetFloat("MusicVolume", -10f);   // Тише музыку
    }
    
    public void EnterStealth()
    {
        isInStealth = true;
        masterMixer.SetFloat("MusicVolume", -15f);   // Очень тихая музыка
        masterMixer.SetFloat("FootstepVolume", -5f); // Приглушённые шаги
    }
    
    public void StartDialogue(AudioSource voiceSource)
    {
        dialogue.TransitionTo(0.3f);
        masterMixer.SetFloat("MusicVolume", -20f);   // Музыка на фоне
        masterMixer.SetFloat("AmbienceVolume", -15f);
        voiceSource.outputAudioMixerGroup = GetVoiceGroup();
        voiceSource.Play();
    }
    
    public void EndDialogue()
    {
        exploration.TransitionTo(0.5f);
        masterMixer.SetFloat("MusicVolume", -10f);
        masterMixer.SetFloat("AmbienceVolume", 0f);
    }
    
    private AudioMixerGroup GetVoiceGroup()
    {
        // Получение группы по имени
        AudioMixerGroup[] groups = masterMixer.FindMatchingGroups("Voice");
        return groups.Length > 0 ? groups[0] : null;
    }
}
```

---

## 6. Лучшие практики
### ✅ Рекомендации:
1. Используйте иерархию групп — упрощает управление
2. Экспонируйте только нужные параметры — не засоряйте список
3. Плавные переходы снапшотов (0.5-2 сек) — избегайте резких скачков
4. Тестируйте на реальном устройстве — звук может отличаться
5. Используйте Ducking (Sidechain Compression) — музыка становится тише при диалогах

### ⚠️ Частые ошибки:
```csharp
// ❌ ОШИБКА: Прямое изменение громкости без преобразования
mixer.SetFloat("MasterVolume", 0.5f); // Будет очень тихо!

// ✅ ПРАВИЛЬНО:
mixer.SetFloat("MasterVolume", Mathf.Log10(0.5f) * 20); // -6 dB

// ❌ ОШИБКА: Мгновенное переключение без затухания
combatSnapshot.TransitionTo(0f); // Резкий щелчок

// ✅ ПРАВИЛЬНО:
combatSnapshot.TransitionTo(0.5f);

// ❌ ОШИБКА: Неправильное имя параметра
mixer.SetFloat("Music Vol", -10f); // Ошибка!

// ✅ ПРАВИЛЬНО: Используйте точное имя из Exposed Parameters
mixer.SetFloat("MusicVolume", -10f);
```

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
