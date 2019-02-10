using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using UnityEngine.Networking;
using System.Text;

public class calendarPopulator : MonoBehaviour
{
    //In this Class I create a calendar that is used to find out dates and populate the calendar
    public GameObject calendarPrefab;
    public GameObject newCalendarDot;
    public GameObject calendarContainer;
    public GameObject monthTrigger;
    public Text yearMonth;
    public class Month
    {
        public int monthTotal;
        public string monthName;
        public int year;
    }
    public Month jan = new Month();
    public Month feb = new Month();
    public Month mar = new Month();
    public Month apr = new Month();
    public Month may = new Month();
    public Month jun = new Month();
    public Month jul = new Month();
    public Month aug = new Month();
    public Month sep = new Month();
    public Month oct = new Month();
    public Month nov = new Month();
    public Month dec = new Month();
    public Month[] calendar;
    public Month currentMonth;
    public float contentHeight;
    public Event[] dates;
    //create all months and add them to a calendar
    public void StartCalendar()
    {
        setMonth(ref jan, 31, "January");
        setMonth(ref feb, 28, "February");
        setMonth(ref mar, 31, "March");
        setMonth(ref apr, 30, "April");
        setMonth(ref may, 31, "May");
        setMonth(ref jun, 30, "June");
        setMonth(ref jul, 31, "July");
        setMonth(ref aug, 31, "August");
        setMonth(ref sep, 30, "September");
        setMonth(ref oct, 31, "October");
        setMonth(ref nov, 30, "November");
        setMonth(ref dec, 31, "December");
        calendar = new Month[] { jan, feb, mar, apr, may, jun, jul, aug, sep, oct, nov, dec };
        currentMonth = aug;
        contentHeight = this.gameObject.GetComponent<RectTransform>().sizeDelta.y;
        populate();
    }

    public void setYearMonth(string s, string m)
    {
        yearMonth.text = s;
        changeMonthColor(m);
    }

    public void setMonth(ref Month m, int totalDays, string monthName)
    {
        m.monthName = monthName;
        m.monthTotal = totalDays;
    }

    public int count = 0;
    public int yearCount = 0;

