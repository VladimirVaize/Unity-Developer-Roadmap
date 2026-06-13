# 🎯 Задача: «Система загрузки уровней и видеороликов из StreamingAssets»
Вы разрабатываете приключенческую игру. Уровни хранятся в JSON файлах, а видеоролики в MP4 формате. 
Всё это находится в папке `StreamingAssets`. 

Ваша задача — реализовать систему, которая:
1. Загружает список уровней из файла `levels.json` (содержит массив с именами файлов уровней)
2. Загружает конкретный уровень по его имени (JSON файл в папке `levels/`)
3. Воспроизводит интро-видео из `videos/intro.mp4` перед началом игры
4. Кэширует большие файлы на Android во `временную папку` для быстрого доступа
5. Показывает прогресс загрузки для больших файлов

## 📝 Данные для тестирования (создайте в StreamingAssets):

### Файл 1: `StreamingAssets/levels.json`
```json
{
    "levels": ["tutorial.json", "level1.json", "level2.json"],
    "version": 1
}
```

### Файл 2: `StreamingAssets/levels/tutorial.json`
```json
{
    "levelName": "Tutorial",
    "difficulty": 1,
    "enemies": 5,
    "timeLimit": 60
}
```

### Файл 3: `StreamingAssets/levels/level1.json`
```json
{
    "levelName": "Forest",
    "difficulty": 2,
    "enemies": 12,
    "timeLimit": 120
}
```

Видео: Добавьте любой короткий MP4 файл в `StreamingAssets/videos/intro.mp4`

---

## 📋 Конкретные задачи для реализации:
1. Создайте класс `LevelManager`:
   - Метод `IEnumerator LoadLevelsList()` — загружает `levels.json` и парсит список
   - Метод `IEnumerator LoadLevelByName(string levelName)` — загружает конкретный уровень
   - Метод `LevelData GetCurrentLevel()` — возвращает текущий уровень
  
2. Создайте класс `VideoIntroPlayer`:
   - Воспроизводит `intro.mp4` из StreamingAssets перед загрузкой первого уровня
   - Добавьте кнопку "Skip" для пропуска видео
   - Показывайте лоадер во время подготовки видео
  
3. Реализуйте кэширование для Android:
   - В методе `CacheLargeFile(string fileName)` скопируйте файл из StreamingAssets в `Application.persistentDataPath`
   - Используйте флаг `isCached`, чтобы не копировать повторно
   - Показывайте прогресс-бар при копировании больших файлов (>10 MB)
  
4. Добавьте обработку ошибок:
   - Если файл не найден — показать сообщение и загрузить уровень по умолчанию
   - Если видео не загружается — пропустить его автоматически
   - Логируйте все ошибки в консоль
  
5. Потоковая обработка больших JSON (бонус):
   - Если JSON уровень больше 5 MB, читайте его построчно и парсите только нужные поля
  
---

## 🧰 Требования к реализации:
- Используйте `UnityWebRequest` для кроссплатформенности
- Для видео используйте `VideoPlayer` компонент
- Добавьте UI Slider для отображения прогресса загрузки/копирования
- Используйте корутины для всех асинхронных операций
- Комментируйте платформо-зависимый код

---

## 🔍 Проверка работоспособности:
1. В редакторе: Загрузите список уровней, выберите уровень, видео должно проиграться
2. На Android: Проверьте, что файлы корректно читаются через `UnityWebRequest`
3. На десктопе: Проверьте быстрое чтение через `File.ReadAllText`
4. Убедитесь, что при повторном запуске кэшированные файлы не копируются заново

---

## 💡 Ожидаемый вывод в консоли:
```text
[LevelManager] Loading levels.json from StreamingAssets...
[LevelManager] Found 3 levels: tutorial, level1, level2
[LevelManager] Loading tutorial.json...
[LevelManager] Level loaded: Tutorial (Difficulty 1, Enemies 5, Time 60s)
[VideoIntroPlayer] Preparing intro.mp4...
[VideoIntroPlayer] Video ready, playing...
[VideoIntroPlayer] Video finished, starting game...
```

```text
[Android] Caching tutorial.json to persistentDataPath...
[Android] Copy progress: 100%
[Android] File cached successfully
```

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
