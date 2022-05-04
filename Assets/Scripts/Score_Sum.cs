using UnityEngine;
using TMPro;

public class Score_Sum : MonoBehaviour
{
    //private GameObject textSum;
    private TMP_Text tmpro;
    private int sumScore;
    public int teamId = 0;
    // Start is called before the first frame update
    void Start()
    {
        //textSum = transform.gameObject;
        sumScore = 0;
        tmpro = transform.gameObject.GetComponent<TMP_Text>();
        tmpro.text = sumScore.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        sumScore = 0;
        var all = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];
        foreach (var item in all)
        {
            //Debug.Log(item.name);
            if ((item.tag == "Player"))
                if (item.GetComponent<PlayerScript>().teamID == teamId)
                    sumScore += item.GetComponent<PlayerScript>().score;           
        }

        tmpro.text = sumScore.ToString();
        if (teamId ==0)
        {
            ScoreRecord.score0 = sumScore;
        }
        else
        {
            ScoreRecord.score1 = sumScore;
        }
    }
}
