# 🌐 Mirror и Photon: Альтернативные сетевые решения для мультиплеера в Unity

Создание многопользовательской игры — одна из самых сложных задач в геймдеве. 
Unity не предоставляет встроенного "коробочного" решения для сетевого кода, поэтому разработчики обращаются к сторонним библиотекам и сервисам. 
Два самых популярных и принципиально разных подхода — это Mirror (бесплатный, open-source, полный контроль) и Photon (коммерческий сервис с готовой инфраструктурой).

---

## 1. Введение: Проблема выбора
В мире Unity для мультиплеера сложились два основных лагеря. С одной стороны — решения с самостоятельным хостингом, где вы полностью контролируете серверный код и инфраструктуру. 
С другой — готовые сервисы (SaaS), которые берут на себя управление серверами, но ограничивают свободу и требуют оплаты.

> Согласно сравнению популярных решений, "лучший движок для мультиплеера Unity — тот, который подходит вашей игре.
> Физически насыщенная игра с синхронизацией тысяч объектов предъявляет другие требования, чем пошаговая карточная игра"

### Ключевые вопросы для выбора:

| Вопрос | Варианты |
| --- | --- |
| Кто управляет серверами? | Самостоятельно (Mirror) / Photon Cloud (Photon) |
| Бюджет | Бесплатно (Mirror) / Оплата за CCU (Photon) |
| Контроль над кодом | Полный (Mirror) / Ограниченный (Photon) |
| Сложность реализации | Высокая (Mirror) / Низкая (Photon) |
| Предсказание и компенсация задержки | Реализовать самому (Mirror) / Встроено (Photon) |

---

## 2. Mirror: Свобода и контроль
Mirror — это open-source библиотека для создания многопользовательских игр с клиент-серверной архитектурой. 
Она является духовным наследником старого UNET от Unity и предлагает полный контроль над сетевым кодом.

### 🔧 Основные характеристики:
| Характеристика | Описание |
| --- | --- |
| Тип | Open-source библиотека |
| Лицензия | MIT (бесплатно) |
| Архитектура | Клиент-серверная (Server Authoritative) |
| Хостинг | Самостоятельный (вы управляете серверами) |
| Лимиты | Нет (зависит от вашей инфраструктуры) |
| Контроль | Полный доступ к исходному коду |

### 📦 Ключевые компоненты Mirror:
1. NetworkManager — управление соединениями, сценами, спавном
2. NetworkIdentity — идентифицирует сетевые объекты
3. NetworkBehaviour — базовый класс для сетевых скриптов
4. NetworkTransform — синхронизация позиции/ротации
5. RPC (Remote Procedure Calls) — вызов методов на всех клиентах
6. SyncVars — автоматическая синхронизация переменных

### 💻 Базовый пример: Движение игрока
```csharp
using Mirror;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float _speed = 5f;
    
    // SyncVar автоматически синхронизируется между всеми клиентами
    [SyncVar] private Color _playerColor = Color.white;
    
    void Update()
    {
        // isLocalPlayer — true только для локального игрока
        if (!isLocalPlayer) return;
        
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        Vector3 direction = new Vector3(horizontal, 0, vertical) * _speed * Time.deltaTime;
        transform.Translate(direction);
        
        // Отправляем команду на сервер для смены цвета (например, по нажатию)
        if (Input.GetKeyDown(KeyCode.C))
        {
            ChangeColorCmd(Random.ColorHSV());
        }
    }
    
    // Command — вызывается клиентом, выполняется на сервере
    [Command]
    private void ChangeColorCmd(Color newColor)
    {
        _playerColor = newColor;
        // ClientRpc — выполняется на всех клиентах
        ApplyColorRpc(newColor);
    }
    
    // ClientRpc — вызывается сервером, выполняется на всех клиентах
    [ClientRpc]
    private void ApplyColorRpc(Color color)
    {
        GetComponent<Renderer>().material.color = color;
    }
}
```

### 🚀 Преимущества Mirror:
1. Полный контроль над серверным кодом — вы можете реализовать любую логику
2. Отсутствие лимитов по CCU — количество игроков ограничено только мощностью вашего сервера
3. Гибкость транспорта — можно использовать UDP, TCP, Steam Networking, LiteNetLib
4. Бесплатно — нет скрытых платежей за масштабирование

