using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Realtime;
using KWY;

namespace PickScene
{
    public class PickManager : Singleton<PickManager>
    {
        [SerializeField] private GameObject mainCamera;
        [SerializeField] private PickControl pickControl;
        [SerializeField] Tilemap hlMap;
        private Color transparent = new Color(1, 1, 1, 0);
        public CharacterBtn ClickedBtn { get; private set; }


        #region Public Methods

        public void ClearHighlight()
        {
            foreach (var pos in hlMap.cellBounds.allPositionsWithin)
            {
                Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
                if (hlMap.HasTile(localPlace))
                {
                    hlMap.SetTileFlags(localPlace, TileFlags.None);
                    hlMap.SetColor(localPlace, transparent);
                }
            }
        }

        public void PickCharacter(CharacterBtn characterBtn)
        {
            this.ClickedBtn = characterBtn;
        }

        public void PickClear()
        {
            ClickedBtn = null;
        }

        public void Timeout()
        {
            //PickControl.Instance.RandomDeployCharacter();
            PickControl.Instance.SavePickData();
            SceneManager.LoadScene("MainGameScene");
        }

        #endregion

        #region MonoBehaviour CallBacks

        private void Start()
        {
            ClearHighlight();
        }

        #endregion
    }
}
