using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    private float timeBetweenAttack;
    public float startTimeBetweenAttack = 0.5f;

    public int damage = 10;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (timeBetweenAttack > 0)
        {
            timeBetweenAttack -= Time.deltaTime;
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (timeBetweenAttack > 0f) return;

        timeBetweenAttack = startTimeBetweenAttack;

        animator.SetTrigger("Attack");
    }
}