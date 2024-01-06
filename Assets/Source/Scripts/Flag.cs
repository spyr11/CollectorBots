using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Flag : MonoBehaviour
{
    [SerializeField] private Collider _mapCollider;

    private bool _isPlaceable;
    private Coroutine _coroutine;

    public event UnityAction OnPlaced;
    public event UnityAction OnCanceled;

    private void OnEnable()
    {
        _isPlaceable = false;
        _coroutine = StartCoroutine(SetNewPosition());
    }

    private void OnDisable()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<CommandCenter>(out _))
        {
            _isPlaceable = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<CommandCenter>(out _))
        {
            _isPlaceable = true;
        }
    }

    private IEnumerator SetNewPosition()
    {
        RaycastHit raycastHit;

        Vector3 targetPosition;

        bool isClicked = false;

        while (isClicked == false)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out raycastHit))
            {
                targetPosition.x = raycastHit.point.x;
                targetPosition.y = transform.position.y;
                targetPosition.z = raycastHit.point.z;

                transform.position = _mapCollider.ClosestPointOnBounds(targetPosition);
            }

            if (Input.GetMouseButtonDown(1))
            {
                OnCanceled?.Invoke();
                gameObject.SetActive(false);
            }

            if (_isPlaceable && Input.GetMouseButtonDown(0))
            {
                isClicked = true;
            }

            yield return null;
        }

        OnPlaced?.Invoke();
    }
}
