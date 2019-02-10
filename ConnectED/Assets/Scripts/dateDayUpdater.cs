using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class dateDayUpdater : MonoBehaviour {
    //this script performs updates to the date dropdown menu. when you update the month update day is called, also if you update the year
    public Dropdown months;
    public Dropdown days;
    public Dropdown year;
    public List<string> thirtyOne = new List<string> {"1","2","3","4","5","6","7","8","9","10","11","12","13","14","15","16","17","18","19","20","21","22","23","24","25","26","27","28","29","30","31"};
    public List<string> thirty = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30"};
    public List<string> twentyNine = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29" };
    public List<string> twentyEight = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28"};
    public void updateDay()
    {
        days.ClearOptions();
        if(months.value == 0 || months.value == 2 || months.value == 4 || months.value == 6 || months.value == 7 || months.value == 9 || months.value == 11){
            days.AddOptions(thirtyOne);
            return;
        }
        //leap year
        if (months.value == 1)
        {
            //if year == 2020 or 2024, 2028...
            if (year.value + 2 % 4 == 0)
                days.AddOptions(twentyNine);
            else
                days.AddOptions(twentyEight);
        }
        else
            days.AddOptions(thirty);
    }

}
