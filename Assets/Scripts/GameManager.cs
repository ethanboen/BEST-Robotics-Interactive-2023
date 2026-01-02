using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    const string INJECT_BUTTON = "joystick button 4";

    public string sceneName;
    public float injHoldTime;

    void Start() {
        
        sceneName = SceneManager.GetActiveScene().name;

    }

    void Update() {

        if (Input.GetKeyDown(INJECT_BUTTON) || Input.GetKeyDown(KeyCode.I)) {

            if (sceneName == "TutorialScene") {

                SceneManager.LoadScene("SampleScene");

            }

        }

        if (Input.GetKey(INJECT_BUTTON) || Input.GetKey(KeyCode.I)) {

            if (sceneName == "SampleScene") {

                injHoldTime += Time.deltaTime;

            }

        }

        if (Input.GetKeyUp(INJECT_BUTTON) || Input.GetKeyUp(KeyCode.I)) {

            if (sceneName == "SampleScene") {

                if (injHoldTime >= 3) {

                    SceneManager.LoadScene("TutorialScene");

                }
                else {

                    injHoldTime = 0;

                }

            }

        }

    }

}
