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
        [Tooltip("스킬 발동 까지 걸리는 시간 (= 스킬 발동 후 경직 시간)")]
        public int triggerTime;
    }
}
