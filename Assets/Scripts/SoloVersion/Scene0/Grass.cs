using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{
    public GameObject eyePrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        SpriteRenderer renderer = collision.GetComponent<SpriteRenderer>();    //初始化sprite renderer
        Color colortr = new Color(1f, 1f, 1f, 0.5f); //进草实现半透明
        renderer.color = colortr;
        //collision.gameObject.SetActive(false);
        //Debug.Log("peng!");  //调试碰撞发生

        GameObject colli = collision.gameObject;
        GameObject eye = Instantiate(eyePrefab);
        eye.transform.SetParent(colli.transform);   //将视野眼睛挂载在游戏对象上，以实现同步移动
        eye.transform.position = colli.transform.position + new Vector3(0, 1.3f, 0f);  //显示入草标记（小眼睛）
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        SpriteRenderer renderer = collision.GetComponent<SpriteRenderer>();    //初始化sprite renderer
        Color colortr = new Color(1f, 1f, 1f, 2.0f);   //出草取消透明效果
        renderer.color = colortr;
        //collision.gameObject.SetActive(true);
        //Debug.Log("hehehe!");  //调试碰撞离去

        GameObject colli = collision.gameObject;  //出草的时候把眼睛取消
        foreach (Transform child in colli.transform)
        {
            //Debug.Log(child.name); 
            if (child.name== "Grasseye(Clone)")
            {
                //Debug.Log("eyeexit");
                Destroy(child.gameObject);
            }
        }

    }
}
