# 🎯 Задача: «Система сохранения прогресса RPG»
Вы разрабатываете RPG игру. Вам нужно реализовать полную систему сохранения и загрузки с JSON (Newtonsoft.JSON), шифрованием (AES) и бинарными слотами.

## 📝 Структура данных для сохранения:
```csharp
public class RPGPlayerData
{
    public string playerName;
    public int level;
    public int experience;
    public float currentHealth;
    public float maxHealth;
    public List<string> unlockedAbilities;
    public Dictionary<string, int> inventory; // itemId -> quantity
    public Vector3 lastPosition;
    public float playTimeSeconds;
}
```

---

## 📋 Задачи для реализации:
1. Сериализация — сохранять объект `RPGPlayerData` в JSON через Newtonsoft.JSON
2. Шифрование — шифровать JSON-строку алгоритмом AES (ключ и IV возьмите из примера выше)
3. Бинарное сохранение — записывать зашифрованные байты в файл с расширением `.rpgdata`
4. Загрузка — читать файл, расшифровывать, десериализовывать в объект
5. Слоты сохранений — поддержка до 3 слотов (slot0, slot1, slot2) в папке `Application.persistentDataPath`
6. Снимок экрана — перед сохранением делать скриншот и сохранять его как PNG (в отдельной папке `screenshots/`)

---

## 🧰 Требования к реализации:
- Создайте класс `RPGSaveManager` с методами:
  - `SaveGame(int slot, RPGPlayerData data)`
  - `RPGPlayerData LoadGame(int slot)`
  - `bool HasSaveInSlot(int slot)`
  - `DeleteSave(int slot)`
  - `CaptureAndSaveScreenshot(int slot)` (делает скриншот 256x256)
 
- Все ошибки (отсутствие файла, повреждённые данные) обрабатывайте через `try-catch`
- При загрузке проверяйте, что файл не повреждён (расшифровка валидна)
- Храните хеш (MD5 или SHA256) сохранения для проверки целостности

---

## 📁 Ожидаемая структура файлов:
```text
persistentDataPath/
├── saves/
│   ├── slot0.rpgdata
│   ├── slot1.rpgdata
│   └── slot2.rpgdata
└── screenshots/
    ├── slot0.png
    ├── slot1.png
    └── slot2.png
```

---

## 🎮 Дополнительный вызов (опционально):
Добавьте автосохранение каждые 60 секунд в текущий активный слот, но не во время боя (флаг `isInCombat`).

---

## 💡 Подсказки:
- Для скриншотов используйте `ScreenCapture.CaptureScreenshot()` или `Texture2D.ReadPixels()` + `EncodeToPNG()`
- Для хеша: `System.Security.Cryptography.MD5.Create().ComputeHash()`
- При повреждении сохранения создавайте новый объект со значениями по умолчанию

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