### ⚠️ Недостатки Mirror:
1. Требует знаний сетевого программирования — сложнее в освоении
2. Самостоятельное управление инфраструктурой — нужно настраивать серверы, балансировку, безопасность
3. Нет встроенного предсказания и компенсации задержки — нужно реализовывать самому
4. Потенциально более высокий трафик — исследования показывают, что Mirror может генерировать больше пакетов по сравнению с Photon

### 🏗️ Пример: Комнатная система с Mirror
Для создания системы комнат (как в Photon) на Mirror используется подход с аддитивными сценами. 
Существуют готовые решения, например, Multi-Room Manager, который позволяет создавать изолированные комнаты на одном сервере
```csharp
// Пример создания комнаты в Mirror (упрощённо)
public class RoomManager : NetworkBehaviour
{
    [Server]
    public void CreateRoom(string roomName, int maxPlayers)
    {
        // Загрузка аддитивной сцены для комнаты
        SceneManager.LoadScene("RoomScene", LoadSceneMode.Additive);
        // Настройка изоляции сетевого трафика для этой комнаты
        // ...
    }
}
```

---

## 3. Photon (PUN и Fusion): Готовый сервис
Photon — это коммерческий сервис для многопользовательских игр, который предоставляет готовую облачную инфраструктуру. 
Самая популярная реализация — Photon Unity Networking (PUN) и её эволюция — Photon Fusion.

### 🔧 Основные характеристики:

| Характеристика | Описание |
| --- | --- |
| Тип | SaaS (облачный сервис) |
| Лицензия | Freemium (бесплатно до 20 CCU) |
| Архитектура | Клиент-серверная (облачная) |
| Хостинг | Photon Cloud (управляется Photon) |
| Лимиты | По количеству CCU (зависит от тарифа) |
| Контроль | Ограниченный (нельзя изменять серверный код) |

### 📦 Решения Photon:

| Решение | Описание | Лучше всего подходит для |
| --- | --- | --- |
| PUN 2 (Photon Unity Networking) | Классическая реализация с комнатами и RPC | Игры с 2-8 игроками на комнату |
| Photon Fusion | Современное решение с предсказанием, роллбэком, компенсацией задержки | Соревновательные игры, шутеры, экшены |
| Photon Quantum | Детерминированный симулятор (целочисленная физика) | Стратегии, файтинги, точные симуляции |

### 💻 Базовый пример: Подключение к комнате в PUN
```csharp
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonConnection : MonoBehaviourPunCallbacks
{
    void Start()
    {
        // Подключение к Photon Cloud
        PhotonNetwork.ConnectUsingSettings();
    }
    
    public override void OnConnectedToMaster()
    {
        Debug.Log("Подключено к Photon Cloud");
        
        // Вход в случайную комнату или создание новой
        PhotonNetwork.JoinRandomRoom();
    }
    
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Нет доступных комнат, создаём новую...");
        
        RoomOptions options = new RoomOptions
        {
            MaxPlayers = 4,
            IsVisible = true,
            IsOpen = true
        };
        
        PhotonNetwork.CreateRoom("Room_" + Random.Range(0, 1000), options);
    }
    
    public override void OnJoinedRoom()
    {
        Debug.Log($"Вошли в комнату: {PhotonNetwork.CurrentRoom.Name}");
        
        // Спавн игрока в комнате
        GameObject player = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
    }
}
```

### 🚀 Преимущества Photon:
1. Не нужно управлять серверами — Photon Cloud делает всё за вас
2. Встроенное предсказание и компенсация задержки (в Fusion)
3. Готовая система комнат и матчмейкинга
4. Отличная документация и сообщество — один из самых популярных решений
5. Кросс-платформенность — работает на всех платформах Unity

### ⚠️ Недостатки Photon:
1. Ограниченный контроль — вы не можете изменить серверный код
2. Платная модель — при росте игроков растут расходы (примерно $95/мес за 100 CCU)
3. Ограничение на бесплатном тарифе — до 20 CCU
4. Более высокое потребление трафика — исследования показывают, что Photon может использовать больше трафика при синхронизации Transform

---

## 4. Сравнение Mirror и Photon в цифрах
Согласно исследованию, проведённому в Университете Турку, производительность Mirror и Photon PUN 2 была сравнена в идентичных условиях

