using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health = 20;

    public int Blinks;    //闪烁次数
    public float time;    //闪烁时间 

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
    public void DamagePlayer(int damage)  //需要被外部类调用，设置成public
    {
        health -= damage;
        HealthBar.HealthCurrent = health;
        if (health<=0)
        {
            myAnim.SetBool("Die",true);
            Invoke("DestroyPlayer", 1.0f);   //Invoke : 在每一帧 Update之后，系统会检查待执行的Invoke
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
