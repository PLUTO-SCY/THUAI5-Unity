/*
 * 用来控制闪电，一定时间之后自动销毁
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyLight : MonoBehaviour
{
    private long playerTime0 = DateTime.Now.Ticks;
    private long playerTime1;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        playerTime1 = DateTime.Now.Ticks;
        if (playerTime1 - playerTime0 > 10000000)
        {
            Destroy(this.gameObject);
        }
    }
}
