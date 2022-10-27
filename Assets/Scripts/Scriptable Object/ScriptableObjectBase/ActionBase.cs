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
        [Tooltip("스킬이 발동 되는 시간")]
        public int castingTime;
        [Tooltip("스킬 발동이 지속 되는 시간 (= 스킬 발동 후 경직 시간)")]
        public int triggerTime;
        public List<Vector2Int> areaOddY = new List<Vector2Int>();
        public List<Vector2Int> areaEvenY = new List<Vector2Int>();
    }
}
