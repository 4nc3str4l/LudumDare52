using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressSlider : MonoBehaviour, IProgressSlider
{

    public Image SlidingImage;
    public TMP_Text Text;

    public void SetFilling(float perc)
    {
        if(perc < 0)
        {
            perc = 0;
        }

        if(perc > 1)
        {
            perc = 1;
        }

        Text.text = Mathf.Round(perc * 100).ToString() + "%";
        SlidingImage.fillAmount = perc;
    }
}
