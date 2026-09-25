using UnityEngine;

public class FinishLine1 : MonoBehaviour
{
    [SerializeField]
    GameManager manager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            manager.StopRunning();
        }
    }

}
