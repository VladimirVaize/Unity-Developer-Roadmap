# 🎵 Адаптивная музыка в Unity: Переключение между миксами, динамические изменения в зависимости от игровой ситуации
Адаптивная музыка (Adaptive Music) — это техника, при которой музыкальное сопровождение меняется в реальном времени в ответ на действия игрока, состояние игры или окружение. 
В Unity это реализуется через комбинацию AudioMixer Snapshots, переключения треков, наложения слоёв и управления параметрами.

---

## 1. Основные подходы к адаптивной музыке
| Подход | Описание | Когда использовать |
| --- | --- | --- |
| Переключение снапшотов (Snapshots) | Изменение громкости, панорамы, эффектов | Смена настроения (спокойное → боевое) |
| Переключение треков | Полная замена текущей музыки | Переход между уровнями, зонами |
| Горизонтальное секвенирование | Перемещение по дорожке (нелинейное воспроизведение) | Открытый мир, исследование | 
| Вертикальное наложение (Layering) | Добавление/удаление инструментов | Нарастание интенсивности |
| Стемминг (Stem Mixing) | Отдельные дорожки (ударные, бас, мелодия) | Максимальная гибкость |

---

## 2. Вертикальное наложение (Vertical Layering) — самый популярный метод
Вертикальное наложение добавляет или убирает музыкальные слои (layers/stems) в зависимости от игровой ситуации.
### 🧱 Структура стемов:
```text
Боевая тема 🎸
├── Слой 1: Бас-гитара (постоянно)
├── Слой 2: Ударные (в бою)
├── Слой 3: Ритм-гитара (при 50% здоровья врага)
├── Слой 4: Соло-гитара (при критическом здоровье игрока)
└── Слой 5: Хор (при победе)
```

### 🎮 Пример реализации вертикального наложения:
```csharp
using UnityEngine;
using UnityEngine.Audio;

public class VerticalMusicSystem : MonoBehaviour
{
    [Header("Audio Sources для каждого слоя")]
    public AudioSource bassLayer;
    public AudioSource drumsLayer;
    public AudioSource rhythmLayer;
    public AudioSource soloLayer;
    
    [Header("Параметры")]
    public float fadeTime = 1.5f;
    
    private bool isInCombat = false;
    private float enemyHealthPercent = 1f;
    private float playerHealthPercent = 1f;
    
    void Update()
    {
        if (isInCombat)
        {
            // Бас всегда включён в бою
            FadeInLayer(bassLayer, fadeTime);
            
            // Ударные включаются в бою
            FadeInLayer(drumsLayer, fadeTime);
            
            // Ритм-гитара при здоровье врага < 50%
            if (enemyHealthPercent < 0.5f)
                FadeInLayer(rhythmLayer, fadeTime);
            else
                FadeOutLayer(rhythmLayer, fadeTime);
            
            // Соло при критическом здоровье игрока (< 20%)
            if (playerHealthPercent < 0.2f)
                FadeInLayer(soloLayer, 0.5f);
            else
                FadeOutLayer(soloLayer, fadeTime);
        }
        else
        {
            // Вне боя всё выключаем
            FadeOutLayer(bassLayer, fadeTime);
            FadeOutLayer(drumsLayer, fadeTime);
            FadeOutLayer(rhythmLayer, fadeTime);
            FadeOutLayer(soloLayer, fadeTime);
        }
    }
    
    private void FadeInLayer(AudioSource source, float duration)
    {
        if (!source.isPlaying) source.Play();
        StartCoroutine(FadeVolume(source, source.volume, 1f, duration));
    }
    
    private void FadeOutLayer(AudioSource source, float duration)
    {
        if (source.isPlaying)
            StartCoroutine(FadeVolume(source, source.volume, 0f, duration, () => source.Stop()));
    }
    
    private System.Collections.IEnumerator FadeVolume(AudioSource source, float start, float end, float duration, System.Action onComplete = null)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }
        source.volume = end;
        onComplete?.Invoke();
    }
    
    // Публичные методы для вызова из игры
    public void StartCombat() => isInCombat = true;
    public void EndCombat() => isInCombat = false;
    public void UpdateEnemyHealth(float percent) => enemyHealthPercent = percent;
    public void UpdatePlayerHealth(float percent) => playerHealthPercent = percent;
}
```

---

## 3. Переключение снапшотов AudioMixer
Снапшоты позволяют менять не только громкость, но и эффекты (реверберация, эквалайзер) для всей музыки.

