using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Laser
{
    public class LaserPointer : MonoBehaviour
    {
        private LineRenderer laser;
        public RaycastHit hit;
        GameObject point;

        [HideInInspector]
        public GameObject currentObject;
        Color originColor;

        public float raycastDistance = 3f;

        int mask = 1 << (int)Define.Layer.UI | 1 << (int)Define.Layer.Grabbable;

        Vector3 zOneVec = new Vector3(0, 0, 1);

        private void Start()
        {
            TryGetComponent(out laser);
            point = transform.GetChild(0).gameObject;
            originColor = laser.material.color;
        }

        private void FixedUpdate()
        {
            //laser.SetPosition(0, transform.position);

            // 충돌 감지 시
            if (Physics.Raycast(transform.position, transform.forward, out hit, raycastDistance, mask))
            {
                point.transform.position = hit.point;
                laser.SetPosition(1, point.transform.localPosition);

                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Grabbable"))
                    currentObject = hit.collider.gameObject;
                else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("UI"))
                    currentObject = hit.collider.gameObject;
            }
            else
            {
                laser.SetPosition(1, zOneVec);
                point.transform.localPosition = zOneVec;

                if (currentObject != null)
                    currentObject = null;
            }
        }

        private void Update()
        {
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) || OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
                laser.material.color = new Color(255, 255, 255, 1f);
            else if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger) || OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger))
                laser.material.color = originColor;
        }
    }
}