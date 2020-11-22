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
			Vector3 randomOffset = new Vector3(Random.value * 2 - 1, Random.value * 2 - 1, Random.value * 2 - 1);
			ObjectPoolManager.GetPooledObject(ratPrefab, position + randomOffset, randomRotation);
		} 
	}
	
	public void SpawnRatsAt(Vector3 position, int amount)
	{
		for (int i = 0; i < amount; i++)
		{
			Quaternion randomRotation = Quaternion.Euler(Random.value * 360, Random.value * 360, Random.value * 360);
			Vector3 randomOffset = new Vector3(Random.value * 2 - 1, Random.value * 2 - 1, Random.value * 2 - 1);
			ObjectPoolManager.GetPooledObject(ratPrefab, position + randomOffset, randomRotation);
		}
	}
}
