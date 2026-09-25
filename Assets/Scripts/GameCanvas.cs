using UnityEngine;
using TMPro;
using System.Collections;

public class GameCanvas : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI countdown;
    [SerializeField]
    GameObject countdownPanel;
    [SerializeField]
    GameManager manager;
    [SerializeField]
    GameObject mainMenuPanel;

    public void StartRace()
    {
        mainMenuPanel.SetActive(false);
        countdownPanel.SetActive(true);

        StartCoroutine(CountDownRoutine());
    }

    IEnumerator CountDownRoutine()
    {        
        int count = 3;

        while (count > 0)
        {
            countdown.text = count.ToString();

            yield return new WaitForSeconds(1);
            count--;
        }

        countdown.text = "GO!";
        yield return new WaitForSeconds(1);

        countdownPanel.SetActive(false);

        yield return new WaitForSeconds(0.5f);
        manager.StartRunning();
    }

}
