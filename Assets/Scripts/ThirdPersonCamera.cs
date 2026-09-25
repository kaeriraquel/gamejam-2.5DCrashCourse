using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] private Transform target;

    [SerializeField] private float distance = 7f;
    [SerializeField] private float height = 4f;

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 cameraPosition = target.position
            - target.forward * distance
            + Vector3.up * height;

        transform.position = cameraPosition;

        transform.LookAt(target);
    }
}