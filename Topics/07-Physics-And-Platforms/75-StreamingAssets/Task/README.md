# 🎯 Task: «Level and Video Loading System from StreamingAssets»
You are developing an adventure game. Levels are stored in JSON files, and videos in MP4 format. 
Everything is located in the `StreamingAssets` folder. 

Your task is to implement a system that:
1. Loads the level list from `levels.json` (contains an array of level filenames)
2. Loads a specific level by its name (JSON file in `levels/` folder)
3. Plays an intro video from `videos/intro.mp4` before starting the game
4. Caches large files on Android to a `temp folder` for fast access
5. Shows loading progress for large files

## 📝 Test Data (create in StreamingAssets):
### File 1: `StreamingAssets/levels.json`
```json
{
    "levels": ["tutorial.json", "level1.json", "level2.json"],
    "version": 1
}
```

### File 2: `StreamingAssets/levels/tutorial.json`
```json
{
    "levelName": "Tutorial",
    "difficulty": 1,
    "enemies": 5,
    "timeLimit": 60
}
```

### File 3: `StreamingAssets/levels/level1.json`
```json
{
    "levelName": "Forest",
    "difficulty": 2,
    "enemies": 12,
    "timeLimit": 120
}
```

Video: Add any short MP4 file to `StreamingAssets/videos/intro.mp4`

---

## 📋 Specific Implementation Tasks:
1. Create a `LevelManager` class:
   - Method `IEnumerator LoadLevelsList()` — loads `levels.json` and parses the list
   - Method `IEnumerator LoadLevelByName(string levelName)` — loads a specific level
   - Method `LevelData GetCurrentLevel()` — returns the current level
  
2. Create a `VideoIntroPlayer` class:
   - Plays `intro.mp4` from StreamingAssets before loading the first level
   - Add a "Skip" button to bypass the video
   - Show a loader during video preparation
  
3. Implement caching for Android:
   - In method `CacheLargeFile(string fileName)`, copy the file from StreamingAssets to `Application.persistentDataPath`
   - Use an `isCached` flag to avoid recopying
   - Show a progress bar for large file copying (>10 MB)
  
4. Add error handling:
   - If a file is not found — show a message and load a default level
   - If a video fails to load — skip it automatically
   - Log all errors to the console
  
5. Stream processing for large JSON (bonus):
   - If a level JSON exceeds 5 MB, read it line by line and parse only needed fields
  
---

## 🧰 Implementation Requirements:
- Use `UnityWebRequest` for cross-platform compatibility
- Use the `VideoPlayer` component for video
- Add a UI Slider to display loading/copying progress
- Use coroutines for all asynchronous operations
- Comment platform-dependent code

---

## 🔍 Verification:
1. In the Editor: Load the level list, select a level, the video should play
2. On Android: Verify that files are correctly read via `UnityWebRequest`
3. On desktop: Verify fast reading via `File.ReadAllText`
4. Ensure that cached files are not recopied on subsequent runs

---

## 💡 Expected Console Output:
```text
[LevelManager] Loading levels.json from StreamingAssets...
[LevelManager] Found 3 levels: tutorial, level1, level2
[LevelManager] Loading tutorial.json...
[LevelManager] Level loaded: Tutorial (Difficulty 1, Enemies 5, Time 60s)
[VideoIntroPlayer] Preparing intro.mp4...
[VideoIntroPlayer] Video ready, playing...
[VideoIntroPlayer] Video finished, starting game...
```

```text
[Android] Caching tutorial.json to persistentDataPath...
[Android] Copy progress: 100%
[Android] File cached successfully
```

---

### ⭐ If this project was useful, put a star on GitHub!
