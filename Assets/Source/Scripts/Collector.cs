using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Renderer))]
public class Collector : MonoBehaviour
{
    [SerializeField] private CommandCenter _commandCenter;
    [SerializeField] private float _speed;

    private Rigidbody _rigidbody;
    private Resource _resource;
    private Transform _parent;

    private Color _builderColor;
    private Color _originColor;

    private Vector3 _targetPosition;
    private Vector3 _homePosition;

    private bool _isPickedUp;

    public bool IsBusy { get; private set; }

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _originColor = GetComponent<Renderer>().material.color;
        _builderColor = Color.green;

        _parent = transform.parent;
        _homePosition = _parent.position;

        _isPickedUp = false;
        IsBusy = false;
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

    public void SetResource(Resource resource)
    {
        IsBusy = true;

        _resource = resource;

        SetPosition(resource.transform.position);
    }

    public void SetNewBasePosition(Vector3 position)
    {
        IsBusy = true;
        SetPosition(position);
        GetComponent<Renderer>().material.color = _builderColor;
    }

    private void SetPosition(Vector3 position)
    {
        _targetPosition = position;
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
        if (other.TryGetComponent<Resource>(out Resource resource) && resource == _resource)
        {
            _resource.transform.position = GetComponentInChildren<BagPoint>().transform.position;
            _resource.transform.SetParent(transform);

            SetPosition(_homePosition);

            _isPickedUp = true;
        }
    }

    private void TryDropItem(Collider other)
    {
        if (other.TryGetComponent<CommandCenter>(out CommandCenter commandCenter)
         && _isPickedUp && _parent == commandCenter.transform)
        {
            _isPickedUp = false;
            IsBusy = false;
        }
    }

    private void TryBuild(Collider other)
    {
        if (other.TryGetComponent<Flag>(out Flag flag) && transform.parent == null)
        {
            IsBusy = false;
            Build();
            UpdateParameters();
        }
    }

    private void Build()
    {
        _targetPosition.y = _parent.position.y;
        _commandCenter = Instantiate(_commandCenter, _targetPosition, Quaternion.identity);
        _commandCenter.transform.SetParent(_parent.parent);
    }

    private void UpdateParameters()
    {
        GetComponent<Renderer>().material.color = _originColor;

        _parent = _commandCenter.transform;
        transform.SetParent(_parent);
        _homePosition = _parent.position;
    }
}