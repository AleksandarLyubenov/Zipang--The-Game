using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;

public class GameSettings : MonoBehaviour
{
    [Header("Drag in the Player GameObject")]
    [SerializeField]
    private GameObject FPSPlayer;
        

    [Header("Drag in the Menu Camera")]
    [SerializeField]
    private GameObject menuCamera;

    [Header("Check if CineMachine was used")]
    [SerializeField]
    private bool isCinemachine = true;

    [Header("Drag in the Cinematic Camera")]
    [SerializeField]
    private GameObject CinematicCamera;

    [SerializeField]
    private PlayableDirector pDirector;

    [Header("If you did NOT use CineMachine")]
    [SerializeField]
    private GameObject animatedCamera;

    private bool isMenuOpen = true;
    private bool menuToggle = false;

    [SerializeField]
    private List<GameObject> buttons = new List<GameObject>();

    private Vector3 playerLocation;

    private Cinemachine.CinemachineVirtualCamera[] vCams;
    
    private void OnEnable()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        vCams = GameObject.FindObjectsOfType<Cinemachine.CinemachineVirtualCamera>();

        foreach (var cam in vCams)
        {
            cam.gameObject.SetActive(false);
        }

        if(FPSPlayer != null)
        {
            FPSPlayer.SetActive(false);
        }
        
        if(CinematicCamera != null)
        {
            CinematicCamera.SetActive(false);
        }

        if(menuCamera != null)
        {
            menuCamera.SetActive(true);
        }

        if(animatedCamera != null)
        {
            animatedCamera.SetActive(false);
        }

        playerLocation = FPSPlayer.transform.position;
        

        Cursor.lockState = CursorLockMode.Confined;

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    public void StartPlayer()
    {
        if(FPSPlayer != null)
        {
            Cursor.lockState = CursorLockMode.Locked;
            if (CinematicCamera != null)
            {
                if (CinematicCamera.activeSelf)
                {
                    CinematicCamera.SetActive(false);
                }
            }
            if (!FPSPlayer.activeSelf)
            {
                FPSPlayer.SetActive(true);
            }
            else
            {
                FPSPlayer.GetComponent<CharacterController>().enabled = false;
                FPSPlayer.transform.position = playerLocation;
                FPSPlayer.GetComponent<CharacterController>().enabled = true;
                
            }
            if(menuCamera != null)
            {
                menuCamera.SetActive(false);
            }
            foreach (GameObject g in buttons)
            {
                g.SetActive(false);
            }
            if (animatedCamera != null)
            {
                animatedCamera.SetActive(false);
                ToggleMenu();
                Cursor.lockState = CursorLockMode.Locked;
            }

            isMenuOpen = false;


            foreach (var cam in vCams)
            {
                cam.gameObject.SetActive(false);
            }
         
        }
    }

    public void StartCinematic()
    {
        if (isCinemachine)
        {
            if (CinematicCamera != null)
            {
                Cursor.lockState = CursorLockMode.Locked;
                if (FPSPlayer.activeSelf)
                {
                    FPSPlayer.SetActive(false);
                }
                
                foreach (var cam in vCams)
                {
                    cam.gameObject.SetActive(true);
                }
                
                CinematicCamera.SetActive(true);
                if (pDirector != null)
                {
                    pDirector.Stop();
                    pDirector.Play();
                }

                foreach (GameObject g in buttons)
                {
                    g.SetActive(false);
                }

                isMenuOpen = false;
            }
        }
        else
        {
            if (animatedCamera != null)
            {
                animatedCamera.SetActive(true);
                ToggleMenu();
                Cursor.lockState = CursorLockMode.Locked;
                if (menuCamera != null)
                {
                    menuCamera.SetActive(false);
                }
                if (FPSPlayer.activeSelf)
                {
                    FPSPlayer.SetActive(false);
                }
            }
        }
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    private void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;
        if(isMenuOpen)
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
        foreach (GameObject g in buttons)
        {
            g.SetActive(isMenuOpen);
        }
        
    }

}
