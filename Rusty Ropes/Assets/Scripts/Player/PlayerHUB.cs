using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerHUB : MonoBehaviour{
    [Header("Properties")]
    public float speedDef=6f;
    public float jumpFcDef=6.6f;
    public float jumpTimeDef=0.35f;
    public float defaultGravity=2;
    public float groundFriction=0.075f;
    [SerializeField] Transform feetPos;
    [SerializeField] float checkRadius=0.3f;
    [SerializeField] LayerMask whatIsGround;
    [Header("Variables")]
    public float accumulatedSpeed=1;
    public float accumulatedSpeedTimer;
    public float speedC;
    public float jumpFcC;
    //public float jumpTimeC;
    [SerializeField]bool isGrounded;
    [SerializeField]float jumpTimer;
    [SerializeField]bool isJumping;
    Rigidbody2D rb;
    float moveInput;
    int faceDir=-1;
    void Start(){
        rb=GetComponent<Rigidbody2D>();
    }
    void Update(){
        MovePlayerJump();
        MovePlayerHorizontal();
        if(accumulatedSpeedTimer>0)accumulatedSpeedTimer-=Time.deltaTime;
        if(accumulatedSpeedTimer<=0){accumulatedSpeed-=0.05f;accumulatedSpeedTimer=0.075f;}
        if(accumulatedSpeed>1){
            var speedMax=1.5f;
            if(accumulatedSpeed>speedMax){accumulatedSpeed=speedMax;}
            speedC=speedDef*accumulatedSpeed*1.5f;//;if(accumulatedSpeed==speedMax){speedC=speedDef*2;}
            jumpFcC=jumpFcDef*accumulatedSpeed;if(accumulatedSpeed>=1.3f){jumpFcC=jumpFcDef*1.1f;}
            //jumpTimeC=jumpTimeDef*accumulatedSpeed*0.9f;
        }else{
            if(accumulatedSpeed<1){accumulatedSpeed=1;}
            speedC=speedDef;
            jumpFcC=jumpFcDef;
            //jumpTimeC=jumpTimeDef;
        }
    }
    void MovePlayerHorizontal(){
        moveInput=Input.GetAxisRaw("Horizontal");
        //anim.SetFloat("Speed", Mathf.Abs(moveInput));
        if(moveInput!=0){
            rb.velocity=new Vector2(speedC*moveInput,rb.velocity.y);
            AddSpeed(0.0025f,1f);
        }
        if(moveInput==0){//Slide
            if(rb.velocity.x>0.5f||rb.velocity.x<-0.5f){
                var t=1;if(rb.velocity.x<0){t=-1;}
                rb.velocity=new Vector2(rb.velocity.x-(groundFriction*t),rb.velocity.y);
            }else{rb.velocity=new Vector2(0,rb.velocity.y);}
        }
    }
    void MovePlayerJump(){
        isGrounded=Physics2D.OverlapCircle(feetPos.position,checkRadius,whatIsGround);//Check if grounded
        if(Input.GetKey(KeyCode.Space)){isGrounded=false;}//Only isGrounded when not holding space
        if(isGrounded){jumpTimer=jumpTimeDef;}
        if(jumpTimer==jumpTimeDef&&Input.GetKeyDown(KeyCode.Space)){
            isJumping=true;
            AddSpeed(0.03f,1.5f);
            rb.velocity=Vector2.up*jumpFcC;
        }
        
        if(isJumping&&Input.GetKey(KeyCode.Space)){
            if(jumpTimer>0){
                rb.velocity=Vector2.up*jumpFcC;
                jumpTimer-=Time.deltaTime;
            }else{isJumping=false;}
        }
        if(Input.GetKeyUp(KeyCode.Space)){isJumping=false;jumpTimer=0;}
        if(!isGrounded&&!isJumping){jumpTimer=0;}
    }
    void AddSpeed(float amnt,float timerAdd=1){
        accumulatedSpeed+=amnt;
        if(accumulatedSpeedTimer<0.5f)accumulatedSpeedTimer+=amnt*timerAdd;
    }
    void OnTriggerEnter2D(Collider2D other){
        
    }
}