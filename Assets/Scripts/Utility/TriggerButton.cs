using UnityEngine;

public class TriggerButton : MonoBehaviour
{
    public bool isTriggered = false;
    void OnTriggerEnter(Collider other)
    {
        isTriggered = true;
    }
    void OnTriggerStay(Collider other)
    {
        isTriggered = true;
    }
    void OnTriggerExit(Collider other)
    {
        isTriggered = false;
    }
}