### 🎛️ Пример: музыка становится "эпичнее" в бою
```csharp
public class MusicSnapshotManager : MonoBehaviour
{
    public AudioMixer musicMixer;
    public AudioMixerSnapshot normalSnapshot;
    public AudioMixerSnapshot combatSnapshot;
    public AudioMixerSnapshot lowHealthSnapshot;
    public AudioMixerSnapshot bossSnapshot;
    
    public float transitionTime = 1f;
    
    public void SetNormalMusic()
    {
        normalSnapshot.TransitionTo(transitionTime);
        Debug.Log("Music: Normal mode");
    }
    
    public void SetCombatMusic()
    {
        combatSnapshot.TransitionTo(transitionTime);
        Debug.Log("Music: Combat mode");
    }
    
    public void SetLowHealthMusic()
    {
        lowHealthSnapshot.TransitionTo(0.5f);
        Debug.Log("Music: Low health - tension!");
    }
    
    public void SetBossMusic()
    {
        bossSnapshot.TransitionTo(2f);
        Debug.Log("Music: Boss battle - epic!");
    }
}
```

### 🔧 Настройка снапшотов в AudioMixer:
| Snapshot | Music Volume | Reverb | Lowpass | Применение |
| --- | --- | --- | --- | --- |
| Normal | 0 dB | 0% | 22000 Hz | Обычное исследование |
| Combat | +3 dB | 20% | 22000 Hz | Бой |
| LowHealth | -2 dB | 40% | 8000 Hz | Напряжение |
| Boss | +6 dB | 50% | 22000 Hz | Битва с боссом | 

---

## 4. Переключение музыкальных треков (Horizontal Sequencing)
Когда нужно полностью сменить музыку (например, при переходе между уровнями).
```csharp
public class MusicTrackManager : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioClip explorationTrack;
    public AudioClip combatTrack;
    public AudioClip bossTrack;
    public AudioClip victoryTrack;
    
    private AudioClip currentTrack;
    private float crossfadeTime = 2f;
    
    public void PlayTrack(AudioClip newTrack)
    {
        if (currentTrack == newTrack) return;
        
        currentTrack = newTrack;
        StartCoroutine(CrossfadeMusic(newTrack));
    }
    
    private System.Collections.IEnumerator CrossfadeMusic(AudioClip newClip)
    {
        float elapsed = 0;
        float startVolume = musicSource.volume;
        
        // Затухание текущей музыки
        while (elapsed < crossfadeTime / 2)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0, elapsed / (crossfadeTime / 2));
            yield return null;
        }
        
        // Смена трека
        musicSource.clip = newClip;
        musicSource.Play();
        
        elapsed = 0;
        // Нарастание новой музыки
        while (elapsed < crossfadeTime / 2)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0, startVolume, elapsed / (crossfadeTime / 2));
            yield return null;
        }
        musicSource.volume = startVolume;
    }
    
    // Примеры вызовов
    public void EnterCombat() => PlayTrack(combatTrack);
    public void EnterBoss() => PlayTrack(bossTrack);
    public void Victory() => PlayTrack(victoryTrack);
}
```

---

## 5. Горизонтальное секвенирование (Non-linear Sequencing)
Музыкальная дорожка разбита на сегменты, между которыми можно переключаться.
```csharp
public class HorizontalSequencer : MonoBehaviour
{
    [System.Serializable]
    public class MusicSegment
    {
        public string name;
        public AudioClip clip;
        public float intensity; // 0-1, определяет, когда использовать
    }
    
    public AudioSource sequencerSource;
    public MusicSegment[] segments;
    private MusicSegment currentSegment;
    
    public void UpdateIntensity(float intensity)
    {
        // Ищем сегмент с подходящей интенсивностью
        MusicSegment bestMatch = null;
        float bestDiff = float.MaxValue;
        
        foreach (var segment in segments)
        {
            float diff = Mathf.Abs(segment.intensity - intensity);
            if (diff < bestDiff)
            {
                bestDiff = diff;
                bestMatch = segment;
            }
        }
        
        if (bestMatch != null && bestMatch != currentSegment)
        {
            currentSegment = bestMatch;
            StartCoroutine(TransitionToSegment(bestMatch));
        }
    }
    
    private System.Collections.IEnumerator TransitionToSegment(MusicSegment segment)
    {
        // Затухание
        float elapsed = 0;
        float startVol = sequencerSource.volume;
        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;
            sequencerSource.volume = Mathf.Lerp(startVol, 0, elapsed / 0.5f);
            yield return null;
        }
        
        // Смена клипа
        sequencerSource.clip = segment.clip;
        sequencerSource.Play();
        
        // Нарастание
        elapsed = 0;
        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;
            sequencerSource.volume = Mathf.Lerp(0, startVol, elapsed / 0.5f);
            yield return null;
        }
        sequencerSource.volume = startVol;
    }
}
```

