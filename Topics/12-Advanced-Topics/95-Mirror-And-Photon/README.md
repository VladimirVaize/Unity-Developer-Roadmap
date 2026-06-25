# 🌐 Mirror and Photon: Alternative Networking Solutions for Unity Multiplayer
Creating a multiplayer game is one of the most challenging tasks in game development. 
Unity doesn't provide a built-in "out-of-the-box" networking solution, so developers turn to third-party libraries and services. 
The two most popular and fundamentally different approaches are Mirror 
(free, open-source, full control) and Photon (commercial service with ready-made infrastructure).

---

## 1. Introduction: The Choice Problem
In the Unity world, two main camps have formed for multiplayer. 
On one side — self-hosted solutions where you fully control server code and infrastructure. 
On the other — ready-made services (SaaS) that handle server management but limit freedom and require payment.

> According to a comparison of popular solutions, "the best multiplayer engine for Unity is the one that fits your game.
> A physics-heavy game syncing thousands of entities has different needs than a turn-based card game"

### Key Questions for Selection:

| Question | Options |
| --- | --- |
| Who manages servers? | Self-hosted (Mirror) / Photon Cloud (Photon) |
| Budget | Free (Mirror) / Pay per CCU (Photon) |
| Code control | Full (Mirror) / Limited (Photon) |
| Implementation complexity | High (Mirror) / Low (Photon) |
| Prediction & Lag Compensation | Implement yourself (Mirror) / Built-in (Photon) |

---

## 2. Mirror: Freedom and Control
Mirror is an open-source library for creating multiplayer games with client-server architecture. 
It is the spiritual successor to Unity's old UNET and offers full control over networking code.

### 🔧 Key Features:

| Feature | Description |
| --- | --- |
| Type | Open-source library |
| License | MIT (free) |
| Architecture | Client-server (Server Authoritative) |
| Hosting | Self-hosted (you manage servers) |
| Limits | None (depends on your infrastructure) |
| Control | Full access to source code |

### 📦 Key Mirror Components:
1. NetworkManager — manages connections, scenes, spawning
2. NetworkIdentity — identifies network objects
3. NetworkBehaviour — base class for network scripts
4. NetworkTransform — synchronizes position/rotation
5. RPC (Remote Procedure Calls) — calls methods on all clients
6. SyncVars — automatic variable synchronization

### 💻 Basic Example: Player Movement
```csharp
using Mirror;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float _speed = 5f;
    
    [SyncVar] private Color _playerColor = Color.white;
    
    void Update()
    {
        if (!isLocalPlayer) return;
        
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        Vector3 direction = new Vector3(horizontal, 0, vertical) * _speed * Time.deltaTime;
        transform.Translate(direction);
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            ChangeColorCmd(Random.ColorHSV());
        }
    }
    
    [Command]
    private void ChangeColorCmd(Color newColor)
    {
        _playerColor = newColor;
        ApplyColorRpc(newColor);
    }
    
    [ClientRpc]
    private void ApplyColorRpc(Color color)
    {
        GetComponent<Renderer>().material.color = color;
    }
}
```

### 🚀 Advantages of Mirror:
1. Full server code control — you can implement any logic
2. No CCU limits — player count limited only by your server capacity
3. Transport flexibility — can use UDP, TCP, Steam Networking, LiteNetLib
4. Free — no hidden scaling costs

### ⚠️ Disadvantages of Mirror:
1. Requires networking knowledge — harder to learn
2. Self-managed infrastructure — need to configure servers, load balancing, security
3. No built-in prediction and lag compensation — must implement yourself
4. Potentially higher traffic — research shows Mirror can generate more packets than Photon

---

## 3. Photon (PUN and Fusion): Ready-Made Service
Photon is a commercial multiplayer service that provides ready-made cloud infrastructure. 
The most popular implementations are Photon Unity Networking (PUN) and its evolution — Photon Fusion.

### 🔧 Key Features:

