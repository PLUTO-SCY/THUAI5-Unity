using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class PlayerMove : MonoBehaviour
{
    public float runSpeed;
    private Rigidbody2D myRigidbody;
    private Animator myAnim;    

    private float moveDir;   //左右移动
    private float moveDir2;  //上下移动

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnim = GetComponent<Animator>();
        //myAnimation["LightAttack"].layer = 5;
    }

    // Update is called once per frame
    void Update()
    {
        Flip();   //设置动画翻转，这样只需要做一个向右的动画，就可以满足左右两个方向的移动
        Run();        
    }

    void Run()  //控制人物上下左右移动
    {
        moveDir = Input.GetAxis("Horizontal");
        Vector2 playerVel = new Vector2(moveDir * runSpeed, myRigidbody.velocity.y);
        myRigidbody.velocity = playerVel;
        bool playerHasXAxisSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        myAnim.SetBool("Right_Run",playerHasXAxisSpeed);

        moveDir2 = Input.GetAxis("Vertical");
        Vector2 playerVel2 = new Vector2(myRigidbody.velocity.x, moveDir2 * runSpeed);
        myRigidbody.velocity = playerVel2;
        bool playerHasXAxisSpeed2 = Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon;
        //myAnim.SetBool("Right_Run", playerHasXAxisSpeed);
    }
    void Flip()   //设置动画翻转
    {
        bool playerHasXAxisSpeed = Math.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        if (myRigidbody.velocity.x>0.1f)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        if (myRigidbody.velocity.x < -0.1f)
        {
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
    }
    
    
}
