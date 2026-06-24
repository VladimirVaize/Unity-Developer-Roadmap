# 🌐 Netcode for GameObjects (NGO): Authoritative Server, Client-Server Architecture, RPC
Netcode for GameObjects (NGO) is Unity's official high-level SDK for creating multiplayer games with a client-server architecture. 
It's built on top of Unity Transport Layer and provides tools for state synchronization, Remote Procedure Calls (RPCs), and network object management.

---

## 1. Core NGO Components
### 🧩 NetworkManager — The Network Hub
`NetworkManager` is a required component that manages all network settings and sessions. 
It contains transport configuration, player prefabs, and methods to start a session.

Setting Up NetworkManager:
1. Create an empty GameObject and name it `NetworkManager`
2. Add the `NetworkManager` component (Netcode → NetworkManager)
3. In the Inspector, select `UnityTransport` as the protocol
4. Assign a player prefab to the `PlayerPrefab` field

Starting a Session:
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
            if (GUILayout.Button("Host")) m_NetworkManager.StartHost();     // Server + Client
            if (GUILayout.Button("Server")) m_NetworkManager.StartServer(); // Server only
            if (GUILayout.Button("Client")) m_NetworkManager.StartClient(); // Client only
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

### 🧩 NetworkObject — Networked Object
`NetworkObject` is the component that makes a GameObject networked. Without it, the object won't synchronize between clients.

Key Properties:
- `NetworkObjectId` — unique network identifier
- `IsOwner` — whether the local client owns this object
- `IsSpawned` — whether the object is spawned in the network

```csharp
public class PlayerController : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        // Check if this is the local player's object
        if (IsOwner)
        {
            Debug.Log("This is my player!");
            // Enable control only for the owner
        }
    }
}
```

> [!Important]
> `NetworkObject` must be on the root GameObject of the prefab. Don't create prefabs with nested `NetworkObjects`.

### 🧩 NetworkBehaviour — Networked Behavior
`NetworkBehaviour` is the base class for all networked scripts. It inherits from `MonoBehaviour` and provides access to network functions.

Key Properties and Methods:
- `IsServer` — whether code is running on the server
- `IsClient` — whether code is running on the client
- `IsHost` — whether the process is a host (server + client)
- `IsOwner` — whether the local client owns this object
- `OnNetworkSpawn()` — called when the object spawns in the network
- `OnNetworkDespawn()` — called when the object despawns

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
            // Death logic
        }
    }
}
```

---

## 2. Authoritative Server Architecture
In a server-authoritative architecture, all critical decisions are made on the server. 
Clients send action requests, and the server validates them and applies changes.

### 🔑 Key Principles:

| Principle | Description |
| --- | --- |
| Server is the source of truth | All states are synchronized from server to clients |
| Client is only a display | Clients show state but don't make decisions |
| Validation on server | Server validates all client actions |
| Anti-cheat | Authoritative server protects against hacking and cheating |

### 🎮 Example: Player Movement with Server Validation
```csharp
public class AuthoritativePlayerMovement : NetworkBehaviour
{
    private float moveSpeed = 5f;
    private Vector3 targetPosition;
    
    void Update()
    {
        if (!IsOwner) return;
        
        // Collect input only on the client
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        if (horizontal != 0 || vertical != 0)
        {
            Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized;
            // Send movement request to server
            RequestMoveServerRpc(moveDirection);
        }
    }
    
    [ServerRpc]
    private void RequestMoveServerRpc(Vector3 direction)
    {
        // Server validates and applies movement
        Vector3 newPosition = transform.position + direction * moveSpeed * Time.deltaTime;
        
        // Check collisions (on the server)
        if (IsPositionValid(newPosition))
        {
            transform.position = newPosition;
            // Sync position with all clients
            UpdatePositionClientRpc(transform.position);
        }
    }
    
    [ClientRpc]
    private void UpdatePositionClientRpc(Vector3 position)
    {
        // All clients receive the updated position from server
        transform.position = position;
    }
    
    private bool IsPositionValid(Vector3 position)
    {
        // Validate position (on the server)
        return true; // Simplified
    }
}
```

### 🌐 Authoritative Server Workflow:
```text
Client A                    Server                    Client B
    |                          |                          |
    | -- Player action -->     |                          |
    |                          | -- Validate on server   |
    |                          | -- Update state         |
    | <-- New position ---     |                          |
    |                          | -- New position -->      |
    |                          |                          |
