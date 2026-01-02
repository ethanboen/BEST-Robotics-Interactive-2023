using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ControlsManager : MonoBehaviour
{

    public Transform backgroundObject;
    private SpriteRenderer backgroundRenderer;
    public Sprite showOrgans;
    public Sprite hideOrgans;

    public Transform navigationSystem;
    private PathfinderScript pathfinderScript;

    const string JOYSTICK_AXIS_X = "Joystick X";
    const string JOYSTICK_AXIS_Y = "Joystick Y";

    const string RED_BUTTON_FR = "joystick button 0";
    const string GREEN_BUTTON_FL = "joystick button 1";
    const string BODY_BUTTON_BR = "joystick button 2";
    const string YELLOW_BUTTON_BL = "joystick button 3";
    const string INJECT_BUTTON = "joystick button 4";
    
    private void UpdateMovement(string axisX, string axisY) {

        float amountX = Mathf.Round(Input.GetAxis(axisX));
        float amountY = -Mathf.Round(Input.GetAxis(axisY));

    }

    void Start() {

        pathfinderScript = navigationSystem.GetComponent<PathfinderScript>();

        backgroundRenderer = backgroundObject.GetComponent<SpriteRenderer>();

    }

    void Update() {

        if (Input.GetKeyDown(RED_BUTTON_FR)) {

            Debug.Log("Red Button");

        }

        if (Input.GetKeyDown(GREEN_BUTTON_FL)) {

            Debug.Log("Green Button");

        }

        if (Input.GetKeyDown(BODY_BUTTON_BR)) {

            if (backgroundRenderer.sprite == showOrgans) {

                backgroundRenderer.sprite = hideOrgans;
                
            }
            else {

                backgroundRenderer.sprite = showOrgans;

            }

        }

        if (Input.GetKeyDown(YELLOW_BUTTON_BL)) {

            Debug.Log("Yellow Button");

        }

        if (Input.GetKeyDown(INJECT_BUTTON)) {

            Debug.Log("Inject Button");

        }

    }
}
