using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [HideInInspector] public Transform target;
    public float mouseSensitivity = 10f;

    private float verticalRotation;
    private float horizontalRotation;

    private void LateUpdate()
    {
        if (target == null)
            return;

        transform.position = target.position;

        var mouseX = Input.GetAxis("Mouse X");
        var mouseY = Input.GetAxis("Mouse Y");

        verticalRotation -= mouseY * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -70f, 70f);

        horizontalRotation += mouseX * mouseSensitivity;

        transform.rotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0);
    }
}
