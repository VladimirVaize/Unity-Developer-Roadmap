# 📝 Task: Create a virtualized chat list with element recycling

### Context:
You are developing an in-game chat UI that can hold up to 10,000 messages. 
Creating 10,000 UI objects directly would cause severe performance issues. 
You need to implement a dynamic list with virtualization and element recycling.

### Requirements:
1. Create a ScrollView (vertical) with `Viewport`, `Content`, and a vertical Scrollbar.
2. Implement a script `VirtualChatList.cs` that:
   - Stores a list of messages (e.g., `List<string>`)
   - Creates a pool of 10 UI elements (e.g., a `ChatMessagePrefab` with a `TextMeshProUGUI` component)
   - During scrolling, calculates which message indices should be visible
   - Reuses elements: takes an element from the pool that went off-screen and fills it with new text
   - Subscribes to the ScrollRect's `OnValueChanged` event
  
3. Add test data generation:
   - A button "Add 1000 messages" — adds 1000 messages to the data list and refreshes the display
  
4. Performance indicator (optional):
   - Display the number of physical objects (should remain ~10) vs the total data count (grows)
  
### Expected result:
- Scrolling through 10,000 messages is smooth (60+ FPS)
- Hierarchy always contains only 10–12 objects (not 10,000)
- New messages appear without delays

### Hints:
- Use `RectTransform.anchoredPosition` to position elements
- Each element has a fixed height (e.g., 80 pixels)
- Call a `Refresh()` method when the list size changes or during scrolling
- For the buffer, often use `visibleCount + 2` elements

---

### ⭐ If this project was useful, put a star on GitHub!
