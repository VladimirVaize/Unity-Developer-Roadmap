# 🎯 Task: «Clicker Race Network Game»
You are developing a simple multiplayer game where players compete in click speed. The first player to reach 10 points wins.

## 📝 What to Implement:
### Part 1: Basic Network Setup
1. Create a new 3D Unity project (Unity 2022.3 or newer)
2. Install the `com.unity.netcode.gameobjects` package via Package Manager
3. Create `Scripts/` and `Prefabs/` folders in `Assets`
4. Create a `NetworkManager` GameObject and configure it:
   - Add the `NetworkManager` component
   - Select `UnityTransport` as the protocol
   - Create a player prefab (Capsule or Cube) with `NetworkObject`
   - Assign the prefab to the `PlayerPrefab` field
  
### Part 2: Server Game Logic (GameManager)
5. Create a `GameManager` script based on `NetworkBehaviour`:
   - `NetworkVariable<int> targetScore = 10` — target to win
   - `NetworkVariable<int> gameState` — 0 (waiting), 1 (playing), 2 (finished)
   - Method `StartGameServerRpc()` — starts the game (server only)
   - Method `EndGameServerRpc(ulong winnerId)` — ends the game (server only)
  
6. Add auto-start logic:
   - When the second player connects, the game starts automatically
   - Use `NetworkManager.ConnectedClientsIds` to count players
  
### Part 3: Client Player Logic (PlayerController)
7. Create a `PlayerController` script based on `NetworkBehaviour`:
   - `NetworkVariable<int> score = 0` — player's score
   - Method `ClickServerRpc()` — sends a click request to the server
   - Server validation: click counts only if the game is active
  
8. Implement win condition:
   - When `score >= targetScore`, send an RPC to the server
   - The server declares the winner and ends the game
  
### Part 4: Input Handling
9. On the client, handle:
    - Left mouse button click (or Space) → `ClickServerRpc()`
    - Only if the player is the owner (`IsOwner`)
  
10. Disable input for non-owners:
```csharp
public override void OnNetworkSpawn()
{
    enabled = IsOwner; // Disable script for non-owners
}
```

### Part 5: UI and State Display
11. Create a Canvas with elements:
    - Game status text ("Waiting for players", "Game in progress", "Winner: Name")
    - Score text for each player
    - "Start" button (active only for host)
   
12. Use `NetworkVariable` for game state synchronization:
    - Update UI via `OnNetworkSpawn()` and variable change events
   
### Part 6: Events and Notifications
13. Implement RPCs for notifications:
    - `NotifyPlayerJoinedClientRpc(string playerName)` — new player joined
    - `NotifyScoreUpdateClientRpc(ulong playerId, int newScore)` — score update
    - `NotifyWinnerClientRpc(ulong winnerId)` — winner announced
   
14. Use `[Rpc(SendTo.NotServer)]` to send notifications to all clients

### Part 7: Testing
15. Test the game in the Editor:
    - Start Host (server + client) → create 2 Game View windows
    - Connect a second client (via Client button)
    - Verify both players see each other
    - Verify click mechanics and scoring
    - Ensure the server validates all actions
   
---

## 🧰 Implementation Requirements:
- Use ServerRpc for all player actions
- Use ClientRpc for notifications and updates
- Use NetworkVariable for game state and scores
- The server must be authoritative — all decisions are made on the server
- Remember: `IsOwner` determines who owns the object
- In `DEVELOPMENT_BUILD`, log all network events

---

## 🔍 Verification:
1. Start the game as Host — a player should appear
2. Start a second instance as Client — connects to the server
3. When the second player connects, the game starts automatically
4. Clicking the mouse increases the score by 1
5. At 10 points, the game ends and a winner is declared
6. After the game ends, clicks are not counted

---

## 🏆 Bonus Task (Optional):
1. High Score Saving: Add winner saving to `PlayerPrefs`
2. Timer: Add a time limit (30 seconds)
3. Power-ups: Implement random bonuses (×2 points per click)
4. Player Names: Add ability to enter a name before connecting
5. Animations: Add visual effects on click (explosion, particles)

---

### ⭐ If this project was useful, put a star on GitHub!