### 📊 Результаты производительности (50 игроков):

| Метрика | Mirror | Photon PUN 2 |
| --- | --- | --- |
| FPS (десктоп) | ~574 fps | ~981 fps |
| CPU (клиент) | 30-40% | 30-40% |
| Latency | ~34 ms | ~11 ms |
| Пакетов в секунду | ~11,900 | ~8,200 |
| Средний размер пакета | ~757 байт | ~1005 байт |
| GC Alloc | Растёт с игроками | Стабилен |

> Ключевое наблюдение: Photon показал более высокую производительность и стабильность в бенчмарках.
> Однако исследователи отметили, что на высоких нагрузках (40-50 игроков)
> Photon демонстрировал заметные задержки в синхронизации Transform,
> в то время как Mirror оставался стабильнее с точки зрения пользовательского опыта

### 🎯 Практический пример: Выбор решения
```csharp
// Служебный класс для демонстрации выбора архитектуры
public static class NetworkArchitectureDecider
{
    public static string RecommendSolution(GameRequirements requirements)
    {
        if (requirements.IsCompetitive && requirements.RequiresLagCompensation)
            return "Photon Fusion (встроенное предсказание и роллбэк)";
        
        if (requirements.Budget == 0 && requirements.CanSelfHost)
            return "Mirror (бесплатно, полный контроль)";
        
        if (requirements.ExpectedPlayers > 100)
            return "Mirror + AWS (экономически выгоднее при масштабе)";
        
        if (requirements.NeedsQuickPrototype && requirements.HasNoServerExperience)
            return "Photon PUN (быстрый старт, готовая инфраструктура)";
        
        if (requirements.RequiresCustomServerLogic)
            return "Mirror (можно изменять любой код)";
        
        return "Рекомендуется начать с Photon PUN для MVP, затем оценить миграцию на Mirror при росте";
    }
}

public class GameRequirements
{
    public bool IsCompetitive;
    public bool RequiresLagCompensation;
    public float Budget;
    public bool CanSelfHost;
    public int ExpectedPlayers;
    public bool NeedsQuickPrototype;
    public bool HasNoServerExperience;
    public bool RequiresCustomServerLogic;
}
```

---

## 5. Интеграция со Steam
Оба решения могут интегрироваться со Steam для использования его социальных функций:
### 🎮 Mirror + Steamworks.NET
Существуют готовые шаблоны для интеграции Mirror со Steam, использующие Steam Relay для P2P-соединений
```csharp
// Пример из шаблона Mirror + Steamworks
// Использование Steam Lobby вместо стандартного NetworkManager
// Создание комнат через Steam, приглашения друзей
// Steam Relay для NAT-пробивания
```

### 🎮 Photon + Steam
Photon можно использовать вместе со Steam для:
- Авторизации через Steam
- Приглашений через Steam Overlay
- Сети Photon для игрового трафика (более надёжно, чем Steam P2P)

---

## 6. Лучшие практики и рекомендации
### ✅ Рекомендации по выбору:
1. Быстрый старт / MVP → Photon PUN
2. Соревновательная игра с предсказанием → Photon Fusion
3. Полный контроль, кастомизация, бюджет 0 → Mirror
4. Крупный проект с собственными серверами → Mirror + AWS
5. Исследовательский / академический проект → Mirror (open-source, можно изучать)

### ⚠️ Частые ошибки:
```csharp
// ❌ ОШИБКА: Смешивание PUN и Mirror в одном проекте
// Фреймворки конфликтуют из-за разных подходов к сетевой синхронизации

// ❌ ОШИБКА: Не учитывать CCU-лимиты Photon
// Бесплатный тариф Photon — только 20 CCU [citation:5]

// ❌ ОШИБКА: Игнорирование накладных расходов трафика
// Photon может потреблять больше трафика при синхронизации Transform [citation:6]

// ❌ ОШИБКА: Неправильная настройка сервера Mirror под нагрузку
// Требуется правильная конфигурация AWS / выделенного сервера [citation:7]

// ✅ ПРАВИЛЬНО: Провести стресс-тесты перед релизом
// Использовать Server Build для тестирования производительности [citation:6]
```

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
