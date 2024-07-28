using UnityEngine;

public class FallingLoopBehaviour : StateMachineBehaviour
{
    // Referência ao PlayerController
    private PlayerController playerController;

    // Este método é chamado quando o estado da animação é inicializado
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Obter o PlayerController a partir do Animator
        playerController = animator.GetComponentInParent<PlayerController>();

        if (playerController == null)
        {
            Debug.LogError("PlayerController não encontrado no Animator.");
        }
    }

    // Este método é chamado a cada frame enquanto a animação está em execução
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playerController != null)
        {
            // Verificar se o personagem está no chão usando o PlayerController
            if (playerController.isGrounded)
            {
                // Quando o personagem está no chão, sair do estado de queda
                animator.SetBool("IsFalling", false);
                playerController.PlayTargetAnimation("Land", true);
            }
        }
    }
}