| Feature | Description |
| --- | --- |
| Type | SaaS (cloud service) |
| License | Freemium (free up to 20 CCU) |
| Architecture | Client-server (cloud-based) |
| Hosting | Photon Cloud (managed by Photon) |
| Limits | By CCU count (depends on plan) |
| Control | Limited (cannot modify server code) |

### 📦 Photon Solutions:

| Solution | Description | Best for |
| --- | --- | --- |
| PUN 2 (Photon Unity Networking) | Classic implementation with rooms and RPC | Games with 2-8 players per room |
| Photon Fusion | Modern solution with prediction, rollback, lag compensation | Competitive games, shooters, action |
| Photon Quantum | Deterministic simulator (integer physics) | Strategy, fighting, precise simulations |

### 💻 Basic Example: Connecting to a Room in PUN
```csharp
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonConnection : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Cloud");
        PhotonNetwork.JoinRandomRoom();
    }
    
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("No rooms available, creating new one...");
        
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
        Debug.Log($"Joined room: {PhotonNetwork.CurrentRoom.Name}");
        PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
    }
}
```

### 🚀 Advantages of Photon:
1. No server management — Photon Cloud handles everything
2. Built-in prediction and lag compensation (in Fusion)
3. Ready-made room and matchmaking system
4. Excellent documentation and community — one of the most popular solutions
5. Cross-platform — works on all Unity platforms

### ⚠️ Disadvantages of Photon:
1. Limited control — cannot modify server code
2. Paid model — costs grow with player base (~$95/month for 100 CCU)
3. Free tier limit — up to 20 CCU
4. Higher traffic consumption — research shows Photon may use more traffic for Transform synchronization

---

## 4. Mirror vs Photon: A Comparative Analysis
According to a study conducted at Turku University, Mirror and Photon PUN 2 were compared under identical conditions

### 📊 Performance Results (50 players):

| Metric | Mirror | Photon PUN 2 |
| --- | --- | --- |
| FPS (desktop) | ~574 fps | ~981 fps |
| CPU (client) | 30-40% | 30-40% |
| Latency | ~34 ms | ~11 ms |
| Packets per second | ~11,900 | ~8,200 |
| Average packet size | ~757 bytes | ~1005 bytes |
| GC Alloc | Grows with players | Stable |

> Key observation: Photon showed higher performance and stability in benchmarks.
> However, researchers noted that at high loads (40-50 players), Photon showed noticeable delays in Transform synchronization,
> while Mirror remained more stable in terms of user experience

---

## 5. Steam Integration
Both solutions can integrate with Steam for social features:
### 🎮 Mirror + Steamworks.NET
Ready-made templates exist for Mirror + Steam integration using Steam Relay for P2P connections

### 🎮 Photon + Steam
Photon can be used alongside Steam for:
- Authentication via Steam
- Invitations via Steam Overlay
- Networking via Photon for game traffic (more reliable than Steam P2P)

---

## 6. Best Practices and Recommendations
### ✅ Selection Recommendations:
1. Quick start / MVP → Photon PUN
2. Competitive game with prediction → Photon Fusion
3. Full control, customization, zero budget → Mirror
4. Large project with own servers → Mirror + AWS
5. Research / academic projects → Mirror (open-source, studyable)

### ⚠️ Common Mistakes:
```csharp
// ❌ ERROR: Mixing PUN and Mirror in one project
// Frameworks conflict due to different networking approaches

// ❌ ERROR: Not accounting for Photon CCU limits
// Free Photon tier is only 20 CCU [citation:5]

// ❌ ERROR: Ignoring traffic overhead
// Photon may consume more traffic for Transform sync [citation:6]

// ❌ ERROR: Improper Mirror server configuration under load
// Requires proper AWS / dedicated server configuration [citation:7]

// ✅ CORRECT: Conduct stress tests before release
// Use Server Build for performance testing [citation:6]
```

---

### ⭐ If this project was useful, put a star on GitHub!
