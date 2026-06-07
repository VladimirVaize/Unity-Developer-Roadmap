# 🧠 Жизненный цикл приложения в Unity: Обработка фонового режима
В мобильной и десктопной разработке критически важно правильно реагировать на сворачивание, 
потерю фокуса или закрытие приложения. Unity предоставляет три ключевых события `MonoBehaviour` для этого.

---

## 1. `OnApplicationFocus(bool hasFocus)`
Вызывается, когда приложение получает или теряет фокус.

| `hasFocus` | Состояние |
| --- | --- |
| `true` | Приложение в фокусе (пользователь взаимодействует) |
| `false` | Фокус утерян (свайпнули шторку, открыли другое приложение) |

```csharp
void OnApplicationFocus(bool hasFocus)
{
    if (hasFocus)
    {
        Debug.Log("Приложение в фокусе");
        // Возобновить анимации, звук, UI-обновления
    }
    else
    {
        Debug.Log("Фокус потерян");
        // Поставить игру на паузу, сохранить состояние
    }
}
```

---

## 2. `OnApplicationPause(bool pauseStatus)`
Вызывается при переходе приложения в фоновый режим и обратно.

| `pauseStatus` | Состояние |
| --- | --- |
| `true` | Приложение уходит в фон |
| `false` | Приложение возвращается из фона |

```csharp
void OnApplicationPause(bool pauseStatus)
{
    if (pauseStatus)
    {
        Debug.Log("Игра свернута или заблокирован экран");
        Time.timeScale = 0; // Пауза игрового времени
        SaveGame();         // Сохранить прогресс
    }
    else
    {
        Debug.Log("Возврат в игру");
        Time.timeScale = 1;
        RefreshUI();
    }
}
```

> [!Important]
> На iOS `OnApplicationPause(true)` вызывается при сворачивании, а на Android — при нажатии Home или переключении задач.

---

## 3. `OnApplicationQuit()`
Вызывается непосредственно перед закрытием приложения.
```csharp
void OnApplicationQuit()
{
    Debug.Log("Приложение закрывается");
    SaveGame();               // Финальное сохранение
    PlayerPrefs.Save();       // Принудительная запись
    DisconnectFromServer();   // Закрыть сетевые соединения
}
```

> [!Warning]
> На мобильных устройствах `OnApplicationQuit` не гарантирован при принудительном закрытии (свайп из списка задач). На десктопе — вызывается всегда.

---

## 4. Обработка фонового режима (полный пример)
```csharp
public class AppLifecycleHandler : MonoBehaviour
{
    private bool isGamePaused = false;

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            EnterBackground();
        else
            ReturnFromBackground();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            EnterBackground();
        else
            ReturnFromBackground();
    }

    private void EnterBackground()
    {
        if (isGamePaused) return;
        isGamePaused = true;
        Time.timeScale = 0;
        AudioListener.pause = true;
        Debug.Log("Фоновый режим активирован");
    }

    private void ReturnFromBackground()
    {
        if (!isGamePaused) return;
        isGamePaused = false;
        Time.timeScale = 1;
        AudioListener.pause = false;
        Debug.Log("Возврат из фона");
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("LastSessionTime", System.DateTime.Now.Hour);
        PlayerPrefs.Save();
    }
}
```

---

## 📌 Рекомендации
- Используйте оба метода (`OnApplicationFocus` и `OnApplicationPause`) для надёжности — они вызываются в разных ситуациях.
- Не делайте тяжёлых операций в `OnApplicationPause(false)` на андроиде — пользователь может закрыть приложение до их завершения.
- Для сохранения данных используйте `PlayerPrefs.Save()` после записи.
- На мобильных устройствах избегайте `Application.Quit()` — это нарушает платформенные соглашения.

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
