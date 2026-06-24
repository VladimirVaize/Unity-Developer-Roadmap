# 🌐 Netcode for GameObjects (NGO): Авторитетный сервер, клиент-серверная архитектура, RPC
Netcode for GameObjects (NGO) — это официальный высокоуровневый SDK от Unity для создания многопользовательских игр с архитектурой клиент-сервер. 
Он построен поверх Unity Transport Layer и предоставляет инструменты для синхронизации состояния, вызова удалённых процедур (RPC) и управления сетевыми объектами.

---

## 1. Базовые компоненты NGO
### 🧩 NetworkManager — Центральный узел сети
`NetworkManager` — это обязательный компонент, который управляет всеми сетевыми настройками и сессиями. 
Он содержит конфигурацию транспорта, префабы игроков и методы для запуска сессии.

Настройка NetworkManager:
1. Создайте пустой GameObject и назовите его `NetworkManager`
2. Добавьте компонент `NetworkManager` (Netcode → NetworkManager)
3. В инспекторе выберите `UnityTransport` в качестве протокола
4. Назначьте префаб игрока в поле `PlayerPrefab`

Запуск сессии:
```csharp
using Unity.Netcode;
using UnityEngine;

public class NetworkStarter : MonoBehaviour
{
    private NetworkManager m_NetworkManager;

    void Start()
    {
        m_NetworkManager = GetComponent<NetworkManager>();
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (!m_NetworkManager.IsClient && !m_NetworkManager.IsServer)
        {
            if (GUILayout.Button("Host")) m_NetworkManager.StartHost();     // Сервер + клиент
            if (GUILayout.Button("Server")) m_NetworkManager.StartServer(); // Только сервер
            if (GUILayout.Button("Client")) m_NetworkManager.StartClient(); // Только клиент
        }
        else
        {
            var mode = m_NetworkManager.IsHost ? "Host" : 
                       m_NetworkManager.IsServer ? "Server" : "Client";
            GUILayout.Label("Mode: " + mode);
        }
        GUILayout.EndArea();
    }
}
```

### 🧩 NetworkObject — Сетевой объект
`NetworkObject` — это компонент, который делает GameObject сетевым. 
Без него объект не будет синхронизироваться между клиентами.

Ключевые свойства:
- `NetworkObjectId` — уникальный идентификатор объекта в сети
- `IsOwner` — принадлежит ли объект локальному клиенту
- `IsSpawned` — спавнен ли объект в сети

```csharp
public class PlayerController : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        // Проверяем, является ли этот объект локальным игроком
        if (IsOwner)
        {
            Debug.Log("Это мой игрок!");
            // Активируем управление только для владельца
        }
    }
}
```

> [!Important]
> `NetworkObject` должен быть на корневом объекте префаба.
> Не создавайте префабы с вложенными `NetworkObject`.

### 🧩 NetworkBehaviour — Сетевое поведение
`NetworkBehaviour` — это базовый класс для всех сетевых скриптов. Он наследуется от `MonoBehaviour` и даёт доступ к сетевым функциям.

Ключевые свойства и методы:
- `IsServer` — выполняется ли код на сервере
- `IsClient` — выполняется ли код на клиенте
- `IsHost` — является ли процесс хостом (сервер + клиент)
- `IsOwner` — принадлежит ли объект локальному клиенту
- `OnNetworkSpawn()` — вызывается при спавне объекта в сети
- `OnNetworkDespawn()` — вызывается при деспавне объекта

```csharp
public class HealthComponent : NetworkBehaviour
{
    public NetworkVariable<int> health = new NetworkVariable<int>(100);
    
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            health.Value = 100;
        }
    }
    
    [ServerRpc]
    public void TakeDamageServerRpc(int damage)
    {
        if (!IsServer) return;
        health.Value -= damage;
        if (health.Value <= 0)
        {
            // Логика смерти
        }
    }
}
```

---

