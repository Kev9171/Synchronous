using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KWY
{
    public class ActionBase : ScriptableObject
    {
        public int cost;
        public Sprite icon;
        public new string name;
        [Tooltip("��ų�� �ߵ� �Ǵ� �ð�")]
        public int castingTime;
        [Tooltip("��ų �ߵ��� ���� �Ǵ� �ð� (= ��ų �ߵ� �� ���� �ð�)")]
        public int triggerTime;
        public List<Vector2Int> areaOddY = new List<Vector2Int>();
        public List<Vector2Int> areaEvenY = new List<Vector2Int>();
    }
}
