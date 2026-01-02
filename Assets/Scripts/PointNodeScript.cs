using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PointType {

    VALVE,
    HARVEST,
    BIOPSY,
    BYPASS,
    STENT,
    PACEMAKER,
    HEMCTRL,
    END,
    START,
    NONE

}

public class PointNodeScript : MonoBehaviour {

    public string pointLocationName;
    public Transform parentPath;
    public PointType pointType;
    public Transform uiBinding;

}
