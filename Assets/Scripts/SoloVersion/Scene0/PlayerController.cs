using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController1 : MonoBehaviour
{
    public GameObject myPrefab;    //闪电的预制体
    //public int damage;
    public GameObject bulletPrefab;
    public GameObject RecoveryPrefab;


    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        LightAttack(); //技能释放闪电
        Attack();      //尚未处理好
        Fire();
        Recovery();
    }

    void Attack()
    {
        if(Input.GetButtonDown("Attack"))
        {

        }
    }

    void LightAttack()  //控制释放闪电的技能
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("wuhu");
            GameObject startLight = myPrefab;
            GameObject bullet = Instantiate(startLight);
            bullet.transform.position = transform.position + new Vector3(5.0f, 0.3f, 0);
        }
    }
    private void Fire() //用来子弹发射的函数
    {
        // Texture2D Tex = Resources.Load ("shoot2") as Texture2D;
        // SpriteRenderer spr = GetComponent<SpriteRenderer> ();  
        // Sprite spriteA = Sprite.Create (Tex, spr.sprite.textureRect, new Vector2 (0.5f, 0.5f));
        //GetComponent<SpriteRenderer> ().sprite = showShoot;
        if (Input.GetKeyDown(KeyCode.J))
        {
            GameObject bulletUse = bulletPrefab;
            GameObject bullet = Instantiate(bulletUse);
            bullet.transform.position = transform.position + new Vector3(1.5f, 0.3f, 0);
        }            
    }

    void Recovery() //用来子弹发射的函数
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Instantiate(RecoveryPrefab, transform.position, Quaternion.identity);
        }
    }

}
