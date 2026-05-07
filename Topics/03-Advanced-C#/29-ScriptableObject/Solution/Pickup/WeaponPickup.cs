using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [Header("ScriptableObject References")]
    [Tooltip("Drag a weapon asset (.asset) here")]
    public WeaponSO weaponData;
    
    [Tooltip("Drag global score asset here")]
    public GlobalScoreSO globalScore;
    
    [Header("Settings")]
    [SerializeField] private bool destroyOnPickup = true;
    [SerializeField] private float pickupCooldown = 0.1f;
    
    private bool canPickup = true;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!canPickup) return;
        
        if (other.CompareTag("Player"))
        {
            Pickup();
        }
    }
    
    private void Pickup()
    {
        if (weaponData == null)
        {
            Debug.LogError($"[WeaponPickup] {gameObject.name}: weaponData not assigned!");
            return;
        }
        
        if (globalScore == null)
        {
            Debug.LogError($"[WeaponPickup] {gameObject.name}: globalScore not assigned!");
            return;
        }
        
        Debug.Log($"Picked up: {weaponData.weaponName} | Damage: {weaponData.damage} | Attack Speed: {weaponData.attackSpeed}");
        
        globalScore.AddScore(weaponData.damage);
        
        // Pickup effect (optional)
        OnPickupEffect();
        
        // Destroy object if needed
        if (destroyOnPickup)
        {
            canPickup = false;
            Destroy(gameObject, pickupCooldown);
        }
    }
    
    private void OnPickupEffect()
    {
        // Here you can add:
        // - play sound
        // - spawn particles
        // - animation
        Debug.Log($"Pickup effect for {weaponData.weaponName}");
    }
    
    // Editor-only debugging
    private void OnValidate()
    {
        if (weaponData != null && gameObject.name == "GameObject")
        {
            // Auto-rename object for convenience
            gameObject.name = $"Pickup_{weaponData.weaponName}";
        }
    }
}
