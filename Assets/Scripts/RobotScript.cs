using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RobotScript : MonoBehaviour {

    const string GREEN_BUTTON_FL = "joystick button 1";

    public Transform configManagerObject;
    private ConfigManager configManager;

    public Transform canvasObj;
    public bool uiCanBeUsed;

    public Transform cameraObj;
    private Camera camComponent;

    public Transform progressBar;
    private ProgressBarScript progressBarScript;

    public Transform navigationSystem;
    private PathfinderScript navSysPathfinder;

    public Transform actionManagerObj;
    private ActionManager actionManager;

    public Transform startPoint;

    public float speed; 

    public bool camFollowingRobot;

    public float actionTimeLeft;
    public bool doingAction;

    public Transform currentPoint;
    public bool moving;
    public Transform[] currentPath;
    public int currentPathIndex;
    public float interpolatedMagnitude;

    public Color plaqueColor;
    public Color stentColor;
    public Color tankFilledColor;
    public Color tankEmptyColor;

    public bool hasStent;
    public int valveCount;
    public bool hasPacemaker;
    public bool hasVein;
    public bool pickedUpVein;
    public bool hasBrainSample;
    public bool bloodPickup;
    public bool hasPlaque;

    public Transform veinIndicator;
    public Transform biopIndicator;

    public Transform tankIndicator;
    private SpriteRenderer tankSprite;

    public Transform plaqueIndicator;
    private SpriteRenderer plaqueSprite;

    private SerialPort port;

    private void UpdateCam() {

        if (camFollowingRobot) {

            camComponent.orthographicSize = 0.5f;
            cameraObj.position = transform.position + new Vector3(0, 0, -10);

        }
        else {

            camComponent.orthographicSize = 5.75f;
            cameraObj.position = new Vector3(0, 0, -10);

        }

    }

    public PointType GetPointType() {

        if (currentPoint != null) {

            return currentPoint.GetComponent<PointNodeScript>().pointType;

        }

        return PointType.NONE;

    }

    public bool ActionNotDone() {

        Transform foundObj = actionManager.GetObjectAtPointIfAny(currentPoint);
        PointType pointType = GetPointType();

        if (pointType == PointType.VALVE) {

            if (foundObj != null) {

                SpriteRenderer renderer = foundObj.GetComponent<SpriteRenderer>();

                if (valveCount > 0 && renderer.sprite != actionManager.valveReplacement) {

                    return true;

                }

            }

        }
        else if (pointType == PointType.STENT) {

            if (foundObj != null) {

                SpriteRenderer renderer = foundObj.GetComponent<SpriteRenderer>();

                if (hasStent) {

                    return true;

                }

            }

        }
        else if (pointType == PointType.HARVEST) {

            if (foundObj != null) {

                if (!hasVein && !pickedUpVein) {

                    return true;

                }

            }

        }
        else if (pointType == PointType.BIOPSY) {

            if (foundObj != null) {

                if (!hasBrainSample) {

                    return true;

                }

            }

        }
        else if (pointType == PointType.HEMCTRL) {

            if (foundObj != null) {

                if (!bloodPickup) {

                    return true;

                }

            }

        }
        else if (pointType == PointType.BYPASS) {

            if (foundObj != null) {

                if (hasVein) {

                    return true;

                }

            }

        }
        else if (pointType == PointType.PACEMAKER) {

            if (foundObj != null) {

                if (hasPacemaker) {

                    return true;

                }

            }

        }
        else if (pointType == PointType.END) {

            return true;

        }

        return false;

    }

    public void DoAction() {

        Transform foundObj = actionManager.GetObjectAtPointIfAny(currentPoint);
        PointType pointType = GetPointType();

        if (pointType == PointType.VALVE) {

            if (foundObj != null) {

                SpriteRenderer renderer = foundObj.GetComponent<SpriteRenderer>();

                if (valveCount > 0 && renderer.sprite != actionManager.valveReplacement) {

                    valveCount -= 1;
                    renderer.sprite = actionManager.valveReplacement;

                }

            }

        }
        else if (pointType == PointType.STENT) {

            if (foundObj != null) {

                SpriteRenderer renderer = foundObj.GetComponent<SpriteRenderer>();

                if (hasStent) {

                    hasPlaque = true;
                    hasStent = false;
                    renderer.sprite = actionManager.stent;

                }

            }

        }
        else if (pointType == PointType.HARVEST) {

            if (foundObj != null) {

                if (!hasVein && !pickedUpVein) {

                    hasVein = true;
                    pickedUpVein = true;
                    foundObj.gameObject.SetActive(false);

                }

            }

        }
        else if (pointType == PointType.BIOPSY) {

            if (foundObj != null) {

                if (!hasBrainSample) {

                    hasBrainSample = true;
                    foundObj.gameObject.SetActive(false);

                }

            }

        }
        else if (pointType == PointType.HEMCTRL) {

            if (foundObj != null) {

                if (!bloodPickup) {

                    bloodPickup = true;
                    foundObj.gameObject.SetActive(false);

                }

            }

        }
        else if (pointType == PointType.BYPASS) {

            if (foundObj != null) {

                if (hasVein) {

                    hasVein = false;
                    foundObj.gameObject.SetActive(true);

                }

            }

        }
        else if (pointType == PointType.PACEMAKER) {

            if (foundObj != null) {

                if (hasPacemaker) {

                    hasPacemaker = false;
                    foundObj.gameObject.SetActive(true);

                }

            }

        }
        else if (pointType == PointType.END) {

            if (port != null) {

                port.Open();
                port.Write("0");
                port.Close();

            }

            SceneManager.LoadScene("TutorialScene");            

        }

    }

    public void MoveTo(Transform point) {

        if (!moving && !doingAction) {

            if (point != currentPoint) {

                transform.position = currentPoint.position;

                currentPath = navSysPathfinder.BuildPath(currentPoint, point);
                interpolatedMagnitude = 0;
                currentPathIndex = 0;
                moving = true;

                currentPoint = null;

            }
            
        }

    }

    private void UpdateMovement() {

        if (moving) {

            Transform nodeA = currentPath[currentPathIndex];
            Transform nodeB = currentPath[currentPathIndex + 1];

            Vector3 offsetPos = nodeB.position - nodeA.position;
            float distance = offsetPos.magnitude;
                
            interpolatedMagnitude += speed / distance * Time.deltaTime; 

            transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(offsetPos.y, offsetPos.x));

            if (interpolatedMagnitude >= 1) {

                interpolatedMagnitude = 0;
                currentPathIndex++;

                if (currentPathIndex == currentPath.Length - 1) {

                    currentPoint = currentPath[currentPath.Length - 1];
                    transform.position = currentPoint.position;

                    moving = false;
                    currentPath = null;
                    currentPathIndex = 0;

                }

            }
            else {

                Vector3 interpolatedPos = offsetPos * interpolatedMagnitude;
                Vector3 actualPos = nodeA.position + interpolatedPos;

                transform.position = actualPos;

            }

        }

    }

    private void UpdateActions() {

        if (!moving && doingAction) {

            progressBar.position = transform.position + new Vector3(0, 0, -0.1f);
            progressBarScript.progress = actionTimeLeft / 5;

            actionTimeLeft -= Time.deltaTime;

            if (actionTimeLeft <= 0) {

                progressBar.position = new Vector3(100, 100, 100);

                actionTimeLeft = 0;
                doingAction = false;

                DoAction();

            }

            if (GetPointType() == PointType.END) {

                DoAction();

            }
            
        }

    }

    private void UpdateAppearance() {

        veinIndicator.gameObject.SetActive(hasVein);
        biopIndicator.gameObject.SetActive(hasBrainSample);
        plaqueIndicator.gameObject.SetActive(hasStent || hasPlaque);

        if (hasStent) {

            plaqueSprite.color = stentColor;

        }
        else if (hasPlaque) {

            plaqueSprite.color = plaqueColor;

        }

        if (bloodPickup) {

            tankSprite.color = tankFilledColor;

        }
        else {

            tankSprite.color = tankEmptyColor;

        }

    }

    public void DisableUITips() {

        if (uiCanBeUsed) {

            for (int i = 0; i < canvasObj.childCount; i++) {

                Transform child = canvasObj.GetChild(i);
                child.gameObject.SetActive(false);

            }

            uiCanBeUsed = false;

        }

    }

    public void FlipUITip() {

        if (uiCanBeUsed) {

            Transform pointUI = currentPoint.GetComponent<PointNodeScript>().uiBinding;
            
            if (pointUI != null) {

                pointUI.gameObject.SetActive(!pointUI.gameObject.activeSelf);

            }

        }

    }

    void Start() {

        configManager = configManagerObject.GetComponent<ConfigManager>();

        camComponent = cameraObj.GetComponent<Camera>();
        navSysPathfinder = navigationSystem.GetComponent<PathfinderScript>();
        actionManager = actionManagerObj.GetComponent<ActionManager>();
        progressBarScript = progressBar.GetComponent<ProgressBarScript>();

        tankSprite = tankIndicator.GetComponent<SpriteRenderer>();
        plaqueSprite = plaqueIndicator.GetComponent<SpriteRenderer>();

        currentPoint = startPoint;
        transform.position = startPoint.position;

        if (configManager.comPort != "") {

            port = new SerialPort(configManager.comPort, configManager.baudRate);

            port.Open();
            port.Write("1");
            port.Close();

        }
        
    }

    void Update() {

        UpdateActions();
        UpdateMovement();
        UpdateCam();
        UpdateAppearance();

        if (!moving) {

            uiCanBeUsed = true;

            if (Input.GetKeyDown(GREEN_BUTTON_FL) || Input.GetKeyDown(KeyCode.A)) {

                if (!doingAction && ActionNotDone()) {

                    doingAction = true;
                    actionTimeLeft = 5;
                    progressBarScript.progress = 5;

                }

            }

        }      

    }

}
