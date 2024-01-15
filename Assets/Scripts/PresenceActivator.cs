using UnityEngine;

public class PresenceActivator : MonoBehaviour
{
    public GameController controller;

     IActivatable activatable;
    private void Start()
    {
        activatable = GetComponent<IActivatable>();
    }
    private void OnTriggerEnter(Collider other)
    {
       if (other.gameObject.CompareTag("Player") == true)
            controller.SetActive(activatable);
    }
}
