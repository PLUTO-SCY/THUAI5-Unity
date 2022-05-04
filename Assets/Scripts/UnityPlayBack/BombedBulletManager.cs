using Communication.Proto;
using UnityEngine;
using static Communication.Proto.MessageToClient.Types;  //later find that this can simplify the code

public class BombedBulletManager : MonoBehaviour
{
    public GameObject[] bullet = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject BombedBulletMap(GameObjMessage bulletMsg)
    {
        GameObject oneBullet;
        switch (bulletMsg.MessageOfBombedBullet.Type)
        { 
            case BulletType.AtomBomb:
                oneBullet = bullet[0];
                break;
            case BulletType.CommonBullet2:
                oneBullet = bullet[1];
                break;
            case BulletType.FastBullet:
                oneBullet = bullet[2];
                break;
            case BulletType.NullBulletType:
                oneBullet = bullet[3];
                break;
            case BulletType.OrdinaryBullet:
                oneBullet = bullet[4];
                break;
            case BulletType.LineBullet:
                oneBullet = bullet[5];
                break;
            default:
                oneBullet = null;
                break;
        }
        return oneBullet;
    }
}
