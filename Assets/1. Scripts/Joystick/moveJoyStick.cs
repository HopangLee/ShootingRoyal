using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveJoyStick : JoyStickParent
{
    public Character target;
    private JoyStick _joyStick;

    protected override void Awake()
    {
        _joyStick = this.GetComponent<JoyStick>();
    }

    protected override void Start()
    {
        //target = _joyStick.target;
        
    }

    public override void OnTouch(Vector2 m_vecMove)
    {     
        target.setMoveVec(m_vecMove);
    }

    public override void OnDrag(Vector2 m_vecMove)
    {
    }

    public override void OnPointerDown(Vector2 m_vecMove)
    {

    }

    public override void OnPointerUp(Vector2 m_vecMove)
    {

    }

    public override void SetTarget(Character _target)
    {
        target = _target;
        if (target == null) Debug.LogError("<Color=Red><a>Missing</a></Color> target is null");
    }
}
