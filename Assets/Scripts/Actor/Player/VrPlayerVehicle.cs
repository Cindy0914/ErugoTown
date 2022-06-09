using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx.Triggers;
using UniRx;

public class VrPlayerVehicle : MonoBehaviour
{
    // 어떤것을 탈것인가
    // 어떤 기능을 넣을것인가
    /// 어떻게 작동할 것인가
    // 움직일수는 있게 할것인가
    /// 움직인다면 움직이는 방향은 어느쪽으로 할것인가
    // 움직이는 주체가 무엇인가
    // 물리엔진은 적용받아야 하는가

    VrPlayer player;
    LaserPointer laser;
    GameObject currentInteracteObject;

    private void Start()
    {
        player = ContentsManager.Instance.vrPlayer;
        player.r_laser.TryGetComponent(out laser);
    }

    private void CheckInteracteObjectByTag()
    {
        switch (currentInteracteObject.tag)
        {
            case "Car":
                InteracteCar();
                break;
            case "SkateBoard":
                InteracteSkateBoard();
                break;
        }
    }

    private void InteracteCar()
    {
    }

    private void InteracteSkateBoard()
    {
        this.FixedUpdateAsObservable()
            .Where(_ => player.state.Equals(VrPlayer.State.Skateing))
            .Subscribe(_ =>
            {

            });

        void SkateBoardMovement()
        {
            // 휠 컬라이더
            // ws 전방 후방
            // ad 방향전환
            // 부스터
            // 점프시 보드 비활성화
            // 비활성화시 인벤토리에 등록
            // 사용시 인벤토리에서
        }
    }
}
