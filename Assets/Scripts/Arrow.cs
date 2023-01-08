using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float Amplitude = 0.5f;
    public float Frequency = 1f;
    public float Distance = 0.5f;

    public Transform Target;

    private Vector3 m_InitialPosition;

    private MeshRenderer m_Renderer;

    private bool m_Visible = true;

    private void Awake()
    {
        m_Renderer = GetComponent<MeshRenderer>();
        m_InitialPosition = transform.position;
       
        if(Target != null)
        {
            transform.SetParent(null);
        }
    }

    private void Update()
    {
        if(Target == null)
        {
            transform.position = m_InitialPosition + new Vector3(0f, Mathf.Sin(Time.time * Frequency) * Amplitude, 0f);
        }
        else
        {
            transform.position = Target.transform.position + Vector3.up * Distance + new Vector3(0f, Mathf.Sin(Time.time * Frequency) * Amplitude, 0f);
        }
    }

    public void SetVisible(bool visible)
    {
        m_Visible = visible;
        m_Renderer.enabled = m_Visible;
    }
}
