using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public enum State
    {
        None, Spawn, Idle, Partrol, Assault, Flee, Dead
    }

    public float speed;
    public float assaultSpeed;
    public float patrolDistance;
    public float idleTime;
    public float respawnTime;

    [Space]
    public Transform checkLeft;
    public Transform checkRight;
    public LayerMask groundLayer;

    [Space]
    public float damage;
    public float hitForce;
    public TriggerArea2D detectBox;
    public TriggerArea2D hitBox;

    [Space]
    public AudioClip dieClip;

    [SerializeField] State state = State.None;
    Vector3 originPos;
    Vector3 target = Vector3.negativeInfinity;
    Transform player;
    int originLayer;
    float idleCounter;
    float respawnCounter;
    Transform torch;

    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;
    AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        detectBox.TriggerEnter2D += OnDetectTriggerEnter2D;
        detectBox.TriggerExit2D += OnDetectTriggerExit2D;
        hitBox.TriggerEnter2D += OnHitTriggerEnter2D;
        hitBox.TriggerExit2D += OnHitTriggerExit2D;

        // SetState(State.Partrol);
        state = State.Spawn;
        originLayer = gameObject.layer;
    }

    void Update()
    {
        if (state == State.Idle)
        {
            idleCounter -= Time.deltaTime;
            if (idleCounter < 0)
            {
                if (player != null)
                    SetState(State.Assault);
                else
                    SetState(State.Partrol);
            }
        }
        else if (state == State.Dead)
        {
            respawnCounter -= Time.deltaTime;
            if (respawnCounter < 0)
            {
                SetState(State.Spawn);
            }
        }

        animator.SetFloat("VelocityX", Mathf.Abs(rb.velocity.x));
    }

    void FixedUpdate()
    {
        if (state == State.Partrol || state == State.Assault)
        {
            var dir = target.x - transform.position.x;
            if (Mathf.Abs(dir) <= .2f)
            {
                SetState(State.Idle);
            }
            else
            {
                int check = CheckDirection();
                if (check != 0)
                {
                    target = RandomTarget(direction: check);
                    dir = target.x - transform.position.x;
                }

                var velocityX = (dir > 0 ? 1 : -1) * 
                    (state == State.Assault || state == State.Flee? assaultSpeed : speed);
                rb.velocity = new Vector2(velocityX, rb.velocity.y);

                FlipSprite();
            }
        }
        else if (state == State.Assault)
        {
            var dir = target.x - transform.position.x;
            if (Mathf.Abs(dir) <= .2f)
            {
                if (player != null)
                    target = player.position;
                else 
                    SetState(State.Idle);
            }
            else
            {
                int check = CheckDirection();
                if (check != 0)
                {
                    rb.velocity = Vector2.zero;
                }
                else
                {
                    var velocityX = (dir > 0 ? 1 : -1) * 
                        (state == State.Assault || state == State.Flee ? assaultSpeed : speed);
                    rb.velocity = new Vector2(velocityX, rb.velocity.y);
                }

                FlipSprite();
            }
        }
        else if (state == State.Flee)
        {
            int dir = 1;
            if (torch.position.x > transform.position.x)
                dir = -1;
            int check = CheckDirection();
            if (check == -dir)
            {
                rb.velocity = Vector2.zero;
            }
            else
            {
                rb.velocity = new Vector2(assaultSpeed * dir, rb.velocity.y);
            }

            FlipSprite();
        }
    }

    void SetState(State newState)
    {
        if (state == newState) return;
        ExitState(state);
        EnterState(newState);
        state = newState;
    }

    void EnterState(State newState)
    {
        if (newState == State.Idle)
        {
            if (state == State.Assault)
                idleCounter = idleTime * .5f + Utils.RandomSigned(0, idleTime * .08f);
            else 
                idleCounter = idleTime + Utils.RandomSigned(0, idleTime * .3f);
            rb.velocity = Vector2.zero;
        }
        else if (newState == State.Partrol)
        {
            target = RandomTarget();
        }
        else if (newState == State.Assault)
        {
            target = player.transform.position;
        }
        else if (newState == State.Dead)
        {
            respawnCounter = respawnTime;
            rb.velocity = Vector2.zero;
            hitBox.gameObject.SetActive(false);
            animator.SetBool("IsDead", true);
            gameObject.layer = LayerMask.NameToLayer("Death");
        }
    }

    void ExitState(State oldState)
    {   
        if (oldState == State.Dead)
        {
            animator.SetBool("IsDead", false);
            hitBox.gameObject.SetActive(true);
            gameObject.layer = originLayer;
        }
    }

    Vector3 RandomTarget(bool turnBack = false, int direction = 0)
    {
        var distance = patrolDistance + Utils.RandomSigned(0, patrolDistance * 0.1f);
        if (turnBack)
        {
            distance *= -Mathf.Sign(rb.velocity.x);
        }
        else if (direction != 0)
        {
            distance *= direction;
        }
        else
        {
            distance *= Utils.RandomSign();
        }
        return transform.position + new Vector3(distance, 0, 0);
    }

    void FlipSprite()
    {
        if (rb.velocity.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (rb.velocity.x < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    int CheckDirection()
    {
        var distanceGround = 1.2f;
        var distanceWall = .3f;
        var leftWall = Physics2D.Raycast(checkLeft.position, Vector2.left, distanceWall, groundLayer);
        var leftGround = Physics2D.Raycast(checkLeft.position, Vector2.down, distanceGround, groundLayer);
        var rightWall = Physics2D.Raycast(checkRight.position, Vector2.right, distanceWall, groundLayer);
        var rightGround = Physics2D.Raycast(checkRight.position, Vector2.down, distanceGround, groundLayer);

        Debug.DrawRay(checkLeft.position, Vector2.left * distanceWall, leftWall? Color.red : Color.green);
        Debug.DrawRay(checkLeft.position, Vector2.down * distanceGround, leftGround? Color.red : Color.green);
        Debug.DrawRay(checkRight.position, Vector2.right * distanceWall, rightWall? Color.red : Color.green);
        Debug.DrawRay(checkRight.position, Vector2.down * distanceGround, rightGround? Color.red : Color.green);

        if (!leftGround)
            return 1;
        if (!rightGround)
            return -1;
        if (leftWall)
            return 1;
        if (rightWall)
            return -1;
        return 0;
    }

    public void TakeDamage()
    {
        if (state == State.Spawn) return;
        Utils.PlayRandomPitch(audioSource, dieClip);
        SetState(State.Dead);
    }

    public void OnSpawnAnimFinished()
    {
        SetState(State.Partrol);
    }

    void OnDetectTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && !other.isTrigger)
        {
            if (state == State.Flee || state == State.Dead || state == State.Spawn) return;
            if (other.GetComponent<PlayerMovement>().state == PlayerMovement.State.Dead) return;
            player = other.transform;
            SetState(State.Assault);
        }
    }

    private void OnDetectTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && !other.isTrigger)
        {
            if (other.transform == player)
                player = null;
            if (state == State.Assault)
                SetState(State.Idle);
        }
    }

    void OnHitTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && !other.isTrigger)
        {
            SetState(State.Idle);
            var dir = other.transform.position.x > transform.position.x? 1f : -1f;
            var force = new Vector3(dir, 0) * hitForce;
            other.GetComponent<PlayerStatus>().TakeDamage(damage, force);
        }
        else if (other.gameObject.tag == "Light")
        {
            torch = other.transform;
            SetState(State.Flee);
        }
    }

    void OnHitTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Light")
        {
            torch = null;
            SetState(State.Idle);
        }
    }

}
