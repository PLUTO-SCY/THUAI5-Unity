using Communication.Proto;
using System;
using UnityEngine;


public class BulletScript : MonoBehaviour
{
    private BulletType type;
    private double facingDirection;
    private long guid;
    private long parentTeamID;
    private PlaceType place;

    private Vector2 position;
    private bool isfirstUpdate;
    private float moveThreshold = 5;
    private Vector3 position2;

    public float moveSpeed = 3.8f;
    private int i;


    // Start is called before the first frame update
    void Start()
    {
        type = BulletType.AtomBomb;
        facingDirection = 0.0f;
        guid = 0;
        parentTeamID = 0;
        place = PlaceType.Land;

        isfirstUpdate = true;

        position = new Vector3();
        i = 0;
    }


    // Update is called once per frame
    void Update()
    {
        if (transform.position == position2)
        {
            i++;
        }
        if (i>40)
        {
            Destroy(this.gameObject);
        }           
        position2 = transform.position;

        Vector2 diffVec;  //Calculate the difference between vectors
        diffVec.x = position.x - transform.position.x;
        diffVec.y = position.y - transform.position.y;
        
        /*
        if (diffVec.magnitude<0.2f)
        {
            Destroy(this.gameObject);
        }        //行不通*/
        
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

        //Judge();

    }

    public void Renew(MessageOfBullet msgOfBullet)
    {
        if (isfirstUpdate)
        {
            isfirstUpdate = false;
            guid = msgOfBullet.Guid;
            parentTeamID = msgOfBullet.ParentTeamID;
            place = PlaceType.Land;
            facingDirection = msgOfBullet.FacingDirection;
            //Debug.Log("face:");
            //Debug.Log(((float)facingDirection * 2 / Mathf.PI - 1) * (float)90);
            transform.Rotate(0, 0, ((float)facingDirection*2/Mathf.PI-1)*(float)90,0);
        }
        /*
        Debug.Log("bulletx:");
        Debug.Log(msgOfBullet.X);
        Debug.Log("bullety:");
        Debug.Log(msgOfBullet.Y);
        */
        position.x = (float)msgOfBullet.Y / (float)1000;
        position.y = (float)50 - (float)msgOfBullet.X / (float)1000;
        /*
        Debug.Log("bullet:");
        Debug.Log(position.x);
        Debug.Log(position.y);
        */
        
    }

    public void die()
    {
        Destroy(this.gameObject);
    }

    public void Judge()
    {
        if (!PlayBackComponent.bullets.ContainsKey(guid))
        {
            Invoke("Die", 0.5f);
        }
    }
    private void Die()
    {
        Destroy(this.gameObject);
    }
}
