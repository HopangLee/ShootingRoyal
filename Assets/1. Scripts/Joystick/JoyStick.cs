using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoyStick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    JoyStickParent joyStickChild;

    RectTransform m_rectBack;
    RectTransform m_rectJoystick;

    [HideInInspector]
    public float m_fRadius;
    float m_fSqr = 0f;

    protected Vector2 m_vecMove;
    Vector2 m_vecNormal;

    [HideInInspector]
    public bool m_bTouch = false;
    [HideInInspector]
    public Character target;

    void Awake()
    {
        joyStickChild = this.GetComponent<JoyStickParent>();
    }

    void Start()
    {
        m_rectBack = this.GetComponent<RectTransform>();
        m_rectJoystick = transform.Find("JoyStick").GetComponent<RectTransform>();
        // JoystickBackground의 반지름입니다.
        m_fRadius = m_rectBack.rect.width * 0.5f;
    }

    public void OnTouch(Vector2 vecTouch)
    {
        Vector2 vec = new Vector2(vecTouch.x - m_rectBack.position.x, vecTouch.y - m_rectBack.position.y);

        // vec값을 m_fRadius 이상이 되지 않도록 합니다.
        vec = Vector2.ClampMagnitude(vec, m_fRadius);
        m_rectJoystick.localPosition = vec;

        // 조이스틱 배경과 조이스틱과의 거리 비율로 이동합니다.
        float fSqr = (m_rectBack.position - m_rectJoystick.position).sqrMagnitude / (m_fRadius * m_fRadius);

        // 터치위치 정규화
        Vector2 vecNormal = vec.normalized;

        m_vecMove = new Vector2(vecNormal.x * Time.deltaTime * fSqr, vecNormal.y * Time.deltaTime * fSqr);

        joyStickChild.OnTouch(m_vecMove);
    }

    public void OnDrag(PointerEventData eventData)
    {
        OnTouch(eventData.position);
        m_bTouch = true;

        joyStickChild.OnDrag(m_vecMove);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnTouch(eventData.position);
        m_bTouch = true;
        joyStickChild.OnPointerDown(m_vecMove);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // 원래 위치로 되돌립니다.
        m_rectJoystick.localPosition = Vector2.zero;
        m_bTouch = false;

        joyStickChild.OnPointerUp(m_vecMove);
    }

    // 플레이어의 캐릭터 세팅해주기
    public void SetTarget(Character _target)
    {
        if (_target == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> PlayMakerManager target for JoyStick UI.SetTarget.", this);
            return;
        }
        target = _target;
        joyStickChild.SetTarget(_target);
    }
}
