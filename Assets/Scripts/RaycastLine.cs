using Unity.VisualScripting;
using UnityEngine;

public class RaycastLine : MonoBehaviour
{
    // The line renderer.
    public LineRenderer Line;

    private Vector3 m_StartPosition;
    private Vector3 m_EndPosition;
    private float m_Duration = 0;
    private float m_EndTime = 0;
    private float m_StartTime;

    private Vector3 m_Direction;
    private float m_Speed;
    private float m_DestroyTime;

    public void Init(Color color, Vector3 startPos, Vector3 endPos, float duration, float speed)
    {
        m_Speed = speed;
        m_Direction = (endPos - startPos).normalized;
        m_StartTime = Time.time;
        m_EndTime = m_StartTime + duration;
        m_Duration = duration;
        m_EndPosition = endPos;
        m_StartPosition = startPos;
        m_DestroyTime = Time.time + duration;

        Line.SetPosition(0, startPos);
        Line.SetPosition(1, endPos);
        Line.startColor = color;
        Line.endColor = color;
        Line.enabled = true;
    }

    private void Update()
    {
        
        if(Time.time > m_DestroyTime)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 pos = Line.GetPosition(0);
        Vector3 pos2 = Line.GetPosition(1);

        pos += m_Direction * m_Speed * Time.deltaTime;

        if(Vector3.Distance(m_StartPosition, pos) >= Vector3.Distance(m_StartPosition, pos2))
        {
            Destroy(gameObject);
        }

        Line.SetPosition(0, pos);

    }
}