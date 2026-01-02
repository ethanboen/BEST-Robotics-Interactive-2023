using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour {

    public Sprite valveReplacement;
    public Sprite stent;

    public ObjectPointBinding[] pointBindings;
    
    public Transform GetObjectAtPointIfAny(Transform point) {

        foreach (ObjectPointBinding pointBinding in pointBindings) {

            if (pointBinding.pointToBind == point) {

                return pointBinding.objToBind;

            }

        }

        return null;

    }

    void Start() {
        
    }

    void Update() {
        
    }
}
