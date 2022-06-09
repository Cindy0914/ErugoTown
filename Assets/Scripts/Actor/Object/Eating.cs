using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eating : MonoBehaviour
{
    VrPlayer player;

    float _time = 0;

    private void Start()
    {
        player = ContentsManager.Instance.vrPlayer;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag("MainCamera") && player.state.Equals(VrPlayer.State.Eating))
        {
            if (_time >= 1f)
            {
                _time = 0;

                var effect = ObjectPoolManager.Instance.Spawn("EatingEffect");
                effect.gameObject.transform.position = transform.position;
                effect.gameObject.transform.LookAt(Camera.main.transform);

                Destroy(gameObject);
            }
            else
                _time += Time.deltaTime;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("MainCamera") && player.state.Equals(VrPlayer.State.Eating))
        {
            _time = 0;
        }
    }
}
