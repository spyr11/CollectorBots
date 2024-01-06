using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Map))]
public class ResourceSpawner : MonoBehaviour
{
    [SerializeField] private Resource _resourcePrefab;
    [SerializeField] private Collider _map;
    [SerializeField] private float _seconds;

    private void Awake()
    {
        _map = GetComponent<Collider>();
    }

    private void Start()
    {
        StartCoroutine(SetBreedPoints(_seconds));
    }

    private IEnumerator SetBreedPoints(float seconds)
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(seconds);

        bool isActive = true;

        while (isActive)
        {
           Instantiate(_resourcePrefab, transform.position + GetPosition(), _resourcePrefab.transform.rotation);

            yield return waitForSeconds;
        }
    }

    private Vector3 GetPosition()
    {
        float mapSizeX = _map.bounds.size.x / 2;
        float mapSizeZ = _map.bounds.size.z / 2;
        float positionY = 1f;
        
        return new Vector3(Random.Range(-mapSizeX, mapSizeX), positionY, Random.Range(-mapSizeZ, mapSizeZ));
    }
}
