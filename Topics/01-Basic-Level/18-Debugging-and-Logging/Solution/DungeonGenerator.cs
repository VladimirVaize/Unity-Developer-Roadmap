using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    private void Start()
    {
        GenerateDungeon(); // Test
    }

    public void GenerateDungeon()
    {
        GameLogger.LogGeneration("Starting dungeon generation...");
        GameLogger.LogGeneration("Created room #" + Random.Range(1, 20));
        GameLogger.LogGeneration("Generation complete. Total enemies: " + Random.Range(5, 50));
    }
}