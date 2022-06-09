using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx.Triggers;
using UniRx;

public class VrPlayerVehicle : MonoBehaviour
{
    // ����� Ż���ΰ�
    // � ����� �������ΰ�
    /// ��� �۵��� ���ΰ�
    // �����ϼ��� �ְ� �Ұ��ΰ�
    /// �����δٸ� �����̴� ������ ��������� �Ұ��ΰ�
    // �����̴� ��ü�� �����ΰ�
    // ���������� ����޾ƾ� �ϴ°�

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
            // �� �ö��̴�
            // ws ���� �Ĺ�
            // ad ������ȯ
            // �ν���
            // ������ ���� ��Ȱ��ȭ
            // ��Ȱ��ȭ�� �κ��丮�� ���
            // ���� �κ��丮����
        }
    }
}
