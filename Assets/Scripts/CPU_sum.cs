using UnityEngine;
using UnityEngine.UI;

public class CPU_sum: MonoBehaviour
{
    public int teamId = 0;
    public int playerId = 0;
    //private GameObject textSum;
    private Text tmpro;
    private int sumCPU;
    
    // Start is called before the first frame update
    void Start()
    {
        //textSum = transform.gameObject;
        sumCPU = 0;
        tmpro = transform.gameObject.GetComponent<Text>();
        tmpro.text = sumCPU.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        sumCPU = 0;
        var all = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];
        foreach (var item in all)
        {
            //Debug.Log(item.name);
            if ((item.tag == "Player"))
                if ((item.GetComponent<PlayerScript>().teamID == teamId) && (item.GetComponent<PlayerScript>().playerID == playerId))
                {
                    sumCPU = item.GetComponent<PlayerScript>().gemNum;
                    break;
                }
        }

        tmpro.text = sumCPU.ToString();
    }
}
