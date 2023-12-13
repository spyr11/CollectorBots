using System.Collections.Generic;
using UnityEngine;

public class CommandCenter : MonoBehaviour
{
    [SerializeField] private Transform _collectorsPath;
    [SerializeField] private Collider _map;

    private Queue<Vector3> _scannedPositions = new Queue<Vector3>();
    private Colletctor[] _collectors;
    private RaycastHit _hit;
    private Vector3 _origin;
    private float _scanDistance;
    private float _scanRadius;
    private float _maxRadius;
    private float _scanStep;

    private void Awake()
    {
        _maxRadius = _map.bounds.size.x > _map.bounds.size.z ? _map.bounds.size.x : _map.bounds.size.z;
        _scanDistance = _map.bounds.extents.x > _map.bounds.extents.z ? _map.bounds.extents.x : _map.bounds.extents.z;

        _scanRadius = 0;
        _scanStep = 1f;

        _collectorsPath.position = transform.position;
        _origin = transform.position;
    }

    private void Start()
    {
        _collectors = new Colletctor[_collectorsPath.childCount];

        for (int i = 0; i < _collectors.Length; i++)
        {
            _collectors[i] = _collectorsPath.transform.GetChild(i).GetComponent<Colletctor>();
        }
    }

    private void FixedUpdate()
    {
        if (_scanRadius < _maxRadius && CanScan())
        {
            TrySendCollector();
            ChangeScanRadius();
        }

        if (_scannedPositions.Count > _collectors.Length + _maxRadius)
        {
            _scannedPositions.Dequeue();
        }
    }

    private void SetIgnorePositions(Vector3 target)
    {
        _scannedPositions.Enqueue(target);
    }

    private void SelectCollector(Vector3 target)
    {
        foreach (var collector in _collectors)
        {
            if (collector.IsBusy == false)
            {
                collector.SetPosition(target);
                return;
            }
        }
    }

    private void TrySendCollector()
    {
        if (Physics.SphereCast(_origin, _scanRadius, transform.forward, out _hit, _scanDistance)
             && _hit.transform.TryGetComponent<Resource>(out Resource resource)
             && _scannedPositions.Contains(resource.transform.position) == false)
        {
            SetIgnorePositions(resource.transform.position);
            SelectCollector(resource.transform.position);
        }
    }

    private bool CanScan()
    {
        int count = 0;

        foreach (var collector in _collectors)
        {
            if (collector.IsBusy)
            {
                count++;
            }
        }

        return count < _collectors.Length;
    }

    private void ChangeScanRadius()
    {
        _scanRadius += _scanStep;

        if (_scanRadius >= _maxRadius)
        {
            _scanRadius = 0;
        }
    }
}
