using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Arrow : MonoBehaviour
{
    Transform targetTransform;
    Character targetChatacter;
    

    Image image;
    public Camera _camera;

    Vector2 arrowVec;

    // Start is called before the first frame update
    void Start()
    {
        image = this.GetComponent<Image>();      
    }

    private void FixedUpdate()
    {
        // 플레이어의 스킬 조이스틱 방향을 향하도록 함
        Vector3 ArrowPos = _camera.WorldToScreenPoint(targetTransform.position) + (Vector3)arrowVec * targetTransform.localScale.x * 50f;

        float x = ArrowPos.x;
        float y = ArrowPos.y;

        // 화살표 위치 옮기기
        this.transform.position = new Vector3(x, y, this.transform.position.z);
        this.transform.eulerAngles = new Vector3(0, 0, -Mathf.Atan2(arrowVec.x, arrowVec.y) * Mathf.Rad2Deg);           
    }

    public void setInvisible()
    {
        if(image.enabled) image.enabled = false;
    }

    public void setVisible()
    {
        if(!image.enabled) image.enabled = true;
    }

    public void setVec(Vector2 vec)
    {
        arrowVec = vec;
    }

    public void SetTarget(GameObject _target)
    {
        targetTransform = _target.GetComponent<Transform>();
        targetChatacter = _target.GetComponent<Character>();
    }
}
