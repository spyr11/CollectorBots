using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Map))]
public class ResourceSpawner : MonoBehaviour
{
    [SerializeField] private Resource _resource;
    [SerializeField] private Collider _map;
    [SerializeField] private float _seconds;

    private void Awake()
    {
        _map = GetComponent<Collider>();
    }

    private void Start()
    {
        StartCoroutine(SetPoints(_seconds));
    }


    private IEnumerator SetPoints(float seconds)
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(seconds);

        bool isActive = true;

        while (isActive)
        {
            Instantiate(_resource,transform.position+ GetPosition(),_resource.transform.rotation);

            yield return waitForSeconds;
        }
    }

    private Vector3 GetPosition()
    {
        float mapSizeX=_map.bounds.size.x/2;
        float mapSizeZ=_map.bounds.size.z/2;
        float positionY = 1f;
        return new Vector3(Random.Range(-mapSizeX, mapSizeX), positionY, Random.Range(-mapSizeZ,mapSizeZ));
    }
}