using UnityEngine;
using UnityEngine.UI;

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(ThirdPersonController))]
    public class PlayerStatusManager : MonoBehaviour
    {
        [Header("Health Settings")]
        public int healthLevel = 10;
        public int maxHealth;
        public int currentHealth;
        public HealthBar healthBar;

        private ThirdPersonController _thirdPersonController;
        private Animator _animator;

        private static readonly int IsDead = Animator.StringToHash("IsDead");
        private static readonly int IsDamaged = Animator.StringToHash("IsDamaged");

        private void Awake()
        {
            _thirdPersonController = GetComponent<ThirdPersonController>();
            _animator = GetComponent<Animator>(); // Assume that Animator is on the same GameObject
        }

        private void Start()
        {
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
            healthBar.SetMaxHealth(maxHealth);
        }

        private int SetMaxHealthFromHealthLevel()
        {
            return healthLevel * 10;
        }

        public void TakeDamage(int damage)
        {
            Debug.Log(damage);
            
            currentHealth -= damage;
            healthBar.SetCurrentHealth(currentHealth);

            if (_animator != null)
            {
                _animator.SetTrigger(IsDamaged); // Trigger damage animation
            }

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                if (_animator != null)
                {
                    _animator.SetBool(IsDead, true); // Set death animation
                }
                // Handle Player Death
                HandlePlayerDeath();
            }
        }

        private void HandlePlayerDeath()
        {
            // Handle player death (e.g., disable controls, show game over screen)
            _thirdPersonController.enabled = false;
            // Additional death handling logic can be added here
        }
    }
}
