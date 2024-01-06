using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CommandCenter))]
public class Wallet : MonoBehaviour
{
    private CommandCenter _commandCenter;

    private int _resourcesToCreateCollector;
    private int _resourcesToCreateBase;
    private int _money;

    private bool _isReadyToBuild;

    public event UnityAction OnDroneCollected;
    public event UnityAction OnBaseCollected;

    private void Start()
    {
        _resourcesToCreateCollector = 3;
        _resourcesToCreateBase = 5;
        _money = 0;

        _isReadyToBuild = false;
    }

    private void OnEnable()
    {
        _commandCenter = GetComponent<CommandCenter>();

        _commandCenter.OnCollected += FillUp;
        _commandCenter.OnBuildReady += SetReadyToBuild;
    }

    private void OnDisable()
    {
        _commandCenter.OnCollected -= FillUp;
        _commandCenter.OnBuildReady -= SetReadyToBuild;
    }

    private void FillUp()
    {
        _money++;

        TrySpendToCollector();
        TrySpendToBase();
    }

    private void SetReadyToBuild()
    {
        _isReadyToBuild = true;
    }

    private void TrySpendToCollector()
    {
        if (_money >= _resourcesToCreateCollector && _isReadyToBuild == false)
        {
            _money -= _resourcesToCreateCollector;

            OnDroneCollected?.Invoke();
        }
    }

    private void TrySpendToBase()
    {
        if (_money >= _resourcesToCreateBase && _isReadyToBuild)
        {
            _isReadyToBuild = false;

            _money -= _resourcesToCreateBase;

            OnBaseCollected?.Invoke();
        }
    }
}