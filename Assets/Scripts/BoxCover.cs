using Assets.Scripts.MonsterAI;
using DG.Tweening;
using UnityEngine;

public class BoxCover : MonoBehaviour
{

    public Vector3 InitialPosition;
    public Vector3 InitialRotation;

    public Vector3 OpenPosition;
    public Vector3 OpenRotation;

    private PumpinBox m_Box;

    private bool m_IsOpen = true;

    private void Awake()
    {
        m_Box = GetComponentInParent<PumpinBox>();
    }

    private void Start()
    {
        Close();
    }

    private void Update()
    {
        if (m_Box.IsFull)
        {
            Close();
            return;
        }

        
        Pumpkin closest = SceneInventory.Instance.GetClosestStorablePumpkin(transform.position);
        float distance = Mathf.Infinity;
        if (closest != null)
        {
            distance = Vector3.Distance(transform.position, closest.transform.position);
        }
        
        if (distance > 10)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    private void Close()
    {
        if (!m_IsOpen)
        {
            return;
        }

        transform.DOLocalMove(InitialPosition, 0.3f);
        transform.DOLocalRotate(InitialRotation, 0.3f);

        m_IsOpen = false;
    }

    private void Open()
    {
        if (m_IsOpen)
        {
            return;
        }

        transform.DOLocalMove(OpenPosition, 0.3f);
        transform.DOLocalRotate(OpenRotation, 0.3f);

        m_IsOpen = true;
    }
}