    //this function populates the calendar based on the months provided
    public void populate()
    {
        //this sets all the information for each day of a month, and if its the 1st adds a box collider which handles changing the text at the top of the calendar
        for (int i = 1; i <= currentMonth.monthTotal; i++)
        {
            newCalendarDot = Instantiate(calendarPrefab, calendarContainer.transform);
            newCalendarDot.GetComponent<dayInfo>().dayNumber = i.ToString();
            newCalendarDot.GetComponent<dayInfo>().Month = currentMonth.monthName;
            newCalendarDot.GetComponent<dayInfo>().Year = (yearCount + 2018).ToString();
            newCalendarDot.transform.GetChild(0).GetComponent<Text>().text = i.ToString();
            if (i == 1)
            {
                newCalendarDot.AddComponent<BoxCollider2D>();
                newCalendarDot.GetComponent<BoxCollider2D>().isTrigger = true;
                newCalendarDot.AddComponent<Rigidbody2D>();
                newCalendarDot.GetComponent<Rigidbody2D>().gravityScale = 0f;
                newCalendarDot.GetComponent<Rigidbody2D>().isKinematic = true;
                newCalendarDot.AddComponent<firstOfMonthTrigger>();
                newCalendarDot.GetComponent<firstOfMonthTrigger>().p = this;
                newCalendarDot.tag = "1";
            }
        }
        count++;
        //this adds in the next month recursively up to 15 months in the future
        if (count < 15)
        {
            RectTransform rt = calendarContainer.gameObject.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, contentHeight * count);
            //Debug.Log("Finished " + currentMonth.monthName);
            switch (currentMonth.monthName)
            {
                case "January":

                    currentMonth = feb;
                    populate();
                    break;
                case "February":
                    currentMonth = mar;
                    populate();
                    break;
                case "March":
                    currentMonth = apr;
                    populate();
                    break;
                case "April":
                    currentMonth = may;
                    populate();
                    break;
                case "May":
                    currentMonth = jun;
                    populate();
                    break;
                case "June":
                    currentMonth = jul;
                    populate();
                    break;
                case "July":
                    currentMonth = aug;
                    populate();
                    break;
                case "August":
                    currentMonth = sep;
                    populate();
                    break;
                case "September":
                    currentMonth = oct;
                    populate();
                    break;
                case "October":
                    currentMonth = nov;
                    populate();
                    break;
                case "November":
                    currentMonth = dec;
                    populate();
                    break;
                case "December":
                    currentMonth = jan;
                    yearCount++;
                    populate();
                    break;
            }
            return;
        }
        //these change the size of the container to fit all of the days we just added
        calendarContainer.AddComponent<ContentSizeFitter>();
        calendarContainer.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        calendarContainer.transform.parent.gameObject.AddComponent<ContentSizeFitter>();
        calendarContainer.transform.parent.gameObject.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        monthTrigger.SetActive(true);
    }

	public Color blue;

    public void show()
    {
        GetComponent<Image>().color = blue;
        GetComponent<Image>().raycastTarget = true;
    }

    public Color grey;
    public Color white;
    public Color brightWhite;
    //when a 1st of the month is triggered this is called
    public void changeMonthColor(string s)
    {
        for (int i = 0; i < calendarContainer.transform.childCount; i++)
        {
            if (calendarContainer.transform.GetChild(i).GetComponent<dayInfo>() != null && !calendarContainer.transform.GetChild(i).GetComponent<dayInfo>().marked)
            {

                if (calendarContainer.transform.GetChild(i).GetComponent<dayInfo>().Month != s)
                {
                    calendarContainer.transform.GetChild(i).GetChild(0).GetComponent<Text>().color = grey;
                }
                else
                {
                    calendarContainer.transform.GetChild(i).GetChild(0).GetComponent<Text>().color = white;
                }
            }
        }
    }


    public void setEventsInCalendar()
    {
        
        instantiateCalendarButtons();

    }
    public GameObject calendarButtonPrefab;
    private GameObject newCalendarButton;
    public GameObject calendarDotPrefab;
    public GameObject calendarButtonContainerPrefab;
    public GameObject emptyCalendarButton;
    private GameObject newCalendarButtonContainerPrefab;
    public GameObject calendarButtonPanel;
    public GameObject calendarButtonDotContainer;
    public GameObject opportunityButtonPanel;
    public GameObject opportunityButtonContainer;
    public GameObject opportunityDotContainer;
    public GameObject myOpportunities;


    //this function Instantiates all of the buttons underneath the calendar, creating two per container then refreshing the snap rect script
    public void instantiateCalendarButtons()
    {
        newCalendarButtonContainerPrefab = Instantiate(calendarButtonContainerPrefab, calendarButtonPanel.transform);
        Instantiate(calendarDotPrefab, calendarButtonDotContainer.transform);
        for (int i = 0; i < dates.Length; i++)
        {
            if(newCalendarButtonContainerPrefab.transform.childCount == 2)
            {
                newCalendarButtonContainerPrefab = Instantiate(calendarButtonContainerPrefab, calendarButtonPanel.transform); 
				Instantiate(calendarDotPrefab, calendarButtonDotContainer.transform);
            }
            newCalendarButton = Instantiate(calendarButtonPrefab, newCalendarButtonContainerPrefab.transform);
            newCalendarButton.GetComponent<CalendarEventButton>().setCalendarEvent(dates[i]);
            if(dates[i].e_organizer == PlayerPrefs.GetString("email").ToLower())
            {
                Debug.Log("Match Found");
                addToOpportunities(newCalendarButton,i);
            }
            //newCalendarButton.GetComponent<CalendarEventButton>().j = j;
            
        }
        if(opportunityButtonContainer != null && opportunityButtonContainer.transform.childCount == 1)
            handleOpportunities();
        if(newCalendarButtonContainerPrefab.transform.childCount == 1)
            Instantiate(emptyCalendarButton, newCalendarButtonContainerPrefab.transform);
        calendarButtonPanel.transform.parent.GetComponent<ScrollSnapRect>().enabled = true;
        calendarButtonPanel.transform.parent.GetComponent<ScrollSnapRect>().Refresh();

    }
    public notificationQueuer noti;
    public GameObject newButtonObj;
    //this adds opportunities to the my opportunities tab same as the calendar buttons
    public void addToOpportunities( GameObject o, int i)
    {
        noti.queueNotifications(o.GetComponent<CalendarEventButton>().e);
        if (opportunityButtonContainer == null){
            opportunityButtonContainer = Instantiate(calendarButtonContainerPrefab,opportunityButtonPanel.transform);
            Instantiate(calendarDotPrefab, opportunityDotContainer.transform);
        }
        if (opportunityButtonContainer.transform.childCount < 2)
        {
            newButtonObj = Instantiate(o, opportunityButtonContainer.transform);
            newButtonObj.GetComponent<CalendarEventButton>().setCalendarEvent(dates[i]);
            myOpportunities.GetComponent<ScrollSnapRect>().enabled = true;
            myOpportunities.GetComponent<ScrollSnapRect>().Refresh();
            return;
        }
		if(opportunityButtonContainer.transform.childCount == 2)
        {
            opportunityButtonContainer = Instantiate(calendarButtonContainerPrefab, opportunityButtonPanel.transform);
            Instantiate(calendarDotPrefab, opportunityDotContainer.transform);
            newButtonObj = Instantiate(o, opportunityButtonContainer.transform);
            newButtonObj.GetComponent<CalendarEventButton>().setCalendarEvent(dates[i]);
            Debug.Log("Opportunity has 2 children");
			myOpportunities.GetComponent<ScrollSnapRect>().enabled = true;
			myOpportunities.GetComponent<ScrollSnapRect>().Refresh();
            return;
        }
    }

    public void handleOpportunities()
    {
        if(opportunityButtonContainer.transform.childCount == 1)
            Instantiate(emptyCalendarButton, opportunityButtonContainer.transform);
        noti.setNotification();
    }
    //this creates a list of days with events we will use to mark the calendar
    public void eventTrigger()
    {
        string[] eventDays = new string[dates.Length];
        int goodEvent = 0;
        for (int i = 0; i < dates.Length; i++)
        {
            //if (PlayerPrefs.GetString("email") == dates[i].e_organizer)
            //{
                eventDays[goodEvent] = dates[i].date[0];
                goodEvent++;
            //}
        }
        string[] temp = new string[goodEvent];
        for (int i = 0; i < goodEvent; i++)
            temp[i] = eventDays[i];

        setEventDate(eventDays);
    }

    //this highlights all of the calendar dates that have events t
    public void setEventDate(string[] allEvents)
    {
        int currentEvent = 0;
        string month = allEvents[0].Substring(0, 2);
        string day = allEvents[0].Substring(3, 2);
        string year = allEvents[0].Substring(6, 4);
        Debug.Log(month + day + year);
        while (int.Parse(month) < 8 && int.Parse(year) <= 2018)
        {
            currentEvent++;
            month = allEvents[currentEvent].Substring(0, 2);
            year = allEvents[currentEvent].Substring(6, 4);
            day = allEvents[currentEvent].Substring(3, 2);
        }
        int inc = 1;
        for (int i = 0; i < calendarContainer.transform.childCount; i = i + inc)
        {
            if (calendarContainer.transform.GetChild(i).GetComponent<dayInfo>() == null)
            { Debug.Log("spacer"); }
            else
            {
                if (calendarContainer.transform.GetChild(i).GetComponent<dayInfo>().Month == calendar[int.Parse(month) - 1].monthName)
                {
                    // if same day and year
                    if (calendarContainer.transform.GetChild(i).GetComponent<dayInfo>().dayNumber == int.Parse(day).ToString() && year == calendarContainer.transform.GetChild(i).GetComponent<dayInfo>().Year)
                    {
                        //Debug.Log("Setting day for event " + month + "/" + day);
                        calendarContainer.transform.GetChild(i).GetChild(0).GetComponent<Text>().color = blue;
                        calendarContainer.transform.GetChild(i).GetComponent<Image>().color = brightWhite;
                        calendarContainer.transform.GetChild(i).GetComponent<dayInfo>().marked = true;


                        currentEvent++;
                        if (currentEvent == allEvents.Length)
                        {
                            return;
                        }
                        while (allEvents[currentEvent].Substring(0, 2) == month && allEvents[currentEvent].Substring(3, 2) == day && allEvents[currentEvent].Substring(6, 4) == year)
                        {
                            currentEvent++;
                            if (currentEvent >= allEvents.Length)
                                return;
                        }
                        if (currentEvent == allEvents.Length)
                        {
                            return;
                        }
                        month = allEvents[currentEvent].Substring(0, 2);
                        day = allEvents[currentEvent].Substring(3, 2);
                        year = allEvents[currentEvent].Substring(6, 4);

                        //if the day is less the month is the same and the year is less or the same
                        //if (int.Parse(day) < int.Parse(calendarContainer.transform.GetChild(i).GetComponent<dayInfo>().dayNumber) && month == calendarContainer.transform.GetChild(i).GetComponent<dayInfo>().Month && int.Parse(year) <= int.Parse(calendarContainer.transform.GetChild(i).GetComponent<dayInfo>().Year))
                        //{
                        //    Debug.Log(month+day+year +" vs "+ calendarContainer.transform.GetChild(i).GetComponent<dayInfo>().Month + calendarContainer.transform.GetChild(i).GetComponent<dayInfo>().dayNumber + calendarContainer.transform.GetChild(i).GetComponent<dayInfo>().Year);
                        //    Debug.Log("inc is neg");
                        //    inc = -1;
                        //}
                        ////if the month and year is lower
                        //if (getMonthNumber(month) < getMonthNumber(calendarContainer.transform.GetChild(i).GetComponent<dayInfo>().Month) && int.Parse(year) <= int.Parse(calendarContainer.transform.GetChild(i).GetComponent<dayInfo>().Year))
                        //{
                        //    Debug.Log("inc is neg");
                        //    inc = -1;
                        //}
                        //else
                        //{
                        //    Debug.Log("inc is pos");
                        //    inc = 1;
                        //}
                    }

                }




            }
        }
    }
    

    public int getMonthNumber(string m)
    {
        switch (m)
        {
            case "January":
                return 1;
            case "February":
                return 2;
            case "March":
                return 3;
            case "April":
                return 4;
            case "May":
                return 5;
            case "June":
                return 6;
            case "July":
                return 7;
            case "August":
                return 8;
            case "September":
                return 9;
            case "October":
                return 10;
            case "November":
                return 11;
            case "December":
                return 12;
        }

        return 1;
    }

    //This is a webcall that handles calling in a list of events by date
    private string prefillurl = "https://connected-dev-214119.appspot.com/_ah/api/connected/v1/events/prefill/dates";
    private Jsonparser j;
    private string jsonString;
    timePrefill prefill;
    public void populateEvents()
    {
        j = GameObject.FindWithTag("Player").GetComponent<Jsonparser>();
        StartCoroutine(prefillLister());
    }
    IEnumerator prefillLister()
    {
        int retry = 0;
        FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        FirebaseUser user = auth.CurrentUser;


        Debug.Log("Prefilling events by date");
        //using (UnityWebRequest www = UnityWebRequest.Get("https://webhook.site/8e284497-5145-481d-8a18-0883dfd599e5"))
        using (UnityWebRequest www = UnityWebRequest.Get(prefillurl))
        {



            www.SetRequestHeader("Authorization", "Bearer " + j.token);
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
                if (www.responseCode.ToString() == "503" & retry < 3)
                {
                    Debug.Log("Trying again : get prefill");
                    populateEvents();
                }
            }
            else
            {
                Debug.Log(www.responseCode);
                byte[] results = www.downloadHandler.data;
                jsonString = "";
                jsonString = Encoding.UTF8.GetString(results);
                Debug.Log(jsonString);
                prefill = JsonUtility.FromJson<timePrefill>(jsonString);
                StartCoroutine(Populator());
            }
        };
    }
    private Event[] allEvents;
    private string getEventurl = "https://connected-dev-214119.appspot.com/_ah/api/connected/v1/events/";

    //this gets all of the events from the prefill lister
    IEnumerator Populator()
    {
        FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        FirebaseUser user = auth.CurrentUser;
        allEvents = new Event[prefill.events.Length];
        Debug.Log(allEvents.Length);
        Event e;
        for (int i = 0; i < prefill.events.Length; i++)
        {
            string[] trueURL = prefill.events[i].Split('_');
            //using (UnityWebRequest www = UnityWebRequest.Get("https://webhook.site/8e284497-5145-481d-8a18-0883dfd599e5"))
            using (UnityWebRequest www = UnityWebRequest.Get(getEventurl + trueURL[0]+"/"+trueURL[1]))
            {



                www.SetRequestHeader("Authorization", "Bearer " + j.token);
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
                    e = JsonUtility.FromJson<Event>(jsonString);

                    allEvents[i] = e;
                    Debug.Log(allEvents[i].e_orig_title);
                    if(i == prefill.events.Length - 1){
                        dates = new Event[allEvents.Length];
                        dates = allEvents;
                        setEventsInCalendar();
                        eventTrigger();
                    }
                }
            };
        }
    }


}