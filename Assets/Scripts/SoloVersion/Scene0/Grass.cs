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
        SpriteRenderer renderer = collision.GetComponent<SpriteRenderer>();    //��ʼ��sprite renderer
        Color colortr = new Color(1f, 1f, 1f, 0.5f); //����ʵ�ְ�͸��
        renderer.color = colortr;
        //collision.gameObject.SetActive(false);
        //Debug.Log("peng!");  //������ײ����

        GameObject colli = collision.gameObject;
        GameObject eye = Instantiate(eyePrefab);
        eye.transform.SetParent(colli.transform);   //����Ұ�۾���������Ϸ�����ϣ���ʵ��ͬ���ƶ�
        eye.transform.position = colli.transform.position + new Vector3(0, 1.3f, 0f);  //��ʾ��ݱ�ǣ�С�۾���
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        SpriteRenderer renderer = collision.GetComponent<SpriteRenderer>();    //��ʼ��sprite renderer
        Color colortr = new Color(1f, 1f, 1f, 2.0f);   //����ȡ��͸��Ч��
        renderer.color = colortr;
        //collision.gameObject.SetActive(true);
        //Debug.Log("hehehe!");  //������ײ��ȥ

        GameObject colli = collision.gameObject;  //���ݵ�ʱ����۾�ȡ��
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
