using UnityEngine;
using UnityEngine.AI;

namespace TE
{
    public class NPCAttacker : MonoBehaviour
    {
        public Transform playerTransform; // O transform do player
        public float chaseDistance = 15f; // Dist�ncia para come�ar a perseguir o player
        public float attackDistance = 2f; // Dist�ncia para atacar o player
        public float attackRate = 1f; // Tempo entre ataques

        public NavMeshAgent navMeshAgent;
        public Animator animator;
        private float nextAttackTime;
        private bool isChasing = false;
        private bool isHited = false;

        private void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            float distanceToPlayer = Vector3.Distance(playerTransform.position, transform.position);

            if (distanceToPlayer <= chaseDistance && isHited)
            {
                isChasing = true;
            }
            else if (distanceToPlayer > chaseDistance)
            {
                isChasing = false;
                isHited = false;
                navMeshAgent.isStopped = true;
                animator.SetBool("isWalking", false);
            }

            if (isChasing)
            {
                ChasePlayer();

                if (distanceToPlayer <= attackDistance && Time.time >= nextAttackTime)
                {
                    AttackPlayer();
                }
            }
        }

        private void ChasePlayer()
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(playerTransform.position);
            animator.SetBool("isWalking", true);
        }

        private void AttackPlayer()
        {
            navMeshAgent.isStopped = true;
            animator.SetBool("isWalking", false);
            animator.SetTrigger("attack");
            nextAttackTime = Time.time + attackRate;
        }

        // M�todo para receber dano e iniciar persegui��o
        public void TakeDamage()
        {
            // Aqui voc� pode adicionar a l�gica de redu��o de sa�de do NPC
            isHited = true;
        }
    }
}
