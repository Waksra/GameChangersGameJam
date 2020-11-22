using System.Collections;
using UnityEngine;

public class TimedPesantSpawning : MonoBehaviour
{
	public Vector2 minMaxTimeBetween = new Vector2(2, 5);

	public GameObject objectToSpawn;

	private void Start()
	{
		StartCoroutine(TimedSpawn());
	}

	private IEnumerator TimedSpawn()
	{
		while (true)
		{
			yield return new WaitForSeconds(Random.Range(minMaxTimeBetween.x, minMaxTimeBetween.y));

			ObjectPoolManager.GetPooledObject(objectToSpawn, transform.position, Quaternion.identity);
		}
	}
}
