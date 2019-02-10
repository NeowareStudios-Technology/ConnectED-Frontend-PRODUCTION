using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Logout : MonoBehaviour {
    //this script deletes your stored account info and restarts the app
    public void pressed()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(0);
    }
}
