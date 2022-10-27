using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using KWY;

namespace UI
{
    public static class PanelBuilder
    {
        public static void ShowPlayerSkillInfoPanel(Transform parent, PlayerSkillBase psb)
        {
            GameObject playerSkillInfoPanel = GameObject.Instantiate(
                Resources.Load(
                    "Prefabs/UI/Game/PlayerSkillInfoPanel", 
                    typeof(GameObject)
                    )) as GameObject;

            playerSkillInfoPanel.transform.SetParent(parent, false);
            playerSkillInfoPanel.GetComponent<PlayerSkillInfoPanel>().SetData(psb);
        }

        public static void ShowCharacterInfoPanel(Transform parent, CharacterBase cb)
        {
            GameObject characterInfoPanel = GameObject.Instantiate(
                Resources.Load(
                    "Prefabs/UI/Game/CharacterInfoPanel",
                    typeof(GameObject)
                    )) as GameObject;

            characterInfoPanel.transform.SetParent(parent, false);
            characterInfoPanel.GetComponent<CharacterInfoPanel>().SetData(cb);
        }

        public static void ShowSkillInfoPanel(Transform parent, SkillBase sb)
        {
            GameObject skillInfoPanel = GameObject.Instantiate(
                Resources.Load(
                    "Prefabs/UI/Game/SkillInfoPanel",
                    typeof(GameObject)
                    )) as GameObject;

            skillInfoPanel.transform.SetParent(parent, false);
            skillInfoPanel.GetComponent<SkillInfoPanel>().SetData(sb);
        }

        public static GameObject ShowBuffInfoPanel(Transform parent, BuffBase bb)
        {
            GameObject buffInfoPanel = GameObject.Instantiate(
                Resources.Load(
                    "Prefabs/UI/Game/BuffInfoPanel",
                    typeof(GameObject)
                    )) as GameObject;

            buffInfoPanel.transform.SetParent(parent, false);
            buffInfoPanel.GetComponent<BuffInfoPanel>().SetData(bb);

            return buffInfoPanel;
        }

        public static void ShowResultPanel(Transform parent, WINLOSE result, ResultData data)
        {
            GameObject resultPanel = GameObject.Instantiate(
                Resources.Load(
                    "Prefabs/UI/Game/ResultPanel",
                    typeof(GameObject)
                    )) as GameObject;

            resultPanel.transform.SetParent(parent, false);
            resultPanel.GetComponent<ResultPanel>().SetData(result, data);
        }

        public static void ShowFadeOutText(Transform parent, string content)
        {
            GameObject fadeOutText = GameObject.Instantiate(
                Resources.Load(
                    "Prefabs/UI/Game/FadeOutText",
                    typeof(GameObject))) as GameObject;

            fadeOutText.transform.SetParent(parent, false);
            fadeOutText.GetComponent<FadeOutText>().Init(content);
        }
    }
}
