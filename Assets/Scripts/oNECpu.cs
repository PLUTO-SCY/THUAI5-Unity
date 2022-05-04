/*
 * 加入了道具浮动系统
 * latest update： 20220228
 */

using UnityEngine;

public class oNECpu : MonoBehaviour
{
    private Vector2 position;

    public float degreesPerSecond = 15.0f;
    public float amplitude = 0.5f;
    public float frequency = 1f;  //上下浮动的频率
    public float frequency2 = 0.5f;//旋转浮动的频率


    // Position Storage Variables
    Vector3 posOffset = new Vector3();
    Vector3 tempPos = new Vector3();

    // Start is called before the first frame update
    void Start()
    {
        position = new Vector2();
        position = transform.position;
        //oldPos = transform.localPosition; // 将最初的位置保存到oldPos  
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = position;
        posOffset = transform.position;

        //transform.Rotate(new Vector3(0f, 0f,Time.deltaTime * degreesPerSecond), Space.World);
        transform.Rotate(new Vector3(0f, 0f, (float)0.02 * Mathf.Sin(Time.fixedTime * Mathf.PI * frequency2) * amplitude), Space.World);
        //上面这行能让道具三维旋转，就不必了

        // Float up/down with a Sin()
        tempPos = posOffset;
        tempPos.y += (float)0.55 * Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

        transform.position = tempPos;
    }


}
