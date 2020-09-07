﻿using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

// 게임 오버 여부, 게임 UI 매니저와 연결, 플레이어 생성을 관리하는 게임 매니저
public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
  // 외부에서 싱글톤 오브젝트를 가져올 때 사용할 프로퍼티
  public static GameManager instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if(m_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<GameManager>();
            }

            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }

    private static GameManager m_instance; // 싱글톤이 할당될 static 변수

    public GameObject[] playerPrefabs; // 생성할 플레이어 캐릭터 프리팹

    public bool isGameover { get; private set; } // 게임 오버 상태

    // 주기적으로 자동 실행되는, 동기화 메서드
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 로컬 오브젝트라면 쓰기 부분이 실행됨
        if (stream.IsWriting)
        {

        }
        else
        {
            // 리모트 오브젝트라면 읽기 부분이 실행됨
                       
        }
    }

    private void Awake()
    {
        // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면
        if(instance != this)
        {
            // 자신을 파괴
            Destroy(gameObject);
        }
    }

    // 게임 시작과 동시에 플레이어가 될 게임 오브젝트를 생성
    private void Start()
    {
        // 생성할 랜덤 위치 지정 
        float x;
        float y;

        int xValue = Random.Range(0, 2);
        int yValue = Random.Range(0, 2);

        if (xValue == 0)
        {
            x = Random.Range(-8f, -5f);
        }
        else
        {
            x = Random.Range(5f, 8f);
        }

        if (yValue == 0)
        {
            y = Random.Range(-8f, -5f);
        }
        else
        {
            y = Random.Range(5f, 8f);
        }

        Vector3 randomSpawnPos = new Vector3(x, y, 0f);

        // 네트워크 상의 모든 클라이언트들에서 생성 실행
        // 단, 해당 게임 오브젝트의 주도권은, 생성 메서드를 직접 실행한 클라이언트에게 있음
        PhotonNetwork.Instantiate(playerPrefabs[(int)SelectCharManager.CharSellectd].name, randomSpawnPos, Quaternion.identity);        
    }

    // 게임 오버 처리
    public void EndGame()
    {
        // 게임 오버 상태를 참으로 변경
        isGameover = true;

        // 게임 오버 UI를 활성화
        //UIManager.instance.SetActiveGameoverUI(true);
    }
}
