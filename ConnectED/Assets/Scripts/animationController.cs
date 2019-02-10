using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationController : MonoBehaviour {
    public GameObject Details;
	public void hideDetails()
    {
        Details.GetComponent<Animator>().SetBool("Show", false);
    }
    public void showDetails()
    {
        Details.GetComponent<Animator>().SetBool("Show", true);
    }
}
