using UnityEngine;

namespace Actor
{
    public class Damageable : MonoBehaviour
    {
        [SerializeField] private float maxHealth;
        
        public float CurrentHealth { get; private set; }

        private void Awake()
        {
            CurrentHealth = maxHealth;
        }

        public void Damage(float amount)
        {
            CurrentHealth -= amount;
            if(CurrentHealth <= 0)
                Kill();
        }

        public void Kill()
        {
            
        }
    }
}