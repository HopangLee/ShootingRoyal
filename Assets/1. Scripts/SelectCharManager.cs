using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class SelectCharManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public enum Team {Red, Blue};

    public const int maxPlayerPerRoom = 2;

    public Image[] CharButtons; // 캐릭터를 고르는 버튼
    public Sprite[] CharImgs; // 캐릭터 스프라이트
    public Image[] RedPlayerChar; // 빨간팀 플레이어 캐릭터를 보여줌
    public Image[] BluePlayerChar; // 파란팀 플레이어 캐릭터를 보여줌

    private int CharLength;

    [HideInInspector]
    public int CharNum = 0;

    [HideInInspector]
    public static EnumCharacter.CharEnum CharSellectd;

    [HideInInspector]
    public int teamNumber = 0;
    private int position = 0;
    public static Team team; // 어느 팀인지 확인

    [HideInInspector]
    public bool isSelected = false;

    [SerializeField]
    private Text select;
    [SerializeField]
    private Image selectBtn;

    private bool[] SelectInfo;

    public Text timer; // 메인씬 전환 타이머
    public GameObject delayPanel;
    bool timerStart = false;

    void Start()
    {
        SelectInfo = new bool[maxPlayerPerRoom];
        for(int i = 0; i < maxPlayerPerRoom; i++)
        {
            SelectInfo[i] = false;
        }

        teamNumber = LobbyManager.TeamNumber;
       
        
        CharLength = CharButtons.Length;
        ClickCharButton(0);        
    }

    void Update()
    {
        // 룸안에 플레이어가 4명으로 모두 채워졌을 경우 (프로토타입에서는 2명 1대 1)
        if (PhotonNetwork.PlayerList.Length == maxPlayerPerRoom)
        {                                
            bool canBattle = true;
            foreach(bool s in SelectInfo)
            {
                if (!s) canBattle = false;
            }
            // 메인 씬으로 이동
            if (!timerStart && canBattle)
            {
                //PhotonNetwork.LoadLevel("Main");
                StartCoroutine("TimerDelay");
            }
            else if(timerStart && !canBattle)
            {
                StopCoroutine("TimerDelay");
                timer.text = "3";
                delayPanel.SetActive(false);
                timerStart = false;
            }
        }
        // 룸안에 플레이어가 4명 이상이 된 경우
        else if (PhotonNetwork.PlayerList.Length > maxPlayerPerRoom)
        {
            // 마지막으로 들어온 플레이어가 다른 룸 찾기  
        }
    }

    IEnumerator TimerDelay()
    {
        timerStart = true;
        delayPanel.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        timer.text = "2";
        yield return new WaitForSeconds(1.0f);
        timer.text = "1";
        yield return new WaitForSeconds(1.0f);
        PhotonNetwork.LoadLevel("Main");
        timerStart = false;
    }

    public void ClickCharButton(int n)
    {
        CharNum = n;
        for(int i = 0; i < CharLength; i++)
        {
            Image img = CharButtons[i];
            if (i == n)
            {
                img.color = new Color(img.color.r, img.color.g, img.color.b, 1.0f);
            }
            else
            {              
                img.color = new Color(img.color.r, img.color.g, img.color.b, 0.45f);
            }
        }
    }

    public void SelectCharacter()
    {
        CharSellectd = (EnumCharacter.CharEnum)CharNum;
        if (!isSelected)
        {            
            isSelected = true;
            select.text = "Cancel";
            selectBtn.color = new Color(selectBtn.color.r, selectBtn.color.g, selectBtn.color.b, 0.75f);
        }
        else
        {
            isSelected = false;
            select.text = "Select";
            selectBtn.color = new Color(selectBtn.color.r, selectBtn.color.g, selectBtn.color.b, 1.0f);            
        }
 
     
        if (photonView == null) Debug.Log("photonview null");
        photonView.RPC("ChangeChar", RpcTarget.All, teamNumber, CharNum, isSelected);
    }

    [PunRPC]
    public void ChangeChar(int teamNumtp, int CharNumtp, bool isSelectedtp)
    {
        SelectInfo[teamNumtp] = isSelectedtp;

        if (teamNumtp >= maxPlayerPerRoom / 2)
        {
            team = Team.Blue;
        }
        else team = Team.Red;

        int positiontp = teamNumtp % (maxPlayerPerRoom / 2);

        if (isSelectedtp)
        {

            if (team == Team.Red)
            {
                RedPlayerChar[positiontp].sprite = CharImgs[CharNumtp];
            }
            else
            {
                BluePlayerChar[positiontp].sprite = CharImgs[CharNumtp];
            }
        }
        else
        {         
            if (team == Team.Red)
            {
                RedPlayerChar[positiontp].sprite = null;
            }
            else
            {
                BluePlayerChar[positiontp].sprite = null;
            }
        }
    }

     
    public override void OnPlayerEnteredRoom(Player other)
    {
        Debug.Log("Player #"+other.UserId+" 입장");
        if (photonView == null) Debug.Log("photonview null");
        photonView.RPC("ChangeChar", RpcTarget.All, teamNumber, CharNum, isSelected);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
}
