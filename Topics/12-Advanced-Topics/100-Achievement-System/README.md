# 🏆 Achievement System and Analytics: Platform Integration (Google Play, iOS GameCenter), Third-Party SDKs
Achievements and analytics are two sides of the same coin in modern mobile development. 
Achievements boost player engagement and retention, while analytics provide insights into how players interact with content. 
This guide covers integration with platform services and using third-party SDKs for a unified system.

---

## 1. Why Achievements and Analytics Matter
| Component | Purpose | Metric Impact |
| --- | --- | --- |
| Achievements | Progression, rewards, goals | Retention +23% |
| Leaderboards | Social competition | Session time ↑ |
| Analytics | Behavioral data, optimization | LTV ↑, Churn ↓ |

> Core idea: Achievements create "content hooks" that retain players, while analytics provide data to improve them.

---

## 2. Unity Social API — Universal Interface
Unity provides a built-in API for achievements and leaderboards via the `Social` class. 
It automatically uses platform-specific implementations (Game Center on iOS, Google Play Games on Android).

### 📋 Supported Features:
| Method | Description |
| --- | --- |
| `Social.localUser.Authenticate()` | User authentication |
| `Social.ReportProgress()` | Report achievement progress |
| `Social.LoadAchievements()` | Load achievement list |
| `Social.ReportScore()` | Submit score to leaderboard |
| `Social.LoadScores()` | Load leaderboard |

### 💻 Basic Example:
```csharp
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class AchievementManager : MonoBehaviour
{
    void Start()
    {
        Social.localUser.Authenticate(ProcessAuthentication);
    }

    void ProcessAuthentication(bool success)
    {
        if (success)
        {
            Debug.Log("Authentication successful");
            Social.LoadAchievements(ProcessLoadedAchievements);
        }
        else
        {
            Debug.LogError("Authentication failed");
        }
    }

    void ProcessLoadedAchievements(IAchievement[] achievements)
    {
        Debug.Log($"Loaded achievements: {achievements.Length}");
    }

    public void UnlockAchievement(string id, double progress = 100.0)
    {
        Social.ReportProgress(id, progress, success =>
        {
            if (success)
                Debug.Log($"Achievement {id} updated");
            else
                Debug.LogError($"Update failed {id}");
        });
    }

    public void SubmitScore(string leaderboardId, long score)
    {
        Social.ReportScore(score, leaderboardId, success =>
        {
            if (success)
                Debug.Log($"Score {score} submitted");
        });
    }
}
```

### 🎯 Social API Limitations:
1. No access to complex features (e.g., "step-based" achievements on iOS have specifics).
2. Advanced functionality requires platform SDK.

---

## 3. Google Play Games Integration (Android)
Android Unity projects use the Google Play Games Plugin for Unity.

### ⚙️ Setup:
1. Download the plugin from GitHub or via Unity Package Manager.
2. Import the `.unitypackage` into your project.
3. Set Build Platform to Android.
4. Configure in Google Play Console:
   - Enable Google Play Games Services
   - Create achievements and leaderboards
   - Get XML resources with IDs
  
### 📝 Extended Configuration Example:
```csharp
#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class GPGSAuthentication : MonoBehaviour
{
    void Start()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .EnableSavedGames()
            .Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();

        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
                Debug.Log("GPG authentication successful");
        });
    }
}
#endif
```

---

## 4. iOS Game Center Integration
iOS uses GameKit — Apple's native framework. Unity's Social API automatically uses it for Social calls.

### ⚙️ iOS Setup:
1. App Store Connect:
   - Enable Game Center for your app
   - Create achievements and leaderboards
   - Note their IDs (e.g., `com.game.achievement.finished_level_1`)
  
2. Xcode:
   - After project export, enable Game Center capability
  
3. Code doesn't change — Unity handles platform differences internally.

### 🍎 iOS Specifics:
- Achievements cannot be "deactivated" — only unlocked with progress.
- No "step-based" achievements — percentage value is used.
- `ReportProgress(100.0)` unlocks the achievement.

---

## 5. Third-Party SDKs: Platform Unification
Manual integration for each platform is time-consuming. 
Third-party SDKs provide a unified API for all platforms.

### 🛠️ Popular Solutions:
| SDK | Features | Supported Platforms |
| --- | --- | --- |
| Essential Kit | Ready modules, 23% ↑ retention | iOS, Android |
| Easy Mobile | Visual editor, constant generation | iOS, Android |
| CloudOnce | Unified API, built-in editor | iOS, Android, Amazon |
| Hiro Achievements | Sub-achievements and rewards | Server-based |

