using DG.Tweening;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    private Vector3 InitialPosition;


    private Camera m_Camera;

    public static CameraController Instance;

    private Pumpkin m_WatchingPumpkin;

    private void Awake()
    {
        Instance = this;
        m_Camera = GetComponent<Camera>();
        InitialPosition = transform.localPosition;
    }

    public void Shake()
    {
        // Shake the camera using DoTween.
        transform.DOShakePosition(0.1f, 1, 1, 1)
            // When the shake is finished, reset the camera position.
            .OnComplete(() => transform.localPosition = InitialPosition);
    }

    public void GoToPortal(Portal p, Pumpkin toWatch)
    {
        transform.SetParent(null);
        transform.DOMove(p.CameraPosition.position, 1.5f);
        transform.DORotate(p.CameraPosition.position, 1.5f);
    }

    private void Update()
    {
        if(m_WatchingPumpkin == null)
        {
            return;
        }
        transform.LookAt(m_WatchingPumpkin.transform.position);
    }

}
