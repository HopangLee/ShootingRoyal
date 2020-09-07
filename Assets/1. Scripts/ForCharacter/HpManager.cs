using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpManager : MonoBehaviour
{
    private Camera _camera;

    public static float height = 100f;

    private Transform target; // 캐릭터를 추적하기 위해서
    private Character character; // 체력, 마나 정보를 받기 위해

    [SerializeField]
    private Image Hpbar;
    [SerializeField]
    private Image BackHpbar;
    [SerializeField]
    private Image Effectbar;
    [SerializeField]
    private Image Mpbar;

    [SerializeField]
    private float lerpSpeed;

    public bool isVisible = false;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void LateUpdate()
    {    
        if(target != null)
        {
            Vector3 screenPos = _camera.WorldToScreenPoint(target.position);
            float x = screenPos.x;
            float y = screenPos.y;

            this.transform.position = new Vector3(x, y + height, this.transform.position.z);
        }
    }

    private void FixedUpdate()
    {
        Hpbar.fillAmount = (float)character.currentHp / (float)character.maxHp;
        Mpbar.fillAmount = (float)character.currentMp / (float)character.maxMp;

        if(character.isMine) Debug.Log("hp: "+Hpbar.fillAmount);
    }

    public void SetTarget(GameObject _target)
    {
        target = _target.GetComponent<Transform>();
        character = _target.GetComponent<Character>();
    }

    public void setInvisible()
    {
        Hpbar.enabled = false;
        BackHpbar.enabled = false;
        Effectbar.enabled = false;
        Mpbar.enabled = false;
        isVisible = false;

        Debug.Log("set invisible");
    }

    public void setVisible()
    {
        Hpbar.enabled = true;
        BackHpbar.enabled = true;
        Effectbar.enabled = true;
        Mpbar.enabled = true;
        isVisible = true;

        Debug.Log("set visible");
    }
}
