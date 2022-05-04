using UnityEngine;

//this script is to
// Makes objects float up & down while gently spinning.
public class PropJitter : MonoBehaviour
{

    public float degreesPerSecond = 15.0f;
    public float amplitude = 0.5f;
    public float frequency = 1f;  //���¸�����Ƶ��
    public float frequency2 = 0.5f;//��ת������Ƶ��


    // Position Storage Variables
    Vector3 posOffset = new Vector3();
    Vector3 tempPos = new Vector3();


    // Start is called before the first frame update
    void Start()
    {
        //oldPos = transform.localPosition; // �������λ�ñ��浽oldPos  
        posOffset = transform.position;
    }


    // Update is called once per frame
    void Update()
    {
        //transform.Rotate(new Vector3(0f, 0f,Time.deltaTime * degreesPerSecond), Space.World);
        transform.Rotate(new Vector3(0f, 0f, (float)0.02 * Mathf.Sin(Time.fixedTime * Mathf.PI * frequency2) * amplitude), Space.World);
        //�����������õ�����ά��ת���Ͳ�����

        // Float up/down with a Sin()
        tempPos = posOffset;
        tempPos.y += (float)0.55 * Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

        transform.position = tempPos;
    }
}
