using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class firstOfMonthTrigger : MonoBehaviour {

    public calendarPopulator p;
    public dayInfo info;
    //this is the trigger that happens when you are scrolling in the calendar
    private void OnTriggerEnter2D(Collider2D collision)
    {
        info = GetComponent<dayInfo>();
        Debug.Log("Changing day to: " + info.Month);
        p.setYearMonth(info.Year + "\n" + info.Month,info.Month);
    }
}
