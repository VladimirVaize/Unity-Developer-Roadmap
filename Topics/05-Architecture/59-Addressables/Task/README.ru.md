# 🎯 Практическая задача: Система загрузки скинов оружия через Addressables
## 📋 Описание задачи
Вам нужно создать систему динамической загрузки и выгрузки скинов для оружия, используя Addressables. 
Система должна загружать модель оружия только когда игрок открывает инвентарь, и выгружать её при закрытии, экономя память.

---

## 🧱 Структура задачи
### 📁 Часть 1: Подготовка Addressables
1. Создайте в Unity три модели оружия (или 3D-объекта): `Sword`, `Axe`, `Bow`
2. В окне `Window > Asset Management > Addressables > Groups` отметьте их как Addressable
3. Присвойте им адреса: `weapons/sword`, `weapons/axe`, `weapons/bow`
4. Создайте метку (label) `weapon` и добавьте её всем трём моделям

### 📁 Часть 2: Скрипт WeaponSlot
Создайте скрипт `WeaponSlot` на пустом объекте-контейнере. Он должен содержать:
- `[SerializeField] private string _weaponAddress` — адрес оружия для загрузки
- Поле для хранения `AsyncOperationHandle<GameObject>`
- Метод `public void LoadWeapon()` — асинхронная загрузка модели по адресу
- Метод `public void UnloadWeapon()` — выгрузка через `Addressables.Release()`
- При загрузке — инстанциировать модель как дочерний объект
- При выгрузке — уничтожить инстанс и освободить хендл

### 📁 Часть 3: Скрипт InventoryUI
Создайте скрипт `InventoryUI` на UI-панели инвентаря. Он должен:
- Содержать три кнопки: "Меч", "Топор", "Лук"
- Содержать ссылку на `WeaponSlot`
- При нажатии на кнопку — загружать соответствующее оружие
- При закрытии инвентаря (OnDisable) — выгружать текущее оружие

### 📁 Часть 4: Загрузка с меткой
Добавьте в `InventoryUI` метод `public void LoadAllWeapons()` с `[ContextMenu]`:
- Загружает все оружия по метке `weapon` через `LoadAssetsAsync`
- Инстанциирует их в ряд на сцене для превью
- Правильно выгружает после использования

---

## ✅ Критерии выполнения
1. ✅ Загрузка должна быть асинхронной (корутина или событие)
2. ✅ Хендл загрузки сохраняется и используется для выгрузки
3. ✅ Каждый `LoadAssetAsync` имеет парный `Release`
4. ✅ Использован `AssetReference` или строка с проверкой `RuntimeKeyIsValid()` (⭐ бонус)
5. ✅ Метод выгрузки вызывается при закрытии инвентаря

---

## 🧪 Ожидаемый результат
1. Игра запускается — память чистая
2. Открываем инвентарь → нажимаем "Меч" → модель меча появляется на сцене
3. Закрываем инвентарь → модель исчезает, память освобождается (можно проверить через `Addressables Event Viewer` или `Profiler`)

---

## 🧩 Дополнительное задание (⭐⭐)
Добавьте кеширование загруженных оружий:
- Храните Dictionary<string, AsyncOperationHandle> с загруженными моделями
- При повторной загрузке того же оружия — используйте уже загруженную копию (не увеличивайте счётчик лишний раз)
- Добавьте кнопку "Выгрузить всё" — выгружает все кешированные ассеты

---

## 🧠 Подсказки
```csharp
// Пример начала загрузки
AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(_weaponAddress);
handle.Completed += OnWeaponLoaded;

// В OnWeaponLoaded:
if (handle.Status == AsyncOperationStatus.Succeeded)
{
    _currentWeapon = Instantiate(handle.Result, _slotTransform);
}
```

```csharp
// Пример загрузки по метке
var handle = Addressables.LoadAssetsAsync<GameObject>(
    "weapon", 
    OnWeaponLoadedCallback,
    Addressables.MergeMode.Intersection
);
```

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
