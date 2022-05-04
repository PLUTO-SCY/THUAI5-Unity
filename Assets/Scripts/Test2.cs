using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClassLibrary2;  //纯粹为了检验Unity中引用其他dll库的情况

public class Test2 : MonoBehaviour
{
    public GameObject gemm;
    // Start is called before the first frame update
    void Start()
    {
        Class1 cl2 = new Class1();
        Debug.Log(cl2.Paint());
        for (int i =0;i<1000;i++)
        {
            Instantiate(gemm, new Vector3(1000, 1000, 0), Quaternion.identity);
        }        
    }

    // Update is called once per frame
    void Update()
    {     
    }
}
