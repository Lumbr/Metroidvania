using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDetector : MonoBehaviour
{
    public int side;
    public float delay;
    float delayTimer;
    bool pressed;
    private void Update()
    {
        transform.localPosition = Vector3.zero;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            if (GetComponentInParent<PlayerMovement>().isGrounded)
            {
                GetComponentInParent<PlayerMovement>().registerMove = true;
                GetComponentInParent<PlayerMovement>().OnWall = false;
                return;
            }
            if (!GetComponentInParent<PlayerMovement>().OnWall && !GetComponentInParent<PlayerMovement>().isGrounded)
            {
                if (Input.GetKey(KeyCode.Space))
                {
                    pressed = true;
                }
                if (pressed)
                {
                    delayTimer -= Time.deltaTime;
                    if (delayTimer < 0) pressed = false;
                    return;
                }
                else delayTimer = delay;
                transform.parent.position += GetComponentInParent<PlayerMovement>().lastMove < 0 ? (Vector3)Vector2.left * 0.4f : (Vector3)Vector2.right * 0.4f;
                GetComponentInParent<PlayerMovement>().OnWall = true;
                GetComponentInParent<PlayerMovement>().airDashed = false;
                GetComponentInParent<PlayerMovement>().registerMove = false;
                GetComponentInParent<PlayerMovement>().lastMove = transform.parent.eulerAngles.y > 0 ? side : -side;
            }
        }
        
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground" && Input.GetAxisRaw("Horizontal") != -GetComponentInParent<PlayerMovement>().lastMove && !Input.GetKey(KeyCode.Space))
        {
            GetComponentInParent<PlayerMovement>().registerMove = true;
            GetComponentInParent<PlayerMovement>().OnWall  = false;
        }
    }
}
