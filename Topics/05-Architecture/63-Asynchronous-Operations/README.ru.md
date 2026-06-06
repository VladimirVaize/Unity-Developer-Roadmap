# ⏳ Асинхронные операции в Unity: Загрузка без зависаний
Этот материал покрывает ключевые механизмы асинхронной работы в Unity: 
`AsyncOperation`, `yield return`, обратные вызовы (callbacks), `ResourceRequest` и `AssetBundleRequest`. 
Эти инструменты позволяют загружать сцены, ресурсы и AssetBundle без блокировки основного потока и «зависаний» (фризов) игры.

---

## 📖 1. AsyncOperation (Базовый класс асинхронных операций)
### 🎯 Для чего нужно:
`AsyncOperation` — это базовый класс для всех асинхронных операций в Unity (загрузка сцены, выгрузка, загрузка AssetBundle и т.д.). 
Он позволяет отслеживать прогресс операции и выполнять действия после её завершения, не блокируя игровой процесс.

### ⚙️ Как использовать:
```csharp
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private AsyncOperation asyncLoad;

    public void LoadSceneAsync(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        // Начинаем асинхронную загрузку сцены
        asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        
        // Запрещаем активацию сцены сразу (чтобы показать экран загрузки)
        asyncLoad.allowSceneActivation = false;

        // Ждём, пока прогресс не достигнет 90% (условно "загрузка завершена")
        while (asyncLoad.progress < 0.9f)
        {
            Debug.Log($"Прогресс загрузки: {asyncLoad.progress * 100}%");
            yield return null; // ждём один кадр
        }

        Debug.Log("Загрузка завершена! Активируем сцену...");
        asyncLoad.allowSceneActivation = true;
    }
}
```

### 📌 Пример из реальной жизни:
Вы делаете RPG с большими уровнями. При переходе между зонами показывается экран загрузки с прогресс-баром, а сама игра продолжает работать (не зависает).

---

## 🔄 2. yield return (Корутины и ожидание)
### 🎯 Для чего нужно:
`yield return` — это инструкция в корутинах (IEnumerator), которая говорит Unity «приостанови выполнение этого метода до следующего кадра (или до другого события)». 
Это основа асинхронного кода в Unity без многопоточности.

### ⚙️ Как использовать:
```csharp
using System.Collections;
using UnityEngine;

public class CoroutineExamples : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(ExampleCoroutine());
    }

    IEnumerator ExampleCoroutine()
    {
        Debug.Log("Начало корутины - кадр 1");
        
        // Ждём 2 секунды (реального времени)
        yield return new WaitForSeconds(2f);
        Debug.Log("Прошло 2 секунды");
        
        // Ждём 1 кадр (до следующего Update)
        yield return null;
        Debug.Log("Следующий кадр");
        
        // Ждём завершения другой асинхронной операции
        AsyncOperation op = SceneManager.LoadSceneAsync("Menu");
        yield return op; // корутина приостановится, пока сцена не загрузится
        Debug.Log("Сцена загружена!");
        
        // Ждём до конца кадра (после всех Update и рендеринга)
        yield return new WaitForEndOfFrame();
        Debug.Log("Конец кадра, можно делать скриншот");
    }
}
```

### 📌 Пример из реальной жизни:
Вы спавните 100 врагов, но не всех в одном кадре (иначе будет фриз). 
Используете `yield return null` после каждого спавна, чтобы распределить нагрузку на несколько кадров.

---

## 📞 3. Обратные вызовы (Callbacks)
### 🎯 Для чего нужно:
Обратные вызовы (callbacks) — это функции, которые передаются как параметры и вызываются после завершения асинхронной операции. 
Они позволяют организовать код без «адской вложенности» корутин.

### ⚙️ Как использовать:
```csharp
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CallbackExample : MonoBehaviour
{
    // Создаём делегат для обратного вызова (или используем Action)
    public void LoadSceneWithCallback(string sceneName, Action onComplete)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName, onComplete));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName, Action onComplete)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
            yield return null;

        op.allowSceneActivation = true;
        
        // Ждём один кадр после активации, чтобы сцена успела инициализироваться
        yield return null;
        
        // Вызываем callback, если он был передан
        onComplete?.Invoke();
        Debug.Log("Callback вызван!");
    }

    void Start()
    {
        LoadSceneWithCallback("GameLevel", () =>
        {
            Debug.Log("Уровень загружен! Можно инициализировать игрока.");
            FindObjectOfType<Player>().Initialize();
        });
    }
}
```

