# 🎧 AudioMixer in Unity: Groups, Effects (Reverb, Echo), Snapshots for Dynamic Mixing
AudioMixer is Unity's powerful tool for professional audio management. 
It allows routing audio streams, applying real-time effects, and dynamically changing mixing parameters through snapshots.

---

## 1. Core Concepts of AudioMixer
| Concept | Description |
| --- | --- |
| Audio Mixer | Asset containing the entire mixing scheme |
| Group | Channel through which sounds pass. Has volume, pan, effects |
| Effect | Audio processing (reverb, echo, filters, compressor, etc.) |
| Send / Receive | Routing mechanism between groups |
| Snapshot | Saved state of all group parameters |
| Exposed Parameter | Parameter accessible from scripts |

### 📁 Creating an AudioMixer
1. Project Window → Create → Audio Mixer
2. Name it, e.g., `MasterMixer`
3. Double-click to open the Audio Mixer window

---

## 2. Audio Mixer Groups
Groups are organized hierarchically. The root group is `Master`, all others are children.

### 🧱 Typical Structure:
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

### 🎮 Assigning a Group in Code:
```csharp
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public AudioMixerGroup sfxGroup;
    public AudioMixerGroup musicGroup;
    
    private AudioSource sfxSource;
    private AudioSource musicSource;
    
    void Start()
    {
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.outputAudioMixerGroup = sfxGroup;
        
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.outputAudioMixerGroup = musicGroup;
        
        PlaySFX();
    }
    
    void PlaySFX()
    {
        sfxSource.clip = Resources.Load<AudioClip>("Sounds/explosion");
        sfxSource.Play();
    }
}
```

### 🎛️ Controlling Group Volume from Code:
```csharp
public class VolumeController : MonoBehaviour
{
    public AudioMixer masterMixer;
    
    public void SetMasterVolume(float volume)
    {
        // volume: from 0.0001 to 1 (logarithmic scale)
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
> In AudioMixer, volume parameters use decibels (dB).
> Conversion: `dB = 20 * log10(linear)`.
> Linear value 1 → 0 dB, 0.5 → -6 dB, 0.1 → -20 dB.

### 🔓 Exposing a Parameter
1. Right-click on a group's Volume slider in the AudioMixer window
2. Select Expose 'Volume' to script
3. In the Exposed Parameters window, give it a name (e.g., `MasterVolume`)
4. Use `mixer.SetFloat("MasterVolume", value)`

---

## 3. Audio Mixer Effects
Unity provides ready-to-use effects that can be added to groups.

### 📋 Available Effects:
| Effect | Description |
| --- | --- |
| Atmospheric Reverb | Reverb for open spaces |
| Reverberation | Classic reverb (caves, halls) |
| Echo | Echo with delay |
| Lowpass Filter | Passes low frequencies ("underwater" sound) |
| Highpass Filter | Passes high frequencies |
| Chorus | Chorus effect (thickens sound) |
| Flanger | "Jet plane" effect |
| Distortion | Distortion (rock/metal guitar) |
| Compressor | Volume leveling |
| ParamEQ | Equalizer |

### 🏗️ Adding an Effect:
1. Select a group in the AudioMixer
2. In the group inspector, click Add Effect
3. Choose the desired effect
4. Adjust parameters in the inspector

### 🌊 Example 1: Reverb
```csharp
public class ReverbZone : MonoBehaviour
{
    public AudioMixerGroup reverbGroup;
    public float reverbTime = 1.5f;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            reverbGroup.audioMixer.SetFloat("ReverbTime", reverbTime);
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            reverbGroup.audioMixer.SetFloat("ReverbTime", 0.5f);
        }
    }
}
```

### 🗣️ Example 2: Echo for Radio Voice
```csharp
public class RadioEffect : MonoBehaviour
{
    public AudioMixerGroup voiceGroup;
    
    public void EnableRadioEffect()
    {
        voiceGroup.audioMixer.SetFloat("EchoDelay", 200f);
        voiceGroup.audioMixer.SetFloat("EchoDecay", 0.5f);
        voiceGroup.audioMixer.SetFloat("EchoWetMix", 0.8f);
    }
    
