using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class PlayerControls : MonoBehaviour
{
	public float speed = 0.05f;
    public float rotation = 0;
    [SerializeField] GameObject _Head;

    // Start is called before the first frame update
    void Start()
    {
		Debug.Log("Start is called for "+name);
    }

    // Update is called once per frame
    void Update()
    {
        //Destroy(gameObject);
        float yaw = Input.GetAxis("Mouse X");
        float pitch = Input.GetAxis("Mouse Y");
        transform.Rotate(0, yaw, 0);

        _Head.transform.Rotate(pitch, 0, 0);

        //Vector3 moveVector = new Vector3(Input.GetAxis("Horizontal") * speed, 0, Input.GetAxis("Vertical") * speed);

        float strafe = Input.GetAxis("Horizontal") * speed;
        float translation = Input.GetAxis("Vertical") * speed;

        transform.Translate(-strafe, 0, -translation);
    }
}
