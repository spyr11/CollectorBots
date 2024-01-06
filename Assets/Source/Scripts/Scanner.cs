using UnityEngine;

[RequireComponent(typeof(CommandCenter))]
public class Scanner : MonoBehaviour
{
    private Vector3 _originPosition;

    private float _currentRadius;
    private float _maxRadius;
    private float _minRadius;
    private float _step;

    private void Start()
    {
        _originPosition = transform.position;

        _step = 0.1f;
        _minRadius = 1f;
        _maxRadius = 25f;
        _currentRadius = _maxRadius;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(_originPosition, _currentRadius);
    }

    public Collider[] GetScan()
    {
        ChangeRadius();

        return Physics.OverlapSphere(_originPosition, _currentRadius);
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
