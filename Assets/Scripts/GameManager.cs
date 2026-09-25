using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    PlayerMovement Player;

    public void StartRunning()
    {
        Player.StartRunning();
    }

    public void StopRunning()
    {
        Player.StopRunning();
    }


}
