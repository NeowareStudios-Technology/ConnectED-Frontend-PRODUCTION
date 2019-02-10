using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using UnityEngine.Networking;
using System.Text;
using UnityEngine.SceneManagement;

public class DetailsTeamInstantiator : MonoBehaviour
{
    //this initializes teams on the details page
    public Team team;
    public RawImage pic;
    public Text teamName;
    public Text capacity;
    public Text eventsDone;
    public Button button;
    private Jsonparser j;
    public GameObject teamPage;

   



    //this initializes the team button
    public void finishSet(Team t, GameObject teamPage,int i,int eventCapacity){

        team = t;
        teamName.text = t.t_name;
        capacity.text = i.ToString() +" / "+eventCapacity;
        if (t.t_photo != null && t.t_photo.Length > 300)
        {
            Texture2D tex = new Texture2D(200, 200);


            byte[] img = System.Convert.FromBase64String(t.t_photo);
            tex.LoadImage(img, false);

            pic.texture = tex;

        }
        //you can press the button to see the team page for it
        GetComponent<Button>().onClick.AddListener(() => teamPage.SetActive(true));
        GetComponent<Button>().onClick.AddListener(() => teamPage.GetComponent<Image>().color = Color.white);
        GetComponent<Button>().onClick.AddListener(() => teamPage.transform.GetChild(0).gameObject.SetActive(true));
        GetComponent<Button>().onClick.AddListener(() => teamPage.GetComponent<TeamPageInit>().setTeamPage(t));

    }

}
