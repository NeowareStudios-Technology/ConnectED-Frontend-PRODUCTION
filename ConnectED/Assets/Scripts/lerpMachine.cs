using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lerpMachine : MonoBehaviour {
    //this script controls the different views in the details page
    public Transform TrueCenter;
    public Transform DetailsCenter;
    public Transform TeamsCenter;
    public Transform UpdatesCenter;
    public Transform CurretCenter;
    public Vector3 DistanceToCover;
    public int current = 1;
    public float lerpValue = 0.0f;
    public bool Lerping = false;

    private void Start()
    {
        CurretCenter = DetailsCenter;
    }
    //based on which button is pressed the update function will make the gameobject lerp
    private void Update()
    {
        if(lerpValue > 1f)
        {
            lerpValue = 0f;
            Lerping = false;
            return;
        }
        if (Lerping == true)
        {
            lerpValue = lerpValue + .1f;
            if(current == 2)
                this.gameObject.transform.position = Vector3.Lerp(this.gameObject.transform.position, TrueCenter.position, lerpValue);
            if (current == 1)
                this.gameObject.transform.position = Vector3.Lerp(this.gameObject.transform.position, DetailsCenter.position, lerpValue);
            if(current == 3)
                this.gameObject.transform.position = Vector3.Lerp(this.gameObject.transform.position, UpdatesCenter.position, lerpValue);
        }
        else
        {
            Lerping = false;
            lerpValue = 0f;
        }
    }

    //when you press a button this script is called
    public void LerpTo(int i)
    {
        
    if (i == current)
        {
            return;
        }
    if(i == 1)
        {
            current = 1;
            DistanceToCover = DetailsCenter.position;
            CurretCenter = DetailsCenter;
            
            Lerping = true;
        }
    if(i == 2)
        {
            current = 2;
            DistanceToCover = TeamsCenter.position;
            CurretCenter = TeamsCenter;
            Lerping = true;
        }
    if(i == 3)
        {
            current = 3;
            DistanceToCover = UpdatesCenter.position;
            CurretCenter = UpdatesCenter;
            Lerping = true;
        }
    }
}
