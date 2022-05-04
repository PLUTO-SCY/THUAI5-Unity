using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    private float _timeCounter = 0;

    private void Update()
    {
        _timeCounter += Time.deltaTime;
        if (_timeCounter > 6)  //多少时间之后消失
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif            
        }
    }
}
