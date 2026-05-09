using UnityEngine;

[System.Serializable]
public struct InventoryItem
{
    public string itemName;
    [SerializeField] private float weight;
    [HideInInspector] public int price;

    public float Weight => weight;
}
