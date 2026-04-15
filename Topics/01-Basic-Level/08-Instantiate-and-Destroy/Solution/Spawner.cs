using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> _spawnedObjects = new List<GameObject>();
    [SerializeField, Range(1f, 5f)] private float _spawnDelay;
    [SerializeField, Range(5f, 10f)] private float _spawnRange;
    [SerializeField, Range(10f, 25f)] private float _spawnHeight;

    private float _timer;
    private float _destroyDelay = 4f;

    private void Awake()
    {
        if (_spawnedObjects == null || _spawnedObjects.Count == 0)
        {
            Debug.LogWarning("SpawnedObjects is not assigned!");
            enabled = false;
            return;
        }
    }

    void Start()
    {
        transform.position = Vector3.up * _spawnHeight;
    }

    void Update()
    {
        if (_timer >= _spawnDelay)
        {
            Vector3 randPosition = transform.position + Vector3.right * Random.Range(-_spawnRange, _spawnRange);
            GameObject randObject = _spawnedObjects[Random.Range(0, _spawnedObjects.Count)];
            GameObject newObj = Instantiate(randObject, randPosition, Quaternion.identity);
            Destroy(newObj, _destroyDelay);

            _timer = 0;
        }
        else
        {
            _timer += Time.deltaTime;
        }
    }
}
