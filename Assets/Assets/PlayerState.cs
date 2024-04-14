using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{

    [SerializeField] float walkXVelocity = 4f;
    [SerializeField] float runXVelocity = 4f;
    [SerializeField] float sneakSpeed = 3f;
    [SerializeField] float run4Speed = 5f;
    [SerializeField] float rollSpeed = 5f;
    [SerializeField] float climbUpSpeed = 3f;
    [SerializeField] float climbDownSpeed = 5f;

    [SerializeField] float jumpHeighWalk = 5f;
    [SerializeField] float jumpHeighRun = 7f;
    float jumpSpeed = 4f;
    float jumpYVelocity = 8f;

    bool isSliding = false;
    bool isAttacking = false;

    bool crouchInput = false;

    Animator animator;
    Rigidbody2D physics;
    SpriteRenderer sprite;

    enum State {Idle, Walk, Jump, Glide, Run, Slide, Crouch, Sneak, Roll, Run4, AttackFront, AttackUp, AttackDown, Climb, ClimbUp, ClimbDown, Dancing }

    State state = State.Idle;
    bool isGrounded = false;
    bool jumpInput = false;
    bool running = false;

    float horizontalInput = 0f;

    private void Update()
    {

        jumpInput = Input.GetKey(KeyCode.Space);
        horizontalInput = Input.GetAxisRaw("Horizontal");

        crouchInput = Input.GetKeyDown(KeyCode.C);

        if (state != State.Climb && state != State.ClimbUp && state != State.ClimbDown)
        {
            if (horizontalInput > 0f)
            {
                sprite.flipX = true;
            }
            else if (horizontalInput < 0f)
            {
                sprite.flipX = false;
            }
        }


        switch (state)
        {
            case State.Idle: IdleState(); break;
            case State.AttackFront: AttackFrontState(); break;
            case State.AttackUp: AttackUpState(); break;
            case State.AttackDown: AttackDownState(); break;
            case State.Walk: WalkState(); break;
            case State.Run: RunState(); break;
            case State.Slide: SlideState(); break;
            case State.Crouch: CrouchState(); break;
            case State.Sneak: SneakState(); break;
            case State.Run4: Run4State(); break;
            case State.Roll: RollState(); break;
            case State.Jump: JumpState(); break;
            case State.Glide: GlideState(); break;
            case State.Climb: ClimbState(); break;
            case State.ClimbUp: ClimbUpState(); break;
            case State.ClimbDown: ClimbDownState(); break;
            case State.Dancing: DancingState(); break;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (running == false)
            {
                running = true;
            }
            else
            {
                running = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

    }


    void IdleState()
    {
        animator.Play("Idle");

        if (isGrounded == true)
        {
            if (jumpInput)
            {
                jumpYVelocity = jumpHeighWalk;
                jumpSpeed = walkXVelocity;
                state = State.Jump;
            }
            else if (Input.GetKeyDown(KeyCode.F) && Input.GetKey(KeyCode.W))
            {
                state = State.AttackUp;
            }
            else if (Input.GetKeyDown(KeyCode.F) && Input.GetKey(KeyCode.S))
            {
                state = State.AttackDown;
            }
            else if (Input.GetKeyDown(KeyCode.F))
            {
                state = State.AttackFront;
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                state = State.Dancing;
            }
            else if (crouchInput)
            {
                state = State.Crouch;
            }
            else if (horizontalInput != 0f && running == false)
            {
                state = State.Walk;
            }
            else if (horizontalInput != 0f && running == true)
            {
                state = State.Run;
            }
        }
    }

    void AttackFrontState()
    {
        if (isAttacking == false)
        {
            isAttacking = true;
            animator.Play("attackf");
            StartCoroutine(AttackTime());
        }

    }

    void AttackUpState()
    {
        if (isAttacking == false)
        {
            isAttacking = true;
            animator.Play("attackup");
            StartCoroutine(AttackTime());
        }

    }

    void AttackDownState()
    {
        if (isAttacking == false)
        {
            isAttacking = true;
            animator.Play("attackdown");
            StartCoroutine(AttackTime());
        }

    }

    IEnumerator AttackTime()
    {

        yield return new WaitForSeconds(0.4f);
        isAttacking = false;
        state = State.Idle;
    }

        void WalkState()
    {
        animator.Play("walk");
        physics.velocity = walkXVelocity * horizontalInput * Vector2.right;

        if (isGrounded && jumpInput)
        {
            jumpYVelocity = jumpHeighWalk;
            jumpSpeed = walkXVelocity;
            state = State.Jump;
        }
        else if (crouchInput)
        {
            state = State.Crouch;
        }
        else if (horizontalInput == 0f)
        {
            state = State.Idle;
        }
        else if (horizontalInput != 0f && running == true)
        {
            state = State.Run;
        }
    }

    void RunState()
    {
        animator.Play("run");
        physics.velocity = runXVelocity * horizontalInput * Vector2.right;

        if (isGrounded && jumpInput)
        {
            jumpYVelocity = jumpHeighRun;
            jumpSpeed = runXVelocity;
            state = State.Jump;
        }
        else if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            state = State.Slide;
        }
        else if (crouchInput)
        {
            state = State.Crouch;
        }
        else if (horizontalInput == 0f)
        {
            state = State.Idle;
        }
        else if (horizontalInput != 0f && running == false)
        {
            state = State.Walk;
        }
    }

    void SlideState()
    {
        if (isSliding == false)
        {
            isSliding = true;
            StartCoroutine(SlideTime());
        }

    }

    IEnumerator SlideTime()
    {
        animator.Play("slide");
        physics.velocity = runXVelocity * horizontalInput * Vector2.right*1.6f;
        yield return new WaitForSeconds(0.4f);
        isSliding = false;

        if (isGrounded && jumpInput)
        {
            jumpYVelocity = jumpHeighRun;
            jumpSpeed = runXVelocity;
            state = State.Jump;
        }
        else if (horizontalInput == 0f)
        {
            state = State.Idle;
        }
        else if (horizontalInput != 0f && running == false)
        {
            state = State.Walk;
        }
        else if (horizontalInput != 0f && running == true)
        {
            state = State.Run;
        }

    }

    void CrouchState()
    {
        animator.Play("crouch");

        if (isGrounded == true)
        {
            if (crouchInput)
            {
                if (horizontalInput != 0f && running == false)
                {

                    state = State.Walk;
                }
                else if (horizontalInput != 0f && running == true)
                {
                    state = State.Run;
                }
                else if(horizontalInput == 0f)
                {
                    state = State.Idle;
                }
            }
            else if(horizontalInput != 0f)
            {
                state = State.Sneak;
            }
        }
    }

    void SneakState()
    {
        animator.Play("sneak");
        physics.velocity = sneakSpeed * horizontalInput * Vector2.right;

        if (horizontalInput == 0)
        {
            state = State.Crouch;
        }
        else if(horizontalInput != 0 && running == true)
        {
            state = State.Run4;
        }
        else if (horizontalInput != 0 && Input.GetKeyDown(KeyCode.LeftControl))
        {
            rollSpeed = sneakSpeed;
            state = State.Roll;
        }
        else if (crouchInput)
        {
            if (horizontalInput == 0f)
            {
                state = State.Idle;
            }
            else if (horizontalInput != 0f && running == false)
            {
                state = State.Walk;
            }
        }
    }

    void Run4State()
    {
        animator.Play("run4");
        physics.velocity = run4Speed * horizontalInput * Vector2.right;

        if (horizontalInput == 0)
        {
            state = State.Crouch;
        }
        else if (horizontalInput != 0 && Input.GetKeyDown(KeyCode.LeftControl))
        {
            rollSpeed = run4Speed;
            state = State.Roll;
        }
        else if (horizontalInput != 0 && running == false)
        {
            state = State.Sneak;
        }
        else if (crouchInput)
        {
            if (horizontalInput == 0f)
            {
                state = State.Idle;
            }
            else if (horizontalInput != 0f && running == true)
            {
                state = State.Run;
            }
        }
    }

    void RollState()
    {
        if (isSliding == false)
        {
            isSliding = true;
            StartCoroutine(RollTime());
        }
    }

    IEnumerator RollTime()
    {
        animator.Play("roll");
        physics.velocity = rollSpeed * horizontalInput * Vector2.right * 1.5f;
        yield return new WaitForSeconds(0.4f);
        isSliding = false;

        if (horizontalInput == 0f)
        {
            state = State.Crouch;
        }
        else if (horizontalInput != 0f && running==false)
        {
            state = State.Sneak;
        }
        else if(horizontalInput != 0f && running == true)
        {
            state = State.Run4;
        }

    }

    void JumpState()
    {


        physics.velocity = jumpSpeed * horizontalInput * Vector2.right + jumpYVelocity * Vector2.up;
        Debug.Log(physics.velocity);
        animator.Play("Jump");
        isGrounded = false;
        state = State.Glide;
    }

    void GlideState()
    {
        if (physics.velocity.y > 0f)
        {
            animator.Play("Jump");
        }
        else
        {
            animator.Play("Fall");
        }

        physics.velocity = physics.velocity.y * Vector2.up + walkXVelocity * horizontalInput * Vector2.right;

        if (isGrounded)
        {
            if (horizontalInput != 0f && running == false)
            {
                state = State.Walk;
            }
            else if (horizontalInput != 0f && running == true)
            {
                state = State.Run;
            }
            else if (horizontalInput == 0f)
            {
                state = State.Idle;
            }
        }



    }

    void ClimbState()
    {
        animator.Play("climb");
        physics.gravityScale = 0f;
        physics.velocity = Vector2.zero;

        if (Input.GetKey(KeyCode.S))
        {
            state = State.ClimbDown;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            state = State.ClimbDown;
        }
        else if ((Input.GetKeyDown(KeyCode.Space)))
        {
            physics.gravityScale = 1f;
            state = State.Glide;
        }

    }

    void ClimbUpState()
    {
        animator.Play("climbup");
        transform.Translate(Vector2.up * climbUpSpeed * Time.deltaTime);

        if (Input.GetKey(KeyCode.W) == false)
        {
            if (Input.GetKey(KeyCode.S))
            {
                state = State.ClimbDown;
            }
            else
            {
                state = State.Climb;
            }
        }
        else if ((Input.GetKeyDown(KeyCode.Space)))
        {
            physics.gravityScale = 1f;
            state = State.Glide;
        }

    }

    void ClimbDownState()
    {
        animator.Play("climbdown");
        transform.Translate(Vector2.down * climbDownSpeed * Time.deltaTime);

        if (Input.GetKey(KeyCode.S) == false)
        {
            if (Input.GetKey(KeyCode.W))
            {
                state = State.ClimbUp;
            }
            else
            {
                state = State.Climb;
            }
        }
        else if ((Input.GetKeyDown(KeyCode.Space)))
        {
            physics.gravityScale = 1f;
            state = State.Glide;
        }
    }
    void DancingState()
    {
        animator.Play("dancing");

        if (Input.GetKeyDown(KeyCode.R))
        {
            state = State.Idle;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
            physics.gravityScale = 1f;
            state = State.Idle;
        }
        if (collision.gameObject.tag == "wall")
        {
            state = State.Climb;
        }
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
        physics = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

}

