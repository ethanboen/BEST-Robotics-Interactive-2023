using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBarScript : MonoBehaviour {

    public float progress;
    private Transform progObj;

    void Start() {
        
        progObj = transform.Find("Progress");

    }


    void Update() {
        
        progress = Mathf.Clamp(progress, 0, 1);
        
        progObj.localScale = new Vector3(1 - progress, 1, 1);

    }
}