---

## 6. Динамические изменения в зависимости от игровых параметров
### 🏃‍♂️ Пример 1: Музыка ускоряется при спринте
```csharp
public class MusicSpeedSystem : MonoBehaviour
{
    public AudioSource musicSource;
    public float basePitch = 1f;
    public float sprintPitch = 1.3f;
    public float transitionSpeed = 2f;
    
    private float targetPitch;
    
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetAxis("Vertical") > 0)
            targetPitch = sprintPitch;
        else
            targetPitch = basePitch;
        
        musicSource.pitch = Mathf.Lerp(musicSource.pitch, targetPitch, Time.deltaTime * transitionSpeed);
    }
}
```

### 🧠 Пример 2: Интенсивность = (Скорость * Опасность)
```csharp
public class AdaptiveIntensitySystem : MonoBehaviour
{
    [Header("Music Layers")]
    public AudioSource ambientLayer;
    public AudioSource tensionLayer;
    public AudioSource actionLayer;
    
    [Header("Game Parameters")]
    public FloatVariable playerSpeed;
    public FloatVariable enemyProximity;
    public BoolVariable isInCombat;
    
    void Update()
    {
        float intensity = CalculateIntensity();
        
        // ambient: всегда включён
        if (!ambientLayer.isPlaying) ambientLayer.Play();
        
        // tension: включается при средней интенсивности
        SetLayerVolume(tensionLayer, Mathf.Clamp01(intensity - 0.3f) / 0.7f);
        
        // action: включается при высокой интенсивности
        SetLayerVolume(actionLayer, Mathf.Clamp01(intensity - 0.7f) / 0.3f);
    }
    
    private float CalculateIntensity()
    {
        float speedFactor = playerSpeed.Value / 10f;      // 0-1
        float dangerFactor = isInCombat.Value ? 0.7f : Mathf.Clamp01(1 - enemyProximity.Value / 20f);
        
        return Mathf.Clamp01((speedFactor + dangerFactor) / 2f);
    }
    
    private void SetLayerVolume(AudioSource source, float targetVolume)
    {
        source.volume = Mathf.Lerp(source.volume, targetVolume, Time.deltaTime * 3f);
        if (targetVolume > 0.05f && !source.isPlaying) source.Play();
        else if (targetVolume <= 0.05f && source.isPlaying && source.volume < 0.01f) source.Stop();
    }
}
```

---

