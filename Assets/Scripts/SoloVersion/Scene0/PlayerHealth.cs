using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health = 20;

    public int Blinks;    //��˸����
    public float time;    //��˸ʱ�� 

    private Renderer myRenderer;
    private Animator myAnim;

    // Start is called before the first frame update
    void Start()
    {
        //HealthBar.HealthMax = health;
        //HealthBar.HealthCurrent = health;
        myRenderer = GetComponent<Renderer>();
        myAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void DamagePlayer(int damage)  //��Ҫ���ⲿ����ã����ó�public
    {
        health -= damage;
        HealthBar.HealthCurrent = health;
        if (health<=0)
        {
            myAnim.SetBool("Die",true);
            Invoke("DestroyPlayer", 1.0f);   //Invoke : ��ÿһ֡ Update֮��ϵͳ�����ִ�е�Invoke
            //Destroy(gameObject);
        }
        BlinkPlayer(Blinks, time);
    }
    void DestroyPlayer()
    {
        Destroy(gameObject);
    }

    void BlinkPlayer(int numBlinks,float seconds)
    {
        StartCoroutine(DoBlinks(numBlinks, seconds));
    }
    IEnumerator DoBlinks(int numBlinks,float seconds)
    {
        for(int i=0;i<numBlinks*2;i++)
        {
            myRenderer.enabled = !myRenderer.enabled;
            yield return new WaitForSeconds(seconds);
        }
        myRenderer.enabled = true;
    }
}
