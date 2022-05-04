using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health;
    public int damage;

    public float flashTime = 0.2f;

    private SpriteRenderer sr;
    private Color originalColor;

    private PlayerHealth playerHealth;

    // Start is called before the first frame update
    public void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;

        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
    }

    // Update is called once per frame
    public void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    //-------------------------------------
    //以下小段为受伤后闪烁代码
    void FlashColor(float time)
    {
        sr.color = Color.red;
        Invoke("ResetColor", time);
    }
    void ResetColor()
    {
        sr.color = originalColor;
    }
    //--------------------------------------

    void OnTriggerEnter2D(Collider2D collision) //被碰撞到就减生命值
    {
        bool isAttack = false;
        int hurt=0;
        //||(collision.gameObject.layer == 9)  

        if ((collision.gameObject.layer == 7) )  //撞到人了
        {
            isAttack = true;
            hurt = 1;
        }

        //针对各种攻击方式进行分别处理
        switch (collision.tag)
        {
            case "Bullet":
                isAttack = true;
                hurt = 2;
                break;
            case "AttackLight":
                isAttack = true;
                hurt = 5;
                break;
        }

        if (isAttack==true)
        {
            health-=hurt;
            FlashColor(flashTime);

            if ((playerHealth != null) & (collision.gameObject.layer == 7))
            {
                playerHealth.DamagePlayer(damage);
            }
        }
    }
}
