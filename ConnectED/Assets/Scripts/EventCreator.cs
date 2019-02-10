using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text;
using System;
using UnityEngine.SceneManagement;


public class EventCreator : MonoBehaviour
{
    //this code takes input from the event creation pages and uses that to create an event on the database
    public InputField title;
    public InputField description;
    public Dropdown month;
    public Dropdown day;
    public Dropdown year;
    public InputField city;
    public InputField State;
    public InputField Street;
    public InputField zipcode;
    public InputField NumberofVolunteers;
    public InputField LeaderOne;
    public InputField LeaderTwo;
    public InputField LeaderThree;
    public InputField TagOne;
    public InputField TagTwo;
    public InputField TagThree;
    public RawImage image;
    public bool isPrivate;
    public bool skillsRequired;
    public InputField SkillOne;
    public string[] teams;
    public InputField SkillTwo;
    public InputField SkillThree;
    public string qr;
    public string env = null;
    public Jsonparser j;
    public timeUpdater st;
    public timeUpdater en;
    public returnPressedFields f;
    public returnPressedFields s;
	private IEnumerator coroutine;
    public EventSpawner es;
    private string dbevents = "https://connected-dev-214119.appspot.com/_ah/api/connected/v1/events";

    public string[] getDate()
    {
        string s;
		s = (year.value + 2018).ToString();
        s += "-";
        if (month.value < 9)
            s += '0' + (month.value + 1).ToString();
        else
            s += (month.value + 1).ToString();
		Debug.Log(s);
        s += "-";
        if(day.value < 9)
            s += '0' + (day.value + 1).ToString();
        else
            s += (day.value + 1).ToString();
        Debug.Log(s);
        return new string[] {s};
    }

    public void privacy(int i){
        if (i == 1)
            isPrivate = true;
        else
            isPrivate = false;
    }

    public void skillRequired(int i)
    {
        if (i == 0)
            skillsRequired = false;
        else
            skillsRequired = true;
    }

    public void environment(int i)
    {
        if(i == 0){
            env = "i";
        }
        if(i == 1){
            env = "o";
        }
        if(i == 2){
            env = "b";
        }
    }

    public QREncodeTest QREncode;
    public void initEvent()
    {
        Event e = new Event();
        if(NumberofVolunteers.text != "" || NumberofVolunteers.text !=null)
        e.capacity = int.Parse(NumberofVolunteers.text);
        e.city = city.text;
        e.date = getDate();
        e.day = new string[] { "mon","tue"};
        e.e_desc = description.text;
        e.e_title = title.text;
        e.education = "";
        e.end = en.time();
        e.env = env;
        e.interests = f.returnFields();
        if (isPrivate)
            e.privacy = "p";
        else
            e.privacy = "o";
        e.qr = "";
        if(skillsRequired)
            e.req_skills = s.returnFields();
        e.start = st.time();
        e.state = State.text;
        e.street = Street.text;
        e.zip_code = zipcode.text;
        e.qr = QREncode.Encode(PlayerPrefs.GetString("email")+"/"+e.e_title.Replace(" ","+"));
        if (image.texture != null && image.color.a == 1)
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
            e.e_photo = Convert.ToBase64String(myTexture2D.EncodeToJPG());
            //read in with texture2d.loadimage(bytedata);
        }
        Debug.Log(e.e_title);
        string t = "Bearer " + j.token;
        string newEvent = JsonUtility.ToJson(e);
        Debug.Log(newEvent);
        byte[] bodyRaw2 = Encoding.UTF8.GetBytes(newEvent);
        UnityWebRequest www2 = UnityWebRequest.Post(dbevents, newEvent);
        www2.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw2);
        www2.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www2.SetRequestHeader("Authorization", t);
        www2.SetRequestHeader("Content-Type", "application/json");
        coroutine = Post(www2);
        StartCoroutine(coroutine);
    }

    private string leadersURL = "https://connected-dev-214119.appspot.com/_ah/api/connected/v1/events/";
    private IEnumerator Post(UnityWebRequest www)
    {
        yield return www.SendWebRequest();

        Debug.Log("Status Code: " + www.responseCode);
        Debug.Log(www.error);
        Debug.Log(www.uploadHandler.data);
        Debug.Log(www.downloadHandler.data);
        Debug.Log(www.GetRequestHeader("Authorization"));
        if(www.responseCode.ToString() == "503"||www.responseCode.ToString() == "400"){
            Debug.Log("try again: make event");
            initEvent();
        }
        //on successful webcall
        if (www.responseCode.ToString() == "200" && LeaderOne.text != "")
        {
            Leaders leaders = new Leaders();
            leaders.leaders = new string[3];
            leaders.leaders[0] = LeaderOne.text;
            leaders.leaders[1] = LeaderTwo.text;
            leaders.leaders[2] = LeaderThree.text;
            string newLeaders = JsonUtility.ToJson(leaders);
            UnityWebRequest www2 = UnityWebRequest.Put(leadersURL + title.text.Replace(" ","+")+"/leaders", newLeaders);
            byte[] bodyRaw2 = Encoding.UTF8.GetBytes(newLeaders);
            www2.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw2);
            www2.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            www2.SetRequestHeader("Authorization", "Bearer " + j.token);
            www2.SetRequestHeader("Content-Type", "application/json");
            coroutine = setLeaders(www2);
            StartCoroutine(coroutine);
		
        }
        else{
            es.populateEvents();
            //SceneManager.LoadScene(0);
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
            initEvent();
        }
        if (www.responseCode.ToString() == "200")
        {
            Debug.Log("Leaders set");
            es.populateEvents();
            //SceneManager.LoadScene(0);
        }
    }

}
//this is how you set up json objects in c#
[System.Serializable]
public class Event
{
    public string[] attendees;
    public int capacity;
    public string city;
    public string[] date;
    public string[] day;
    public string e_desc;
    public double e_lat;
    public double e_lon;
    public string e_organizer;
    public string e_orig_title;
    public string e_title;
    public string[] leaders;
    public string education;
    public string[] end;
    public string env;
    public int funds_raised;
    public string[] interests;
    public int is_registered;
    public int num_attendees;
    public int num_pending_attendees;
    public string[] pending_attendees;
    public string privacy;
    public string[] req_skills;
    public string[] start;
    public string state;
    public string street;
    public string[] teams;
    public string zip_code;
	public string qr;
	public string e_photo;
}
public class Leaders{
    public string[] leaders;
}