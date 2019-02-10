using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class loadingRotation : MonoBehaviour {
    //this spins the loading symbol
    public Transform transform;
    // Update is called once per frame
    public bool right = true;
    public float z;
    public int speed = 10;
	void Update () {
        if (right)
            z += Time.deltaTime * speed;
        else
            z -= Time.deltaTime * speed;

        transform.rotation = Quaternion.Euler(0, 0, z);
    }
}
