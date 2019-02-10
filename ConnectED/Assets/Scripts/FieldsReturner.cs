using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FieldsReturner : MonoBehaviour {
    //this returns the fields when you click on one of them
    private FieldsController f;
    public void onPress()
    {
       
            this.GetComponent<spriteSwitcher>().turnOn();
			f = transform.parent.parent.parent.parent.GetComponent<FieldsController>();
            f.returnField(this.gameObject);

    }
}
