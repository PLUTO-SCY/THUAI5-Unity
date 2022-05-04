/*
 * 加入了道具浮动系统
 * latest update： 20220228
 */

using Communication.Proto;
using UnityEngine;

public class PropScript : MonoBehaviour
{
    public PropType type;
    private double facingDirection;
    private long guid;
    private int size;  //宝石大小???还需要考虑大小？？？
    private PlaceType place;
    private bool isfirstUpdate;

    public float moveSpeed = 4.0f;
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
        type = PropType.AddLife;
        facingDirection=0.0f;
        guid = 0;
        size = 0;  //宝石大小???还需要考虑大小？？？
        place = PlaceType.Grass1;
        isfirstUpdate = true;

        position = new Vector2();
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


    public void Renew(MessageOfProp msgOfProp)
    {
        if (isfirstUpdate)    //fixed attributes
        {
            type = msgOfProp.Type;
            isfirstUpdate = false;
            guid = msgOfProp.Guid;
            size = msgOfProp.Size;  //宝石大小???还需要考虑大小？？？
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
        Debug.Log("Picked(x) Hide(√)");
    }
    public void unPicked()
    {
        //Debug.Log("Picked");
        //Destroy(this.gameObject); //
        gameObject.SetActive(true);
        Debug.Log("unPicked(x) unHide(√)");
    }
}
