using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform LookTarget = null;

    [SerializeField] Vector3 Offset = new Vector3(0, 10, 0);
    
    
    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, LookTarget.position + Offset, Time.deltaTime);
    }
}
