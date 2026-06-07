# 🎯 Задача: «Сохранение времени последнего сеанса и возобновление игры»
Вы разрабатываете мобильную игру. Вам нужно реализовать систему, которая:
1. При сворачивании (Pause/Focus loss):
   - Ставит игру на паузу (`Time.timeScale = 0`)
   - Останавливает все звуки (`AudioListener.pause = true`)
   - Сохраняет текущее время (`System.DateTime.Now`) в` PlayerPrefs` с ключом `"LastPauseTime"`
  
2. При возвращении из фона:
   - Снимает паузу
   - Включает звук
   - Выводит в консоль сообщение: `"Returned after X seconds"`, где X — разница между текущим временем и `"LastPauseTime"`
  
3. При закрытии приложения (Quit):
   - Сохраняет текущее время в `PlayerPrefs` с ключом `"LastQuitTime"`
  
4. При первом запуске (Start):
   - Проверяет, было ли аварийное завершение (если `"LastQuitTime"` отсутствует, а `"LastPauseTime"` есть — значит, игра была убита в фоне)
  
---

## 📝 Требования к коду:
- Используйте `OnApplicationPause`, `OnApplicationFocus` и `OnApplicationQuit`.
- Предусмотрите защиту от дублирования действий (флаг `isPaused`).
- Напишите код в одном скрипте `LifecycleTracker`.

---

## 🔍 Ожидаемый вывод в консоли (пример):
```text
[Start] Last quit time not found. Possible crash on last session.
[Focus] Focus lost
[Pause] Paused at 14:30:05
[Focus] Focus regained
[Return] Returned after 12 seconds
[Quit] Quitting at 14:30:22
```

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
