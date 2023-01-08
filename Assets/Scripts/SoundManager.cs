using UnityEngine;
using TMPro;

public class SoundManager : MonoBehaviour
{
    public float Voulume = 1;
    public TMP_Text TxtVolume;

  

    public static SoundManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if(!PlayerPrefs.HasKey("volumne"))
        {
            PlayerPrefs.SetFloat("volumne", Voulume);
        }
        Voulume = PlayerPrefs.GetFloat("volumne");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Voulume += 0.05f;
            Voulume = Mathf.Min(1, Voulume);
            UpdateVolumeUI();
        }

        if (Input.GetKeyDown(KeyCode.L)) 
        {
            Voulume -= 0.05f;
            Voulume = Mathf.Max(0, Voulume);
            UpdateVolumeUI();
        }
    }

    private void UpdateVolumeUI()
    {
        TxtVolume.text = $"[K] Up / [L] Low Vol, Current: {Mathf.RoundToInt(Voulume * 100)}%";
    }
}
