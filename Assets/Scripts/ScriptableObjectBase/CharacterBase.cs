using UnityEngine;
using System.Collections.Generic;

namespace KWY
{
    /// <summary>
    /// 캐릭터에 대한 기본 정보가 담겨 있는 클래스 (최초 캐릭터 정보 로드 시 사용)
    /// </summary>
    [CreateAssetMenu(fileName = "Character", menuName = "Character/CharacterBase")]
    public class CharacterBase : ScriptableObject
    {
        public CID cid;
        public string characterName;
        public string characterEx;
        public float hp;
        public float atk;
        public float spd;
        public Sprite icon;
        // 나중에 캐릭터와 패시브 object 분리... 하기 일단 통합...
        public Sprite passiveIcon;
        public string passiveEx;

        public List<SID> skills = new List<SID>();

        public override string ToString()
        {
            return string.Format("CID: {0}, CharacterName: {1}, HP: {2}, ATK: {3}, SPD: {4}", cid, characterName, hp, atk, spd);
        }
    }
}
