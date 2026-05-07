using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Game/Weapon")]
public class WeaponSO : ScriptableObject
{
    [Header("Basic Parameters")]
    public string weaponName = "New Weapon";
    public int damage = 10;
    public float attackSpeed = 1.0f;
    
    [Header("Visuals")]
    public Sprite icon;
    public GameObject prefab;
    
    [Header("Description")]
    [TextArea(3, 5)]
    public string description;
}
