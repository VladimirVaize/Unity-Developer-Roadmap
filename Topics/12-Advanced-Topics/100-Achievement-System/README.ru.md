# 🏆 Система достижений и аналитика: Интеграция с платформами (Google Play, iOS GameCenter), сторонние SDK

Достижения и аналитика — это две стороны одной медали в современной мобильной разработке. 
Достижения повышают вовлечённость игроков и удерживают их в игре, а аналитика позволяет понять, как игроки взаимодействуют с контентом. 
В этом руководстве мы разберём интеграцию с платформенными сервисами и использование сторонних SDK для создания единой системы.

---

## 1. Зачем нужны достижения и аналитика?

| Компонент | Назначение | Влияние на метрики |
| --- | --- | --- |
| Достижения | Прогрессия, награды, цели | Retention +23% |
| Лидерборды | Социальное соревнование | Сессионное время ↑ |
| Аналитика | Поведенческие данные, оптимизация | LTV ↑, Churn ↓ |

> Ключевая идея: Достижения создают "контентные крючки", которые удерживают игроков, а аналитика даёт данные для их улучшения.

---

## 2. Unity Social API — универсальный интерфейс
Unity предоставляет встроенный API для работы с достижениями и лидербордами через класс `Social`. 
Он автоматически использует платформенные реализации (Game Center на iOS, Google Play Games на Android).

### 📋 Поддерживаемые функции:
| Метод | Описание |
| --- | --- |
| `Social.localUser.Authenticate()` | Аутентификация пользователя |
| `Social.ReportProgress()` | Отчёт о прогрессе достижения |
| `Social.LoadAchievements()` | Загрузка списка достижений |
| `Social.ReportScore()` | Отправка счета в лидерборд |
| `Social.LoadScores()` | Загрузка таблицы лидеров |

### 💻 Базовый пример:
```csharp
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class AchievementManager : MonoBehaviour
{
    void Start()
    {
        // Аутентификация пользователя
        Social.localUser.Authenticate(ProcessAuthentication);
    }

    void ProcessAuthentication(bool success)
    {
        if (success)
        {
            Debug.Log("Аутентификация успешна");
            // Загрузка существующих достижений
            Social.LoadAchievements(ProcessLoadedAchievements);
        }
        else
        {
            Debug.LogError("Аутентификация не удалась");
        }
    }

    void ProcessLoadedAchievements(IAchievement[] achievements)
    {
        Debug.Log($"Загружено достижений: {achievements.Length}");
        // Здесь можно синхронизировать локальный прогресс
    }

    // Отчёт о прогрессе достижения
    public void UnlockAchievement(string id, double progress = 100.0)
    {
        Social.ReportProgress(id, progress, success =>
        {
            if (success)
                Debug.Log($"Достижение {id} обновлено");
            else
                Debug.LogError($"Ошибка обновления {id}");
        });
    }

    // Отправка счета в лидерборд
    public void SubmitScore(string leaderboardId, long score)
    {
        Social.ReportScore(score, leaderboardId, success =>
        {
            if (success)
                Debug.Log($"Счет {score} отправлен");
        });
    }
}
```

### 🎯 Ограничения Social API:
1. Нет доступа к сложным функциям (например, "пошаговые" достижения на iOS имеют особенности).
2. Для расширенной функциональности требуется платформенный SDK.

---

## 3. Интеграция с Google Play Games (Android)
Для Android Unity-проектов используется Google Play Games Plugin for Unity.

### ⚙️ Настройка плагина:
1. Скачайте плагин с GitHub или через Unity Package Manager.
2. Импортируйте `.unitypackage` в проект.
3. Убедитесь, что Build Platform установлена на Android.
4. Настройте Google Play Console:
   - Включите Google Play Games Services
   - Создайте достижения и лидерборды
   - Получите XML-ресурсы с идентификаторами
  
### 📝 Пример расширенной настройки:
```csharp
#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class GPGSAuthentication : MonoBehaviour
{
    void Start()
    {
        // Настройка игрового сервиса
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .EnableSavedGames()
            .Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();

        // Аутентификация
        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                Debug.Log("GPG аутентификация успешна");
                // Здесь можно работать с достижениями через Social API
            }
        });
    }
}
#endif
```

---

## 4. Интеграция с iOS Game Center
На iOS используется GameKit — нативный фреймворк Apple. Unity Social API автоматически использует его для вызовов `Social`.

### ⚙️ Настройка для iOS:
1. App Store Connect:
   - Включите Game Center для вашего приложения
   - Создайте достижения и лидерборды
   - Запомните идентификаторы (например, `com.game.achievement.finished_level_1`)
  
2. Xcode:
   - После экспорта проекта включите Capability Game Center
  
