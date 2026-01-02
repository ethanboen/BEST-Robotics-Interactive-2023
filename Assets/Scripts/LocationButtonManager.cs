using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationButtonManager : MonoBehaviour {

    const string JOYSTICK_AXIS_X = "Joystick X";
    const string JOYSTICK_AXIS_Y = "Joystick Y";

    const string RED_BUTTON_FR = "joystick button 0";
    const string BODY_BUTTON_BR = "joystick button 2";
    const string YELLOW_BUTTON_BL = "joystick button 3";

    public bool joystickConnected;

    public bool travelMapOpen;

    public int selectedColumn = 0;
    public int selectedRow = 0;

    private float cooldown = 0;

    public Transform robot;
    private RobotScript robotScript;

    public Transform mapObj;
    public Transform currentButton;

    public Transform[] buttonsLeft;
    public Transform[] buttonsRight;

    public Color colorUnSelected;
    public Color colorSelected;

    private void SetInput(int amountX, int amountY) {

        if (amountX != 0) {

            if (selectedColumn == 0 && amountX == 1) {
 
                selectedColumn = 1;

            }
            else if (selectedColumn == 1 && amountX == -1) {

                selectedColumn = 0;

            }

        }

        if (amountY != 0) {

            if (cooldown == 0) {

                selectedRow = Mathf.Clamp(selectedRow + amountY, 0, buttonsLeft.Length - 1);
                cooldown = 0.5f;

            }
            else {

                cooldown -= Time.deltaTime;

                if (cooldown <= 0) {

                    cooldown = 0;

                }

            }

        }
        else {

            cooldown = 0;

        }

    }

    private void CheckArrowKeys() {

        int amountX = 0;
        int amountY = 0;

        if (Input.GetKeyDown(KeyCode.UpArrow)) {

            amountY = -1;

        }

        if (Input.GetKeyDown(KeyCode.DownArrow)) {

            amountY = 1;

        }

        if (Input.GetKeyDown(KeyCode.LeftArrow)) {

            amountX = -1;

        }

        if (Input.GetKeyDown(KeyCode.RightArrow)) {

            amountX = 1;

        }

        SetInput(amountX, amountY);

    }

    private void CheckJoystick(string axisX, string axisY) {

        int amountX = (int) Mathf.Round(Input.GetAxis(axisX));
        int amountY = (int) Mathf.Round(Input.GetAxis(axisY));

        SetInput(amountX, amountY);

    }

    private void UpdateButtonSelected() {

        for (int i = 0; i < buttonsLeft.Length; i++) {

            Transform btn = buttonsLeft[i];
            SpriteRenderer spriteRenderer = btn.GetComponent<SpriteRenderer>();
            LocationButtonScript locationButtonScript = btn.GetComponent<LocationButtonScript>();

            if (selectedRow == i && selectedColumn == 0) {

                currentButton = btn;
                spriteRenderer.color = colorSelected;
                locationButtonScript.SelectPoint(true);

            }
            else {

                spriteRenderer.color = colorUnSelected;
                locationButtonScript.SelectPoint(false);

            }

        }

        for (int i = 0; i < buttonsRight.Length; i++) {

            Transform btn = buttonsRight[i];
            SpriteRenderer spriteRenderer = btn.GetComponent<SpriteRenderer>();
            LocationButtonScript locationButtonScript = btn.GetComponent<LocationButtonScript>();

            if (selectedRow == i && selectedColumn == 1) {

                currentButton = btn;
                spriteRenderer.color = colorSelected;
                locationButtonScript.SelectPoint(true);

            }
            else {

                spriteRenderer.color = colorUnSelected;
                locationButtonScript.SelectPoint(false);

            }

        }

    }

    private void SetTravelButtonsActive(bool value) {

        for (int i = 0; i < transform.childCount; i++) {

            Transform child = transform.GetChild(i);

            child.gameObject.SetActive(value);

        }

    }

    private void UpdateUI() {

        SetTravelButtonsActive(travelMapOpen);
        mapObj.gameObject.SetActive(travelMapOpen);

        

        // robotScript.camFollowingRobot = !travelMapOpen;

    }

    private void UpdateCam() {

        if (travelMapOpen) {

            robotScript.camFollowingRobot = false;

        }
        else {

            robotScript.camFollowingRobot = true;

        }

    }

    void Start() {
        
        robotScript = robot.GetComponent<RobotScript>();

    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.J)) {

            joystickConnected = !joystickConnected;

        }

        if (travelMapOpen) {

            if (joystickConnected) {

                CheckJoystick(JOYSTICK_AXIS_X, JOYSTICK_AXIS_Y);

            }
            else {

                CheckArrowKeys();

            }

            UpdateButtonSelected();
            robotScript.DisableUITips();

        }

        if (Input.GetKeyDown(RED_BUTTON_FR) || Input.GetKeyDown(KeyCode.S)) {

            if (travelMapOpen) {

                travelMapOpen = false;
                UpdateUI();

                LocationButtonScript locationButtonScript = currentButton.GetComponent<LocationButtonScript>();
                robotScript.MoveTo(locationButtonScript.point);

            }

        }

        if (Input.GetKeyDown(BODY_BUTTON_BR) || Input.GetKeyDown(KeyCode.W)) {

            if (!robotScript.moving && !robotScript.doingAction) {

                travelMapOpen = !travelMapOpen;

            }
            else {

                travelMapOpen = false;

            }

            UpdateUI();

        }

        if (Input.GetKeyDown(YELLOW_BUTTON_BL) || Input.GetKeyDown(KeyCode.Q)) {

            if (!travelMapOpen && !robotScript.moving) {

                robotScript.FlipUITip();

            }            

        }

        UpdateCam();

    }

}
