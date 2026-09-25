using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("ALGO entró al trigger: " + other.gameObject.name);
        PlayerMovement player = other.GetComponent<PlayerMovement>();

        if (player != null)
        {
            player.TriggerWin();
        }
    }
}