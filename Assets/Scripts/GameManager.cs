using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    PlayerMovement Player;

    public void StartRunning()
    {
        Player.StartRace();
    }

    public void StopRunning()
    {
        Player.TriggerWin();
    }


}
