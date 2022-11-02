using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KWY
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        readonly Vector3 SimulDes = new Vector3(0, 0, -10); // �⺻ ī�޶� ��ġ
        readonly Vector3 TurnReadyDes = new Vector3(2.3f, -0.3f, -10); // �� ����� ���� ������ �ʿ䰡 ���� (�� ����� ��������� ���� or �ϵ� �ڵ�)

        float speed = 0.01f;

        bool simul = false;


        public void SetCameraTurnReady()
        {
            simul = false;
        }

        public void SetCameraSimul()
        {
            simul = true;
        }

        public void Start()
        {
            simul = false;
        }

        private void Update()
        {
            if (simul)
            {
                transform.position = Vector3.Lerp(gameObject.transform.position, SimulDes, speed);
            }
            else
            {
                transform.position = Vector3.Lerp(gameObject.transform.position, TurnReadyDes, speed);
            }
            //transform.position = Vector3.Lerp(gameObject.transform.position, des, 0.1f);
            //transform.position = Vector3.SmoothDamp(gameObject.transform.position, des, ref zero, 1f);
            //transform.position = Vector3.MoveTowards(transform.position, des, speed * Time.deltaTime);
        }
    }
}
