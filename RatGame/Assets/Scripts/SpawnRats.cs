using UnityEngine;

public class SpawnRats : MonoBehaviour
{
	public int spawnAmount = 5;

	public GameObject ratPrefab;

	public void SpawnRatsAt(Vector3 position)
	{
		for (int i = 0; i < spawnAmount; i++)
		{
			Quaternion randomRotation = Quaternion.Euler(Random.value * 360, Random.value * 360, Random.value * 360);
			
			ObjectPoolManager.GetPooledObject(ratPrefab, position, randomRotation);
		}
	}
}
