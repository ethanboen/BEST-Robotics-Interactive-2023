using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationButtonScript : MonoBehaviour {

    public Transform point;

    public Sprite defaultTexture;
    public Sprite selectedTexture;

    private Transform pointObj;
    private SpriteRenderer pointRenderer;

    public void SelectPoint(bool selected) {

        if (selected) {

            pointRenderer.sprite = selectedTexture;
        }
        else {

            pointRenderer.sprite = defaultTexture;

        }

    }

    void Start() {
        
        pointObj = transform.Find("PointDisplay");
        pointRenderer = pointObj.GetComponent<SpriteRenderer>();

        pointObj.position = point.position + new Vector3(0, 0, -0.1f);
        pointRenderer.sprite = defaultTexture;

    }

}
