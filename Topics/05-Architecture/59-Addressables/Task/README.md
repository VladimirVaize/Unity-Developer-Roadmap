# 🎯 Practical Task: Weapon Skin Loading System with Addressables
## 📋 Task Description
You need to create a dynamic weapon skin loading and unloading system using Addressables. 
The system should load the weapon model only when the player opens the inventory, and unload it when closed to save memory.

---

## 🧱 Task Structure
### 📁 Part 1: Addressables Setup
1. Create three weapon models (or 3D objects) in Unity: `Sword`, `Axe`, `Bow`
2. In `Window > Asset Management > Addressables > Groups`, mark them as Addressable
3. Assign addresses: `weapons/sword`, `weapons/axe`, `weapons/bow`
4. Create a label `weapon` and add it to all three models

### 📁 Part 2: WeaponSlot Script
Create a `WeaponSlot` script on an empty container object. It should contain:
- `[SerializeField] private string _weaponAddress` — address of weapon to load
- A field to store `AsyncOperationHandle<GameObject>`
- Method `public void LoadWeapon()` — async load model by address
- Method `public void UnloadWeapon()` — unload via `Addressables.Release()`
- On load — instantiate model as a child object
- On unload — destroy instance and release handle

### 📁 Part 3: InventoryUI Script
Create an `InventoryUI` script on an inventory UI panel. It should:
- Contain three buttons: "Sword", "Axe", "Bow"
- Contain a reference to `WeaponSlot`
- On button click — load the corresponding weapon
- On inventory close (OnDisable) — unload the current weapon

### 📁 Part 4: Label-based Loading
Add a method `public void LoadAllWeapons()` with `[ContextMenu]` to `InventoryUI`:
- Loads all weapons via the `weapon` label using `LoadAssetsAsync`
- Instantiates them in a row on the scene for preview
- Properly unloads after use

---

## ✅ Completion Criteria
1. ✅ Loading must be asynchronous (coroutine or event)
2. ✅ Load handle is saved and used for unloading
3. ✅ Every `LoadAssetAsync` has a matching `Release`
4. ✅ Used `AssetReference` or string with `RuntimeKeyIsValid()` check (⭐ bonus)
5. ✅ Unload method is called when inventory closes

---

## 🧪 Expected Result
1. Game starts — memory is clean
2. Open inventory → click "Sword" → sword model appears on scene
3. Close inventory → model disappears, memory is freed (verify via `Addressables Event Viewer` or `Profiler`)

---

## 🧩 Bonus Task (⭐⭐)
Add caching of loaded weapons:
- Store Dictionary<string, AsyncOperationHandle> with loaded models
- On subsequent load of the same weapon — use already loaded copy (don't increment counter unnecessarily)
- Add an "Unload All" button — unloads all cached assets

---

## 🧠 Hints
```csharp
// Load start example
AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(_weaponAddress);
handle.Completed += OnWeaponLoaded;

// In OnWeaponLoaded:
if (handle.Status == AsyncOperationStatus.Succeeded)
{
    _currentWeapon = Instantiate(handle.Result, _slotTransform);
}
```

```csharp
// Label-based load example
var handle = Addressables.LoadAssetsAsync<GameObject>(
    "weapon", 
    OnWeaponLoadedCallback,
    Addressables.MergeMode.Intersection
);
```

---

### ⭐ If this project was useful, put a star on GitHub!
