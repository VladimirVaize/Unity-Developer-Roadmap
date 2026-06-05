# 🎯 Practical Task: DLC System Using AssetBundles
## 📋 Task Description

You need to create a DLC (Downloadable Content) loading system using AssetBundles. 
Imagine you have a base game, and the player can purchase an additional pack: 
a new level with unique enemy models, textures, and sounds.

---

## 🧱 Task Structure
### 📁 Part 1: Create DLC Content
1. Create a scene `DLCLevel` with simple environment (platforms, light, camera)
2. Create a unique enemy prefab `DLCEnemy.prefab`
3. Create a texture `DLCEnvironment.png` (can be a simple colored square) and a material based on it
4. Create a short audio clip `DLCBackgroundMusic.mp3` (use any short file or placeholder)

### 📁 Part 2: Configure AssetBundle
1. Assign each created resource the AssetBundle name: `dlc_content`
2. Write an editor script `DLCBundleBuilder.cs` in the `Editor` folder that:
   - Builds the `dlc_content` bundle into the `AssetBundles/` folder (outside `Assets`)
   - Copies the built bundle to `Assets/StreamingAssets/DLC/` (for local testing)
   - Logs the built file path to the console
  
### 📁 Part 3: DLC Download Manager
Create a script `DLCDownloader.cs`:

Requirements:
- Check if DLC is purchased (simulate via `PlayerPrefs.GetInt("DLC_Owned", 0)`)
- If DLC is purchased:
  - Load the bundle from `StreamingAssets/DLC/dlc_content`
  - From the bundle, load the scene `DLCLevel`, enemy prefab, material, and audio clip
  - Instantiate the enemy in the scene and play the music
 
- Handle errors: if bundle is not found or corrupted — log a message to the console
- Have an `UnloadDLC()` method to unload the bundle from memory (`bundle.Unload(true)`)

### 📁 Part 4: Test UI
Create a simple UI on the scene with buttons:
- "Check DLC" — checks for DLC availability and loads it
- "Unload DLC" — unloads the bundle and destroys all created objects
- "Simulate Purchase" — sets the purchase flag in `PlayerPrefs`

### 📁 Part 5: Patch Simulation (Bonus)
Create a second DLC variant with updated content:
- Change the enemy texture (new color)
- Change the enemy's speed in the prefab
- Build a new bundle with the same name but a different version
- Implement version checking in `DLCDownloader` using the manifest

---

## ✅ Completion Criteria
1. Bundle builds successfully via editor script
2. DLC loads and works only after "purchase"
3. All resources from the bundle (scene, prefab, material, audio) load correctly
4. Bundle unloading frees memory and destroys created objects
5. On load error (missing file, corruption), the game does not crash

---

## 🧩 Bonus Tasks (⭐⭐)
### ⭐ Task 1: Implement caching with versioning using `UnityWebRequest` (as if the bundle is downloaded from a server)

### ⭐⭐ Task 2: Add download progress display in the UI (use a coroutine with `yield return request`)

### ⭐⭐ Task 3: Implement a dependency system — split DLC into 2 bundles: `dlc_models` (prefabs) and `dlc_audio` (sounds) — and load them in the correct order

---

## 🧪 Expected Result
### After completing the task:
1. When launching the game, pressing "Simulate Purchase" → "Check DLC" — an enemy appears in the scene, music plays, textures display
2. The console shows logs of bundle loading and each asset
3. After "Unload DLC" — the enemy disappears, music stops, the bundle is unloaded from memory
4. On subsequent loading — everything works again

---

### ⭐ If this project was useful, put a star on GitHub!
