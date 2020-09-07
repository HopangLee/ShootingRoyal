using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;


/*
 * 캐릭터의 기본적인 기능을 담고 있는 클래스
 * 이동 조이스틱을 통해 이동, 회전
 * 공격 조이스틱을 통해 기본 공격 및 화살표 방향 표시
 * 스킬 조이스틱을 통해 스킬 공격 및 화살표 방향 표시
 */

public class Character : MonoBehaviourPunCallbacks, IPunObservable
{
    public enum States{}; // 캐릭터의 상태이상 (속박, 느려짐, 기절, 화상, 젖음, 실명)

    [Header("캐릭터 정보")]

    public List<States> state; // 캐릭터의 현재 상태를 모두 담은 리스트

    [HideInInspector]
    public float slowValue = 0f; // 느려진 속도 값 (디버프)
    [HideInInspector]
    public float fastValue = 0f; // 빨라진 속도 값 (버프)
    private float normalValue = 50f; // 기본적으로 더해주는 속도 값

    [Range (0, 1000)]
    public float speed = 330f; // 특정 공식을 통해 속도값을 반환할 예정
    [HideInInspector]
    public int maxHp = 2000; // 최대 체력
    [HideInInspector]
    public int maxMp = 100; // 최대 마나

    //[HideInInspector]    
    public int currentHp; // 현재 체력
    //[HideInInspector]
    public int currentMp; // 현재 마나

    public float realSpeed // 실제 적용되는 속도값
    {
        get
        {
            return 2f + Mathf.Sqrt((normalValue + speed - slowValue + fastValue) / 90f);
        }
    }

    [Header("팀 정보")]
    public SelectCharManager.Team team;

    [Header("마스터 플레이어 정보")]
    public bool isMine = false;

    [HideInInspector]
    public Vector2 moveVec = Vector2.zero; // 이동 방향
    [HideInInspector]
    public Vector2 attackVec = Vector2.zero; // 기본공격 방향
    [HideInInspector]
    public Vector2 skillVec = Vector2.zero; // 스킬 사용 방향

    Transform mTransform;
    Rigidbody2D mRigidbody;

    JoyStick moveJoyStick;
    JoyStick attackJoyStick;
    JoyStick skillJoyStick;
    Arrow arrow;

    [SerializeField]
    public HpManager hpManager;

    private Vector2 arrowVec = Vector2.zero;

    [Header("시야 정보")]
    [Range(0, 2000)]
    public float viewRadius;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    private void Awake()
    {
        if (photonView.IsMine)
        {
            moveJoyStick = FindObjectOfType<moveJoyStick>().GetComponent<JoyStick>();
            attackJoyStick = FindObjectOfType<attackJoyStick>().GetComponent<JoyStick>();
            skillJoyStick = FindObjectOfType<skillJoyStick>().GetComponent<JoyStick>();
            arrow = FindObjectOfType<Arrow>().GetComponent<Arrow>();            

            moveJoyStick.SetTarget(this);
            attackJoyStick.SetTarget(this);
            skillJoyStick.SetTarget(this);
            arrow.SetTarget(this.gameObject);            

            isMine = true;
            team = SelectCharManager.team;
        }
        else
        {            
            isMine = false;            
        }

        mTransform = this.GetComponent<Transform>();
        mRigidbody = this.GetComponent<Rigidbody2D>();

        GameObject HpMpUI = GameObject.Instantiate(Resources.Load("Hp Mp UI") as GameObject, this.mTransform.position + new Vector3(0f, HpManager.height, 0f), Quaternion.identity);
        HpMpUI.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);

        hpManager = HpMpUI.GetComponent<HpManager>();
        hpManager.SetTarget(this.gameObject);

        // 내캐릭터가 아니고 나랑 다른 팀일 경우 hpUI를 일단 보이지 않도록 한다.
        if (!isMine && team != SelectCharManager.team)
        {
            hpManager.setInvisible();
        }
    }

    void Start()
    {
        CameraCtrl _cameraCtrl = this.gameObject.GetComponent<CameraCtrl>();
        if (_cameraCtrl != null)
        {
            if (photonView.IsMine)
            {
                _cameraCtrl.OnStartFollowing();
            }
        }
        else
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
        }

        if (isMine)
        {
            StartCoroutine("FindTargetsWithDelay", .2f);
        }

        currentHp = maxHp;
        currentMp = maxMp;
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            SetUIVisible();
        }
    }

    void SetUIVisible()
    {
        Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(mTransform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
           
            Character hitedTarget = targetsInViewRadius[i].GetComponent<Character>();

            if (hitedTarget == null) continue; // 탐지된 대상이 캐릭터가 아니면 패스;

            //Debug.Log("o");

            HpManager UI = hitedTarget.hpManager;

            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            float dstToTarget = Vector3.Distance(transform.position, target.position);
            
            // 해당 타겟이 그림자로 막혀있지 않아 볼 수 있다면
            if (!Physics2D.Raycast(mTransform.position, dirToTarget, dstToTarget, obstacleMask))
            {
                if(!UI.isVisible)
                    UI.setVisible();
            }
            else
            {
                // 타겟이 그림자로 막혀 볼 수 없다면
                if (UI.isVisible)
                    UI.setInvisible();
            }
        }
    }

    void FixedUpdate()
    {
        // 플레이어의 캐릭터인 경우
        if (photonView.IsMine)
        {          

            // 조이스틱을 조종하고 있으면 캐릭터가 움직임
            if (moveJoyStick.m_bTouch)
            {                
                mRigidbody.MovePosition((Vector2)mTransform.position + moveVec);
            }

            // 캐릭터 회전
            mTransform.eulerAngles = new Vector3(0, 0, -Mathf.Atan2(moveVec.x, moveVec.y) * Mathf.Rad2Deg);          
        }

        mRigidbody.velocity = Vector2.zero;
    }

    public void setMoveVec(Vector2 vec)
    {
        moveVec = vec * realSpeed;
    }    

    // 기본 공격 함수
    public void Attack(Vector2 vec)
    {
        attackVec = vec;
    }

    // 스킬 사용 함수
    public void useSkill(Vector2 vec)
    {
        skillVec = vec;
    }

    // 체력을 감소시킬 때 부르는 함수
    public void DecreaseHp(int value)
    {
        if (currentHp - value > 0) currentHp -= value;
        else currentHp = 0;
    }

    // 체력을 증가시킬 때 부르는 함수
    public void IncreaseHp(int value)
    {
        if (currentHp + value < maxHp) currentHp += value;
        else currentHp = maxHp;
    }

    // 마나를 감소시킬 때 부르는 함수 -> 감소량이 해당 감소량만큼 남았으면 true, 해당 스킬을 쓸 마나가 없으면 false
    public bool DecreaseMp(int value)
    {
        if (currentMp - value >= 0)
        {
            currentMp -= value;
            return true;
        }
        else return false;
    }

    // 마나를 증가시킬 때 부르는 함수
    public void IncreaseMp(int value)
    {
        if (currentMp + value < maxMp) currentMp += value;
        else currentMp = maxMp;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
        if (stream.IsWriting)
        {
            stream.SendNext(maxHp);
            stream.SendNext(currentHp);

            stream.SendNext(maxMp);
            stream.SendNext(currentMp);
        }
        else
        {
            this.maxHp = (int)stream.ReceiveNext();
            this.currentHp = (int)stream.ReceiveNext();

            this.maxMp = (int)stream.ReceiveNext();
            this.currentMp = (int)stream.ReceiveNext();
        }
        
    }
}
