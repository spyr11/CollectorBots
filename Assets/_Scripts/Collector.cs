using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Collector : MonoBehaviour
{
    [SerializeField] private CommandCenter _commandCenter;
    [SerializeField] private float _speed;

    public bool IsBusy { get; private set; }

    private Color _originColor;
    private Rigidbody _rigidbody;
    private Vector3 _targetPosition;
    private Vector3 _homePosition;
    private bool _isPickedUp;

    private void Start()
    {
        IsBusy = false;

        _rigidbody = GetComponent<Rigidbody>();

        _originColor = GetComponent<Renderer>().material.color;

        _homePosition = transform.parent.position;

        _isPickedUp = false;
    }

    private void FixedUpdate()
    {
        if (IsBusy)
        {
            Move();
        }
        else
        {
            _rigidbody.velocity = Vector3.zero;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        TryPickUp(other);

        TryDropItem(other);

        TryBuild(other);
    }

    public void SetPosition(Vector3 targetPosition)
    {
        IsBusy = true;

        _targetPosition = targetPosition;
    }

    private void Move()
    {
        LookAtTarget();
        _rigidbody.velocity = transform.forward * _speed * Time.fixedDeltaTime;
    }

    private void LookAtTarget()
    {
        Quaternion lookRotation = Quaternion.LookRotation(_targetPosition - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, _speed * Time.deltaTime);
    }

    private void TryPickUp(Collider other)
    {
        if (other.TryGetComponent<Resource>(out Resource resource) && resource.transform.position == _targetPosition)
        {
            resource.transform.position = GetComponentInChildren<BagPoint>().transform.position;
            resource.transform.SetParent(transform);
            _isPickedUp = true;

            SetPosition(_homePosition);
        }
    }

    private void TryDropItem(Collider other)
    {
        if (other.TryGetComponent<CommandCenter>(out CommandCenter commandCenter) && _isPickedUp
            && transform.IsChildOf(commandCenter.transform))
        {
            IsBusy = false;
            _isPickedUp = false;
        }
    }

    private void TryBuild(Collider other)
    {
        if (other.TryGetComponent<Flag>(out Flag flag) && transform.parent == null
           && flag.transform.position == _targetPosition)
        {
            IsBusy = false;

            Build(flag.transform.position);

            UpdateParameters();
        }
    }

    private void Build(Vector3 targetPosition)
    {
        _commandCenter = Instantiate(_commandCenter, targetPosition, Quaternion.identity);
    }

    private void UpdateParameters()
    {
        GetComponent<Renderer>().material.color = _originColor;

        transform.SetParent(_commandCenter.transform);
        _homePosition = _commandCenter.transform.position;
    }
}