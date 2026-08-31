using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;

    private void Awake()
    {
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void LateUpdate()
    {
        if (cameraTransform == null)
            return;

        transform.rotation = cameraTransform.rotation;
    }
}