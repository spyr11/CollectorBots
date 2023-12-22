using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CommandCenter))]
public class Scanner : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask;

    private Collider _map;
    private CommandCenter _center;
    private Vector3 _originPosition;
    private Queue<Vector3> _positions;
    private float _currentRadius;
    private float _maxRadius;
    private float _minRadius;
    private float _step;

    private void Awake()
    {
        _map = FindObjectOfType<Map>().GetComponent<Collider>();
        _center = GetComponent<CommandCenter>();
        _positions = new Queue<Vector3>();

        _step = 0.1f;
        _minRadius = 1f;
        _currentRadius = _minRadius;
        _maxRadius = _map.bounds.size.x > _map.bounds.size.z ? _map.bounds.size.x : _map.bounds.size.z;

        _originPosition = transform.position;
    }

    private void Update()
    {
        if (CanUse())
        {
            ChangeRadius();
            SetTargets(Locate());
        }
        else
        {
            _currentRadius = _minRadius;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(_originPosition, _currentRadius);
    }

    public Vector3 GetPosition()
    {
        return _positions.Dequeue();
    }

    public int GetPositionsCount()
    {
        return _positions.Count;
    }

    private void SetTargets(Collider[] colliders)
    {
        foreach (Collider collider in colliders)
        {
            if (collider.isTrigger == false)
            {
                collider.isTrigger = true;
                _positions.Enqueue(collider.transform.position);
            }
        }
    }

    private Collider[] Locate()
    {
        return Physics.OverlapSphere(_originPosition, _currentRadius, _layerMask);
    }

    private bool CanUse()
    {
        return _center.GetFreeCollectorsCount() > _positions.Count;
    }

    private void ChangeRadius()
    {
        _currentRadius += _step;

        if (_currentRadius >= _maxRadius)
        {
            _currentRadius = _minRadius;
        }
    }
}
