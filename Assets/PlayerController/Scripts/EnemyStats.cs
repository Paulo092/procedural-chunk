using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace TE
{
    public class EnemyStats : MonoBehaviour
    {

        public int healthLevel = 10;
        public int maxHealth;
        public int currentHealth;
        public HealthBar healthBar;

        public GameObject healthBarPrefab;
        private GameObject instantiatedHealthBar; // A instância da barra de vida
        private RectTransform healthBarRectTransform; // O RectTransform da barra de vida instanciada


        private Camera mainCamera;
        public Transform npcTransform; // O transform do NPC
        public Canvas canvas; // O canvas onde a barra de vida será instanciada
        public Transform playerTransform; // O transform do player
        public float showDistance = 10f; // A distância para mostrar a barra de vida

        Animator animator;

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
        }

        void Start()
        {
            mainCamera = Camera.main;

            // Instancia a barra de vida como filha do Canvas
            instantiatedHealthBar = Instantiate(healthBarPrefab, canvas.transform);
            healthBarRectTransform = instantiatedHealthBar.GetComponent<RectTransform>();
            healthBar = instantiatedHealthBar.GetComponent<HealthBar>();

            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
            healthBar.SetMaxHealth(maxHealth);
        }

        void Update()
        {
            // Verifica a distância entre o player e o NPC
            float distance = Vector3.Distance(playerTransform.position, npcTransform.position);

            if (distance < showDistance)
            {
                // Mostra a barra de vida se o player estiver próximo
                instantiatedHealthBar.SetActive(true);

                // Converte a posição do NPC no mundo para a posição da tela
                Vector2 screenPosition = mainCamera.WorldToScreenPoint(npcTransform.position + Vector3.up * 3 ); // Ajuste a altura conforme necessário

                // Atualiza a posição da barra de vida no Canvas
                healthBarRectTransform.position = screenPosition;
            }
            else
            {
                // Oculta a barra de vida se o player estiver distante
                instantiatedHealthBar.SetActive(false);
            }
        } 

        private int SetMaxHealthFromHealthLevel()
        {
            maxHealth = healthLevel * 10;
            return maxHealth;
        }

        public void TakeDamage(int damage)
        {
            currentHealth = currentHealth - damage;

            healthBar.SetCurrentHealth(currentHealth);

            animator.Play("macacoHited");

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                animator.Play("macacoDead");
                //Handle Player Death
            }
        }

        public void DestroyIt() 
        {
            Destroy(gameObject);
            Destroy(instantiatedHealthBar);
        }
    }
}
