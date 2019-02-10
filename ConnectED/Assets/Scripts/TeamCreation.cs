using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
public class TeamCreation : MonoBehaviour {
    //this is used to create a team based on input provided
    public RawImage image;
    public InputField TeamName;
    public InputField TeamDesc;
    public bool expire;
    public bool priv = false;
    public InputField leader1;
    public InputField leader2;
    public InputField leader3;
    public Text city;
    public Text state;
    private Team team;
    public GameObject TeamSetup;
    public GameObject Leaders;
    private IEnumerator coroutine;
    public Jsonparser j;
    private string dbteams ="https://connected-dev-214119.appspot.com/_ah/api/connected/v1/teams";
	// Use this for initialization

    //this is called when you press the privacy buttons
    public void privacy(int i){
        if (i == 1)
            priv = false;
        else
            priv = true;
        
    }
    //this controls the team creation setup
    public void next(){

        if(TeamName.text != "" && TeamDesc.text != ""){

            TeamSetup.SetActive(false);
            Leaders.SetActive(true);
        }
            
    }
    //when you press the make team button first set everything make the web call then set the leaders through another web call
    public void makeTeam()
    {

        if(city.text == null || state.text == null){
            return;
        }

		GetComponent<Animator>().ResetTrigger("Team");
        GetComponent<Animator>().SetTrigger("Hide");


        team = new Team();
        team.t_desc = TeamDesc.text;
        team.t_name = TeamName.text;
        if(priv)
            team.t_privacy = "p";
        else
            team.t_privacy = "o";
        team.t_city = city.text;
        team.t_state = state.text;
        if (image.color.a == 1)
        {
            RenderTexture tmp = RenderTexture.GetTemporary(image.texture.width, image.texture.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
            Graphics.Blit(image.texture, tmp);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = tmp;
            Texture2D myTexture2D = new Texture2D(image.texture.width, image.texture.height);
            myTexture2D.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
            myTexture2D.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(tmp);
            //https://support.unity3d.com/hc/en-us/articles/206486626-How-can-I-get-pixels-from-unreadable-textures-
            team.t_photo = Convert.ToBase64String(myTexture2D.EncodeToJPG());
            //read in with texture2d.loadimage(bytedata);
        }else{
            team.t_photo = "";
        }

        string t = "Bearer " + j.token;
        string newEvent = JsonUtility.ToJson(team);
        byte[] bodyRaw2 = Encoding.UTF8.GetBytes(newEvent);
        UnityWebRequest www2 = UnityWebRequest.Post(dbteams, newEvent);
        www2.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw2);
        www2.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www2.SetRequestHeader("Authorization", t);
        coroutine = Post(www2);
        StartCoroutine(coroutine);
    }

    private string leadersURL = "https://connected-dev-214119.appspot.com/_ah/api/connected/v1/teams/";
    private IEnumerator Post(UnityWebRequest www)
    {
        yield return www.SendWebRequest();

        Debug.Log("Status Code: " + www.responseCode);
        Debug.Log(www.error);
        Debug.Log(www.downloadHandler.text);
        Debug.Log(www.url);
        Debug.Log(www.GetRequestHeader("Authorization"));
        if (www.responseCode.ToString() == "503")
        {
            Debug.Log("try again :maketeam");
        }
        if (www.responseCode.ToString() == "200" && leader1.text != "")
        {
            //if it was successful and there are leaders
            Leaders leaders = new Leaders();
            leaders.leaders = new string[3];
            leaders.leaders[0] = leader1.text;
            leaders.leaders[1] = leader2.text;
            leaders.leaders[2] = leader3.text;
            string newLeaders = JsonUtility.ToJson(leaders);
            UnityWebRequest www2 = UnityWebRequest.Put(leadersURL + TeamName.text.Replace(" ", "+") + "/leaders", newLeaders);
            byte[] bodyRaw2 = Encoding.UTF8.GetBytes(newLeaders);
            www2.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw2);
            www2.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            www2.SetRequestHeader("Authorization", "Bearer " + j.token);
            www2.SetRequestHeader("Content-Type", "application/json");
            coroutine = setLeaders(www2);
            StartCoroutine(coroutine);

        }
        //otherwise reset the app to include new team
        else
        {
            SceneManager.LoadScene(0);
        }
      

    }
    private IEnumerator setLeaders(UnityWebRequest www)
    {
        yield return www.SendWebRequest();

        Debug.Log("Status Code: " + www.responseCode);
        Debug.Log(www.error);
        Debug.Log(www.uploadHandler.data);
        Debug.Log(www.downloadHandler.data);
        Debug.Log(www.downloadHandler.text);
        Debug.Log(www.GetRequestHeader("Authorization"));
        if (www.responseCode.ToString() == "503")
        {
            Debug.Log("try again: set leaders");

        }
        if (www.responseCode.ToString() == "200")
        {
            //reset the app to include new team
            Debug.Log("Leaders set");
            SceneManager.LoadScene(0);
        }
    }

}

[System.Serializable]
public class Team
{
	public int funds_raised;
    public int is_registered;
	public string t_city;
	public string t_desc;
    public int t_hours;
    public string[] t_leaders;
    public int t_member_num;
    public string[] t_members;
    public string t_name;
	public string t_organizer;
    public string t_orig_name;
    public int t_pending_member_num;
    public string[] t_pending_members;
    public string t_photo;
    public string t_privacy;
    public string t_state;
}