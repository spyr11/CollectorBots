using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Colletctor : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float _speed;

    public bool IsBusy { get; private set; }

    private Vector3 _targetPosition;
    private Transform _target;
    private bool _isPickedUp;

    private void Awake()
    {
        IsBusy = false;

        _rigidbody = GetComponent<Rigidbody>();
        _targetPosition = transform.position;
        _isPickedUp = false;
        _target = null;
    }

    private void Update()
    {
        MoveOnTarget();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Resource>(out Resource resource) && resource.transform.position == _targetPosition)
        {
            _target = resource.transform;

            PickUp();
            moveHome();
        }

        if (other.TryGetComponent<CommandCenter>(out CommandCenter commandCenters) && _isPickedUp && IsBusy)
        {
            DropItem();
        }
    }

    public void SetPosition(Vector3 targetPosition)
    {
        _targetPosition = targetPosition;
        IsBusy = true;
    }

    private void MoveOnTarget()
    {
        transform.LookAt(_targetPosition);
        _rigidbody.velocity = transform.forward * _speed * Time.deltaTime;
    }

    private void PickUp()
    {
        _target.position = GetComponentInChildren<BagPoint>().transform.position;
        _target.SetParent(transform);
        _isPickedUp = true;
    }

    private void DropItem()
    {
        IsBusy = false;
        _isPickedUp = false;
    }

    private void moveHome()
    {
        SetPosition(transform.parent.position);
    }
}
