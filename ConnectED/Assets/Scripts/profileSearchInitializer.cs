using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class profileSearchInitializer : MonoBehaviour {
    //this initializes the profiles in the search menu
    public RawImage pic;
    public Text name;
    public string email;
    public Button button;
    public Search search;
    public void setPic(string s)
    {
        if (s.Length > 300)
        {
            Texture2D tex = new Texture2D(200, 200);


            byte[] img = System.Convert.FromBase64String(s);
            tex.LoadImage(img, false);

            pic.texture = tex;

        }
    }
    public void setSearch(Search s)
    {
        search = s;
    }
    public void setNameEmail(string s, string e){
        name.text = s;
        email = e;
        //if you click on the person it will set them for the leader spot. which is the only search functionality so far.
        button.onClick.AddListener(() => search.setLeader(s));
    }
}


