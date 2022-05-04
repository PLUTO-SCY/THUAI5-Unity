using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading;

public class SceneTransfer : MonoBehaviour
{
    private float _timeCounter = 0;
    InputField inputField;
    private void Start()
    {
        inputField = GetComponent<InputField>();
        transform.GetComponent<InputField>().onEndEdit.AddListener(End_Value);
    }
    private void Update()
    {
        /*
        _timeCounter += Time.deltaTime;
        if (_timeCounter > 3)
        {
            SceneManager.LoadScene(1);
        }
        */        
    }

    public void End_Value(string inp)
    {     
        Debug.Log(inp+ ".thuaipb");
        RecordPLName.fileName = inp;
        Thread.Sleep(1000);
        SceneManager.LoadScene(1);       
    }
}
