using DG.Tweening;
using System;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool IsOpen = false;

    public static event Action OnDoorOpen;
    public static event Action OnDoorClosed;

    public static Door Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void Open()
    {
        if (IsOpen)
        {
            return;
        }
        IsOpen = true;
        transform.DORotate(new Vector3(0, -90, 0), 1);
        OnDoorOpen?.Invoke();
        JukeBox.Instance.PlaySound(JukeBox.Instance.OpenDoor, 0.7f);
    }

    public void Close()
    {
        if (!IsOpen)
        {
            return;
        }
        IsOpen = false;
        transform.DORotate(new Vector3(0, 0, 0), 1);
        OnDoorClosed?.Invoke();
        JukeBox.Instance.PlaySound(JukeBox.Instance.OpenDoor, 0.7f);
    }
}