```

> 💡 Advantages: Security, unified state for all players, anti-cheat.

> ⚠️ Disadvantages: Latency between action and display (fixed with client-side prediction).

### 🏗️ Non-Authoritative Server
In a non-authoritative server, clients make their own decisions and send results to the server for distribution. This is simpler to implement but vulnerable to cheating.

| Server Type | Advantages | Disadvantages |
| --- | --- | --- |
| Authoritative | Secure, anti-cheat | More complex, latency |
| Non-authoritative | Simpler to implement | Vulnerable to cheats, insecure |

---

## 3. Client-Server Architecture
NGO supports two operating modes:

### 🏠 Host
- One player acts as both server and client
- Ideal for small games (up to 8-16 players)
- Easy to set up for development

### 🖥️ Server
- Dedicated server with no graphical interface
- For large games (up to 100+ players)
- More performant and scalable

### 🎮 Example: Mode Selection
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
        netManager.StartHost(); // Starts both server and client
    }
    
    public void StartAsServer()
    {
        netManager.StartServer(); // Server only, no player
    }
    
    public void StartAsClient(string ipAddress)
    {
        // Connect to server by IP
        netManager.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>()
                  .SetConnectionData(ipAddress, 7777);
        netManager.StartClient();
    }
}
```

### 🚀 Migrating from Host to Server
When transitioning from client-hosted to dedicated server, consider:
1. Replace `StartHost` with `StartServer` — server shouldn't create a player for itself
2. Adapt `if (IsServer)` logic — shouldn't assume a player exists
3. Change listen address — from `127.0.0.1` to `0.0.0.0` (all interfaces)
4. Use command-line arguments to pass port and other parameters

```csharp
// Get port from command-line arguments
int port = 7777;
string[] args = System.Environment.GetCommandLineArgs();
for (int i = 0; i < args.Length; i++)
{
    if (args[i] == "-port" && i + 1 < args.Length)
    {
        int.TryParse(args[i + 1], out port);
    }
}

// Configure transport
var transport = NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();
transport.SetConnectionData("0.0.0.0", (ushort)port);
```

---

## 4. RPC (Remote Procedure Calls)
RPC is a mechanism for calling methods on remote clients or the server. It's the primary way to send events and commands in NGO.

### 📝 Declaring an RPC
An RPC method must have:
- `[Rpc]` attribute specifying the recipient
- `Rpc` suffix in the method name
- Inheritance from `NetworkBehaviour`

```csharp
[Rpc(SendTo.Server)]
public void MyServerRpc(int someData) { }

[Rpc(SendTo.NotServer)]
public void MyClientRpc(string message) { }

[Rpc(SendTo.Owner)]
public void MyOwnerRpc(float value) { }
```

### 📋 RPC Types

| Attribute | Recipient | Description |
| --- | --- | --- |
| `[Rpc(SendTo.Server)]` | Server | Client → Server |
| `[Rpc(SendTo.NotServer)]` | Clients (excluding server) | Server → Clients |
| `[Rpc(SendTo.Owner)]` | Object owner | Server → Owner |
| `[Rpc(SendTo.ClientsAndHost)]` | Clients + Host | Server → All Clients |
| `[Rpc(SendTo.Everyone)]` | Everyone (including sender) | Anyone → All |
| `[Rpc(SendTo.SpecifiedInParams)]` | Specified list | Anyone → Specific Clients |

### 🎮 Example: Chat and Game Events
```csharp
public class ChatSystem : NetworkBehaviour
{
    // Send message from client to server
    [Rpc(SendTo.Server)]
    public void SendMessageServerRpc(string playerName, string message)
    {
        // Server receives and broadcasts to all
        BroadcastMessageClientRpc(playerName, message);
    }
    
    // Broadcast message from server to all clients
    [Rpc(SendTo.NotServer)]
    private void BroadcastMessageClientRpc(string playerName, string message)
    {
        Debug.Log($"[{playerName}]: {message}");
        // Display in UI
    }
    
    // Send notification only to the owner
    [Rpc(SendTo.Owner)]
    public void NotifyOwnerRpc(string eventMessage)
    {
        Debug.Log($"Owner notification: {eventMessage}");
    }
}
```

