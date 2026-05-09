using UnityEngine;

public class Inventory : MonoBehaviour
{
    public InventoryItem[] items;
    [SerializeField] private float maxWeight;
    private string ownerName = "Hero";

    private void Start()
    {
        if (GetTotalWeight() > maxWeight)
        {
            Debug.LogWarning($"Inventory of {ownerName} is overloaded!");
        }
    }

    public float GetTotalWeight()
    {
        float total = 0f;

        foreach (InventoryItem item in items)
        {
            total += item.Weight;
        }

        return total;
    }
}
