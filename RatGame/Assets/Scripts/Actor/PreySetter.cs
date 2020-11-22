using UnityEngine;

namespace Actor
{
	[RequireComponent(typeof(ActorBase))]
	public class PreySetter : MonoBehaviour
	{

		private ActorBase _actorBase;

		public bool IsSetAsPrey { get; private set; } = false;
		
		private void Awake()
		{
			_actorBase = GetComponent<ActorBase>();
		}

		private void OnEnable()
		{
			SetAsPrey();
		}

		private void OnDisable()
		{
			RemoveAsPrey();
		}

		public void SetAsPrey()
		{
			if(IsSetAsPrey)
				return;
			
			BoidManager.Instance.AddPrey(_actorBase);
			IsSetAsPrey = true;
		}

		public void RemoveAsPrey()
		{
			if(!IsSetAsPrey)
				return;
			
			BoidManager.Instance.RemovePrey(_actorBase);
			IsSetAsPrey = false;
		}
	}
}
