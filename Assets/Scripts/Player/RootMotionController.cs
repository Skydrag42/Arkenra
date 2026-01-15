using UnityEngine;

public class RootMotionController : MonoBehaviour
{
    private Animator animator;
    private PlayerController playerController;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponentInParent<PlayerController>();
    }

    private void OnAnimatorMove()
    {
        playerController.MoveFromRootMotion(animator.velocity);
    }
}
