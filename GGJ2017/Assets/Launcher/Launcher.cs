using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Launcher : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        Application.RequestUserAuthorization(UserAuthorization.WebCam);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
