# 🧪 Practical Task: Global Event System via ScriptableObject Channels

Goal: Implement a loosely coupled architecture in Unity using ScriptableObject Channels. 
You will create a system where multiple independent components react to game events without direct references to each other.

---

## 📥 Context
You are developing a 2D platformer. The game has:
- A player who collects coins and loses health when colliding with enemies.
- UI that displays score and health.
- An achievement system.
- An audio manager that plays sounds.
- An enemy spawner that reacts to enemy deaths.

Requirement: All these systems must not have direct references to each other (except through event channels).

---

## 🎯 Tasks
### Step 1: Create ScriptableObject Channels
1. Create a base class `EventChannelSO<T>` (Generic).
2. Create concrete channels using `[CreateAssetMenu]`:
   - `IntEventChannel` — for score and health.
   - `StringEventChannel` — for achievement messages.
   - `GameObjectEventChannel` — for events with GameObjects (enemy died, player hit).
  
3. In the `Assets/Channels/` folder, create assets:
   - `ScoreChannel` (IntEventChannel)
   - `HealthChannel` (IntEventChannel)
   - `AchievementChannel` (StringEventChannel)
   - `EnemyDeathChannel` (GameObjectEventChannel)
  
### Step 2: Publishers (Senders)
Create `PlayerController.cs`:
```csharp
public class PlayerController : MonoBehaviour {
    [SerializeField] private IntEventChannel scoreChannel;
    [SerializeField] private IntEventChannel healthChannel;
    [SerializeField] private GameObjectEventChannel enemyDeathChannel;
    private int score = 0;
    private int health = 100;
    
    void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Coin")) {
            score += 10;
            scoreChannel.RaiseEvent(score);
            Destroy(other.gameObject);
        }
        else if(other.CompareTag("Enemy")) {
            health -= 25;
            healthChannel.RaiseEvent(health);
            if(health <= 0) Debug.Log("Game Over");
        }
    }
    
    public void KillEnemy(GameObject enemy) {
        enemyDeathChannel.RaiseEvent(enemy);
        Destroy(enemy);
    }
}
```

### Step 3: Subscribers (Receivers)
Create three independent components:

A. UIManager.cs (updates score and health text)
```csharp
public class UIManager : MonoBehaviour {
    [SerializeField] private IntEventChannel scoreChannel;
    [SerializeField] private IntEventChannel healthChannel;
    [SerializeField] private Text scoreText, healthText;
    
    void Start() {
        scoreChannel.OnEventRaised += UpdateScore;
        healthChannel.OnEventRaised += UpdateHealth;
    }
    
    void UpdateScore(int newScore) => scoreText.text = $"Score: {newScore}";
    void UpdateHealth(int newHealth) => healthText.text = $"HP: {newHealth}";
    
    void OnDestroy() {
        scoreChannel.OnEventRaised -= UpdateScore;
        healthChannel.OnEventRaised -= UpdateHealth;
    }
}
```

B. AchievementManager.cs (grants achievements)
```csharp
public class AchievementManager : MonoBehaviour {
    [SerializeField] private IntEventChannel scoreChannel;
    [SerializeField] private StringEventChannel achievementChannel;
    
    void Start() => scoreChannel.OnEventRaised += CheckAchievements;
    
    void CheckAchievements(int score) {
        if(score == 50) achievementChannel.RaiseEvent("Bronze Collector");
        if(score == 100) achievementChannel.RaiseEvent("Silver Collector");
        if(score == 200) achievementChannel.RaiseEvent("Gold Collector");
    }
    
    void OnDestroy() => scoreChannel.OnEventRaised -= CheckAchievements;
}
```

C. AudioManager.cs (plays sounds)
```csharp
public class AudioManager : MonoBehaviour {
    [SerializeField] private IntEventChannel scoreChannel;
    [SerializeField] private GameObjectEventChannel enemyDeathChannel;
    [SerializeField] private AudioClip coinSound, enemyDeathSound;
    
    void Start() {
        scoreChannel.OnEventRaised += (score) => PlaySound(coinSound);
        enemyDeathChannel.OnEventRaised += (enemy) => PlaySound(enemyDeathSound);
    }
    
    void PlaySound(AudioClip clip) => AudioSource.PlayClipAtPoint(clip, Vector3.zero);
}
```

### Step 4: Enemy Spawner (subscribes to enemy death)
```csharp
public class EnemySpawner : MonoBehaviour {
    [SerializeField] private GameObjectEventChannel enemyDeathChannel;
    [SerializeField] private GameObject enemyPrefab;
    private int enemiesAlive = 0;
    
    void Start() {
        enemyDeathChannel.OnEventRaised += OnEnemyDied;
        SpawnEnemy();
    }
    
    void OnEnemyDied(GameObject deadEnemy) {
        enemiesAlive--;
        Debug.Log($"Enemies left: {enemiesAlive}");
        if(enemiesAlive == 0) SpawnEnemy();
    }
    
    void SpawnEnemy() {
        Instantiate(enemyPrefab, RandomPosition(), Quaternion.identity);
        enemiesAlive++;
    }
}
```

### Step 5: Inspector Setup (no code!)
1. In your scene, create: `Player`, `UIManager`, `AchievementManager`, `AudioManager`, `EnemySpawner`.
2. Drag the appropriate channels into each component (ScoreChannel, HealthChannel, etc.).
3. Run the game and verify:
   - Collecting a coin → UI update, sound.
   - Taking damage → UI update.
   - Killing an enemy → sound, new enemy spawns.
   - Reaching 50/100/200 points → console message (or UI).
  
---

## 🎁 Bonus (⭐)
- Add a `GameOverChannel` (bool). When the player dies, the spawner stops, UI shows "Game Over".
- Create `DebugEventListener.cs` that logs all events (useful for debugging).
- Use `UnityEvent` instead of `UnityAction` in the channel so you can bind methods in the Inspector without code.

---

## ✅ Success Criteria
- At least 3 event channels created (Int, String, GameObject).
- PlayerController raises events without knowing about UI/Audio/Achievements.
- UIManager, AudioManager, AchievementManager, EnemySpawner subscribe to appropriate channels.
- No direct references (only serialized fields for channels).
- Subscriptions are unsubscribed in `OnDestroy`.
- The system works: coins +10 points, damage -25 HP, at 0 HP Game Over (if implemented).

---

### ⭐ If this project was useful, put a star on GitHub!
