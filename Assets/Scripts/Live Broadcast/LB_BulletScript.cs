/**
 * 注意：直播和回放的子弹一定要分开做
 */

using Communication.Proto;
using System;
using UnityEngine;


public class LB_BulletScript : MonoBehaviour
{
    private BulletType type;
    private double facingDirection;
    private long guid;
    private long parentTeamID;
    private PlaceType place;

    private Vector2 position;
    private Vector2 oldPosition;

    private Vector3 position2;

    private bool isfirstUpdate;
    private float moveThreshold = 5;

    public float moveSpeed = 3.9f;


    // Start is called before the first frame update
    void Start()
    {
        type = BulletType.AtomBomb;
        facingDirection = 0.0f;
        guid = 0;
        parentTeamID = 0;
        place = PlaceType.Land;

        isfirstUpdate = true;

        oldPosition = new Vector2();
        oldPosition.x = -1000;
        oldPosition.y = -1000;

        position = new Vector2();
    }


    // Update is called once per frame
    void Update()
    {
        if (transform.position == position2)
            Destroy(this.gameObject);
        position2 = transform.position;


        Vector2 diffVec;  //Calculate the difference between vectors
        diffVec.x = position.x - transform.position.x;
        diffVec.y = position.y - transform.position.y;
               
        

        if (diffVec.magnitude > moveThreshold)
        {
            transform.position = position;  //vec3 = vec2 actually
        }
        else
        {
            //use Mathf.Lerp  named"线性差值"
            Vector3 pos = transform.position;
            pos.x = Mathf.Lerp(pos.x, position.x, moveSpeed * Time.deltaTime);
            pos.y = Mathf.Lerp(pos.y, position.y, moveSpeed * Time.deltaTime);
            transform.position = pos;
        }

        Judge();
    }

    public void Renew(MessageOfBullet msgOfBullet)
    {
        position.x = (float)msgOfBullet.Y / (float)1250 - (float)20;
        position.y = (float)20 - (float)msgOfBullet.X / (float)1250;

        if (isfirstUpdate)
        {            
            guid = msgOfBullet.Guid;
            parentTeamID = msgOfBullet.ParentTeamID;
            place = PlaceType.Land;
            isfirstUpdate = false;
        }
        else
        {
            if (oldPosition == position)
                Destroy(this.gameObject);
        }
        oldPosition = position;

        facingDirection = msgOfBullet.FacingDirection;
    }

    public void die()
    {
        Destroy(this.gameObject);
    }

    public void Judge()
    {
        if (!LiveBroadcast.bullets.ContainsKey(guid))
        {
            Invoke("Die", 0.1f);
            Debug.Log("should die");
        }
    }
}
