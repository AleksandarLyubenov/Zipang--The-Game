using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
	public Vector3 offset;
	public Transform target;

	[Range(-1,3)]
	public float scale = 1;

    // Start is called before the first frame update
    void Start()
    {
		Debug.Log("Calculating spaceship offset...");
		offset = transform.position - target.position;
    }

    // Update is called once per frame
    void Update()
    {
		transform.position = target.position + offset * scale; 
    }
}
