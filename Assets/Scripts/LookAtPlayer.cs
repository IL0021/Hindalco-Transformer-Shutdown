using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public float offset = 0f;
    public bool AllowX = false;
    public float rotationSpeed = 5f;

    void LateUpdate()
    {
        Vector3 direction = Camera.main.transform.position - transform.position;

        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

        if (AllowX)
        {
            Quaternion targetYRotation = Quaternion.Euler(-targetRotation.eulerAngles.x, targetRotation.eulerAngles.y + offset, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetYRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            Quaternion targetYRotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y + offset, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetYRotation, rotationSpeed * Time.deltaTime);
        }
    }
}