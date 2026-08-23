using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void Attack()
    {
        if (animator == null)
            return;

        animator.SetTrigger("Attack");
    }

    public void GetHit()
    {
        if (animator == null)
            return;

        animator.SetTrigger("GetHit");
    }

    public void Dizzy()
    {
        if (animator == null)
            return;

        animator.SetTrigger("Dizzy");
    }

    public void Die()
    {
        if (animator == null)
            return;

        animator.SetTrigger("Die");
    }
}