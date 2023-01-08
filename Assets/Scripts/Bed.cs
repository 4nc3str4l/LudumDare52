using Assets.Scripts;
using UnityEngine;

public class Bed : MonoBehaviour
{

    public GameObject Arrow;
    public GameObject Light;

    private void Awake()
    {
        Arrow.SetActive(false);
        Light.SetActive(false);
    }

    private void OnEnable()
    {
        GameController.OnStatusChanged += GameController_OnStatusChanged;
    }

    private void OnDisable()
    {
        GameController.OnStatusChanged -= GameController_OnStatusChanged;
    }

    private void GameController_OnStatusChanged(GameStatus obj)
    {
        if(obj == GameStatus.AT_HOME_AFTER_OBJECTIVE)
        {
            Arrow.SetActive(true);
            Light.SetActive(true);
        }
        else
        {
            Arrow.SetActive(false);
            Light.SetActive(false);
        }
    }



    public void GoToSleep()
    {
        GameController.Instance.ChangeState(GameStatus.SLEEPING);
    }
}
