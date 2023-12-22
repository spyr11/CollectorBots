using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Resource : MonoBehaviour
{
    private void OnEnable()
    {
       GetComponent<Collider>().isTrigger = false;
    }
}
