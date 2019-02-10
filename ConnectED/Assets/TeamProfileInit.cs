using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamProfileInit : MonoBehaviour {
    //this initializes the profiles on the team page
    // Use this for initialization
    public RawImage pic;
    public Text Name;
    public Text Title;
    private Profile profile;
    public void setTeamProfile(Profile p,string orig)
    {
        profile = p;
        Name.text = p.first_name + " " + p.last_name;
        if (p.photo.Length > 300)
        {
            Texture2D tex = new Texture2D(200, 200);
            byte[] img = System.Convert.FromBase64String(p.photo);
            Debug.Log(img);
            tex.LoadImage(img, false);

            pic.texture = tex;
        }
    }
}
