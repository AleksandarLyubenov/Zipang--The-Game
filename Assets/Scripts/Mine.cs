using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
	public Transform target;
	public float detectionRange;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		float distance = (target.position - transform.position).magnitude;
		//Debug.Log("Distance from mine to player: " + distance);
		if (distance<detectionRange) {
			Destroy(gameObject);
			Debug.Log("KABOOM!");

			target.GetComponent<Health>().TakeDamage(4);
		}
    }
}
