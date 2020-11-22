using Actor;
using UnityEngine;

public class WallOfDeath : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ActorBase actorBase))
            actorBase.gameObject.SetActive(false);
    }
}
