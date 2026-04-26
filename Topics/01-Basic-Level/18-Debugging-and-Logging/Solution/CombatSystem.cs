using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    private void Start()
    {
        Attack(gameObject, 10); // Test
    }

    public void Attack(GameObject target, int damage)
    {
        GameLogger.LogCombat($"Attack on {target.name} with {damage} damage");
        
        if (Random.value < 0.5f)
            GameLogger.LogCriticalError("CRITICAL ERROR: target not found in the combat system!");
    }
}