### 🎯 Sending RPC to Specific Clients
Use `SendTo.SpecifiedInParams` to send RPCs to specific clients:
```csharp
// Declare RPC with target override capability
[Rpc(SendTo.SpecifiedInParams, AllowTargetOverride = true)]
public void PrivateMessageRpc(string message, RpcParams rpcParams)
{
    Debug.Log($"Private message: {message}");
}

// Send to a specific client
public void SendPrivateMessage(ulong clientId, string message)
{
    PrivateMessageRpc(message, RpcTarget.Single(clientId, RpcTargetUse.Temp));
}

// Send to a group of clients
public void SendToGroup(ulong[] clientIds, string message)
{
    PrivateMessageRpc(message, RpcTarget.Group(clientIds, RpcTargetUse.Temp));
}
```

> 💡 Using `RpcTargetUse.Temp` reduces garbage collection pressure by using temporary, cached objects.

---

## 5. Practical Example: Mini-Game with RPCs
```csharp
using Unity.Netcode;
using UnityEngine;

public class NetworkGameManager : NetworkBehaviour
{
    public NetworkVariable<int> gameScore = new NetworkVariable<int>(0);
    public NetworkVariable<bool> isGameActive = new NetworkVariable<bool>(false);
    
    // Server starts the game
    [Rpc(SendTo.Server)]
    public void StartGameServerRpc()
    {
        if (!IsServer) return;
        
        isGameActive.Value = true;
        gameScore.Value = 0;
        
        // Notify all players
        NotifyGameStartClientRpc("Game started!");
    }
    
    [Rpc(SendTo.NotServer)]
    private void NotifyGameStartClientRpc(string message)
    {
        Debug.Log(message);
        // Show UI
    }
    
    // Player sends an action
    [Rpc(SendTo.Server)]
    public void PlayerActionServerRpc(ulong playerId, int actionValue)
    {
        if (!IsServer || !isGameActive.Value) return;
        
        // Server validates and updates score
        if (IsValidAction(actionValue))
        {
            gameScore.Value += actionValue;
            NotifyScoreUpdateClientRpc(playerId, gameScore.Value);
        }
    }
    
    [Rpc(SendTo.NotServer)]
    private void NotifyScoreUpdateClientRpc(ulong playerId, int newScore)
    {
        Debug.Log($"Player {playerId} updated score: {newScore}");
        // Update UI
    }
    
    private bool IsValidAction(int actionValue)
    {
        // Server-side validation
        return actionValue > 0 && actionValue < 100;
    }
}
```

---

## 6. Best Practices and Common Mistakes
### ✅ Recommendations:
1. Always check `IsServer` and `IsClient` before performing network operations
2. Use `NetworkVariable` for state and `RPC` for events
3. Validate input on the server — clients can send invalid data
4. Use `NetworkObject.SpawnWithObservers = false` for objects not needed by all
5. Disable unnecessary components on client objects (e.g., `PlayerInput.enabled = IsOwner`)
6. Use a dedicated server instead of Host for large projects

### ❌ Common Mistakes:
```csharp
// ❌ ERROR: Client modifies state directly
void Update()
{
    if (Input.GetKey(KeyCode.W))
    {
        transform.position += Vector3.forward; // Not synchronized!
    }
}

// ✅ CORRECT: Send request to server
void Update()
{
    if (Input.GetKey(KeyCode.W) && IsOwner)
    {
        RequestMoveServerRpc(Vector3.forward);
    }
}

// ❌ ERROR: Checking IsServer on client
void OnNetworkSpawn()
{
    if (IsServer) // Always false on client!
    {
        // This code never runs on client
    }
}

// ✅ CORRECT: Use IsClient for client logic
void OnNetworkSpawn()
{
    if (IsClient && IsOwner)
    {
        // Enable control
    }
}

// ❌ ERROR: NetworkBehaviour without NetworkObject
public class MyNetworkScript : NetworkBehaviour // Won't work without NetworkObject!
{
    // ...
}

// ✅ CORRECT: Always add NetworkObject + NetworkBehaviour together
```

---

### ⭐ If this project was useful, put a star on GitHub!
