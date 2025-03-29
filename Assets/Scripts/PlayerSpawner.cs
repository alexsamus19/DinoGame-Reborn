using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private float offsetFromLeftEdge = 200f;

    private void Start()
    {
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("player prefab nema");
            return;
        }

        Vector2 spawnPos = CalculateLeftSpawnPosition();
        Instantiate(playerPrefab, spawnPos, Quaternion.identity);
    }

    private Vector2 CalculateLeftSpawnPosition()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            Debug.LogError("nema cameri");
            return Vector2.zero;
        }

        float leftEdgeX = mainCam.ScreenToWorldPoint(Vector3.zero).x;
        float spawnX = leftEdgeX + offsetFromLeftEdge;
        float spawnY = 0f;

        return new Vector2(spawnX, spawnY);
    }
}