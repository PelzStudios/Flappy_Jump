using UnityEngine;

public class RingManager : MonoBehaviour
{
    [SerializeField] private GameObject ringPrefab;
    [SerializeField] private float spawnRate = 2.5f;
    [SerializeField] private float ringSpeed = 5f;
    [SerializeField] private float minHoleY = -3f;
    [SerializeField] private float maxHoleY = 3f;
    
    private float spawnTimer = 0f;
    private float currentSpeed;
    
    private void Start()
    {
        currentSpeed = ringSpeed;
        
        if (ringPrefab == null)
        {
            Debug.LogError("RingManager: Ring Prefab is not assigned! Assign it in Inspector!");
        }
    }
    
    private void Update()
    {
        spawnTimer += Time.deltaTime;
        
        if (spawnTimer >= spawnRate)
        {
            SpawnRing();
            spawnTimer = 0f;
        }
    }
    
    private void SpawnRing()
    {
        if (ringPrefab == null)
        {
            Debug.LogError("Cannot spawn ring - prefab is null!");
            return;
        }
        
        // Random Y position for the hole
        float holeY = Random.Range(minHoleY, maxHoleY);
        
        // Spawn ring from RIGHT side of camera view (X = 6, just off-screen)
        Vector3 spawnPos = new Vector3(6f, holeY, 0);
        GameObject newRing = Instantiate(ringPrefab, spawnPos, Quaternion.identity, transform);
        
        // Set speed
        Ring ringScript = newRing.GetComponent<Ring>();
        if (ringScript != null)
        {
            ringScript.SetSpeed(currentSpeed);
        }
        
        Debug.Log($"Ring spawned at Position=({spawnPos.x}, {holeY}), Speed={currentSpeed}");
    }
    
    public void IncreaseDifficulty()
    {
        currentSpeed += 0.5f;
        spawnRate = Mathf.Max(1f, spawnRate - 0.1f);
        Debug.Log($"Difficulty increased! Speed={currentSpeed}, SpawnRate={spawnRate}");
    }
    
    public void ResetRings()
    {
        currentSpeed = ringSpeed;
        spawnTimer = 0f;
        
        // Destroy all child rings
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        
        Debug.Log("Rings reset");
    }
}