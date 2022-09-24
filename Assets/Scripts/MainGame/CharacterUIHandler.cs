using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KWY
{
    public class CharacterUIHandler : MonoBehaviour
    {
        [SerializeField]
        CharacterPanel characterPanel1;

        [SerializeField]
        CharacterPanel characterPanel2;

        [SerializeField]
        CharacterPanel characterPanel3;

        Dictionary<int, CharacterPanel> charaUIs = new Dictionary<int, CharacterPanel>();
        Dictionary<Character, CharacterPanel> charaPanels = new Dictionary<Character, CharacterPanel>();

        /// <summary>
        /// 자신의 캐릭터에 대해서 가져오고 UI를 맞는 캐릭터와 연결 should be called once
        /// </summary>
        /// <param name="charaList">자기 팀의 PlayableCharacter List</param>
        public void InitData(List<PlayableCharacter> charaList)
        {
            if (charaList.Count != 3)
            {
                Debug.LogError("INVALID LIST COUNT at charaList");
                return;
            }

            Character c1 = charaList[0].Chara.GetComponent<Character>();
            characterPanel1.SetData(c1.Cb, c1.Buffs);
            charaUIs.Add(charaList[0].Id, characterPanel1);
            charaPanels.Add(c1, characterPanel1);

            Character c2 = charaList[1].Chara.GetComponent<Character>();
            characterPanel2.SetData(c2.Cb, c2.Buffs);
            charaUIs.Add(charaList[1].Id, characterPanel2);
            charaPanels.Add(c2, characterPanel2);

            Character c3 = charaList[2].Chara.GetComponent<Character>();
            characterPanel3.SetData(c3.Cb, c3.Buffs);
            charaUIs.Add(charaList[2].Id, characterPanel3);
            charaPanels.Add(c3, characterPanel3);
        }

        public void UpdateCharacterStatusUI(Character chara)
        {
            Debug.Log("UpdateCharacterStatusUI");
        }

        public void UpdateCharacterActionIcon(int id)
        {

        }
    }
}