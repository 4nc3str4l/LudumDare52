using UnityEngine;

public class ShotgunShell : MonoBehaviour
{
    public Rigidbody Body;
    public Collider m_Collider;

    private MeshRenderer[] m_Renderers;

    private void Awake()
    {
        m_Renderers = GetComponentsInChildren<MeshRenderer>();
        m_Collider = GetComponentInChildren<Collider>();
    }

    public void Eject()
    {
        transform.SetParent(null);
        Body.isKinematic = false;
        Body.useGravity = true;
        Body.AddForce(Vector3.up * 200);
        Destroy(gameObject, 5);
    }

    public void Show()
    {
        m_Collider.enabled = true;
        foreach(MeshRenderer r in m_Renderers)
        {
            r.enabled = true;
        }
    }

    public void Hide()
    {
        m_Collider.enabled = false;
        foreach (MeshRenderer r in m_Renderers)
        {
            r.enabled = false;
        }
    }
}
