using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField]
    Transform followTarget;

    Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        _camera.transform.LookAt(followTarget);

        Vector3 pos = _camera.transform.position;
        pos.x = followTarget.position.x - 2f;
        _camera.transform.position = pos;
    }


}