3. Код не отличается от Social API — Unity обрабатывает платформенные различия внутри.

### 🍎 Особенности iOS:
- Достижения не могут быть "деактивированы" — только разблокированы с прогрессом.
- Нет "шаговых" достижений — используется процентное значение.
- `ReportProgress(100.0)` разблокирует достижение.

---

## 5. Сторонние SDK: унификация платформ
Ручная интеграция каждой платформы занимает много времени. 
Сторонние SDK предоставляют единый API для всех платформ.

### 🛠️ Популярные решения:
| SDK | Особенности | Поддерживаемые платформы |
| --- | --- | --- |
| Essential Kit | Готовые модули, 23% ↑ retention | iOS, Android |
| Easy Mobile | Визуальный редактор, генерация констант | iOS, Android |
| CloudOnce | Единый API, встроенный редактор | iOS, Android, Amazon |
| Hiro Achievements | Система с под-достижениями и наградами | Серверное решение |

### 📝 Пример Easy Mobile:
```csharp
using EasyMobile;

public class EasyMobileAchievements : MonoBehaviour
{
    void Start()
    {
        // Аутентификация через Easy Mobile
        GameServices.Init();
    }

    public void UnlockAchievement(string id)
    {
        // Использование констант из EM_GameServicesConstants
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

### 🏗️ Архитектура с абстракцией:
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

## 6. Аналитика: Unity Analytics и сторонние решения
Аналитика помогает отслеживать поведение игроков, воронки конверсии и эффективность достижений.

### 📊 Подключение Unity Analytics:
1. Window → General → Services → включите Unity Analytics
2. В коде отправляйте события:
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

### 🌐 Популярные аналитические SDK:
| SDK | Особенности |
| --- | --- |
| Unity Analytics | Встроенный, бесплатный для всех |
| Firebase Analytics | Бесплатно, интеграция с Google Play |
| AppsFlyer | Маркетинговая атрибуция |
| GameAnalytics | Специализированный для игр |

---

## 7. Синхронизация локального и платформенного состояния
Одна из главных проблем — рассинхронизация локального состояния и данных на платформе.

### 🔄 Проблемы и решения:

| Проблема | Решение |
| --- | --- |
| Сброс счётчика после переустановки | Сохраняйте в облаке (Play Games Saved Games / iCloud) |
| Ретроактивные достижения | Храните исторические данные игрока |
| Офлайн-прогресс | Кешируйте и синхронизируйте при восстановлении сети |

### 💻 Пример гибридного подхода:
```csharp
public class AchievementSyncService : MonoBehaviour
{
    private Dictionary<string, double> _localProgress = new Dictionary<string, double>();
    private bool _isOnline = false;

    // Сохранение прогресса локально
    public void SaveLocalProgress(string id, double progress)
    {
        if (_localProgress.ContainsKey(id))
            _localProgress[id] = Math.Max(_localProgress[id], progress);
        else
            _localProgress.Add(id, progress);

        // При наличии сети — отправляем на платформу
        if (_isOnline)
            Social.ReportProgress(id, progress, null);
    }

    // Синхронизация при восстановлении сети
    public void SyncAllProgress()
    {
        if (!_isOnline) return;

        foreach (var kvp in _localProgress)
        {
            Social.ReportProgress(kvp.Key, kvp.Value, success =>
            {
                if (success)
                    Debug.Log($"Синхронизировано {kvp.Key}");
            });
        }
    }

    // Загрузка состояния с платформы
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

## 8. Лучшие практики и частые ошибки
### ✅ Рекомендации:
1. Используйте абстракцию — не привязывайте код к конкретному SDK
2. Кешируйте прогресс локально — офлайн-режим критичен для мобильных игр
3. Тестируйте на реальных устройствах — эмуляторы не всегда поддерживают Game Services
4. Генерируйте константы — избегайте "магических строк" (Easy Mobile, Essential Kit)
5. Синхронизируйте при запуске — загружайте состояние платформы и обновляйте локальное

### ❌ Частые ошибки:
```csharp
// ❌ ОШИБКА: Отчёт о прогрессе только при разблокировке
// Если прогресс 100% — достижение разблокируется

// ✅ ПРАВИЛЬНО: Отправляйте прогресс постепенно
Social.ReportProgress("kill_100_goblins", 50.0, null);
Social.ReportProgress("kill_100_goblins", 75.0, null);
Social.ReportProgress("kill_100_goblins", 100.0, null);

// ❌ ОШИБКА: Жёсткая привязка к Social API
Social.ReportProgress("com.game.achievement.id", 100.0, null);

// ✅ ПРАВИЛЬНО: Абстракция + константы
AchievementService.Instance.Unlock(Constants.ACHIEVEMENT_FIRST_PLAY);
```

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
