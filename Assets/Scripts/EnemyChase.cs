using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    public Transform Player;
    public PlayerControls PlayerControls;
    int MoveSpeed = 4;
    int MaxDist = 15;
    int MinDist = 1;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start is called for " + name);
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(PlayerControls.transform.position);

        if (Vector3.Distance(transform.position, PlayerControls.transform.position) >= MinDist)
        {
            transform.position += transform.forward * MoveSpeed * Time.deltaTime;

            if (Vector3.Distance(transform.position, PlayerControls.transform.position) <= MaxDist)
            {
                Debug.Log("GAME OVER!");
                return;
            }

        }
    }
}