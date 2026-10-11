using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public GameObject[] enemyPrefabs;
    public float spawnTime = 3f;
    public Transform[] spawnPoints;


    void Start ()
    {
        InvokeRepeating ("Spawn", spawnTime, spawnTime);
    }


    void Spawn ()
    {
        if(playerHealth == null || playerHealth.currentHealth <= 0f ||
            enemyPrefabs == null || enemyPrefabs.Length == 0 ||
            spawnPoints == null || spawnPoints.Length == 0)
        {
            return;
        }

        // Chọn đều giữa các prefab đã gán, bỏ qua ô còn trống.
        GameObject enemy = null;
        int validCount = 0;
        foreach (GameObject prefab in enemyPrefabs)
        {
            if (prefab == null) continue;
            validCount++;
            if (Random.Range(0, validCount) == 0) enemy = prefab;
        }
        if (enemy == null) return;

        int spawnPointIndex = Random.Range (0, spawnPoints.Length);
        if (spawnPoints[spawnPointIndex] == null) return;

        Instantiate (enemy, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);
    }
}
