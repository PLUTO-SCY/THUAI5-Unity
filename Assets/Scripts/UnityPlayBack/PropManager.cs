using Communication.Proto;
using UnityEngine;
using static Communication.Proto.MessageToClient.Types;  //later find that this can simplify the code

public class PropManager : MonoBehaviour
{

    public GameObject propAddSpeed;
    public GameObject propAddLife;
    public GameObject propGem;
    public GameObject propShield;
    public GameObject propSpear;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public GameObject PropMap(GameObjMessage objValue)
    {
        GameObject propObj;
        switch (objValue.MessageOfProp.Type)
        {      
                
            case PropType.AddLife:
                propObj = propAddLife;
                break;
            case PropType.AddSpeed:
                propObj = propAddSpeed;
                break;
            case PropType.Gem:
                propObj = propGem;
                break;
            case PropType.Shield:
                propObj = propShield;
                break;
            case PropType.Spear:
                propObj = propSpear;
                break;

            default: 
                propObj = null;
                break;
        }
        return propObj;
    }
}
