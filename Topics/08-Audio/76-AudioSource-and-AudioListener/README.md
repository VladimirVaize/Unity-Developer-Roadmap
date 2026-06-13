# 🔊 AudioSource and AudioListener: Playing Sounds, 2D vs 3D Sound, Spatial Positioning
Sound in Unity is an integral part of immersing the player in the game world. 
Two main components are responsible for sound playback: AudioListener (the player's "ears") and AudioSource (the "sound source"). 
In this guide, we'll cover their setup, differences between 2D and 3D sound, and spatial positioning.

---

## 1. AudioListener — The "Ears" in the Game World
AudioListener is a component that represents the listener in the scene. 
It is usually attached to the Main Camera, since the camera is the player's "eyes" and the listener is their "ears". 
There can be only one active AudioListener in a scene.

### 🎛️ Key AudioListener Properties:
| Property | Description |
| --- | --- |
| `Volume` | Global volume (via `AudioListener.volume`) |
| `Pause` | Pauses all sounds (`AudioListener.pause`) |
| `VelocityUpdateMode` | Velocity update mode for Doppler effect |

### 📝 Example of working with AudioListener:
```csharp
using UnityEngine;

public class GlobalAudioManager : MonoBehaviour
{
    void Start()
    {
        AudioListener.volume = 1f;
        
        AudioListener currentListener = FindObjectOfType<AudioListener>();
        if (currentListener != null)
        {
            Debug.Log("AudioListener found on object: " + currentListener.gameObject.name);
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            AudioListener.volume = AudioListener.volume > 0 ? 0f : 1f;
            Debug.Log($"Global volume: {AudioListener.volume}");
        }
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            AudioListener.pause = !AudioListener.pause;
            Debug.Log(AudioListener.pause ? "Sound paused" : "Sound resumed");
        }
    }
}
```

> [!Important]
> If there's more than one `AudioListener` in the scene, Unity will show a warning. Disable extra ones.

---

## 2. AudioSource — Sound Source
AudioSource is a component that plays audio clips (`AudioClip`). 
It can play both simple 2D sounds (background music) and 3D sounds considering position in space.

### 🎛️ Key AudioSource Properties:
| Property | Description |
| --- | --- |
| `AudioClip` | Reference to the sound file |
| `Output` | Audio mixer group for effects |
| `Mute` | Mute the sound |
| `Loop` | Loop playback |
| `Play On Awake` | Auto-start when created |
| `Volume` | Source volume (0-1) |
| `Pitch` | Pitch (1 = normal, >1 higher, <1 lower) |
| `Pan` | Stereo panning (2D sound, -1 to 1) |
| `Spatial Blend` | Balance between 2D and 3D (0 = full 2D, 1 = full 3D) |

### 📝 Basic playback example:
```csharp
using UnityEngine;

public class SimpleSoundPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip backgroundMusic;
    
    private AudioSource audioSource;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        
        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        audioSource.volume = 0.5f;
        audioSource.Play();
    }
    
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            PlayJumpSound();
        }
    }
    
    private void PlayJumpSound()
    {
        audioSource.PlayOneShot(jumpSound, 0.8f);
    }
}
```

### 🎵 Playback methods:
| Method | Description |
| --- | --- |
| `Play()` | Normal playback (starts from beginning) |
| `PlayOneShot(AudioClip clip, float volumeScale)` | Overlay sound on top of current |
| `PlayClipAtPoint(AudioClip clip, Vector3 position, float volume)` | Static method for one-time sound at a point |
| `Stop()` | Stop playback |
| `Pause()` | Pause with ability to resume |
| `UnPause()` | Resume from pause |

---

## 3. 2D vs 3D Sound
The key difference between 2D and 3D sound is whether the position relative to the listener matters.

### 🖥️ 2D Sound
- Independent of position in space
- Volume and panorama don't change with movement
- Used for: UI sounds, background music, narrator voice, system alerts

Settings:
- `Spatial Blend = 0`
- `Pan` set manually (or stereo)

### 🌍 3D Sound
- Depends on distance to AudioListener
- Volume decreases with distance (attenuation)
- Supports Doppler effect
- Used for: footsteps, gunshots, ambient sounds, enemy sounds

Settings:
- `Spatial Blend = 1`
- Adjust attenuation curve in `AudioSource → 3D Sound Settings`

### 📊 Property comparison:
| Feature | 2D Sound | 3D Sound |
| --- | --- | --- |
| `Spatial Blend` | 0 | 1 |
| Distance dependency | No | Yes |
| Doppler effect | No | Yes (optional) |
| Panning | Stereo/mono in mixer | Automatic based on position |
| Performance | High (fewer calculations) | Lower (requires positioning) |

---

## 4. Sound Positioning in 3D Space
For 3D sound, where the AudioSource is relative to the AudioListener is critical. 
Unity automatically calculates volume, panorama, and delay based on distance and direction.

### 📍 3D Sound Settings (AudioSource → 3D Sound Settings):
| Parameter | Description | Recommendations |
| --- | --- | --- |
| `Volume Rolloff` | Shape of the volume attenuation curve | `Logarithmic` (realistic), `Linear` (simple) |
| `Min Distance` | Distance at which volume is maximum | 1-5 units (for small objects) |
| `Max Distance` | Distance at which sound is completely inaudible | 20-500 units (depends on scene) |
| `Spread` | Sound spread angle (degrees) | 0 = point, 360 = omnidirectional |
| `Doppler Level` | Intensity of Doppler effect | 1 = normal, 0 = off |

### 📈 Rolloff curve types:
1. Logarithmic Rolloff — most realistic (sound fades quickly nearby, slowly far away)
2. Linear Rolloff — linear volume decrease
3. Custom Rolloff — manual curve adjustment via Animation Curve

---

## 5. Advanced: AudioMixer and Effects
For advanced sound work, use AudioMixer — it allows creating groups, adding effects (reverb, echo, delay), and controlling them programmatically.

### 🎛️ AudioMixer example:
```csharp
using UnityEngine;
using UnityEngine.Audio;

public class AudioMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer masterMixer;
    
    public void SetMasterVolume(float volume)
    {
        float dB = Mathf.Log10(Mathf.Max(0.0001f, volume)) * 20f;
        masterMixer.SetFloat("MasterVolume", dB);
    }
}
```

---

## 6. Common Mistakes and Tips

### ❌ Common mistakes:
1. Multiple AudioListeners in scene → warning and possible artifacts
2. Forgot to enable `Play On Awake` → sound doesn't play on start
3. `Spatial Blend = 0` but expecting 3D effects → sound will be flat
4. `Max Distance` too small → sound cuts off abruptly
5. 3D sound too loud up close → clipping distortion

### ✅ Recommendations:
1. Always ensure exactly one active `AudioListener` in the scene
2. Use `AudioMixer` for global volume control
3. Use `PlayOneShot` for one-off sounds instead of creating new `AudioSource`
4. Tune `Min Distance` and `Max Distance` to your world's scale
5. Preload `AudioClips` in `Start()` to avoid first-play delays
6. Use a separate `AudioSource` with `Spatial Blend = 0` for background music

---

### ⭐ If this project was useful, put a star on GitHub!
