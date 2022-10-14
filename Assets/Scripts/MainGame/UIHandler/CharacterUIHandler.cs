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

        [SerializeField]
        SelSkillPanel selSkillPanel1;

        [SerializeField]
        SelSkillPanel selSkillPanel2;

        [SerializeField]
        SelSkillPanel selSkillPanel3;

        [SerializeField]
        MainGameData data;

        Dictionary<int, CharacterPanel> charaUIs = new Dictionary<int, CharacterPanel>();

        Dictionary<Character, CharacterPanel> charaPanels = new Dictionary<Character, CharacterPanel>();
        Dictionary<int, SelSkillPanel> selSkillUIs = new Dictionary<int, SelSkillPanel>();

        // 캐릭터 패널을 선택하여 캐릭터 선택 가능 여부
        private bool charaPanelSelectable = false;

        public bool CharaPanelSelectable
        {
            set
            {
                foreach (CharacterPanel cp in charaPanels.Values)
                {
                    cp.Selectable = value;
                }
                charaPanelSelectable = value;
            }
            get
            {
                return charaPanelSelectable;
            }
        }


        /// <summary>
        /// 자신의 캐릭터에 대해서 가져오고 UI를 맞는 캐릭터와 연결 should be called once
        /// </summary>
        /// <param name="charaList">자기 팀의 PlayableCharacter List</param>
        public void InitData(List<PlayableCharacter> charaList)
        {
            if (charaList.Count != 3)
            {
                Debug.LogError($"INVALID LIST COUNT at charaList : {charaList.Count}");
                return;
            }

            Character c1 = charaList[0].CharaObject.GetComponent<Character>();
            characterPanel1.Init(c1);
            selSkillPanel1.SetData(c1.name, c1.Cb.skills);
            charaUIs.Add(charaList[0].Id, characterPanel1);
            selSkillUIs.Add(charaList[0].Id, selSkillPanel1);
            charaPanels.Add(c1, characterPanel1);

            Character c2 = charaList[1].CharaObject.GetComponent<Character>();
            characterPanel2.Init(c2);
            selSkillPanel2.SetData(c2.name, c2.Cb.skills);
            charaUIs.Add(charaList[1].Id, characterPanel2);
            selSkillUIs.Add(charaList[1].Id, selSkillPanel2);
            charaPanels.Add(c2, characterPanel2);

            Character c3 = charaList[2].CharaObject.GetComponent<Character>();
            characterPanel3.Init(c3);
            selSkillPanel3.SetData(c3.name, c3.Cb.skills);
            charaUIs.Add(charaList[2].Id, characterPanel3);
            selSkillUIs.Add(charaList[2].Id, selSkillPanel3);
            charaPanels.Add(c3, characterPanel3);


        }

        public void ShowSkillSelPanel(int id)
        {
            HideAllSkillSelPanel();

            if (selSkillUIs.ContainsKey(id))
            {
                selSkillUIs[id].gameObject.SetActive(true);
            }
        }

        public void ShowSkillSelPanel(Character chara)
        {
            ShowSkillSelPanel(chara.Pc.Id);
        }

        public void UpdateCharacterStatusUI(Character chara)
        {
            if (charaPanels.TryGetValue(chara, out _))
            {
                charaPanels[chara].UpdateUI(chara);
            }
            else
            {
                Debug.LogError($"Can not find character: {chara.Pc.Id}");
            }
        }

        public void UpdateCharacterActionIcon(Character chara)
        {
            Debug.Log("UpdateCharacterAcionIcon");
        }

        public void HideAllSkillSelPanel()
        {
            foreach(SelSkillPanel s in selSkillUIs.Values)
            {
                s.gameObject.SetActive(false);
            }
        }

        
    }
}