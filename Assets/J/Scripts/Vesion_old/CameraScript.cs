using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraScript : MonoBehaviour {

    [Header("Camera shake parameter")]
    public float CameraShakeDuration;
    public float CameraShakeStrenth;


    Camera _camera;
	// Use this for initialization
	void Start () {
        _camera = this.GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
       
    }
    public void Shake() {
        _camera.transform.DOShakePosition(CameraShakeDuration, CameraShakeStrenth);
        
    }
}
