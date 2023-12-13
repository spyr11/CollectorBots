using UnityEngine;

public class Resource : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<CommandCenter>(out CommandCenter commandCenter))
        {
            Destroy(gameObject);
        }
    }
}