## 7. Полный пример: Адаптивная музыка для RPG
```csharp
using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class FullAdaptiveMusicSystem : MonoBehaviour
{
    [System.Serializable]
    public class MusicState
    {
        public string stateName;
        public AudioMixerSnapshot snapshot;
        public AudioClip[] tracks;
        public float intensity;
        public float transitionTime = 1f;
    }
    
    [Header("Mixer")]
    public AudioMixer audioMixer;
    
    [Header("States")]
    public MusicState explorationState;
    public MusicState combatState;
    public MusicState bossState;
    public MusicState victoryState;
    
    [Header("Layers (для вертикального наложения)")]
    public AudioSource melodyLayer;
    public AudioSource bassLayer;
    public AudioSource drumsLayer;
    public AudioSource stringsLayer;
    
    [Header("Parameters")]
    public float dangerLevel = 0f;        // 0-1
    public float victoryTimer = 0f;
    
    private MusicState currentState;
    private AudioSource currentMusicSource;
    private int currentTrackIndex = 0;
    
    void Start()
    {
        currentMusicSource = gameObject.AddComponent<AudioSource>();
        currentMusicSource.loop = true;
        currentMusicSource.playOnAwake = false;
        
        SetState(explorationState);
    }
    
    void Update()
    {
        UpdateVerticalLayers();
        
        if (victoryTimer > 0)
        {
            victoryTimer -= Time.deltaTime;
            if (victoryTimer <= 0 && currentState != explorationState)
                SetState(explorationState);
        }
    }
    
    private void UpdateVerticalLayers()
    {
        // Интенсивность боя влияет на слои
        float targetMelody = Mathf.Clamp01(dangerLevel);
        float targetBass = Mathf.Clamp01(dangerLevel * 1.2f);
        float targetDrums = Mathf.Clamp01(dangerLevel * 1.5f);
        float targetStrings = Mathf.Clamp01(dangerLevel - 0.5f) / 0.5f;
        
        SetLayerVolume(melodyLayer, targetMelody);
        SetLayerVolume(bassLayer, targetBass);
        SetLayerVolume(drumsLayer, targetDrums);
        SetLayerVolume(stringsLayer, targetStrings);
    }
    
    private void SetLayerVolume(AudioSource source, float target)
    {
        source.volume = Mathf.Lerp(source.volume, target, Time.deltaTime * 4f);
        if (target > 0.05f && !source.isPlaying) source.Play();
        else if (target <= 0.05f && source.isPlaying && source.volume < 0.01f) source.Stop();
    }
    
    public void SetState(MusicState newState)
    {
        if (currentState == newState) return;
        
        currentState = newState;
        
        // Переключаем снапшот
        newState.snapshot.TransitionTo(newState.transitionTime);
        
        // Переключаем трек
        if (newState.tracks.Length > 0)
        {
            currentTrackIndex = 0;
            StartCoroutine(TransitionToNewTrack(newState.tracks[currentTrackIndex]));
        }
        
        Debug.Log($"Music state changed to: {newState.stateName}");
    }
    
    private IEnumerator TransitionToNewTrack(AudioClip newClip)
    {
        float fadeTime = 1f;
        float elapsed = 0;
        float startVol = currentMusicSource.volume;
        
        // Fade out
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            currentMusicSource.volume = Mathf.Lerp(startVol, 0, elapsed / fadeTime);
            yield return null;
        }
        
        currentMusicSource.clip = newClip;
        currentMusicSource.Play();
        
        // Fade in
        elapsed = 0;
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            currentMusicSource.volume = Mathf.Lerp(0, startVol, elapsed / fadeTime);
            yield return null;
        }
        currentMusicSource.volume = startVol;
    }
    
    // Публичные методы для вызова из игры
    public void StartCombat(float danger = 0.7f)
    {
        dangerLevel = danger;
        SetState(combatState);
    }
    
    public void StartBossFight()
    {
        dangerLevel = 1f;
        SetState(bossState);
    }
    
    public void Victory()
    {
        victoryTimer = 5f;
        SetState(victoryState);
    }
    
    public void UpdateDanger(float newDanger)
    {
        dangerLevel = Mathf.Clamp01(newDanger);
        
        // Если в бою, обновляем интенсивность слоёв
        if (currentState == combatState || currentState == bossState)
            UpdateVerticalLayers();
    }
}
```

---

## 8. Интеграция с игровыми событиями
```csharp
public class GameEventListener : MonoBehaviour
{
    public FullAdaptiveMusicSystem musicSystem;
    
    private void OnEnable()
    {
        // Подписка на игровые события
        EventManager.OnCombatStart += () => musicSystem.StartCombat(0.7f);
        EventManager.OnCombatEnd += () => musicSystem.UpdateDanger(0f);
        EventManager.OnBossSpawn += () => musicSystem.StartBossFight();
        EventManager.OnPlayerLowHealth += () => musicSystem.UpdateDanger(0.9f);
        EventManager.OnBossDefeated += () => musicSystem.Victory();
    }
}
```

---

## 9. Лучшие практики
### ✅ Рекомендации:
1. Плавные переходы — всегда используйте затухания (crossfade), не обрезайте музыку резко
2. Синхронизация по тактам — для профессионального звучания синхронизируйте переходы с музыкальным тактом
3. Не перегружайте слоями — 3-5 слоёв достаточно для большинства ситуаций
4. Предзагружайте треки — используйте `AudioClip.LoadAudioData()` для предотвращения задержек
5. Тестируйте все переходы — убедитесь, что музыка не "конфликтует" сама с собой

### ❌ Частые ошибки:
```csharp
// ❌ ОШИБКА: Резкая смена громкости
musicSource.volume = 0; // Щелчок!

// ✅ ПРАВИЛЬНО: Плавное затухание
StartCoroutine(FadeVolume(musicSource, 1f, 0f, 0.5f));

// ❌ ОШИБКА: Несколько AudioSource играют одну и ту же музыку
// ✅ ПРАВИЛЬНО: Используйте один источник и переключайте clip

// ❌ ОШИБКА: Слишком много снапшотов (10+)
// ✅ ПРАВИЛЬНО: 3-5 основных состояний + параметры громкости
```

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
