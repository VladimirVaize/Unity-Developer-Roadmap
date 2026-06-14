# 🎵 Adaptive Music in Unity: Switching Between Mixes, Dynamic Changes Based on Game Situation
Adaptive Music is a technique where the musical accompaniment changes in real time in response to player actions, game state, or environment. 
In Unity, this is implemented through a combination of AudioMixer Snapshots, track switching, layering, and parameter control.

---

## 1. Main Approaches to Adaptive Music
| Approach | Description | When to Use |
| --- | --- | --- |
| Snapshot Switching | Changing volume, pan, effects | Mood change (calm → combat) |
| Track Switching | Complete music replacement | Level/zone transitions |
| Horizontal Sequencing | Non-linear playback | Open world, exploration |
| Vertical Layering | Adding/removing instruments | Intensity ramping |
| Stem Mixing | Separate tracks (drums, bass, melody) | Maximum flexibility |

---

## 2. Vertical Layering — The Most Popular Method
Vertical layering adds or removes musical layers/stems based on game situation.

### 🧱 Stem Structure:
```text
Combat Theme 🎸
├── Layer 1: Bass Guitar (always)
├── Layer 2: Drums (in combat)
├── Layer 3: Rhythm Guitar (enemy health < 50%)
├── Layer 4: Lead Guitar (player critical health)
└── Layer 5: Choir (on victory)
```

### 🎮 Implementation Example:
```csharp
using UnityEngine;
using UnityEngine.Audio;

public class VerticalMusicSystem : MonoBehaviour
{
    [Header("Audio Sources for each layer")]
    public AudioSource bassLayer;
    public AudioSource drumsLayer;
    public AudioSource rhythmLayer;
    public AudioSource soloLayer;
    
    [Header("Parameters")]
    public float fadeTime = 1.5f;
    
    private bool isInCombat = false;
    private float enemyHealthPercent = 1f;
    private float playerHealthPercent = 1f;
    
    void Update()
    {
        if (isInCombat)
        {
            FadeInLayer(bassLayer, fadeTime);
            FadeInLayer(drumsLayer, fadeTime);
            
            if (enemyHealthPercent < 0.5f)
                FadeInLayer(rhythmLayer, fadeTime);
            else
                FadeOutLayer(rhythmLayer, fadeTime);
            
            if (playerHealthPercent < 0.2f)
                FadeInLayer(soloLayer, 0.5f);
            else
                FadeOutLayer(soloLayer, fadeTime);
        }
        else
        {
            FadeOutLayer(bassLayer, fadeTime);
            FadeOutLayer(drumsLayer, fadeTime);
            FadeOutLayer(rhythmLayer, fadeTime);
            FadeOutLayer(soloLayer, fadeTime);
        }
    }
    
    private void FadeInLayer(AudioSource source, float duration) { /* ... */ }
    private void FadeOutLayer(AudioSource source, float duration) { /* ... */ }
    
    public void StartCombat() => isInCombat = true;
    public void EndCombat() => isInCombat = false;
    public void UpdateEnemyHealth(float percent) => enemyHealthPercent = percent;
    public void UpdatePlayerHealth(float percent) => playerHealthPercent = percent;
}
```

---

## 3. Switching AudioMixer Snapshots
Snapshots allow changing not only volume but also effects for all music.

### 🎛️ Example: Music becomes "epic" in combat
```csharp
public class MusicSnapshotManager : MonoBehaviour
{
    public AudioMixer musicMixer;
    public AudioMixerSnapshot normalSnapshot;
    public AudioMixerSnapshot combatSnapshot;
    public AudioMixerSnapshot lowHealthSnapshot;
    public AudioMixerSnapshot bossSnapshot;
    
    public float transitionTime = 1f;
    
    public void SetNormalMusic() => normalSnapshot.TransitionTo(transitionTime);
    public void SetCombatMusic() => combatSnapshot.TransitionTo(transitionTime);
    public void SetLowHealthMusic() => lowHealthSnapshot.TransitionTo(0.5f);
    public void SetBossMusic() => bossSnapshot.TransitionTo(2f);
}
```

---

