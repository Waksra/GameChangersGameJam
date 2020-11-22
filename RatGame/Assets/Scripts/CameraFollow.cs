using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Vector3 Offset = new Vector3(0, 10, 0);
    
    
    private void LateUpdate()
    {
        Vector3 asdf = BoidManager.Instance.GetCenterOfSwarm();
        
        transform.position = Vector3.Lerp(transform.position, asdf + Offset, Time.deltaTime);
    }
}
