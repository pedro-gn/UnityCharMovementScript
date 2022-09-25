using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement_controller : MonoBehaviour
{
    public float speed = 200;
    float attackDelayTime = 0.25f;
    float attackDelayCounter = 0;
    public float jumpPower = 16f;
    public int attackDamage = 25;
    float coyoteTime = 0.2f;
    float coyoteTimeCounter;
    float jumpBufferTime = 0.2f;
    float jumpBufferCounter;
    Animator animControl;
    Vector2 move;
    Rigidbody2D rb;
    bool isFacingRight = true;
    bool isGrounded = false;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask enemyLayer;
    public float attackRadius = 0.4f;
    const float groundCheckRadius = 0.2f;
    [SerializeField] Transform groundCheckCollider;
    [SerializeField] Transform attackCollider;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animControl = GetComponent<Animator>();
        animControl.SetLayerWeight(0,0);
        animControl.SetLayerWeight(1,1);
    }

    // Update is called once per frame
    void Update(){
        Jump();
        Attack();
    }


    void Attack(){


        if(attackDelayCounter <= 0 ){

            if(Input.GetMouseButtonDown(0)){
                attackDelayCounter = attackDelayTime;
                
                animControl.SetTrigger("isAttacking");
                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackCollider.position, attackRadius, enemyLayer);

                foreach(Collider2D enemy in hitEnemies){
                    enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
                }
            }
        }else{
            attackDelayCounter -= Time.deltaTime;
        }

        
        
    }
    void FixedUpdate(){
        Move();
    }

    void Jump(){
        GroundCheck();

        //Jump animations variables
        if(rb.velocity.y < -5 && !isGrounded){
            animControl.SetBool("isGrounded", isGrounded);
            animControl.SetBool("isFalling", true);
            animControl.SetBool("isJumping", false);
        }else if(rb.velocity.y > 1){
            animControl.SetBool("isJumping",true);
        }else{
            animControl.SetBool("isFalling", false);
            animControl.SetBool("isJumping", false);
        }



        if (isGrounded){
            coyoteTimeCounter = coyoteTime;
        }else{
            coyoteTimeCounter -= Time.deltaTime;
        }

        if(Input.GetButtonDown("Jump")){
            jumpBufferCounter = jumpBufferTime;
        }else{
            jumpBufferCounter -= Time.deltaTime;
        }

        if(coyoteTimeCounter > 0f && jumpBufferCounter > 0f ){
            rb.velocity = new Vector2( rb.velocity.x , jumpPower);
            jumpBufferCounter = 0;
            //animControl.SetBool("isJumping" , true);
        }
        if(Input.GetButtonUp("Jump") && rb.velocity.y > 0f){
           rb.velocity = new Vector2( rb.velocity.x , rb.velocity.y * 0.5f);
           coyoteTimeCounter = 0;
           
        }
    }
    void GroundCheck(){
        isGrounded = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckCollider.position, groundCheckRadius, groundLayer);
        if (colliders.Length > 0){
            isGrounded = true;
            animControl.SetBool("isGrounded", true);
        }else{
            animControl.SetBool("isGrounded", false);
        }
            
            
    }

    void OnDrawGizmos(){
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(groundCheckCollider.position, groundCheckRadius);
        Gizmos.DrawSphere(attackCollider.position, attackRadius);
    }

    void Move(){
        move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        rb.velocity = new Vector2( move.x  * speed * Time.deltaTime , rb.velocity.y);


        if(Input.GetButtonUp("Jump") && rb.velocity.y > 0f){
           rb.velocity = new Vector2( rb.velocity.x , rb.velocity.y * 0.5f);
        }

        AnimationHandler(); 


        void AnimationHandler(){

            if( move.x != 0 ) {
                animControl.SetBool("isRunning", true);
            }
            else{
                animControl.SetBool("isRunning", false);
            }

            if(move.x > 0 && !isFacingRight){
                Flip();
            }
            if(move.x < 0 && isFacingRight){
                Flip();
            }


            void Flip(){
                transform.Rotate(0f, 180f, 0f);
                isFacingRight = !isFacingRight;
            }
        }
    }

    


}
