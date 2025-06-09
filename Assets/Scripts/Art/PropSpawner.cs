using UnityEngine;

public class PropSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] _coralPrefabs;
    [SerializeField] private int _gridWidth = 5;
    [SerializeField] private int _gridDepth = 5;
    [SerializeField] private float _spacing = 2f;
    [SerializeField] private float _randomOffset = 0.5f;
    [SerializeField] private float _maxRotationX = 10f; 
    [SerializeField] private float _maxRotationY = 20f; 

    void Start()
    {
        int prefabIndex = 0;

        for (int x = 0; x < _gridWidth; x++)
        {
            for (int z = 0; z < _gridDepth; z++)
            {
                if (prefabIndex >= _coralPrefabs.Length) return;

                Vector3 basePosition = new Vector3(x * _spacing, 0f, z * _spacing);
                Vector3 offset = new Vector3(
                    Random.Range(-_randomOffset, _randomOffset),
                    0f,
                    Random.Range(-_randomOffset, _randomOffset)
                );

                Vector3 spawnPos = transform.position + basePosition + offset;

                float randomScale = Random.Range(1f, 1.5f);
                float xRot = Random.Range(-_maxRotationX, _maxRotationX);
                float yRot = Random.Range(-_maxRotationY, _maxRotationY);
                Quaternion randomRotation = Quaternion.Euler(xRot, yRot, 0f);

                GameObject newProp = Instantiate(_coralPrefabs[prefabIndex], spawnPos, randomRotation);
                newProp.transform.localScale = Vector3.one * randomScale;

                prefabIndex++;
            }
        }
    }
}