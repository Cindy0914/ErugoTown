using UnityEngine;

public class VrPlayer : MonoBehaviour
{
    public enum State
    {
        Basic = 0,
        Shooting,
        Swimming,
        Eating,
        Boarding,
        Fishing,
        Falling,
        Skateing,
    }

    public bool canMove = true;
    public State state = State.Basic;

    public Collider col { get; private set; }
    public Rigidbody rigid { get; private set; }

    [HideInInspector]
    public VrPlayerGameInteraction playerInteraction;
    [HideInInspector]
    public VrPlayerLoco playerLoco;
    [HideInInspector]
    public VrPlayerShooting playerShooting;
    [HideInInspector]
    public VrPlayerGrab playerGrab;
    [HideInInspector]
    public VrPlayerFishing playerFishing;
    [HideInInspector]
    public Inventory inventory;

    public UI_MainText mainText;

    public GameObject l_laser;
    public GameObject r_laser;

    public GameObject leftHandMesh;
    public GameObject RightHandMesh;

    public CustomDistanceGrabber l_grabber;
    public CustomDistanceGrabber r_grabber;

    private void Awake()
    {
        col = GetComponent<Collider>();
        rigid = GetComponent<Rigidbody>();

        TryGetComponent(out playerInteraction);
        TryGetComponent(out playerLoco);
        TryGetComponent(out playerShooting);
        TryGetComponent(out playerGrab);
        TryGetComponent(out playerFishing);
        TryGetComponent(out inventory);
    }
}

