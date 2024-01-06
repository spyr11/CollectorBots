using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Scanner))]
[RequireComponent(typeof(Wallet))]
public class CommandCenter : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Collector _collector;

    private List<Collector> _collectors;
    private Queue<Resource> _resources;
    private MainData _mainData;
    private Scanner _scanner;
    private Wallet _wallet;
    private Flag _flag;
    private bool _isReadyToBuild;

    public event UnityAction OnCollected;
    public event UnityAction OnBuildReady;

    private void Awake()
    {
        _scanner = GetComponent<Scanner>();
        _wallet = GetComponent<Wallet>();
    }

    private void Start()
    {
        _resources = new Queue<Resource>();
        _collectors = new List<Collector>();

        _mainData = GetComponentInParent<MainData>();

        _flag = _mainData.GetComponentInChildren<Flag>();
        _flag.gameObject.SetActive(false);

        for (int i = 0; i < transform.childCount; i++)
        {
            _collectors.Add(transform.GetChild(i).GetComponent<Collector>());
        }
    }

    private void OnEnable()
    {
        _wallet.OnDroneCollected += AddCollector;
        _wallet.OnBaseCollected += SendBuild;
    }

    private void OnDisable()
    {
        _wallet.OnDroneCollected -= AddCollector;
        _wallet.OnBaseCollected -= SendBuild;

        if (_flag.isActiveAndEnabled)
        {
            _flag.OnCanceled -= DeactivateFlag;
        }
    }

    private void Update()
    {
        if (_resources.Count > 0 && _isReadyToBuild == false)
        {
            TrySendCollectors();
        }
        else
        {
            EnableScan();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.TryGetComponent<Resource>(out Resource resource) && CanEnter(resource.transform))
        {
            OnCollected?.Invoke();

            Destroy(resource.gameObject);
            _mainData.RemoveResource(resource);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_flag.isActiveAndEnabled == false)
        {
            _flag.transform.position = transform.position;
            _flag.gameObject.SetActive(true);

            _flag.OnPlaced += TrySendBuild;
            _flag.OnCanceled += DeactivateFlag;
        }
    }

    private bool CanEnter(Transform resource)
    {
        return resource.parent != null && transform == resource.parent.parent;
    }

    private void EnableScan()
    {
        _mainData.SortByResource(ref _resources, _scanner.GetScan());
    }

    private void AddCollector()
    {
        _collectors.Add(CreateCollector());
    }

    private Collector CreateCollector()
    {
        Collector collector = Instantiate(_collector, transform.position, transform.rotation);
        collector.transform.SetParent(transform);

        return collector;
    }

    private void TrySendCollectors()
    {
        Collector collector = FindFreeCollector();

        if (collector == null)
        {
            return;
        }

        collector.SetResource(_resources.Dequeue());
    }

    private void TrySendBuild()
    {
        OnBuildReady?.Invoke();
    }

    private void SendBuild()
    {
        _isReadyToBuild = true;

        StartCoroutine(SelectDroneToBuild());
    }

    private IEnumerator SelectDroneToBuild()
    {
        Collector collector;

        while ((collector = FindFreeCollector()) == null)
        {
            yield return null;
        }

        ReleaseCollector(collector);
        collector.SetNewBasePosition(_flag.transform.position);

        DeactivateFlag();

        _isReadyToBuild = false;
    }

    private Collector FindFreeCollector()
    {
        foreach (var collector in _collectors)
        {
            if (collector.IsBusy == false)
            {
                return collector;
            }
        }

        return null;
    }

    private void ReleaseCollector(Collector collector)
    {
        _collectors.Remove(collector);

        collector.transform.SetParent(null);
    }

    private void DeactivateFlag()
    {
        _flag.OnPlaced -= TrySendBuild;
    }
}