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

        int time = 0;
        int tMax;
        ActionData nowActionData;
        STATE nowState = 0;

        #region Public Methods

        public void ClearHighlight()
        {
            //Vector3 pos = new Vector3(0, 0, 0);
            //Vector3Int range;
            //for (float i = -4; i < 5; i += 0.5f)
            //{
            //    for (float j = -6; j < 6; j += 0.7f)
            //    {
            //        pos.x = j;
            //        pos.y = i;
            //        range = hlMap.WorldToCell(pos);
            //        this.hlMap.SetTileFlags(range, TileFlags.None);
            //        this.hlMap.SetColor(range, transparent);
            //    }
            //}
            foreach (var pos in hlMap.cellBounds.allPositionsWithin)
            {
                Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
                Vector3 place = hlMap.CellToWorld(localPlace);
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
            PickControl.Instance.RandomDeployCharacter();
            SceneManager.LoadScene("MainGameScene");
        }

        #endregion

        #region MonoBehaviour CallBacks

        private void Start()
        {
            //data.LoadData();
            //SetState(0);
            ClearHighlight();
        }

        #endregion
    }
}
