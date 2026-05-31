# 🧪 Практическое задание: Внедрение зависимостей (DI) с Zenject / Extenject

Цель: Закрепить навыки работы с DI-контейнером Zenject (Extenject) в Unity. 
Вы создадите систему управления игровыми сервисами, научитесь регистрировать 
зависимости и использовать инъекции в различных сценариях.

---

## 📥 Исходные условия
У вас есть проект Unity, в котором необходимо реализовать несколько игровых сервисов:
- Сервис подсчёта очков (`IScoreService`)
- Сервис сохранения данных (`ISaveService`)
- Сервис генерации врагов (`IEnemySpawnService`)

Все сервисы должны быть зарегистрированы в DI-контейнере Zenject.

---

## 🎯 Задачи
### Этап 1: Установка Zenject / Extenject
1. Установите Extenject через Package Manager (URL: https://github.com/Mathijs-Bakker/Extenject.git?path=UnityProject/Assets/Plugins/Zenject/Source)
2. Или скачайте `.unitypackage` с <a href="https://github.com/Mathijs-Bakker/Extenject/releases">Releases</a>.

### Этап 2: Создание сервисов и их регистрация
1. Создайте интерфейс `IScoreService` с методами:
   - `void AddScore(int points)`
   - `int GetScore()`
   - `void ResetScore()`
  
2. Создайте реализацию `ScoreService`, которая хранит текущий счёт в приватном поле.
3. Создайте интерфейс `ISaveService` с методами:
   - `void SaveInt(string key, int value)`
   - `int LoadInt(string key, int defaultValue)`
  
4. Создайте реализацию `PlayerPrefsSaveService`, использующую `PlayerPrefs`.
5. Создайте интерфейс `IEnemySpawnService` с методом `void SpawnEnemy(Vector3 position)`.
6. Создайте реализацию `EnemySpawnService`, которая инстанцирует врага из префаба.
7. Создайте Installer (`GameServicesInstaller : MonoInstaller`) и зарегистрируйте все три сервиса:
   - `IScoreService` как синглтон (`.AsSingle()`)
   - `ISaveService` как синглтон
   - `IEnemySpawnService` как синглтон
  
### Этап 3: Использование инъекций в MonoBehaviour
1. Создайте скрипт `GameManager` на сцене.
2. Через поле-инъекцию (`[Inject]`) получите все три сервиса.
3. В методе `Start()`:
   - Загрузите сохранённый счёт через `ISaveService`.
   - Если счёта нет, установите 0.
   - Выведите текущий счёт в консоль.
  
4. Добавьте обработку нажатия клавиши `Space`:
   - При нажатии прибавляйте 10 очков через `IScoreService`.
   - Сохраняйте новое значение через `ISaveService`.
   - Спавните врага через `IEnemySpawnService` в случайной позиции.
  
### Этап 4: Constructor Injection в обычном C# классе
1. Создайте обычный C# класс `ScoreDisplay` (не `MonoBehaviour`).
2. Добавьте конструктор, принимающий `IScoreService`.
3. Добавьте метод `void DisplayScore()`, который выводит счёт в консоль.
4. Зарегистрируйте `ScoreDisplay` в контейнере через `.AsSingle()`.
5. В `GameManager` внедрите `ScoreDisplay` через конструктор (для этого `GameManager` должен быть зарегистрирован в контейнере — самый простой способ: добавьте к нему `[Inject]` поле).

### Этап 5: Использование фабрики для динамического создания объектов
1. Создайте интерфейс `IPickupItem` с методом `void Collect()`.
2. Создайте класс `CoinPickup : IPickupItem`, реализующий `Collect()` (прибавляет очки).
3. Создайте фабрику для `CoinPickup`:
```csharp
public class CoinPickupFactory : PlaceholderFactory<CoinPickup> { }
```

4. Зарегистрируйте фабрику в Installer:
```csharp
Container.BindFactory<CoinPickup, CoinPickupFactory>();
```

5. В `EnemySpawnService` после спавна врага создайте монетку через фабрику и разместите рядом с врагом.

### Этап 6: Валидация и отладка
1. В редакторе Unity выберите `Zenject → Validate Current Scene`.
2. Убедитесь, что все зависимости корректно разрешаются.
3. Если есть ошибки — исправьте их.

---

## ⭐ Дополнительное задание (со звёздочкой)
1. Создайте альтернативную реализацию `ISaveService` — `FileSaveService`, которая сохраняет данные в JSON-файл.
2. Реализуйте условную привязку: в зависимости от символа препроцессора (`UNITY_EDITOR`) регистрируйте `PlayerPrefsSaveService` или `FileSaveService`.

```csharp
#if UNITY_EDITOR
    Container.Bind<ISaveService>().To<PlayerPrefsSaveService>().AsSingle();
#else
    Container.Bind<ISaveService>().To<FileSaveService>().AsSingle();
#endif
```

---

## ✅ Критерии успеха
- Все сервисы зарегистрированы в Installer.
- `GameManager` корректно получает зависимости через `[Inject]`.
- При нажатии `Space` очки увеличиваются, сохраняются, враг спавнится.
- `ScoreDisplay` (обычный C# класс) успешно внедрён и выводит счёт.
- Фабрика создаёт монетки без ошибок.
- Валидация сцены (Zenject → Validate) не выдаёт ошибок.
- (⭐) Условная привязка работает корректно в редакторе и в билде.

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
