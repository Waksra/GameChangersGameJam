using UnityEngine;
using UnityEngine.Events;

namespace Actor
{
    public class Damageable : MonoBehaviour
    {
        [SerializeField] private float maxHealth;
        public UnityEvent<Vector3> onDamaged;
        public UnityEvent<Vector3> onKill;
        
        public float CurrentHealth { get; private set; }

        private void Awake()
        {
            CurrentHealth = maxHealth;
        }

        private void OnEnable()
        {
            CurrentHealth = maxHealth;
        }

        public void Damage(float amount)
        {
            CurrentHealth -= amount;
            if(CurrentHealth <= 0)
            {
                Kill();
                return;
            }
            onDamaged.Invoke(transform.position);
        }

        public void Heal(float amount)
        {
            CurrentHealth += amount;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth);
        }

        public void Kill()
        {
            onKill.Invoke(transform.position);
            gameObject.SetActive(false);
        }
    }
}