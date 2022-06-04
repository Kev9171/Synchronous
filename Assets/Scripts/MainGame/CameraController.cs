using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KWY
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        readonly Vector3 SimulDes = new Vector3(0, 0, -10); // �⺻ ī�޶� ��ġ
        readonly Vector3 TurnReadyDes = new Vector3(2, -0.3f, -10); // �� ����� ���� ������ �ʿ䰡 ���� (�� ����� ��������� ���� or �ϵ� �ڵ�)

        Vector3 des;
        float speed = 1.0f;

        public void SetCameraTurnReady()
        {
            des = TurnReadyDes;
        }

        public void SetCameraSimul()
        {
            des = SimulDes;
        }

        public void Start()
        {
        }

        private void Update()
        {
            transform.position = Vector3.Lerp(transform.position, des, speed * Time.deltaTime);
        }
    }
}
