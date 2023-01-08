using UnityEngine;
using TMPro;

public class GameOverScene : MonoBehaviour
{
    public TMP_Text NightsSurvived;

    public void Start()
    {
        Cursor.lockState =CursorLockMode.None;
        Cursor.visible = true;
        NightsSurvived.text = PlayerPrefs.GetInt("Score").ToString();
    }
}
