using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditScheduleController : MonoBehaviour {

	public GameObject mon;
    public GameObject tue;
    public GameObject wed;
    public GameObject thu;
    public GameObject fri;
    public GameObject sat;
    public GameObject sun;
    public GameObject am;
    public GameObject pm;
    public GameObject eve;
    public ProfileEdit p;
    //this sets all of the values for the editschedule spriteswitcher pressed returns true or false
    public void editSchedule()
    {
        p.mon = mon.GetComponent<spriteSwitcher>().pressed;
        p.tue = tue.GetComponent<spriteSwitcher>().pressed;
        p.wed = wed.GetComponent<spriteSwitcher>().pressed;
        p.thu = thu.GetComponent<spriteSwitcher>().pressed;
        p.fri = fri.GetComponent<spriteSwitcher>().pressed;
        p.sat = sat.GetComponent<spriteSwitcher>().pressed;
        p.sun = sun.GetComponent<spriteSwitcher>().pressed;
        p.timeday = getTimeDay();
        p.scheduleCheck = true;

    }
    public string getTimeDay()
    {
        string s = "";
        if (am.GetComponent<spriteSwitcher>().pressed)
            s += "a";
        if (pm.GetComponent<spriteSwitcher>().pressed)
            s += "p";
        if (eve.GetComponent<spriteSwitcher>().pressed)
            s += "e";
        return s;
    }
}
