using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KWY
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        readonly Vector3 SimulDes = new Vector3(0, 0, -10); // 기본 카메라 위치
        readonly Vector3 TurnReadyDes = new Vector3(2, -0.3f, -10); // 맵 사이즈에 따라 조정할 필요가 있음 (맵 사이즈에 상대적으로 조정 or 하드 코딩)

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
