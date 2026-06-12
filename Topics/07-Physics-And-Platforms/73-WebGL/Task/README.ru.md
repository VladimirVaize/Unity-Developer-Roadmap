# 🎯 Задача: «Веб-сохранялка с уведомлениями»
Вы разрабатываете игру для браузера. Нужно реализовать систему, которая:
1. Сохраняет игровой прогресс в `localStorage` браузера (вызов JS из C#)
2. Отображает браузерное уведомление при достижении нового рекорда (через `window.alert` или `Notification API`)
3. Загружает сохранения при старте игры (вызов C# из JS)
4. Обрабатывает сворачивание вкладки (ставит игру на паузу)
5. Выводит информацию о браузере (платформа, язык, разрешение экрана)

---

## 📝 Шаблон кода для заполнения:
```csharp
using UnityEngine;
using System.Runtime.InteropServices;

public class WebGLSaveManager : MonoBehaviour
{
    [Header("UI References")]
    public TMPro.TextMeshProUGUI browserInfoText;
    public TMPro.TextMeshProUGUI saveStatusText;
    
    private int _currentScore = 0;
    private int _highScore = 0;
    
    // TODO: 1. Объявите DllImport методы для JS-функций
    
    void Start()
    {
        // TODO: 2. Загрузить сохранения из localStorage при старте
        
        // TODO: 3. Вывести информацию о браузере
    }
    
    public void AddScore(int points)
    {
        _currentScore += points;
        
        // TODO: 4. Если новый рекорд - показать уведомление через JS
        
        // TODO: 5. Автосохранение через 1 секунду (используя корутину)
    }
    
    // TODO: 6. Метод, который будет вызван из JS с загруженными данными
    public void OnDataLoadedFromJS(string jsonData)
    {
        // Десериализовать и применить сохранение
    }
    
    // TODO: 7. Метод для сохранения в JS
    private void SaveToJS()
    {
        
    }
    
    // TODO: 8. Обработка фокуса (пауза при сворачивании)
    void OnApplicationFocus(bool hasFocus)
    {
        
    }
}
```

---

## 📋 Конкретные задачи для реализации:
### Часть А: Создание .jslib файла
Создайте файл `Assets/Plugins/WebGL/SaveSystem.jslib` с функциями:
1. `SaveToLocalStorage(key, value)` — сохранение строки в localStorage
2. `LoadFromLocalStorage(key)` — загрузка строки из localStorage
3. `ShowBrowserNotification(title, body)` — показ уведомления (используйте `window.alert` или `Notification API`)
4. `GetBrowserInfo()` — возвращает JSON с платформой, языком, разрешением экрана
5. `CallUnityMethod(methodName, param)` — вызов C# метода из JS (например, `OnDataLoadedFromJS`)

### Часть B: Реализация C# скрипта
1. Импортируйте все JS-методы через `[DllImport("__Internal")]`
2. В `Start()` вызовите `LoadFromLocalStorage("gameSave")` и передайте результат в Unity
3. Реализуйте `OnDataLoadedFromJS` — парсинг JSON и восстановление `_currentScore` и `_highScore`
4. Реализуйте автосохранение через корутину (сохранять в localStorage через 1 секунду после изменений)
5. В `AddScore()` при превышении `_highScore` вызовите `ShowBrowserNotification`
6. Добавьте обработку `OnApplicationFocus(false)` — ставим `Time.timeScale = 0` и на паузу звук
7. При возвращении фокуса снимаем паузу

### Часть C: Дополнительно (опционально)
- Добавьте обработку кнопки "Экспорт сохранения" — скачать JSON файл через JS (создать Blob и ссылку)
- Добавьте обработку кнопки "Импорт" — загрузить файл через браузер

---

## 🧰 Требования к реализации:
- Используйте `#if !UNITY_EDITOR && UNITY_WEBGL` для всех JS-вызовов
- Добавьте обработку ошибок (если localStorage недоступен)
- Используйте корутины для отложенного сохранения
- JSON-формат сохранения должен содержать `score` и `highscore`

---

## 🔍 Ожидаемый результат:
1. При первом запуске игра показывает "No save found"
2. При наборе очков и достижении рекорда появляется браузерное уведомление
3. После закрытия и повторного открытия страницы прогресс восстанавливается
4. При переключении на другую вкладку игра ставится на паузу
5. В консоли браузера (F12) отображаются логи сохранений

---

## 💡 Подсказки:
```javascript
// Пример функции уведомления в .jslib
ShowBrowserNotification: function(title, body) {
    var t = UTF8ToString(title);
    var b = UTF8ToString(body);
    if (window.Notification && Notification.permission === "granted") {
        new Notification(t, { body: b });
    } else {
        window.alert(t + ": " + b);
    }
}
```

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
