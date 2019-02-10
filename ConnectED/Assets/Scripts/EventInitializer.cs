using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EventInitializer : MonoBehaviour {

    private Event e;
    public RawImage image;
    public Text eventTitle;
    public Text Month;
    public Text Day;
    public Text Miles;
    public Text Availability;
    public Text Time;
    public Button button;
    // Use this for initialization

    public void setRegistration(int i){
        e.is_registered = i;
    }
    //this sets the event tiles button actions
    public void deets()
    {
        button.onClick.AddListener(() => GameObject.FindWithTag("Details").GetComponent<DetailChanger>().setDetails(e));
        button.onClick.AddListener(() => GameObject.FindWithTag("Player").GetComponent<Jsonparser>().setExploreTile(this.gameObject));
    }


    //this initializes the event tile
    public void GetEvent(Event a,float f)
    {
        e = a;

        eventTitle.text = e.e_title;
        if (e.date[0] != null)
            Month.text = GetMonth(e.date[0]);
        if (e.day[0] != null)
            Day.text = GetDay(e.date[0]);
        Miles.text = Mathf.Round(f).ToString() + " Miles away";
        Availability.text = e.num_attendees + " / "+e.capacity.ToString();
        Time.text = GetTime(e.start[0]);
        if(e.e_photo.Length > 300){
            Texture2D tex = new Texture2D(200, 200);


            byte[] img = System.Convert.FromBase64String(a.e_photo);
                tex.LoadImage(img, false);

                image.texture = tex;

        }

        deets();
            
    }
    public string GetTime(string s){
       
        string hour = s.Substring(0, 2);
		string ending = s.Substring(2);

            int ihour = int.Parse(hour);
            ihour = ihour - 12;
        if (ihour < 1)
            return (ihour + 12).ToString() + ending + "am";
        else
            return ihour.ToString() + ending + "pm";

    }
            

    // s == 00/00/00
    public string GetMonth(string s){
        string m = s.Substring(0, 2);
        switch(m)
        {
            case "01":
                return "JAN";
            case "02":
                return "FEB";
            case "03":
                return "MAR";
            case "04":
                return "APR";
            case "05":
                return "MAY";
            case "06":
                return "JUN";
            case "07":
                return "JUL";
            case "08":
                return "AUG";
            case "09":
                return "SEP";
            case "10":
                return "OCT";
            case "11":
                return "NOV";
            case "12":
                return "DEC";
            default:
                return "???";
        }
    }
    // s = 00/00/00
    public string GetDay(string s)
    {
        string m = s.Substring(3, 2);
        if (m[0] == '0')
            return m[1].ToString();
        else
            return m;
    }
}