## 2. Авторитетный сервер (Server-Authoritative Architecture)
В архитектуре с авторитетным сервером все важные решения принимаются на сервере. 
Клиенты отправляют запросы на действия, а сервер проверяет их валидность и применяет изменения.

### 🔑 Ключевые принципы:

| Принцип | Описание |
| --- | --- |
| Сервер — источник истины | Все состояния синхронизируются от сервера к клиентам |
| Клиент — только отображение | Клиенты показывают состояние, но не принимают решений |
| Валидация на сервере | Сервер проверяет все действия клиентов на корректность |
| Античит | Авторитетный сервер защищает от взлома и читов |

### 🎮 Пример: Движение игрока с серверной валидацией
```csharp
public class AuthoritativePlayerMovement : NetworkBehaviour
{
    private float moveSpeed = 5f;
    private Vector3 targetPosition;
    
    void Update()
    {
        if (!IsOwner) return;
        
        // Собираем ввод только на клиенте
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        if (horizontal != 0 || vertical != 0)
        {
            Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized;
            // Отправляем запрос на движение серверу
            RequestMoveServerRpc(moveDirection);
        }
    }
    
    [ServerRpc]
    private void RequestMoveServerRpc(Vector3 direction)
    {
        // Сервер проверяет и применяет движение
        Vector3 newPosition = transform.position + direction * moveSpeed * Time.deltaTime;
        
        // Проверка коллизий (на сервере)
        if (IsPositionValid(newPosition))
        {
            transform.position = newPosition;
            // Синхронизация позиции со всеми клиентами
            UpdatePositionClientRpc(transform.position);
        }
    }
    
    [ClientRpc]
    private void UpdatePositionClientRpc(Vector3 position)
    {
        // Все клиенты получают обновлённую позицию от сервера
        transform.position = position;
    }
    
    private bool IsPositionValid(Vector3 position)
    {
        // Проверка на валидность позиции (на сервере)
        return true; // Упрощённо
    }
}
```

### 🌐 Схема работы авторитетного сервера:
```text
Клиент A                    Сервер                    Клиент B
    |                          |                          |
    | -- Действие игрока -->   |                          |
    |                          | -- Проверка на сервере   |
    |                          | -- Обновление состояния  |
    | <-- Новая позиция ---    |                          |
    |                          | -- Новая позиция -->     |
    |                          |                          |
```

> 💡 Преимущества: Безопасность, единое состояние для всех игроков, защита от читов.

> ⚠️ Недостатки: Задержка между действием и отображением (исправляется клиент-сайд предикшеном).

### 🏗️ Отказ от авторитетного клиента (Non-Authoritative Server)
При использовании неавторитетного сервера клиенты сами принимают решения и отправляют результаты серверу для рассылки другим клиентам. Это проще в реализации, но уязвимо для читов.

| Тип сервера | Преимущества | Недостатки |
| --- | --- | --- |
| Авторитетный | Безопасный, античит | Сложнее реализовать, задержки |
| Неавторитетный | Проще реализовать | Уязвим для читов, небезопасный |

---

## 3. Клиент-серверная архитектура (Client-Server Architecture)
NGO поддерживает два режима работы:

### 🏠 Host (Хост)
- Один игрок является одновременно сервером и клиентом
- Идеально для небольших игр (до 8-16 игроков)
- Простой в настройке для разработки

### 🖥️ Server (Сервер)
- Выделенный сервер без графического интерфейса
- Для больших игр (до 100+ игроков)
- Более производительный и масштабируемый

### 🎮 Пример: Выбор режима
```csharp
public class ConnectionManager : MonoBehaviour
{
    private NetworkManager netManager;
    
    void Start()
    {
        netManager = GetComponent<NetworkManager>();
    }
    
    public void StartAsHost()
    {
        netManager.StartHost(); // Запускает и сервер, и клиент
    }
    
    public void StartAsServer()
    {
        netManager.StartServer(); // Только сервер, без игрока
    }
    
    public void StartAsClient(string ipAddress)
    {
        // Подключаемся к серверу по IP
        netManager.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>()
                  .SetConnectionData(ipAddress, 7777);
        netManager.StartClient();
    }
}
```

