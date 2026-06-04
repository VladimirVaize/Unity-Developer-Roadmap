# 📦 Addressables: Асинхронная загрузка и управление памятью

Этот материал посвящен системе Addressables — современному решению Unity для управления ассетами, которое приходит на смену устаревшей системе `Resources`. 
Вы узнаете об асинхронной загрузке контента, правильном управлении памятью и ключевых отличиях от Resources.

---

## 🆚 Addressables vs Resources: Ключевые отличия

| Характеристика | Resources | Addressables |
| --- | --- | --- |
| Выгрузка из памяти | Только через `Resources.UnloadUnusedAssets()` (медленно, глобально) | Поштучно через `Addressables.Release()` (точно, эффективно) |
| Загрузка | Синхронная (`Resources.Load`) | Асинхронная (`LoadAssetAsync`) |
| Размер пакета | Всё в основном билде | Делится на группы (Content Packs) |
| Поддержка удалённого контента | ❌ Нет | ✅ Да (хостинг на CDN) | 
| Управление зависимостями | Ручное | Автоматическое (Reference Counting) |

Главный недостаток Resources: загруженные префабы нельзя выгрузить по отдельности — только глобальной очисткой, что вызывает фризы.

> Когда использовать Addressables? Для средних и крупных проектов (>200 MB или >500 ассетов), для игр с частыми обновлениями (MMO, GaaS), для проектов с условной загрузкой контента.

---

## ⚡ 1. Асинхронная загрузка (Asynchronous Loading)
### 🎯 Для чего нужно:
Addressables загружает ассеты асинхронно, чтобы не блокировать главный поток игры. Пока текстура скачивается с сервера — игра продолжает работать без фризов.

### 📖 Базовый пример (через корутину)
```csharp
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressablesExample : MonoBehaviour
{
    [SerializeField] private string assetAddress = "characters/hero";

    IEnumerator Start()
    {
        // 1. Начинаем асинхронную загрузку
        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(assetAddress);
        
        // 2. Ждём завершения (yield возвращает управление в следующий кадр)
        yield return handle;
        
        // 3. Проверяем успех
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject hero = handle.Result;
            Instantiate(hero, transform);
        }
        else
        {
            Debug.LogError($"Не удалось загрузить: {assetAddress}");
            Addressables.Release(handle); // Важно: релиз даже при ошибке!
        }
    }
}
```

### 📖 Альтернатива: через события (Event-based)
```csharp
void Start()
{
    AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>("weapons/sword");
    handle.Completed += OnLoadComplete;
}

private void OnLoadComplete(AsyncOperationHandle<GameObject> handle)
{
    if (handle.Status == AsyncOperationStatus.Succeeded)
    {
        Instantiate(handle.Result);
    }
    else
    {
        Debug.LogError("Ошибка загрузки меча");
        Addressables.Release(handle);
    }
}
```

### 📖 Продвинутый вариант: через async/await (C#)
```csharp
public async void LoadWeaponAsync()
{
    try 
    {
        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>("weapons/axe");
        await handle.Task; // Не блокирует поток, но ждёт результата
        
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Instantiate(handle.Result);
        }
        Addressables.Release(handle);
    }
    catch (System.Exception e)
    {
        Debug.LogError($"Ошибка: {e.Message}");
    }
}
```

> [!Warning]
> `AsyncOperationHandle.Task` недоступен на WebGL.

---

## 🧹 2. Управление памятью (Memory Management)
### 🎯 Для чего нужно:
Addressables использует счётчик ссылок (reference counting). 
Каждый вызов `LoadAssetAsync` увеличивает счётчик, каждый вызов `Release` — уменьшает. 
Когда счётчик достигает нуля — ассет выгружается из памяти.

### 🔑 Золотое правило: зеркальные вызовы
Каждый Load должен иметь свой Release:
```csharp
public class WeaponManager : MonoBehaviour
{
    private AsyncOperationHandle<GameObject> _swordHandle;

    public void LoadSword()
    {
        _swordHandle = Addressables.LoadAssetAsync<GameObject>("weapons/sword");
        _swordHandle.Completed += (handle) => 
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
                Instantiate(handle.Result);
        };
    }

    public void UnloadSword()
    {
        // Уменьшаем счётчик ссылок — при нуле ассет выгрузится
        Addressables.Release(_swordHandle);
    }
}
```

