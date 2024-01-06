
using System.Collections.Generic;
using UnityEngine;

public class MainData : MonoBehaviour
{
    private List<Resource> _resources;

    private void Awake()
    {
        _resources = new List<Resource>();
    }

    public void SortByResource(ref Queue<Resource> resources, Collider[] colliders)
    {
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent<Resource>(out Resource resource) && _resources.Contains(resource) == false)
            {
                resources.Enqueue(resource);
                _resources.Add(resource);
            }
        }
    }

    public void RemoveResource(Resource resource)
    {
        _resources.Remove(resource);
    }
}
