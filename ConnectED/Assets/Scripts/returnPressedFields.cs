using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class returnPressedFields : MonoBehaviour {
    //this returns all elements pressed in multiple groups
    public List<string> fields;
    private GameObject a;
    public string[] returnFields()
    {
        for (int i = 0; i < this.gameObject.transform.childCount; i++)
        {
            for (int j = 0; j < this.gameObject.transform.GetChild(i).childCount; j++)
            {
                a = this.gameObject.transform.GetChild(i).GetChild(j).gameObject;
                //check if each button was pressed, and if it is, add its text
                if (a.GetComponent<spriteSwitcher>().pressed)
                    fields.Add(a.transform.GetChild(0).GetComponent<Text>().text);
            }
        }

        return fields.ToArray();
    }
}
