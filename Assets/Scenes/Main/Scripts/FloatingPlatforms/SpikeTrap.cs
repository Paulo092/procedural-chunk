using UnityEngine;
using System.Collections;

namespace StarterAssets
{
    public class SpikeTrap : MonoBehaviour
    {
        public int damage = 10; // Dano causado ao player
        public float damageCooldown = 1.0f; // Tempo de cooldown para causar dano novamente
        public float delayStart = 2.0f; // Tempo de delay antes de iniciar a animação
        private float nextDamageTime = 0.0f;

        private BoxCollider boxCollider;
        private Animator animator;

        void Start()
        {
            boxCollider = GetComponent<BoxCollider>();
            animator = GetComponent<Animator>();
            boxCollider.enabled = false; // Desativar o collider inicialmente
            StartCoroutine(StartAnimationAfterDelay());
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && Time.time >= nextDamageTime)
            {
                PlayerStatusManager playerStatus = other.GetComponent<PlayerStatusManager>();
                if (playerStatus != null)
                {
                    playerStatus.TakeDamage(damage);
                    nextDamageTime = Time.time + damageCooldown;
                }
            }
        }

        // Função para ativar o BoxCollider
        public void ActivateCollider()
        {
            boxCollider.enabled = true;
        }

        // Função para desativar o BoxCollider
        public void DeactivateCollider()
        {
            boxCollider.enabled = false;
        }

        // Coroutine para iniciar a animação após o delay
        private IEnumerator StartAnimationAfterDelay()
        {
            yield return new WaitForSeconds(delayStart);
            animator.Play("SpikeTrap_Activate");
        }
    }
}