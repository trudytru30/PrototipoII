using UnityEngine;

public class WinScreen : MonoBehaviour
{
    public GameObject panelVictoria; 

    void Start()
    {
       
        if (panelVictoria != null) panelVictoria.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (panelVictoria != null)
            {
                panelVictoria.SetActive(true); 
                Time.timeScale = 0f;          
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}
