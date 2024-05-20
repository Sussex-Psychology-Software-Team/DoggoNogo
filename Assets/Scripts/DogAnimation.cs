using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogAnimation : MonoBehaviour {

    public Animator animator;

    public float moveSpeed = 7f;
    public float smoothing = .05f;
    public bool moving = false;
    private Vector2 dir = Vector2.zero;

    float horiz_dir = 0f;
    float horiz_move = 0f;
    private Vector2 horiz_vec = Vector2.zero;
    public bool facingRight = true; //Depends on if your animation is by default facing right or left
 
    // Update is called once per frame
    void FixedUpdate(){
        horiz_dir = Input.GetAxisRaw("Horizontal");
        horiz_move = horiz_dir * moveSpeed;
        animator.SetFloat("animSpeed", Mathf.Abs(horiz_move));
        horiz_vec = new Vector2(horiz_move,0f);
        transform.Translate(horiz_vec*Time.deltaTime);
        float h = Input.GetAxis("Horizontal");
        if(h > 0 && !facingRight)
            Flip();
        else if(h < 0 && facingRight)
            Flip();

        //transform.position += horizMove * Time.deltaTime;
        //if(Input.GetKey(KeyCode.A)){
        //    moving = true;
        //    dir = Vector2.left;
        //    transform.Translate(dir * moveSpeed * Time.deltaTime);
        //} else if(Input.GetKey(KeyCode.D)){
        //    moving = true;
        //    dir = Vector2.right;
        //    transform.Translate(dir * moveSpeed * Time.deltaTime);
        //} else if(Input.GetKeyUp(KeyCode.A)){
        //    moving = false;
        //} else if(Input.GetKey(KeyCode.D)){
        //    moving = false;
        //}
   }

   void Flip (){
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
