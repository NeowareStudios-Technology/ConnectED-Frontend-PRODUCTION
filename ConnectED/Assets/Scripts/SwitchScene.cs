using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;
public class SwitchScene : MonoBehaviour {
    private string scene = "splash";
    public Animator form;
    public Animator eventpanel;
    //this controls some of the animations and is used to switch between some scenes
    public void Switch()
    {
        if (scene == "splash" && PlayerPrefs.GetInt("signedin", 0) == 0)
        {
            scene = "form";
            animateform();
        }
        else
            animateevent();
    }

    public void animateform(){
        form.enabled = true;
    }
    public void animateevent(){
        eventpanel.enabled = true;
    }
}
