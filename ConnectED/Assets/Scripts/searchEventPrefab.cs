using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Unity.Editor;
using UnityEngine.Networking;

public class searchEventPrefab : MonoBehaviour {

    public Text title;
    public Text distance;
    public Text day;
    public Text month;
    public RawImage pic;
    public Button button;
    //this sets the events that you search for and allows you to look at the events by clicking on them
    public void setSearchEvent(EventSearch e , int i)
    {
        title.text = e.event_ids[i].Split('_')[1].Replace("+"," ");
        distance.text = Mathf.Round(e.distances[i]).ToString()+" Miles Away";
        day.text = int.Parse(e.event_dates[i].Substring(3, 2)).ToString();
        month.text = monthGetter(e.event_dates[i].Substring(0, 2));

        if (e.event_pics[i].Length > 300)
        {
            Texture2D tex = new Texture2D(200, 200);


            byte[] img = System.Convert.FromBase64String(e.event_pics[i]);
            tex.LoadImage(img, false);

            pic.texture = tex;

        }

        button.onClick.AddListener(() => StartCoroutine(setDetails(e.event_ids[i])));

    }
    private string getEventurl = "https://connected-dev-214119.appspot.com/_ah/api/connected/v1/events/";
    private string jsonString;
    private Event Event;
    public IEnumerator setDetails(string s)
    {

        using (UnityWebRequest www = UnityWebRequest.Get(getEventurl + s.Replace("_","/")))
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
                Event = JsonUtility.FromJson<Event>(jsonString);
                GameObject.FindWithTag("Details").GetComponent<DetailChanger>().setDetails(Event);
                GameObject.FindWithTag("Details").GetComponent<Animator>().SetTrigger("Show");


            }

        }
    }

        public string monthGetter(string month)
        {
            string s = "";
            switch (int.Parse(month))
            {
                case 1:
                    s = "January";
                    break;
                case 2:
                    s = "February";
                    break;
                case 3:
                    s = "March";
                    break;
                case 4:
                    s = "April";
                    break;
                case 5:
                    s = "May";
                    break;
                case 6:
                    s = "June";
                    break;
                case 7:
                    s = "July";
                    break;
                case 8:
                    s = "August";
                    break;
                case 9:
                    s = "September";
                    break;
                case 10:
                    s = "October";
                    break;
                case 11:
                    s = "November";
                    break;
                case 12:
                    s = "December";
                    break;

            }
            return s;
        }
    }

