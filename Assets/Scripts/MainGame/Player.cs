using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using Photon.Pun;

using UI;

namespace KWY
{
    public class Player : MonoBehaviour, ISubject<Player>
    {
        public List<IObserver<Player>> Observers
        {
            get;
            private set;
        } = new List<IObserver<Player>>();

        public int Mp
        {
            get;
            private set;
        } = 0;

        public Image PlayerImg
        {
            get;
            private set;
        }

        public readonly int MaxMp = 10;

        public int SkillCount
        {
            get;
            private set;
        }

        [SerializeField]
        Tilemap map;
        [SerializeField]
        CharacterControl chCtrl;
        [SerializeField]
        MouseInput mouseInput;

        [SerializeField]
        Canvas UICanvas;


        public void InitData(Image icon, int initialMp)
        {
            Mp = initialMp;
            PlayerImg = icon;
        }

        public void AddMp(int value)
        {
            Mp = (Mp + value > MaxMp) ? MaxMp : Mp + value;

            NotifyObservers();
        }

        public void SubMp(int value)
        {
            Mp = (Mp - value < 0) ? 0 : Mp - value; 

            NotifyObservers();
        }

        public bool Skill1(Character selChara, Vector2 mousePosition)
        {
            if (selChara == null) return false;

            if (selChara.BreakDown)
            {
                // 아마 실행 되면 안됨
                PanelBuilder.ShowFadeOutText(UICanvas.transform, "This character is breakdown!");
                return false;
            }

            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

            Vector3Int clickV = map.WorldToCell(mousePosition);

            if (map.HasTile(clickV))
            {
                if (clickV.y % 2 != selChara.TilePos.y % 2)
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        GameManager.Instance.Simulation.ChangeAction((int)selChara.Cb.cid, clickV.y);
                    }
                }
                //selChara.Teleport(clickV);
                selChara.photonView.RPC("Teleport", RpcTarget.MasterClient, clickV.x, clickV.y);
                //GameManager.Instance.Simulation.ShowAction(selChara.Pc.Id);

                SkillCount++;
                return true;
            }

            return false;
        }

        #region ISubject Methods
        public void AddObserver(IObserver<Player> o)
        {
            if (Observers.IndexOf(o) < 0)
            {
                Observers.Add(o);
            }
            else
            {
                Debug.LogWarning($"The observer already exists in list: {o}");
            }
        }

        public void NotifyObservers()
        {
            foreach (IObserver<Player> o in Observers)
            {
                o.OnNotify(this);
            }
        }

        public void RemoveAllObservers()
        {
            Observers.Clear();
        }

        public void RemoveObserver(IObserver<Player> o)
        {
            int idx = Observers.IndexOf(o);
            if (idx >= 0)
            {
                Observers.RemoveAt(idx); // O(n)
            }
            else
            {
                Debug.LogError($"Can not remove the observer; It does not exist in list: {o}");
            }
        }
        #endregion
    }
}