    public void DisableRadioEffect()
    {
        voiceGroup.audioMixer.SetFloat("EchoWetMix", 0f);
    }
}
```

### 🎛️ Example 3: Dynamic Lowpass Filter (Underwater)
```csharp
public class UnderwaterEffect : MonoBehaviour
{
    public AudioMixer masterMixer;
    private float originalLowpass;
    
    void Start()
    {
        masterMixer.GetFloat("LowpassCutoff", out originalLowpass);
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            masterMixer.SetFloat("LowpassCutoff", 800f);
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            masterMixer.SetFloat("LowpassCutoff", originalLowpass);
        }
    }
}
```

### 🔄 Example 4: Send/Receive Routing
Allows sending audio from one group to another without copying.

Setup:
1. Add a Send effect to the source group
2. Add a Receive effect to the target group
3. In Send, select the Receive

---

## 4. Snapshots for Dynamic Mixing
A Snapshot is a capture of ALL parameter states of all groups in the mixer. 
Allows smooth transitions between different audio scenarios.

### 🎬 Typical Snapshots:
| Snapshot | Description |
| --- | --- |
| `Normal` | Normal game state |
| `Paused` | Sounds during pause (muted music) |
| `Combat` | Combat music, emphasis on SFX |
| `Menu` | Calm background music |
| `Dialogue` | Quiet background, loud voice |

### 🛠️ Creating a Snapshot:
1. In AudioMixer window, click Snapshots (top left)
2. Click `+` (Add Snapshot)
3. Give it a name, e.g., `Combat`
4. Change any parameters in groups (volume, effects)
5. The Snapshot saves these changes as a "difference" from the base state

### 🎮 Switching Snapshots from Code:
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

### 🌊 Advanced Example: Dynamic Mixing with Effects
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
            underwaterSnapshot.TransitionTo(transitionTime);
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

---

## 5. Full Example: Dynamic Mixing for RPG
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
        masterMixer.SetFloat("MusicVolume", -3f);
        masterMixer.SetFloat("SFXVolume", 0f);
    }
    
    public void EndCombat()
    {
        isInCombat = false;
        masterMixer.SetFloat("MusicVolume", -10f);
    }
    
    public void EnterStealth()
    {
        isInStealth = true;
        masterMixer.SetFloat("MusicVolume", -15f);
        masterMixer.SetFloat("FootstepVolume", -5f);
    }
    
    public void StartDialogue(AudioSource voiceSource)
    {
        dialogue.TransitionTo(0.3f);
        masterMixer.SetFloat("MusicVolume", -20f);
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
        AudioMixerGroup[] groups = masterMixer.FindMatchingGroups("Voice");
        return groups.Length > 0 ? groups[0] : null;
    }
}
```

---

## 6. Best Practices
### ✅ Recommendations:
1. Use group hierarchy — simplifies management
2. Expose only needed parameters — don't clutter the list
3. Smooth snapshot transitions (0.5-2 sec) — avoid abrupt jumps
4. Test on real devices — sound may differ
5. Use Ducking (Sidechain Compression) — music gets quieter during dialogue

### ⚠️ Common Mistakes:
```csharp
// ❌ ERROR: Direct volume change without conversion
mixer.SetFloat("MasterVolume", 0.5f); // Will be very quiet!

// ✅ CORRECT:
mixer.SetFloat("MasterVolume", Mathf.Log10(0.5f) * 20); // -6 dB

// ❌ ERROR: Instant switching without fading
combatSnapshot.TransitionTo(0f); // Sharp click

// ✅ CORRECT:
combatSnapshot.TransitionTo(0.5f);

// ❌ ERROR: Wrong parameter name
mixer.SetFloat("Music Vol", -10f); // Error!

// ✅ CORRECT: Use exact name from Exposed Parameters
mixer.SetFloat("MusicVolume", -10f);
```

---

### ⭐ If this project was useful, put a star on GitHub!
