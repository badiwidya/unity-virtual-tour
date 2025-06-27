using UnityEngine;

public class UIPositioner : MonoBehaviour
{
    public Transform mainCamera;
    
    public float spawnDistance = 1.0f;
    public float heightOffset = 0.0f;

    public void PositionObjectInFrontOfPlayer(GameObject objectToPosition)
    {
        if (mainCamera == null || objectToPosition == null) return;

        var forwardPosition = mainCamera.position + (mainCamera.forward * spawnDistance);

        var targetPosition = new Vector3(forwardPosition.x, mainCamera.position.y + heightOffset, forwardPosition.z);
        
        var cameraAngles = mainCamera.rotation.eulerAngles;
        var targetRotation = Quaternion.Euler(0, cameraAngles.y, 0);

        objectToPosition.transform.position = targetPosition;
        objectToPosition.transform.rotation = targetRotation;
    }
}
