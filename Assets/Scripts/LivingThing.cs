using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class LivingThing : MonoBehaviour
    {

        public ProgressSlider Slider;

        public float InitialHealth = 100;
        public float Health { get; private set; }
        
        public bool IsAlive { get { return Health > 0; } }

        public event Action<float, float> OnDmg;
        public event Action OnDeath;
        public event Action OnRessurrect;

        private void Awake()
        {
            Health = InitialHealth;
        }

        public void Resurrect()
        {
            Health = InitialHealth;
            OnRessurrect?.Invoke();
        }

        public void DealDmg(float _health)
        {
            if (!IsAlive)
            {
                return;
            }

            float last = Health;
            Health -= _health;
            OnDmg?.Invoke(last, Health);
            if(!IsAlive)
            {
                OnDeath?.Invoke();
            }

            if(Slider != null)
            {
                Slider.SetFilling(Health / InitialHealth);
            }
        }
    }
}