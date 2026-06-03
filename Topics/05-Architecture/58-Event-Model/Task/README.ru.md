# 🧪 Практическое задание: Глобальная система событий через ScriptableObject Channels

Цель: Реализовать слабосвязанную архитектуру в Unity с помощью ScriptableObject Channels (каналов событий). 
Вы создадите систему, в которой несколько независимых компонентов реагируют на игровые события без прямых ссылок друг на друга.

---

## 📥 Контекст
Вы разрабатываете 2D-платформер. В игре есть:
- Игрок, который собирает монеты и теряет здоровье при столкновении с врагами.
- UI, отображающий счёт и здоровье.
- Система достижений (ачивки).
- Аудио-менеджер, проигрывающий звуки.
- Спавнер врагов, который реагирует на смерть врага.

Требование: Все эти системы не должны иметь прямых ссылок друг на друга (кроме каналов событий).

---

## 🎯 Задачи
### Этап 1: Создание каналов событий (ScriptableObject Channels)
1. Создайте базовый класс `EventChannelSO<T>` (Generic).
2. Создайте конкретные каналы через атрибут `[CreateAssetMenu]`:
   - `IntEventChannel` — для счёта и здоровья.
   - `StringEventChannel` — для сообщений достижений.
   - `GameObjectEventChannel` — для событий с объектами (враг умер, игрок столкнулся).
  
3. В папке `Assets/Channels/` создайте ассеты:
   - `ScoreChannel` (IntEventChannel)
   - `HealthChannel` (IntEventChannel)
   - `AchievementChannel` (StringEventChannel)
   - `EnemyDeathChannel` (GameObjectEventChannel)
  
### Этап 2: Отправители (Publishers)
Создайте скрипт `PlayerController.cs`:
```csharp
public class PlayerController : MonoBehaviour {
    [SerializeField] private IntEventChannel scoreChannel;
    [SerializeField] private IntEventChannel healthChannel;
    [SerializeField] private GameObjectEventChannel enemyDeathChannel; // когда игрок убивает врага
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

### Этап 3: Получатели (Subscribers)
Создайте три независимых компонента:

A. UIManager.cs (обновляет текст счёта и здоровья)
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

B. AchievementManager.cs (выдаёт ачивки при достижениях)
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

C. AudioManager.cs (проигрывает звуки)
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

### Этап 4: Спавнер врагов (получатель события смерти врага)
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

### Этап 5: Настройка в инспекторе (без кода!)
1. Создайте на сцене: `Player`, `UIManager`, `AchievementManager`, `AudioManager`, `EnemySpawner`.
2. На каждый компонент перетащите нужные каналы (ScoreChannel, HealthChannel, etc.).
3. Запустите игру и убедитесь:
   - Поднятие монеты → обновление UI, звук.
   - Получение урона → обновление UI.
   - Убийство врага → звук, спавн нового врага.
   - Достижение 50/100/200 очков → сообщение в консоль (или UI).
  
### 🎁 Дополнительно (со звёздочкой ⭐)
- Добавьте канал `GameOverChannel` (bool). При смерти игрока спавнер останавливается, UI показывает "Game Over".
- Создайте `DebugEventListener.cs`, который просто логирует все события (полезно для отладки).
- Используйте `UnityEvent` вместо `UnityAction` в канале, чтобы можно было привязывать методы в инспекторе без кода.

---

## ✅ Критерии успеха
- Созданы минимум 3 канала событий (Int, String, GameObject).
- PlayerController вызывает события, не зная о UI/Audio/Achievements.
- UIManager, AudioManager, AchievementManager, EnemySpawner подписаны на соответствующие каналы.
- Нет прямых ссылок (сериализуемые поля только на каналы).
- При уничтожении объектов подписки отписываются в `OnDestroy`.
- Система работает: монеты +10 очков, урон -25 HP, при 0 HP Game Over (если сделано).

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