### 📝 Easy Mobile Example:
```csharp
using EasyMobile;

public class EasyMobileAchievements : MonoBehaviour
{
    void Start()
    {
        GameServices.Init();
    }

    public void UnlockAchievement(string id)
    {
        GameServices.UnlockAchievement(EM_GameServicesConstants.ACHIEVEMENT_FIRST_PLAY);
    }

    public void ReportAchievementProgress(string id, float progress)
    {
        GameServices.ReportProgress(id, progress);
    }

    public void SubmitScore(string leaderboardId, int score)
    {
        GameServices.ReportScore(score, leaderboardId);
    }
}
```

### 🏗️ Abstraction Architecture:
```csharp
public interface IAchievementService
{
    void Authenticate(System.Action<bool> callback);
    void Unlock(string id);
    void ReportProgress(string id, double progress);
    void ShowAchievementsUI();
}

public class UnitySocialService : IAchievementService { /* ... */ }
public class EasyMobileService : IAchievementService { /* ... */ }
public class HiroService : IAchievementService { /* ... */ }
```

---

## 6. Analytics: Unity Analytics and Third-Party Solutions
Analytics helps track player behavior, conversion funnels, and achievement effectiveness.

### 📊 Unity Analytics Setup:
1. Window → General → Services → enable Unity Analytics
2. Send custom events in code:
```csharp
using UnityEngine.Analytics;

public class GameAnalytics : MonoBehaviour
{
    public void TrackAchievementUnlocked(string achievementId)
    {
        Analytics.CustomEvent("achievement_unlocked", new Dictionary<string, object>
        {
            { "achievement_id", achievementId },
            { "level", PlayerPrefs.GetInt("CurrentLevel", 1) },
            { "session_time", Time.timeSinceLevelLoad }
        });
    }

    public void TrackLevelCompleted(int level, float time, int stars)
    {
        Analytics.CustomEvent("level_completed", new Dictionary<string, object>
        {
            { "level", level },
            { "time", time },
            { "stars", stars }
        });
    }
}
```

### 🌐 Popular Analytics SDKs:
| SDK | Features |
| --- | --- |
| Unity Analytics | Built-in, free for all |
| Firebase Analytics | Free, Google Play integration |
| AppsFlyer | Marketing attribution |
| GameAnalytics | Specialized for games |

---

## 7. Local and Platform State Synchronization
One major problem is desynchronization between local state and platform data.

### 🔄 Problems and Solutions:
| Problem | Solution |
| --- | --- |
| Counter reset after reinstall | Cloud save (Play Games Saved Games / iCloud) |
| Retroactive achievements | Store player historical data |
| Offline progress | Cache and sync when network restores |

### 💻 Hybrid Approach Example:
```csharp
public class AchievementSyncService : MonoBehaviour
{
    private Dictionary<string, double> _localProgress = new Dictionary<string, double>();
    private bool _isOnline = false;

    public void SaveLocalProgress(string id, double progress)
    {
        if (_localProgress.ContainsKey(id))
            _localProgress[id] = Math.Max(_localProgress[id], progress);
        else
            _localProgress.Add(id, progress);

        if (_isOnline)
            Social.ReportProgress(id, progress, null);
    }

    public void SyncAllProgress()
    {
        if (!_isOnline) return;

        foreach (var kvp in _localProgress)
        {
            Social.ReportProgress(kvp.Key, kvp.Value, success =>
            {
                if (success)
                    Debug.Log($"Synced {kvp.Key}");
            });
        }
    }

    public void LoadPlatformAchievements()
    {
        Social.LoadAchievements(achievements =>
        {
            foreach (var achievement in achievements)
            {
                if (_localProgress.ContainsKey(achievement.id))
                    _localProgress[achievement.id] = achievement.percentCompleted;
                else
                    _localProgress.Add(achievement.id, achievement.percentCompleted);
            }
        });
    }
}
```

---

## 8. Best Practices and Common Mistakes
### ✅ Recommendations:
1. Use abstraction — don't bind code to a specific SDK
2. Cache progress locally — offline mode is critical for mobile games
3. Test on real devices — emulators don't always support Game Services
4. Generate constants — avoid "magic strings" (Easy Mobile, Essential Kit)
5. Sync on launch — load platform state and update local

### ❌ Common Mistakes:
```csharp
// ❌ ERROR: Progress report only at unlock time

// ✅ CORRECT: Send progress gradually
Social.ReportProgress("kill_100_goblins", 50.0, null);
Social.ReportProgress("kill_100_goblins", 75.0, null);
Social.ReportProgress("kill_100_goblins", 100.0, null);

// ❌ ERROR: Direct binding to Social API
Social.ReportProgress("com.game.achievement.id", 100.0, null);

// ✅ CORRECT: Abstraction + constants
AchievementService.Instance.Unlock(Constants.ACHIEVEMENT_FIRST_PLAY);
```

---

### ⭐ If this project was useful, put a star on GitHub!
