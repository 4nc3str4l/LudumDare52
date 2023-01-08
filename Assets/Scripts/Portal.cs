using DG.Tweening;
using System;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Transform EntryPoint;
    public Transform CameraPosition;

    private Vector3 m_InitialScale;

    public static event Action<Portal, Pumpkin> OnPumpkinStolen;

    private void Awake()
    {
        m_InitialScale = transform.localScale;
    }


    public void FeedbackOnPumpkinReleased(Pumpkin p)
    {
        transform.DOShakeScale(2).OnComplete(() =>
        {
            transform.localScale = m_InitialScale;
            p.transform.DOMove(transform.position + Vector3.up * 100, 5);
        });

        OnPumpkinStolen?.Invoke(this, p);
    }
}
