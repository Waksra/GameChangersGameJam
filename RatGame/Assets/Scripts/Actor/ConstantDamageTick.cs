using UnityEngine;

namespace Actor
{
	[RequireComponent(typeof(ActorBase))]
	public class ConstantDamageTick : MonoBehaviour
	{
		public float damagePerSecond = 1f;

		private ActorBase _actorBase;

		private void Awake()
		{
			_actorBase = GetComponent<ActorBase>();
		}

		private void Update()
		{
			_actorBase.Damage(damagePerSecond * Time.deltaTime);
		}
	}
}
