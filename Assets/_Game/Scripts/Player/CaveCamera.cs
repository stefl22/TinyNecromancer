using UnityEngine;
using UnityEngine.InputSystem;

// Orbit with middle mouse; zoom with wheel. Right mouse remains shield block.
public class CaveCamera : MonoBehaviour
{
    public Transform target;
    public float distance = 5.2f;
    public float yaw = 0f;
    public float pitch = 24f;
    void LateUpdate()
    {
        if (target == null) return;
        var mouse = Mouse.current;
        if (mouse != null)
        {
            if (mouse.middleButton.isPressed)
            {
                var delta = mouse.delta.ReadValue(); yaw += delta.x * .16f;
                pitch = Mathf.Clamp(pitch - delta.y * .13f, 8, 65);
            }
            distance = Mathf.Clamp(distance - mouse.scroll.ReadValue().y * .0025f, 2.2f, 7f);
        }
        Vector3 pivot = target.position + Vector3.up * 1.3f;
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 direction = rotation * Vector3.back;
        float actual = distance;
        // Ignore character colliders and only shorten against environment meshes.
        foreach (var hit in Physics.SphereCastAll(pivot, .15f, direction, distance, ~0, QueryTriggerInteraction.Ignore))
            if (hit.collider.GetComponentInParent<Health>() == null) actual = Mathf.Min(actual, Mathf.Max(.5f, hit.distance - .08f));
        transform.position = pivot + direction * actual;
        transform.rotation = rotation;
    }
}
