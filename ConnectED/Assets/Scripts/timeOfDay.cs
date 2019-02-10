using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timeOfDay : MonoBehaviour {
    public spriteSwitcher am;
    public spriteSwitcher pm;
    public spriteSwitcher eve;
    public Jsonparser jsonparser;
    private string s;
    //this is used to report the time of day the user reports they would like to volunteer for events
    public string setTime()
    {
        s = "";
        if (am.pressed)
            s = s + "a";
        if (pm.pressed)
            s = s + "p";
        if (eve.pressed)
            s = s + "e";
        return s;
    }

}
