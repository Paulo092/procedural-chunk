using UnityEngine;

public class FallingLoopBehaviour : StateMachineBehaviour
{
    // Refer�ncia ao PlayerController
    private PlayerController playerController;

    // Este m�todo � chamado quando o estado da anima��o � inicializado
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Obter o PlayerController a partir do Animator
        playerController = animator.GetComponentInParent<PlayerController>();

        if (playerController == null)
        {
            Debug.LogError("PlayerController n�o encontrado no Animator.");
        }
    }

    // Este m�todo � chamado a cada frame enquanto a anima��o est� em execu��o
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playerController != null)
        {
            // Verificar se o personagem est� no ch�o usando o PlayerController
            if (playerController.isGrounded)
            {
                // Quando o personagem est� no ch�o, sair do estado de queda
                animator.SetBool("IsFalling", false);
                playerController.PlayTargetAnimation("Land", true);
            }
        }
    }
}
