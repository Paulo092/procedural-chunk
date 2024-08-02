using UnityEngine;

namespace StarterAssets
{
    public class HealingOrb : MonoBehaviour
    {
        [Header("Healing Settings")]
        [Tooltip("Percentage of the player's max health to heal.")]
        [Range(0, 100)]
        public float healPercentage = 25.0f; // Define the percentage of healing

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other);
            // Check if the collider belongs to the player
            if (other.TryGetComponent<PlayerStatusManager>(out PlayerStatusManager playerStatusManager))
            {
                // Calculate the amount of healing
                float healAmount = playerStatusManager.maxHealth * (healPercentage / 100.0f);

                // Apply healing
                playerStatusManager.currentHealth += (int)healAmount;

                // Ensure current health does not exceed max health
                if (playerStatusManager.currentHealth > playerStatusManager.maxHealth)
                {
                    playerStatusManager.currentHealth = playerStatusManager.maxHealth;
                }

                // Update the health bar
                playerStatusManager.healthBar.SetCurrentHealth(playerStatusManager.currentHealth);

                // Optionally destroy the orb after healing
                Destroy(gameObject);
            }
        }
    }
}
