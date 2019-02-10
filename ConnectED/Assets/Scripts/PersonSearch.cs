using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonSearch : MonoBehaviour {

    public Search search;
    public InputField leader;
    //this sets the input field to accept information from the search bar
    public void setAsTarget(){
        search.leader = leader;
    }

}