### 💡 Правильная работа с InstantiateAsync
Addressables имеет специальный метод `InstantiateAsync`, который автоматически управляет счётчиком для инстансов:
```csharp
private AsyncOperationHandle<GameObject> _instanceHandle;

public void SpawnEnemy()
{
    _instanceHandle = Addressables.InstantiateAsync("enemies/goblin", transform.position, Quaternion.identity);
    _instanceHandle.Completed += (handle) => 
    {
        Debug.Log("Враг появился!");
    };
}

public void DespawnEnemy(GameObject enemy)
{
    Addressables.ReleaseInstance(enemy); // Уменьшает счётчик, при нуле — уничтожает
}
```

### ⚠️ Частые ошибки памяти
```csharp
// ❌ НЕПРАВИЛЬНО: Потеря хендла → утечка памяти
void BadExample()
{
    Addressables.LoadAssetAsync<Texture>("ui/icon"); // Хендл нигде не сохранён!
}

// ❌ НЕПРАВИЛЬНО: Релиз до завершения загрузки
void BadExample2()
{
    var handle = Addressables.LoadAssetAsync<GameObject>("hero");
    Addressables.Release(handle); // Слишком рано — ассет не загрузится
}

// ✅ ПРАВИЛЬНО: Релиз когда ассет больше не нужен
private AsyncOperationHandle<GameObject> _heroHandle;

void LoadHero()
{
    _heroHandle = Addressables.LoadAssetAsync<GameObject>("hero");
    _heroHandle.Completed += (h) => Instantiate(h.Result);
}

void OnDestroy()
{
    Addressables.Release(_heroHandle); // Чистим за собой
}
```

---

## 🛠️ 3. AssetReference — типобезопасные ссылки
### 🎯 Для чего нужно:
Вместо строковых адресов (опечатки → ошибки) используйте `AssetReference`. 
В инспекторе появится удобный выпадающий список только с Addressable-ассетами.

```csharp
using UnityEngine.AddressableAssets;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private AssetReference _mainMenuScene;   // Для сцен
    [SerializeField] private AssetReferenceGameObject _enemyPrefab; // Только GameObject
    [SerializeField] private AssetReferenceTexture2D _uiIcon;      // Только Texture2D

    public async void LoadMenu()
    {
        if (!_mainMenuScene.RuntimeKeyIsValid()) return; // Проверка валидности
        
        var handle = _mainMenuScene.LoadSceneAsync(UnityEngine.SceneManagement.LoadSceneMode.Single);
        await handle.Task;
    }
}
```

---

## 📚 4. Загрузка нескольких ассетов (Labels и группы)
### 🎯 Для чего нужно:
Маркируйте ассеты метками (labels), чтобы загружать их группами.

```csharp
[SerializeField] private List<string> _labels = new List<string>() { "enemies", "bosses" };

public void LoadAllEnemies()
{
    // Загружаем все ассеты с метками "enemies" и "bosses"
    var handle = Addressables.LoadAssetsAsync<GameObject>(
        _labels, 
        null, // опциональный callback для каждого загруженного ассета
        Addressables.MergeMode.Union, // Объединение меток
        false // Не фейлиться при частичной ошибке
    );
    
    handle.Completed += (op) =>
    {
        foreach (var enemyPrefab in op.Result)
        {
            Instantiate(enemyPrefab);
        }
        Addressables.Release(op); // Релиз после использования
    };
}
```

---

## 🧠 Итоговая схема работы
```text
1. Настройка: Помечаем ассет как Addressable (окно Addressables Groups)
2. Загрузка: Addressables.LoadAssetAsync<T>("адрес") → получаем AsyncOperationHandle
3. Ожидание: yield / await / Completed-событие
4. Использование: handle.Result — загруженный ассет
5. Выгрузка: Addressables.Release(handle) — уменьшаем счётчик ссылок
```

---

## 🔍 Отладка: Addressables Event Viewer
Окно `Window > Asset Management > Addressables > Event Viewer` показывает:
- Синяя полоса — ассет загружен
- Зелёная часть — текущий счётчик ссылок
- Белая линия — момент загрузки

> [!Important]
> Для работы просмотрщика включите `Send Profiler Events` в настройках Addressables.

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
