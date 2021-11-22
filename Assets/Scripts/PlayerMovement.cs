using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VladsUsefulScripts;
[RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(Collider2D)), RequireComponent(typeof(GroundCheck))]
public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;
    public float speed, maxSpeedMult, jumpPow, drag, lastMove = 1;
    float count;
    public bool isGrounded, registerMove, airDashed, dashing;
    bool _onWall;
    public bool onWall
    {
        get { return _onWall; }
        set
        {
            if (_onWall == value) return;
            _onWall = value;
            if(_onWall == false)
            {
                transform.position = new Vector2(transform.position.x + lastMove / 2, transform.position.y);
            }
        }
    }
    bool pressed, dashed;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        if (!isGrounded && airDashed || !registerMove) return;
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (count <= 0)
            {
                if (!isGrounded) airDashed = true;
                dashed = true;
                dashing = true;
                registerMove = false;
                count = 0.7f;
            }
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Movement();
        Animation();
        Dashing();
    }
    void Movement()
    {
        //Debug.Log(lastMove);
        if (onWall)
        {
            if (Input.GetAxisRaw("Horizontal") == -lastMove)
            {
                registerMove = false;
                rb.velocity = new Vector2(-lastMove, -1);
            }
            else
            {
                onWall = false;
            }
            if (isGrounded)
            {
                //transform.position = new Vector2(transform.position.x + lastMove/2, transform.position.y); 
                onWall = false;
            }
        }
        if (Input.GetKey(KeyCode.Space) && !pressed)
        {
            if (isGrounded)
            {
                pressed = true;
                rb.velocity = new Vector2(rb.velocity.x, jumpPow);
            }
            else if (onWall)
            {
                onWall = false;
                pressed = true;
                rb.velocity = new Vector2(lastMove*0.8f, jumpPow / maxSpeedMult) * maxSpeedMult;
            }
        }

        if (!Input.GetKey(KeyCode.Space)) pressed = false;

        if (Input.GetAxisRaw("Horizontal") != 0 && registerMove)
        {
            if (Input.GetAxisRaw("Horizontal") < 0) lastMove = -1;
            else if (Input.GetAxisRaw("Horizontal") > 0) lastMove = 1;
        }

        if (dashing)
        {
            rb.velocity = Clampers.Drag(rb.velocity, drag);
            if (dashing && count <= 0.4f) 
            { 
                dashing = false;
                registerMove = true;
            }
        }

        if (lastMove < 0) transform.eulerAngles = new Vector2(0, 180);
        else transform.eulerAngles = new Vector2(0, 0);
        if (!dashing && count <= 0.4f) rb.velocity = Clampers.VelCalc(Clampers.Drag(rb.velocity, drag), speed, maxSpeedMult);
    }
    void Animation()
    {
        anim.SetBool("OnWall", onWall);
        anim.SetBool("Grounded", isGrounded);
        if (!registerMove) return;
        if (Mathf.Abs(rb.velocity.x / (speed * maxSpeedMult)) < 0.1) anim.SetFloat("X", 0);
        else if (Mathf.Abs(rb.velocity.x) < speed * maxSpeedMult + 1) anim.SetFloat("X", 0.5f);
        else anim.SetFloat("X", 1);
        if (rb.velocity.y < -10) anim.SetFloat("Y", -1);
        else if (rb.velocity.y < 10) anim.SetFloat("Y", 0f);
        else anim.SetFloat("Y", 1);
        //anim.SetFloat("X", Mathf.Abs(rb.velocity.x / (speed * maxSpeedMult))); 
        anim.SetFloat("Y", rb.velocity.normalized.y);
    }
    void Dashing()
    {
        if (isGrounded) airDashed = false;
        if (count > 0) count -= Time.deltaTime;
        if (count > 0.4f)
        {
            if (dashed)
            {
                dashed = false;
                rb.velocity = new Vector2(lastMove * speed * maxSpeedMult * 4, 0);
            }
            rb.velocity = Clampers.Drag(rb.velocity, drag * 3);
            rb.velocity = new Vector2(rb.velocity.x, 0);
            anim.SetFloat("X", 1);
            anim.SetBool("Grounded", true);

        }
    }
}
