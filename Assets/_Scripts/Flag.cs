using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Flag : MonoBehaviour
{
    public event UnityAction OnPlaced;

    private bool _isPlaceable;

    private void OnEnable()
    {    
        _isPlaceable = false;

        StartCoroutine(SetNewPosition());
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

        Collider map = FindObjectOfType<Map>().GetComponent<Collider>();
        
        bool isClicked = false;

        while (isClicked == false)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out raycastHit))
            {
                targetPosition.x = raycastHit.point.x;
                targetPosition.y = transform.position.y;
                targetPosition.z = raycastHit.point.z;

                Vector3 mapPointOnBound = map.ClosestPointOnBounds(targetPosition);
                transform.position = mapPointOnBound;
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
