using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class RidesPresenter : MonoBehaviour
{
    VrPlayer player;

    [Space]
    [HideInInspector]
    public List<UI_Rollercoaster> rollercoasterBtn;

    [Space]
    [HideInInspector]
    public List<UI_Biking> bikingBtn;

    [Space]
    [HideInInspector]
    public List<UI_DownLoad> downLoadBtn;

    [Space]
    [HideInInspector]
    public List<UI_Pendulum> pendulumBtn;

    [Space]
    [HideInInspector]
    public List<UI_Round> roundBtn;

    Vector3 outVec = Vector3.zero;
    Quaternion outRot = Quaternion.identity;

    bool isIn = false;

    private void Start()
    {
        player = ContentsManager.Instance.vrPlayer;

        for (int i = 0; i < bikingBtn.Count; i++)
        {
            rollercoasterBtn[i].button.onClick.AddListener(OnBtnClickedInRoller);
            bikingBtn[i].button.onClick.AddListener(OnBtnClickedInBiking);
            downLoadBtn[i].button.onClick.AddListener(OnBtnClickedInDownLoad);
            pendulumBtn[i].button.onClick.AddListener(OnBtnClickedInPendulum);
            roundBtn[i].button.onClick.AddListener(OnBtnClickedInRound);
        }
    }

    private void Update()
    {
        if (isIn)
        {
            if (OVRInput.GetDown(OVRInput.Button.Four))
            {
                Fade.Instance.FadeInOut(() =>
                {
                    RidesOut();
                    player.transform.position = outVec;
                    player.transform.rotation = outRot;
                });
            }
        }
    }

    private void FixedUpdate()
    {
        if (isIn)
            ContentsManager.Instance.vrCamera.gameObject.transform.rotation = Quaternion.identity;
    }

    private void OnBtnClickedInRoller()
    {
        Fade.Instance.FadeInOut(() =>
        {
            player.playerInteraction.RidesTrans = rollercoasterBtn[0].inTrans;
            RidesIn(rollercoasterBtn[0].inTrans);
        });
        outVec = rollercoasterBtn[0].outTrans.position;
        outRot = rollercoasterBtn[0].outTrans.rotation;
    }

    private void OnBtnClickedInBiking()
    {
        Fade.Instance.FadeInOut(() =>
        {
            player.playerInteraction.RidesTrans = bikingBtn[0].inTrans;
            RidesIn(bikingBtn[0].inTrans);
        });
        outVec = bikingBtn[0].outTrans.position;
        outRot = bikingBtn[0].outTrans.rotation;
    }

    private void OnBtnClickedInDownLoad()
    {
        Fade.Instance.FadeInOut(() =>
        {
            player.playerInteraction.RidesTrans = downLoadBtn[0].inTrans;
            RidesIn(downLoadBtn[0].inTrans);
        });
        outVec = downLoadBtn[0].outTrans.position;
        outRot = downLoadBtn[0].outTrans.rotation;
    }

    private void OnBtnClickedInPendulum()
    {
        Fade.Instance.FadeInOut(() =>
        {
            player.playerInteraction.RidesTrans = pendulumBtn[0].inTrans;
            RidesIn(pendulumBtn[0].inTrans);
        });
        outVec = pendulumBtn[0].outTrans.position;
        outRot = pendulumBtn[0].outTrans.rotation;
    }

    private void OnBtnClickedInRound()
    {
        Fade.Instance.FadeInOut(() =>
        {
            player.playerInteraction.RidesTrans = roundBtn[0].inTrans;
            RidesIn(roundBtn[0].inTrans);
        });
        outVec = roundBtn[0].outTrans.position;
        outRot = roundBtn[0].outTrans.rotation;
    }

    private void RidesIn(Transform inTrans)
    {
        player.transform.localPosition = Vector3.zero;
        player.rigid.isKinematic = true;

        ContentsManager.Instance.vrCamera.gameObject.transform.rotation = inTrans.rotation;

        player.l_laser.SetActive(false);
        player.r_laser.SetActive(false);
        isIn = true;

        player.canMove = false;
    }

    private void RidesOut()
    {
        player.l_laser.SetActive(true);
        player.r_laser.SetActive(true);
        isIn = false;

        player.playerInteraction.RidesTrans = null;
        player.rigid.isKinematic = false;
        player.canMove = true;
        ContentsManager.Instance.vrCamera.gameObject.transform.rotation = Quaternion.identity;
    }

}
