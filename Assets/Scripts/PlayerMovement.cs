using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public enum State
    {
        None, Normal, Traveling, Hurt, Dead
    }

    public float speed;
    public float jumpForce;
    public float jumpHoldForce;
    public float jumpHoldDuration;
    [Space]
    public Transform groundCheckPoint;
    public LayerMask groundLayer;
    public float groundCheckRadius;
    [Space]
    public float travelSpeed;
    public ParticleSystem travelTrail;
    [Space]
    public AudioClip jumpClip;
    public AudioClip travelClip;
    public AudioClip hurtClip;
    public AudioClip dieClip;
    public AudioClip teleportClip;

    [Space]
    [SerializeField] bool isOnGround;
    [SerializeField] bool isJumping;
    float jumpCounter;
    Torch currentTorch;
    Transform travelTarget;
    int originLayer;
    public State state = State.None;

    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    Animator animator;
    AudioSource audioSource;
    PlayerInput input;
    TriggerArea2D triggerArea;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        input = GetComponent<PlayerInput>();
        audioSource = GetComponent<AudioSource>();
        triggerArea = GetComponentInChildren<TriggerArea2D>();
        triggerArea.TriggerEnter2D += OnInteractTriggerEnter;
        // triggerArea.TriggerStay2D += OnInteractTriggerStay;
        triggerArea.TriggerExit2D += OnInteractTriggerExit;

        originLayer = gameObject.layer;
        state = State.Normal;
    }

    void Update()
    {
        if (state == State.Dead) return;

        GroundCheck();
        ApplyAnimation();

        if (input.interact)
        {
            if (currentTorch != null && state != State.Traveling)
            {
                currentTorch.Toggle();
                currentTorch.ShowHint();
            }
            input.interact = false;
        }

        if (input.travel)
        {
            if (currentTorch != null)
            {
                if (currentTorch.NearestTorch != null && currentTorch.isOn)
                    Travel(currentTorch.NearestTorch.transform);
            }
            input.travel = false;
        }

        if (state == State.Traveling)
        {
            var cols = Physics2D.OverlapCircleAll(transform.position, 1.5f, 1 << LayerMask.NameToLayer("Enemy"));
            if (cols.Length > 0)
            {
                foreach (var item in cols)
                {
                    item.GetComponent<Enemy>()?.TakeDamage();
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (state == State.Normal)
        {
            // Horizontal
            float velocityX = input.horizontal * speed;

            // Vertical
            if (input.jump && isOnGround && !isJumping)
            {
                input.jump = false;
                isJumping = true;
                jumpCounter = Time.time + jumpHoldDuration;

                rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);

                Utils.PlayRandomPitch(audioSource, jumpClip);
            }
            else if (isJumping)
            {
                if (input.jumpHeld)
                {
                    rb.AddForce(new Vector2(0, jumpHoldForce), ForceMode2D.Impulse);
                }
                if (jumpCounter < Time.time)
                    isJumping = false;
            }

            if (!isOnGround)
                input.jump = false;

            rb.velocity = new Vector2(velocityX, rb.velocity.y);

            if (input.horizontal > 0)
                spriteRenderer.flipX = false;
            else if (input.horizontal < 0)
                spriteRenderer.flipX = true;
        }
        else if (state == State.Traveling)
        {
            var dir = travelTarget.position - transform.position;
            if (dir.sqrMagnitude <= .5f)
            {
                state = State.Normal;
                gameObject.layer = originLayer;
                rb.velocity = Vector2.zero;
                travelTrail.Stop();

                animator.SetBool("IsTraveling", false);
            }
            else
            {
                rb.velocity = travelSpeed * dir.normalized;
            }
        }

    }

    void GroundCheck()
    {
        var col = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
        isOnGround = col != null;
    }

    void ApplyAnimation()
    {
        animator.SetFloat("VelocityX", Mathf.Abs(rb.velocity.x));
        animator.SetFloat("VelocityY", rb.velocity.y);
        animator.SetBool("IsOnGround", isOnGround);
    }

    void Travel(Transform target)
    {
        if (state != State.Normal) return;
        state = State.Traveling;

        travelTarget = target;
        gameObject.layer = LayerMask.NameToLayer("Void");
        travelTrail.Play();
        
        animator.SetBool("IsTraveling", true);
        Utils.PlayRandomPitch(audioSource, travelClip);
    }

    public void TakeDamage(Vector3 force)
    {
        if (state == State.Hurt) return;
        state = State.Hurt;
        rb.velocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);
        animator.SetBool("IsHurt", true);
        StartCoroutine(HurtDelay());
        Utils.PlayRandomPitch(audioSource, hurtClip);
    }
    
    IEnumerator HurtDelay()
    {
        yield return new WaitForSeconds(0.5f);
        state = State.Normal;
        animator.SetBool("IsHurt", false);
    }

    public void Die()
    {
        if (state == State.Dead) return;
        state = State.Dead;
        rb.velocity = Vector2.zero;
        animator.SetBool("IsDead", true);
        Utils.PlayRandomPitch(audioSource, dieClip);
        GameManager.Instance.GameOver();
    }

    public void Transport()
    {
        if (state == State.Dead) return;
        state = State.Dead;
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;
        animator.SetBool("IsTeleport", true);
        Utils.PlayRandomPitch(audioSource, teleportClip);
        GameManager.Instance.NextLevel();
    }

    void OnInteractTriggerEnter(Collider2D other) 
    {
        if (other.gameObject.tag == "Torch")
        {
            var torch = other.GetComponent<Torch>();
            if (torch != currentTorch)
            {
                if (currentTorch != null)
                    currentTorch.HideHint();
                currentTorch = torch;
                currentTorch.ShowHint();
            }
        }
    }

    // void OnInteractTriggerStay(Collider2D other)
    // {
    //     if (other.gameObject.tag == "Torch")
    //     {

    //     }
    // }

    void OnInteractTriggerExit(Collider2D other)
    {
        if (other.gameObject.tag == "Torch")
        {
            var torch = other.GetComponent<Torch>();
            if (torch == currentTorch)
            {
                torch.HideHint();
                currentTorch = null;
            }
        }
    }
    
}
