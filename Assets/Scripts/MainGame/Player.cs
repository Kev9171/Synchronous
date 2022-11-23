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
                bool yDiff = false;
                if (clickV.y % 2 != selChara.TilePos.y % 2)
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        //GameManager.Instance.Simulation.ChangeAction(selChara.Pc.Id, clickV.y);
                    }
                    yDiff = true;
                }
                //selChara.Teleport(clickV);
                selChara.photonView.RPC("Teleport", RpcTarget.MasterClient, clickV.x, clickV.y, selChara.Pc.Id, yDiff);
                //GameManager.Instance.Simulation.ShowAction(selChara.Pc.Id);

                SkillCount++;
                return true;
            }

            return false;
        }

        public bool Skill2(Vector2 mousePosition)
        {
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

            Vector3Int clickV = map.WorldToCell(mousePosition);

            if (map.HasTile(clickV))
            {
                //Meteor(clickV, 1);
                TilemapControl TCtrl = GameObject.Find("TilemapControl").GetComponent<TilemapControl>();
                List<Vector2Int> v = clickV.y % 2 == 0 ? MoveManager.MoveData.areaEvenY : MoveManager.MoveData.areaOddY;
                foreach (Vector2Int vec in v)
                {
                    Vector2Int newVec = (Vector2Int)clickV + vec;
                    if (map.HasTile((Vector3Int)newVec))
                    {
                        List<GameObject> ch = TCtrl.getCharList((Vector3Int)newVec);
                        if (ch.Count != 0)
                        {
                            foreach (GameObject c in ch)
                            {
                                DataController.Instance.ModifyCharacterHp(c.GetComponent<Character>().Pc.Id, -100);
                                Debug.Log(c.name + "hit by meteor");
                            }
                        }
                    }
                }
                List<GameObject> ch2 = TCtrl.getCharList(clickV);
                foreach (GameObject c in ch2)
                {
                    DataController.Instance.ModifyCharacterHp(c.GetComponent<Character>().Pc.Id, -100);
                    Debug.Log(c.name + "hit by meteor");
                }
                GameObject obj = PhotonNetwork.Instantiate(
                        "EffectExamples/Fire & Explosion Effects/Prefabs/BigExplosion",
                        map.CellToWorld(clickV),
                        Quaternion.identity);
                StartCoroutine(DestoryAfterTime(obj, 1));
                SkillCount++;
                return true;
            }
            return false;
        }

        IEnumerator DestoryAfterTime(GameObject obj, float time)
        {
            yield return new WaitForSeconds(time);
            PhotonNetwork.Destroy(obj);
        }
        //IEnumerator Meteor(Vector3Int clickV, float time)
        //{
        //    yield return new WaitForSeconds(0.75f);
        //    TilemapControl TCtrl = GameObject.Find("TilemapControl").GetComponent<TilemapControl>();
        //    List<Vector2Int> v = clickV.y % 2 == 0 ? MoveManager.MoveData.areaEvenY : MoveManager.MoveData.areaOddY;
        //    foreach (Vector2Int vec in v)
        //    {
        //        Vector2Int newVec = (Vector2Int)clickV + vec;
        //        if (map.HasTile((Vector3Int)newVec))
        //        {
        //            List<GameObject> ch = TCtrl.getCharList((Vector3Int)newVec);
        //            if (ch.Count != 0)
        //            {
        //                foreach (GameObject c in ch)
        //                {
        //                    DataController.Instance.ModifyCharacterHp(c.GetComponent<Character>().Pc.Id, -100);
        //                    Debug.Log(c.name + "hit by meteor");
        //                }
        //            }
        //        }
        //    }
        //    List<GameObject> ch2 = TCtrl.getCharList(clickV);
        //    foreach (GameObject c in ch2)
        //    {
        //        DataController.Instance.ModifyCharacterHp(c.GetComponent<Character>().Pc.Id, -100);
        //        Debug.Log(c.name + "hit by meteor");
        //    }
        //    GameObject obj = PhotonNetwork.Instantiate(
        //            "EffectExamples/Fire & Explosion Effects/Prefabs/BigExplosion",
        //            map.CellToWorld(clickV),
        //            Quaternion.identity);
        //    yield return new WaitForSeconds(time);
        //    PhotonNetwork.Destroy(obj);
        //    //clone.RemoveAt(clone.Count - 1);
        //}

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