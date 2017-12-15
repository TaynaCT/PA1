using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    public float Speed;

	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    void FixedUpdate() {

        if (Input.GetKey(KeyCode.W))        
            this.transform.Translate(new Vector2(0, Speed * Time.fixedDeltaTime));

        if (Input.GetKey(KeyCode.S))
            this.transform.Translate(new Vector2(0, -Speed * Time.fixedDeltaTime));

        if (Input.GetKey(KeyCode.A))
            this.transform.Translate(new Vector2(-Speed * Time.fixedDeltaTime, 0));

        if (Input.GetKey(KeyCode.D))
            this.transform.Translate(new Vector2(Speed * Time.fixedDeltaTime, 0));
    }
}
