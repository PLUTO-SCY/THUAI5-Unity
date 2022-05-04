using UnityEngine;
using TMPro;

public class EndScore : MonoBehaviour
{
    private TMP_Text tmpro;
    public int teamId = 0;
    // Start is called before the first frame update
    void Start()
    {
        tmpro = transform.gameObject.GetComponent<TMP_Text>();
        if (teamId==0)
        {
            tmpro.text = ScoreRecord.score0.ToString();
        }
        else
        {
            tmpro.text = ScoreRecord.score1.ToString();
        }
    }
}
