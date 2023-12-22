using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Scanner))]
public class CommandCenter : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Collector _collector;
    [SerializeField] private int _maxDrones;

    private List<Collector> _collectors;
    private Color _builderColor;
    private Scanner _scanner;
    private Flag _flag;
    private int _resourceCount;
    private int _resourcesToCreateBase;
    private int _resourcesToCreateCollector;

    private void Awake()
    {
        _scanner = GetComponent<Scanner>();

        _flag = FindObjectOfType<Flag>();
        _flag.gameObject.SetActive(false);

        _builderColor = Color.green;

        _resourcesToCreateBase = 5;
        _resourcesToCreateCollector = 3;
    }

    private void Start()
    {
        _collectors = new List<Collector>();

        if (transform.childCount > 0)
        {
            _collectors.Add(GetComponentInChildren<Collector>());
        }

        for (int i = 0; i < _maxDrones; i++)
        {
            AddCollector();
        }
    }

    private void Update()
    {
        TrySpendOnCollector();

        TrySendToCollect();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.TryGetComponent<Resource>(out Resource resource) && CanEnter(resource.transform))
        {
            _resourceCount++;
            Destroy(other.gameObject);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_flag.isActiveAndEnabled == false)
        {
            _flag.transform.position = transform.position;            
            _flag.gameObject.SetActive(true);
            _flag.OnPlaced += SendBuild;
        }
    }

    public int GetFreeCollectorsCount()
    {
        int count = 0;

        foreach (Collector collector in _collectors)
        {
            if (collector.IsBusy == false)
            {
                count++;
            }
        }

        return count;
    }

    private bool CanEnter(Transform resource)
    {
        return resource.parent != null && resource.IsChildOf(transform);
    }

    private void TrySpendOnCollector()
    {
        if (_resourceCount >= _resourcesToCreateCollector && _flag.isActiveAndEnabled == false)
        {
            _resourceCount -= _resourcesToCreateCollector;

            AddCollector();
        }
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

    private void TrySendToCollect()
    {
        int dronesCount = GetFreeCollectorsCount();
        int positionCount = _scanner.GetPositionsCount();
        int count = dronesCount <= positionCount ? dronesCount : positionCount;

        for (int i = 0; i < count; i++)
        {
            SendCollectors(_scanner.GetPosition());
        }
    }

    private void SendCollectors(Vector3 position)
    {
        Collector collector = FindFreeCollector();

        collector.SetPosition(position);
    }

    private void SendBuild()
    {
        StartCoroutine(TrySpendOnNewBase());
    }

    private IEnumerator TrySpendOnNewBase()
    {
        while (_resourceCount < _resourcesToCreateBase)
        {
            yield return null;
        }

        _flag.OnPlaced -= SendBuild;

        _resourceCount -= _resourcesToCreateBase;

        SelectDroneToBuild(_flag.transform.position);
    }

    private void SelectDroneToBuild(Vector3 position)
    {
        StartCoroutine(TrySelectDroneToBuild(position));
    }

    private IEnumerator TrySelectDroneToBuild(Vector3 position)
    {
        Collector collector;

        while ((collector = FindFreeCollector()) == null)
        {
            yield return null;
        }

        ReleaseCollector(collector, position);
    }

    private void ReleaseCollector(Collector collector, Vector3 position)
    {
        _collectors.Remove(collector);

        collector.SetPosition(position);
        collector.transform.GetComponent<Renderer>().material.color = _builderColor;
        collector.transform.SetParent(null);
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
}