## 4. Switching Music Tracks (Horizontal Sequencing)
When you need to completely change the music (e.g., level transitions).
```csharp
public class MusicTrackManager : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioClip explorationTrack;
    public AudioClip combatTrack;
    public AudioClip bossTrack;
    
    public void PlayTrack(AudioClip newTrack)
    {
        StartCoroutine(CrossfadeMusic(newTrack));
    }
    
    private IEnumerator CrossfadeMusic(AudioClip newClip) { /* ... */ }
}
```

---

## 5. Horizontal Sequencing (Non-linear)
The music track is broken into segments that can be switched between.
```csharp
public class HorizontalSequencer : MonoBehaviour
{
    [System.Serializable]
    public class MusicSegment
    {
        public string name;
        public AudioClip clip;
        public float intensity;
    }
    
    public void UpdateIntensity(float intensity)
    {
        MusicSegment bestMatch = FindBestSegment(intensity);
        if (bestMatch != currentSegment)
            TransitionToSegment(bestMatch);
    }
}
```

---

## 6. Dynamic Changes Based on Game Parameters
### 🏃‍♂️ Example 1: Music speeds up when sprinting
```csharp
public class MusicSpeedSystem : MonoBehaviour
{
    public AudioSource musicSource;
    public float basePitch = 1f;
    public float sprintPitch = 1.3f;
    
    void Update()
    {
        float targetPitch = Input.GetKey(KeyCode.LeftShift) ? sprintPitch : basePitch;
        musicSource.pitch = Mathf.Lerp(musicSource.pitch, targetPitch, Time.deltaTime * 2f);
    }
}
```

### 🧠 Example 2: Intensity = (Speed * Danger)
```csharp
public class AdaptiveIntensitySystem : MonoBehaviour
{
    public AudioSource ambientLayer;
    public AudioSource tensionLayer;
    public AudioSource actionLayer;
    
    void Update()
    {
        float intensity = CalculateIntensity();
        SetLayerVolume(ambientLayer, 1f);
        SetLayerVolume(tensionLayer, Mathf.Clamp01(intensity - 0.3f) / 0.7f);
        SetLayerVolume(actionLayer, Mathf.Clamp01(intensity - 0.7f) / 0.3f);
    }
}
```

---

## 7. Complete Example: Adaptive Music for RPG
```csharp
public class FullAdaptiveMusicSystem : MonoBehaviour
{
    [System.Serializable]
    public class MusicState
    {
        public string stateName;
        public AudioMixerSnapshot snapshot;
        public AudioClip[] tracks;
        public float transitionTime = 1f;
    }
    
    public MusicState explorationState;
    public MusicState combatState;
    public MusicState bossState;
    
    public void SetState(MusicState newState) { /* ... */ }
    public void StartCombat(float danger = 0.7f) { /* ... */ }
    public void StartBossFight() { /* ... */ }
    public void Victory() { /* ... */ }
}
```

---

## 8. Integration with Game Events
```csharp
public class GameEventListener : MonoBehaviour
{
    public FullAdaptiveMusicSystem musicSystem;
    
    private void OnEnable()
    {
        EventManager.OnCombatStart += () => musicSystem.StartCombat(0.7f);
        EventManager.OnBossDefeated += () => musicSystem.Victory();
    }
}
```

---

## 9. Best Practices
### ✅ Recommendations:
1. Smooth transitions — always use crossfades, don't cut music abruptly
2. Beat-synced transitions — for professional sound, sync with musical bars
3. Don't over-layer — 3-5 layers are enough for most situations
4. Preload tracks — use `AudioClip.LoadAudioData()` to prevent delays
5. Test all transitions — ensure music doesn't conflict with itself

### ❌ Common Mistakes:
```csharp
// ❌ ERROR: Abrupt volume change
musicSource.volume = 0; // Click!

// ✅ CORRECT: Smooth fade
StartCoroutine(FadeVolume(musicSource, 1f, 0f, 0.5f));

// ❌ ERROR: Too many snapshots (10+)
// ✅ CORRECT: 3-5 main states + volume parameters
```

---

### ⭐ If this project was useful, put a star on GitHub!
