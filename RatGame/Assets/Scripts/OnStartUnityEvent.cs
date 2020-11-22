using UnityEngine;
using UnityEngine.Events;

public class OnStartUnityEvent : MonoBehaviour
{
	public UnityEvent onStartEvent;
	public UnityEvent<Vector3> onStartEventVector3;

	private void Start()
	{
		onStartEvent?.Invoke();
		onStartEventVector3?.Invoke(transform.position);
	}
}
