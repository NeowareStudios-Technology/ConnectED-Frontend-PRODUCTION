using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Unity.Editor;
using UnityEngine.Networking;

public class searchTeamInitializer : MonoBehaviour {

    public Text title;
    public RawImage pic;
    public Button button;
    public Search search;
    //this sets the team prefabs for the searched teams
    public void setSearchTeam(TeamSearch e, int i, GameObject teamPage, Search s)
    {
        search = s;
        title.text = e.name[i];

        if (e.pic[i].Length > 300)
        {
            Texture2D tex = new Texture2D(200, 200);


            byte[] img = System.Convert.FromBase64String(e.pic[i]);
            tex.LoadImage(img, false);

            pic.texture = tex;

        }
        //if you click on it it will bring up the team page
        button.onClick.AddListener(() => StartCoroutine(setDetails(e.t_id[i],teamPage)));

    }
    private string getEventurl = "https://connected-dev-214119.appspot.com/_ah/api/connected/v1/teams/";
    private string jsonString;
    private Team Team;
    public IEnumerator setDetails(string s,GameObject teamPage)
    {

        using (UnityWebRequest www = UnityWebRequest.Get(getEventurl + s.Replace("_", "/")))
        {



            www.SetRequestHeader("Authorization", "Bearer " + GameObject.FindWithTag("Player").GetComponent<Jsonparser>().token);
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.responseCode);
                Debug.Log(www.url);
                Debug.Log(www.GetRequestHeader("Authorization"));
                Debug.Log(www.GetRequestHeader("Content-Type"));
                Debug.Log(www.error);
                Debug.Log(www.downloadHandler.text);
            }
            else
            {
                Debug.Log(www.responseCode);
                byte[] results = www.downloadHandler.data;
                jsonString = "";
                jsonString = Encoding.UTF8.GetString(results);
                Debug.Log(jsonString);
                Team = JsonUtility.FromJson<Team>(jsonString);
                //set team page instead of details

                //GameObject.FindWithTag("Details").GetComponent<DetailChanger>().setDetails(Team);
                //GameObject.FindWithTag("Details").GetComponent<Animator>().SetTrigger("Show");
                //team page initialization stuff
                teamPage.SetActive(true);
                teamPage.GetComponent<Image>().color = Color.white;
                teamPage.transform.GetChild(0).gameObject.SetActive(true);
                teamPage.GetComponent<TeamPageInit>().setTeamPage(Team);
                search.gameObject.GetComponent<Animator>().SetTrigger("SearchingCancel");
            }

        }
    }
}
