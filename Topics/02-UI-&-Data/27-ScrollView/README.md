# 📜 ScrollView and Dynamic Lists in Unity UI

> [!Note]
> This material covers creating scrollable lists, performance optimization with Virtual Layout, and item recycling (reuse).
> These techniques are critical for inventories, leaderboards, chat windows, shops, and any UI with large amounts of data.

---

## 🧩 ScrollView Basics
ScrollView is a built-in Unity UI component that allows scrolling content beyond the visible area.

### How to create a ScrollView:
1. Right-click in Hierarchy → `UI` → `Scroll View`
2. Unity automatically creates:
   - `ScrollView` (parent with Scroll Rect component)
   - `Viewport` (the mask region through which content is visible)
   - `Content` (the actual moving inner object)
   - `Scrollbar Horizontal` and `Scrollbar Vertical`
  
### Direction settings:
- Vertical — vertical list (checkbox in Scroll Rect component)
- Horizontal — horizontal list
- Both — 2D scrolling

### Manual element adding:
Create an element (e.g., a button or panel with text) as a child of `Content`. 
With vertical scroll enabled, new elements will extend downward.

---

## ⚠️ Problem with Large Lists (Naive approach)
Imagine creating a list of 1000 elements, each with text, an icon, and a button. 
If you create 1000 UI objects as children of `Content`:
- Performance drops dramatically — each object consumes CPU/GPU for rendering, transform updates, and event handling.
- Load time becomes huge.
- Memory usage increases many times over.

This is called a "naive list" and is unsuitable for real projects.

---

## 🚀 Virtual Layout
Virtual Layout is a technique where you create only the elements visible on screen plus a small buffer. 
When the user scrolls, elements are reused: those that go off-screen are filled with new data.

### How it works:
1. Calculate how many elements fit on screen (e.g., 5).
2. Physically create, say, 7 objects (visible + buffer).
3. During scrolling, determine which data index should be at each object's position.
4. Update the text, icon, button inside the existing object (reuse it).

### Advantages:
- Huge memory savings (instead of 1000 objects — only 7)
- Instant loading for any list size
- Smooth scrolling

---

## ♻️ Recycling / Reusing Elements
Recycling is the heart of virtualization. 
Instead of destroying and creating new elements during scrolling, 
you take an element that went off-screen, change its data, and move it to the beginning or end of the list.

### Simple recycling model:
1. At startup, create a pool of N elements (e.g., 10).
2. On scroll event, calculate which data indices should be visible.
3. For each visible index:
   - If an element already has this index assigned — do nothing.
   - If not — take an element from the pool (or create a new one if the pool isn't full yet).
   - Assign the new index to it and update the UI (load text, icon, etc.).
  
4. Elements that are no longer visible are returned to the pool.

### Pseudocode example:
```csharp
void OnScroll()
{
    int firstVisibleIndex = CalculateFirstVisibleIndex();
    for (int i = 0; i < visibleCount; i++)
    {
        int dataIndex = firstVisibleIndex + i;
        GameObject item = GetRecycledItem(); // from pool
        item.GetComponent<MyItemView>().SetData(data[dataIndex]);
        item.transform.SetParent(content, false);
        PositionItem(item, i);
    }
}
```

---

## 🧰 Ready Solutions in Unity

### 1. ScrollView + Content Size Fitter + Layout Group (for static lists)
- Suitable for: small lists (< 50 items)
- Not suitable for: thousands of items

### 2. UI Toolkit (Unity 2021+)
- Built-in ListView with virtualization
- `makeItem` and `bindItem` callbacks

### 3. Asset Store packages
- `Super ScrollView`
- `EnhancedScroller`
- `UI Extensions` (Recycling ScrollRect)

### 4. Custom Virtual ScrollRect
Write your own component inheriting from ScrollRect and manage element positioning via `RectTransform.anchoredPosition`.

---

## 📌 Final Recommendations

| List size | Approach |
|-----------------|----------------------------------------|
| < 30 items | Regular ScrollView + Layout Group |
| 30–200 items | Possibly regular, but consider optimization |
| > 200 items | Mandatory Virtual Layout + recycling |
| > 1000 items | Virtualization + asynchronous data loading |

Golden rule: Never create thousands of UI objects at once. Always reuse them.

---

### ⭐ If this project was useful, put a star on GitHub!