### 📌 Пример из реальной жизни:
Вы загружаете префаб диалогового окна из Resources и после загрузки вызываете callback, который подписывает кнопки на события и показывает окно.

---

## 📦 4. ResourceRequest (Асинхронная загрузка из Resources)
### 🎯 Для чего нужно:
`ResourceRequest` — это наследник `AsyncOperation`, который возвращается при асинхронной загрузке ресурсов из папки `Resources`. 
Позволяет загружать префабы, текстуры, звуки и т.д. без блокировки.

### ⚙️ Как использовать:
```csharp
using System.Collections;
using UnityEngine;

public class ResourceRequestExample : MonoBehaviour
{
    public string prefabPath = "Enemies/Goblin";
    
    IEnumerator Start()
    {
        // Асинхронно загружаем префаб из папки Resources
        ResourceRequest request = Resources.LoadAsync<GameObject>(prefabPath);
        
        // Ждём завершения загрузки
        yield return request;
        
        // Получаем загруженный объект
        GameObject goblinPrefab = request.asset as GameObject;
        
        if (goblinPrefab != null)
        {
            Instantiate(goblinPrefab, Vector3.zero, Quaternion.identity);
            Debug.Log("Гоблин загружен и создан!");
        }
        
        // Альтернатива с обратным вызовом
        StartCoroutine(LoadWithCallback("Weapons/Sword", (asset) =>
        {
            Instantiate(asset);
            Debug.Log("Меч загружен!");
        }));
    }
    
    IEnumerator LoadWithCallback(string path, System.Action<Object> callback)
    {
        ResourceRequest request = Resources.LoadAsync<GameObject>(path);
        yield return request;
        callback?.Invoke(request.asset);
    }
}
```

### 📌 Пример из реальной жизни:
У вас 50 разных врагов. В момент появления врага вы асинхронно загружаете его префаб из `Resources/Enemies/`, чтобы не держать всех в памяти одновременно.

---

## 🧩 5. AssetBundleRequest (Асинхронная загрузка из AssetBundle)
### 🎯 Для чего нужно:
`AssetBundleRequest` — наследник `AsyncOperation` для асинхронной загрузки ассетов из AssetBundle 
(пакеты ресурсов, которые можно загружать отдельно от основного приложения, например, DLC или контент с сервера).

### ⚙️ Как использовать:
```csharp
using System.Collections;
using UnityEngine;

public class AssetBundleExample : MonoBehaviour
{
    public string bundleURL = "https://myserver.com/character_bundle";
    public string assetName = "Hero";

    IEnumerator LoadAssetFromBundle()
    {
        // 1. Асинхронно загружаем AssetBundle из интернета или с диска
        using (WWW www = new WWW(bundleURL))
        {
            yield return www; // ждём загрузки бандла
            
            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.LogError($"Ошибка загрузки бандла: {www.error}");
                yield break;
            }
            
            // 2. Получаем AssetBundle из загруженных данных
            AssetBundle bundle = www.assetBundle;
            
            // 3. Асинхронно загружаем конкретный ассет из бандла
            AssetBundleRequest request = bundle.LoadAssetAsync<GameObject>(assetName);
            yield return request;
            
            // 4. Получаем и используем загруженный объект
            GameObject hero = request.asset as GameObject;
            if (hero != null)
            {
                Instantiate(hero, Vector3.zero, Quaternion.identity);
                Debug.Log("Герой загружен из AssetBundle!");
            }
            
            // 5. Выгружаем бандл, чтобы освободить память
            bundle.Unload(false);
        }
    }
    
    IEnumerator Start()
    {
        yield return StartCoroutine(LoadAssetFromBundle());
    }
}
```

### 📌 Пример из реальной жизни:
Вы выпускаете обновление с новой моделью машины. Сервер отдаёт AssetBundle, игра асинхронно качает его и подгружает модель без перезагрузки приложения.

---

## 🧠 Сравнение всех подходов (Таблица)

| Механизм | Где используется | Блокирует игру? | Идеален для |
| --- | --- | --- | --- |
| `AsyncOperation` | Загрузка сцен, выгрузка | Нет | Экраны загрузки |
| `yield return` | Корутины, ожидание условий | Нет (приостанавливает только корутину) | Спавн объектов, таймеры |
| Callbacks (Action) | Уведомление о завершении | Нет | Организация последовательных операций |
| `ResourceRequest` | Загрузка из папки Resources | Нет | Префабы, конфиги, звуки |
| `AssetBundleRequest` | Загрузка из AssetBundle | Нет | DLC, контент с сервера, моды |

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