### 🚀 Миграция с Host на Server
При переходе с клиент-хоста на выделенный сервер необходимо учитывать:
1. Замените `StartHost` на `StartServer` — сервер не должен создавать игрока для себя
2. Адаптируйте логику `if (IsServer)` — она не должна предполагать наличие игрока
3. Измените IP-адрес прослушивания — с `127.0.0.1` на `0.0.0.0` (все интерфейсы)
4. Используйте аргументы командной строки для передачи порта и других параметров

```csharp
// Получение порта из аргументов командной строки
int port = 7777;
string[] args = System.Environment.GetCommandLineArgs();
for (int i = 0; i < args.Length; i++)
{
    if (args[i] == "-port" && i + 1 < args.Length)
    {
        int.TryParse(args[i + 1], out port);
    }
}

// Настройка транспорта
var transport = NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();
transport.SetConnectionData("0.0.0.0", (ushort)port);
```

---

## 4. RPC (Remote Procedure Calls)
RPC — это механизм вызова методов на удалённых клиентах или сервере. Это основной способ передачи событий и команд в NGO.

### 📝 Объявление RPC
Метод RPC должен иметь:
- Атрибут `[Rpc]` с указанием получателя
- Суффикс `Rpc` в имени метода
- Наследование от `NetworkBehaviour`

```csharp
[Rpc(SendTo.Server)]
public void MyServerRpc(int someData) { }

[Rpc(SendTo.NotServer)]
public void MyClientRpc(string message) { }

[Rpc(SendTo.Owner)]
public void MyOwnerRpc(float value) { }
```

### 📋 Типы RPC
| Атрибут | Получатель | Описание |
| --- | --- | --- |
| `[Rpc(SendTo.Server)]` | Сервер | Клиент → Сервер |
| `[Rpc(SendTo.NotServer)]` | Клиенты (кроме сервера) | Сервер → Клиенты |
| `[Rpc(SendTo.Owner)]` | Владелец объекта | Сервер → Владелец |
| `[Rpc(SendTo.ClientsAndHost)]` | Клиенты + Хост | Сервер → Все клиенты |
| `[Rpc(SendTo.Everyone)]` | Все (включая отправителя) | Любой → Все |
| `[Rpc(SendTo.SpecifiedInParams)]` | Указанный список | Любой → Конкретные клиенты |

### 🎮 Пример: Чат и игровые события
```csharp
public class ChatSystem : NetworkBehaviour
{
    // Отправка сообщения от клиента к серверу
    [Rpc(SendTo.Server)]
    public void SendMessageServerRpc(string playerName, string message)
    {
        // Сервер принимает сообщение и транслирует всем
        BroadcastMessageClientRpc(playerName, message);
    }
    
    // Трансляция сообщения от сервера ко всем клиентам
    [Rpc(SendTo.NotServer)]
    private void BroadcastMessageClientRpc(string playerName, string message)
    {
        Debug.Log($"[{playerName}]: {message}");
        // Отображение в UI
    }
    
    // Отправка уведомления только владельцу
    [Rpc(SendTo.Owner)]
    public void NotifyOwnerRpc(string eventMessage)
    {
        Debug.Log($"Уведомление для владельца: {eventMessage}");
    }
}
```

### 🎯 Отправка RPC конкретным клиентам
Для отправки RPC конкретным клиентам используйте `SendTo.SpecifiedInParams`:
```scharp
// Объявление RPC с возможностью указать получателей
[Rpc(SendTo.SpecifiedInParams, AllowTargetOverride = true)]
public void PrivateMessageRpc(string message, RpcParams rpcParams)
{
    Debug.Log($"Приватное сообщение: {message}");
}

// Отправка конкретному клиенту
public void SendPrivateMessage(ulong clientId, string message)
{
    PrivateMessageRpc(message, RpcTarget.Single(clientId, RpcTargetUse.Temp));
}

// Отправка группе клиентов
public void SendToGroup(ulong[] clientIds, string message)
{
    PrivateMessageRpc(message, RpcTarget.Group(clientIds, RpcTargetUse.Temp));
}
```

