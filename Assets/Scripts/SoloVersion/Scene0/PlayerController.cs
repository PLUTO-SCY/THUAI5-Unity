using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController1 : MonoBehaviour
{
    public GameObject myPrefab;    //�����Ԥ����
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
        LightAttack(); //�����ͷ�����
        Attack();      //��δ�����
        Fire();
        Recovery();
    }

    void Attack()
    {
        if(Input.GetButtonDown("Attack"))
        {

        }
    }

    void LightAttack()  //�����ͷ�����ļ���
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("wuhu");
            GameObject startLight = myPrefab;
            GameObject bullet = Instantiate(startLight);
            bullet.transform.position = transform.position + new Vector3(5.0f, 0.3f, 0);
        }
    }
    private void Fire() //�����ӵ�����ĺ���
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

    void Recovery() //�����ӵ�����ĺ���
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Instantiate(RecoveryPrefab, transform.position, Quaternion.identity);
        }
    }

}
