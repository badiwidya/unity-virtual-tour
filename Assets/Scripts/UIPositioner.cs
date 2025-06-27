using UnityEngine;

public class UIPositioner : MonoBehaviour
{
    public Transform mainCamera;
    
    public float spawnDistance = 1.0f;

    public void PositionObjectInFrontOfPlayer(GameObject objectToPosition)
    {
        if (mainCamera == null || objectToPosition == null) return;

        var targetPosition = mainCamera.position + (mainCamera.forward * spawnDistance);

        var cameraAngles = mainCamera.rotation.eulerAngles;
        var targetRotation = Quaternion.Euler(0, cameraAngles.y, 0);

        objectToPosition.transform.position = targetPosition;
        objectToPosition.transform.rotation = targetRotation;
    }
}