> 💡 Использование `RpcTargetUse.Temp` снижает нагрузку на сборщик мусора, так как объекты создаются временно.

---

## 5. Практический пример: Мини-игра с RPC
```csharp
using Unity.Netcode;
using UnityEngine;

public class NetworkGameManager : NetworkBehaviour
{
    public NetworkVariable<int> gameScore = new NetworkVariable<int>(0);
    public NetworkVariable<bool> isGameActive = new NetworkVariable<bool>(false);
    
    // Сервер запускает игру
    [Rpc(SendTo.Server)]
    public void StartGameServerRpc()
    {
        if (!IsServer) return;
        
        isGameActive.Value = true;
        gameScore.Value = 0;
        
        // Оповещаем всех игроков о начале
        NotifyGameStartClientRpc("Игра началась!");
    }
    
    [Rpc(SendTo.NotServer)]
    private void NotifyGameStartClientRpc(string message)
    {
        Debug.Log(message);
        // Показываем UI
    }
    
    // Игрок отправляет действие
    [Rpc(SendTo.Server)]
    public void PlayerActionServerRpc(ulong playerId, int actionValue)
    {
        if (!IsServer || !isGameActive.Value) return;
        
        // Сервер проверяет и обновляет счёт
        if (IsValidAction(actionValue))
        {
            gameScore.Value += actionValue;
            NotifyScoreUpdateClientRpc(playerId, gameScore.Value);
        }
    }
    
    [Rpc(SendTo.NotServer)]
    private void NotifyScoreUpdateClientRpc(ulong playerId, int newScore)
    {
        Debug.Log($"Игрок {playerId} обновил счёт: {newScore}");
        // Обновление UI
    }
    
    private bool IsValidAction(int actionValue)
    {
        // Валидация на сервере
        return actionValue > 0 && actionValue < 100;
    }
}
```

---

## 6. Лучшие практики и частые ошибки
### ✅ Рекомендации:
1. Всегда проверяйте `IsServer` и `IsClient` перед выполнением сетевых операций
2. Используйте `NetworkVariable` для состояния, а `RPC` для событий
3. Валидируйте ввод на сервере — клиенты могут отправлять некорректные данные
4. Используйте `NetworkObject.SpawnWithObservers = false` для объектов, которые не нужно показывать всем
5. Отключайте ненужные компоненты на клиентских объектах (например, `PlayerInput.enabled = IsOwner`)
6. Для больших проектов используйте выделенный сервер вместо Host

### ❌ Частые ошибки:
```csharp
// ❌ ОШИБКА: Клиент изменяет состояние напрямую
void Update()
{
    if (Input.GetKey(KeyCode.W))
    {
        transform.position += Vector3.forward; // Не синхронизируется!
    }
}

// ✅ ПРАВИЛЬНО: Отправляем запрос серверу
void Update()
{
    if (Input.GetKey(KeyCode.W) && IsOwner)
    {
        RequestMoveServerRpc(Vector3.forward);
    }
}

// ❌ ОШИБКА: Проверка IsServer на клиенте
void OnNetworkSpawn()
{
    if (IsServer) // Всегда false на клиенте!
    {
        // Этот код никогда не выполнится на клиенте
    }
}

// ✅ ПРАВИЛЬНО: Используйте IsClient для клиентской логики
void OnNetworkSpawn()
{
    if (IsClient && IsOwner)
    {
        // Включаем управление
    }
}

// ❌ ОШИБКА: NetworkBehaviour без NetworkObject
public class MyNetworkScript : NetworkBehaviour // Не сработает без NetworkObject!
{
    // ...
}

// ✅ ПРАВИЛЬНО: Всегда добавляйте NetworkObject + NetworkBehaviour вместе
```

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
