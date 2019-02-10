using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamEventInit : MonoBehaviour {

    public RawImage pic;
    public Text eventName;
    public Text distance;
    public Text day;
    public Text month;
    private Event e;
    public void initEvent(Event a){
        e = a;
        if (e.e_photo.Length > 300)
        {
            Texture2D tex = new Texture2D(200, 200);
            byte[] img = System.Convert.FromBase64String(e.e_photo);
            Debug.Log(img);
            tex.LoadImage(img, false);

            pic.texture = tex;
        }
        eventName.text = e.e_title;
    }
    public void getEvent(string s)
    {
        
    }


}
