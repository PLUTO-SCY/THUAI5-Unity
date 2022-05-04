using Communication.Proto;
using UnityEngine;

/*
 BomedBullet 应该是放完一边动画就摧毁自身
 */

public class BombedBulletScript : MonoBehaviour
{
    private double facingDirection;
    private Vector3 position;
    private long guid;
    private BulletType type;
    //  bool isfirst = false;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("start");
        Invoke("die", (float)0.6);
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (isfirst == false)
        {
            isfirst = true;
            Debug.Log("update");
        }
        */
    }

    public void Renew(MessageOfBombedBullet msgOfBullet)
    {
        /*
        transform.position = new Vector3(
            (float)msgOfBullet.Y / (float)1000,
            (float)50 - (float)msgOfBullet.X / (float)1000,
            0);
        */
        guid = msgOfBullet.MappingID;
        type = msgOfBullet.Type;
        facingDirection = msgOfBullet.FacingDirection;
        transform.Rotate(0, 0, ((float)facingDirection / Mathf.PI + 1) * (float)180, 0);
    }

    public void die()
    {
        Destroy(this.gameObject);        
    }
}
