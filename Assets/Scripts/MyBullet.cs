using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyBullet : MonoBehaviour
{
    private float bulletSpeed = 15.0f;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //  匀速移动
        float step = bulletSpeed * Time.deltaTime;
        transform.Translate(0,-step,0, Space.Self);
        //  如果超出视野就摧毁目标
        Vector3 sp = Camera.main.WorldToScreenPoint(transform.position);
        if (sp.x > Screen.width)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(this.gameObject);
    }
}