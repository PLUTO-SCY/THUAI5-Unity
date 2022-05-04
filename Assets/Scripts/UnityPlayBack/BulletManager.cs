using Communication.Proto;
using UnityEngine;
using static Communication.Proto.MessageToClient.Types;  //later find that this can simplify the code

public class BulletManager : MonoBehaviour
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

    public string Jkk()
    {
        return "hhhhhbullet";
    }

    public GameObject BulletMap(GameObjMessage bulletMsg)
    {
        GameObject oneBullet;
        switch (bulletMsg.MessageOfBullet.Type)
        {
            case BulletType.AtomBomb:
                oneBullet = bullet[0];
                break;
            case BulletType.CommonBullet2:
                oneBullet = bullet[2];
                break;
            case BulletType.FastBullet:
                oneBullet = bullet[3];
                break;
            case BulletType.NullBulletType:
                oneBullet = bullet[4];
                break;
            case BulletType.OrdinaryBullet:
                oneBullet = bullet[5];
                break;
            case BulletType.LineBullet:
                oneBullet = bullet[6];
                break;

            default:
                oneBullet = null;
                break;
        }
        return oneBullet;
    }
}
