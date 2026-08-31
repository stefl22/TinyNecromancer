using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;

    [SerializeField] private Vector3 offset = new Vector3(0f, 10f, -8f);

    [SerializeField] private float followSpeed = 8f;

    [SerializeField] private Vector3 lookOffset = new Vector3(0f, 1f, 0f);

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 desiredPosition = target.position + offset;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime
        );

        transform.LookAt(target.position + lookOffset);
    }
}