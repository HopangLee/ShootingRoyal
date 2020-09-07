using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class JoyStickParent : MonoBehaviour
{
    public Arrow arrow;

    protected abstract void Awake();

    protected abstract void Start();

    public abstract void OnTouch(Vector2 m_vecMove);

    public abstract void OnDrag(Vector2 m_vecMove);

    public abstract void OnPointerDown(Vector2 m_vecMove);

    public abstract void OnPointerUp(Vector2 m_vecMove);

    public abstract void SetTarget(Character _target);
}
