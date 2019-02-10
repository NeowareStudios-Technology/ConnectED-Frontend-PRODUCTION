using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using UnityEngine.Networking;
using System.Text;

public class teamInitializer : MonoBehaviour {
    //this initializes the team buttons and allows you to click on them to see the team pages
    private Team team;
    public Jsonparser j;
    public RawImage pic;
    public Text teamName;
    public Text teamMembers;
    public Button button;

    public void setTeamButton(Team t, GameObject teamPage)
    {
        j = GameObject.FindWithTag("Player").GetComponent<Jsonparser>();
        team = t;
        teamName.text = t.t_name;
        teamMembers.text = t.t_member_num.ToString() + " Members" ;
        j.setTeamButton(gameObject);
        if ( t.t_photo != null && t.t_photo.Length > 300)
        {
            Texture2D tex = new Texture2D(200, 200);


            byte[] img = System.Convert.FromBase64String(t.t_photo);
            tex.LoadImage(img, false);

            pic.texture = tex;

        }
        //on click initialize the team page and set it correctly to show up
        GetComponent<Button>().onClick.AddListener(() => teamPage.SetActive(true));
        GetComponent<Button>().onClick.AddListener(() => teamPage.GetComponent<Image>().color = Color.white);
        GetComponent<Button>().onClick.AddListener(() => teamPage.transform.GetChild(0).gameObject.SetActive(true));
        GetComponent<Button>().onClick.AddListener(() => teamPage.GetComponent<TeamPageInit>().setTeamPage(t));
    }
    //this is used as a callback to let this page set the registration status of these teams
    public void setRegistration(int i){
        team.is_registered = i;
    }

}
