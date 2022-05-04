/*
 * �����˵��߸���ϵͳ
 * latest update�� 20220228
 */

using Communication.Proto;
using UnityEngine;

public class PropScript : MonoBehaviour
{
    public PropType type;
    private double facingDirection;
    private long guid;
    private int size;  //��ʯ��С???����Ҫ���Ǵ�С������
    private PlaceType place;
    private bool isfirstUpdate;

    public float moveSpeed = 4.0f;
    private Vector2 position;

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
        type = PropType.AddLife;
        facingDirection=0.0f;
        guid = 0;
        size = 0;  //��ʯ��С???����Ҫ���Ǵ�С������
        place = PlaceType.Grass1;
        isfirstUpdate = true;

        position = new Vector2();
        //oldPos = transform.localPosition; // �������λ�ñ��浽oldPos  
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = position;
        posOffset = transform.position;

        //transform.Rotate(new Vector3(0f, 0f,Time.deltaTime * degreesPerSecond), Space.World);
        transform.Rotate(new Vector3(0f, 0f, (float)0.02 * Mathf.Sin(Time.fixedTime * Mathf.PI * frequency2) * amplitude), Space.World);
        //�����������õ�����ά��ת���Ͳ�����

        // Float up/down with a Sin()
        tempPos = posOffset;
        tempPos.y += (float)0.55 * Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

        transform.position = tempPos;
    }


    public void Renew(MessageOfProp msgOfProp)
    {
        if (isfirstUpdate)    //fixed attributes
        {
            type = msgOfProp.Type;
            isfirstUpdate = false;
            guid = msgOfProp.Guid;
            size = msgOfProp.Size;  //��ʯ��С???����Ҫ���Ǵ�С������
            place = msgOfProp.Place;
        }
        position.x = (float)msgOfProp.Y / (float)1000;
        position.y = (float)50 - (float)msgOfProp.X / (float)1000;
        facingDirection = msgOfProp.FacingDirection;
    }

    public void Picked()
    {
        //Debug.Log("Picked");
        //Destroy(this.gameObject); //
        gameObject.SetActive(false);
        Debug.Log("Picked(x) Hide(��)");
    }
    public void unPicked()
    {
        //Debug.Log("Picked");
        //Destroy(this.gameObject); //
        gameObject.SetActive(true);
        Debug.Log("unPicked(x) unHide(��)");
    }
}
