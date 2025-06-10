using System.Collections.Generic;
using UnityEngine;

public class PropSpawner : MonoBehaviour
{
    [Header("Coral art assets")]
    [SerializeField] private GameObject[] _coralPrefabs;
    [Range(0, 300)]
    [SerializeField] private int _amountOfCoral;
    [Header("Spawning grid settings")]
    [SerializeField] private int _gridWidth;  
    [SerializeField] private int _gridDepth; 
    [SerializeField] private float _spacing;
    [Header("Asset variation settings")]
    [SerializeField] private float _randomOffset;
    [SerializeField] private float _maxRotationX;
    [SerializeField] private float _maxRotationY;
    [SerializeField] private float _minScale;
    [SerializeField] private float _maxScale;
    [Header("Perlin noise settings")]
    [Tooltip("Adjust for tighter or looser clusters")]
    [Range(0,1f)]
    [SerializeField] private float _noiseScale;
    [Tooltip("Adjust for fewer or more corals in clusters")]
    [Range(0, 1f)]
    [SerializeField] private float _noiseLimit;

    public void SpawnProps()
    {
        int coralsSpawned = 0;
        List<Vector3> usedPositions = new List<Vector3>();
        float minSpacingSqr = 0.1f * 0.1f;

        float totalWidth = (_gridWidth - 1) * _spacing;
        float totalDepth = (_gridDepth - 1) * _spacing;

        for (int x = 0; x < _gridWidth; x++)
        {
            for (int z = 0; z < _gridDepth; z++)
            {
                if (coralsSpawned >= _amountOfCoral || _coralPrefabs.Length == 0)
                    return;

                // Base grid position 
                Vector3 basePosition = new Vector3(
                    x * _spacing - totalWidth / 2f,
                    transform.position.y,
                    z * _spacing - totalDepth / 2f
                );

                // Apply Perlin Noise to make the asset placement feel more natural
                float noise = Mathf.PerlinNoise(x * _noiseScale, z * _noiseScale);
                if (noise < _noiseLimit) // Skip if noise value is too low
                    continue;

                // Random offset (smaller = more grid-like, larger = more scattered)
                Vector3 offset = new Vector3(
                    Random.Range(-_randomOffset, _randomOffset),
                    0f,
                    Random.Range(-_randomOffset, _randomOffset)
                );

                Vector3 spawnPos = basePosition + offset;

                // Skip if too close to existing coral
                bool positionTaken = false;
                foreach (Vector3 usedPos in usedPositions)
                {
                    if ((usedPos - spawnPos).sqrMagnitude < minSpacingSqr)
                    {
                        positionTaken = true;
                        break;
                    }
                }
                if (positionTaken) continue;

                // Spawn coral
                float randomScale = Random.Range(_minScale, _maxScale);
                float xRot = Random.Range(-_maxRotationX, _maxRotationX);
                float yRot = Random.Range(-_maxRotationY, _maxRotationY);
                Quaternion randomRotation = Quaternion.Euler(xRot, yRot, 0f);

                GameObject randomPrefab = _coralPrefabs[Random.Range(0, _coralPrefabs.Length)];
                Instantiate(randomPrefab, spawnPos, randomRotation).transform.localScale = Vector3.one * randomScale;

                usedPositions.Add(spawnPos);
                coralsSpawned++;
            }
        }
    }

    //private void OnDrawGizmosSelected()
    //{
    //    if (_gridWidth <= 0 || _gridDepth <= 0) return;

    //    float totalWidth = (_gridWidth - 1) * _spacing;
    //    float totalDepth = (_gridDepth - 1) * _spacing;

    //    Gizmos.color = Color.cyan;

    //    for (int x = 0; x < _gridWidth; x++)
    //    {
    //        for (int z = 0; z < _gridDepth; z++)
    //        {
    //            Vector3 basePosition = new Vector3(
    //                x * _spacing - totalWidth / 2f,
    //                0f,
    //                z * _spacing - totalDepth / 2f
    //            );

    //            Vector3 spawnPos = transform.position + basePosition;

    //            Gizmos.DrawWireCube(spawnPos, Vector3.one * 0.5f); 
    //        }
    //    }
    //}
}


