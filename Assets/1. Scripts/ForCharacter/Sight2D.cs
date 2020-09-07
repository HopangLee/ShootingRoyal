using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 시야각 구현 스크립트
public class Sight2D : MonoBehaviour
{
    [SerializeField]
    private bool m_bDebugMode = false;

    [Header("View Config")]
    [Range(0f, 360f)]
    [SerializeField]
    private float m_horizontalViewAngle = 360f;

    [SerializeField]
    private float m_viewRadius = 1f;
    [Range(-180f, 180f)]
    [SerializeField]
    private float m_viewRotateZ = 0f;

    [SerializeField]
    private LayerMask m_viewTargetMask;
    [SerializeField]
    private LayerMask m_viewObstacleMask;

    private List<Collider2D> hitedTargetContainer = new List<Collider2D>();
    private List<Collider2D> unhitedTargetContainer = new List<Collider2D>();

    private float m_horizontalViewHalfAngle = 0f;

    private void Awake()
    {
        m_horizontalViewHalfAngle = m_horizontalViewAngle * 0.5f;
    }

    // 입력한 -180~180의 값을 Up Vector 기준 Local Direction으로 변환시켜줌.
    private Vector3 AngleToDirZ(float angleInDegree)
    {
        float radian = (angleInDegree - transform.eulerAngles.z) * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radian), Mathf.Cos(radian), 0f);
    }

    private void OnDrawGizmos()
    {
        if (m_bDebugMode)
        {
            m_horizontalViewHalfAngle = m_horizontalViewAngle * 0.5f;

            Vector3 originPos = transform.position;

            Gizmos.DrawWireSphere(originPos, m_viewRadius);

            Vector3 horizontalRightDir = AngleToDirZ(-m_horizontalViewHalfAngle + m_viewRotateZ);
            Vector3 horizontalLeftDir = AngleToDirZ(m_horizontalViewHalfAngle + m_viewRotateZ);
            Vector3 lookDir = AngleToDirZ(m_viewRotateZ);

            Debug.DrawRay(originPos, horizontalLeftDir * m_viewRadius, Color.cyan);
            Debug.DrawRay(originPos, lookDir * m_viewRadius, Color.green);
            Debug.DrawRay(originPos, horizontalRightDir * m_viewRadius, Color.cyan);

            FindViewTargets();
        }
    }

    public List<Collider2D>[] FindViewTargets()
    {
        hitedTargetContainer.Clear();
        unhitedTargetContainer.Clear();

        Vector2 originPos = this.transform.position;
        Collider2D[] hitedTargets = Physics2D.OverlapCircleAll(originPos, m_viewRadius, m_viewTargetMask);

        foreach(Collider2D hitedTarget in hitedTargets)
        {
            Vector2 targetPos = hitedTarget.transform.position;
            Vector2 dir = (targetPos - originPos).normalized;
            Vector2 lookDir = AngleToDirZ(m_viewRotateZ);

            // float angle = Vector3.Angle(lookDir, dir)
            // 아래 두 줄은 위의 코드와 동일하게 동작함. 내부 구현도 동일
            float dot = Vector2.Dot(lookDir, dir);
            float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

            if(angle <= m_horizontalViewHalfAngle)
            {
                RaycastHit2D rayHitedTarget = Physics2D.Raycast(originPos, dir.normalized, m_viewRadius, m_viewObstacleMask);
                if (rayHitedTarget)
                {
                    unhitedTargetContainer.Add(hitedTarget);

                    if (m_bDebugMode)
                        Debug.DrawLine(originPos, rayHitedTarget.point, Color.yellow);
                }
                else
                {
                    hitedTargetContainer.Add(hitedTarget);

                    if (m_bDebugMode)
                        Debug.DrawLine(originPos, targetPos, Color.red);
                }
            }
        }

        List<Collider2D>[] result = new List<Collider2D>[2];
        result[0] = unhitedTargetContainer;
        result[1] = hitedTargetContainer;

        //if (hitedTargetContainer.Count > 0 || unhitedTargetContainer.Count > 0)
        return result;

        /*
        if (hitedTargetContainer.Count > 0)
            return hitedTargetContainer.ToArray();
        else
            return null;
        */
    }
}
