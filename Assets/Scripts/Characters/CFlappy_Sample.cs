using Photon.Pun;

using UnityEngine;
using UnityEngine.Tilemaps;

namespace KWY
{
    public class CFlappy_Sample : MonoBehaviourPunCallbacks, ICharacter, IPunObservable
    {
        # region Const Fields
        [Tooltip("Initial Health Points")]
        const float _HP = 100f;
        [Tooltip("Initial Attack Points")]
        const float _ATK = 10f;
        [Tooltip("Character Type(Class)")]
        const CClass _CLASS = CClass.Class_A;

        #endregion

        #region Tilemap Fields
        public Tilemap map;
        [SerializeField] private float movementSpeed;
        private Vector2 destination;
        #endregion

        #region ICharacter Implementation

        public CClass CLASS { get; private set; } = _CLASS;

        public float ATK { get; private set; } = _ATK;

        public float HP { get; private set; } = _HP;

        public float YCorrectionValue { get; private set; }

        public void Damage(float damage)
        {
            this.HP -= damage;
        }

        public void MoveTo(int row, int col)
        {
            Debug.LogFormat("MoveTo: row({0}), col({1})", row, col);
            Vector3Int gridPosition = map.WorldToCell(new Vector3(row, col, 0));
            Vector2 gridCenter = map.CellToWorld(gridPosition);
            gridCenter.y += YCorrectionValue;
            if (map.HasTile(gridPosition))
            {
                Debug.LogFormat("Grid Pos={0}, World Pos={1}", gridPosition, gridCenter);
                destination = gridCenter;
            }
        }

        public void CastSkill()
        {
            Debug.LogFormat("CastSkill: ");
        }

        #endregion


        #region Private Fields
        public static GameObject LocalPlayerInstance;
        #endregion

        #region Public Fields
        #endregion

        
        #region  IPunObservable implementation
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
            }
            else{
                // Network player, receive data
            }
        }
        #endregion

        #region MonoBehaviour CallBacks

        void Awake()
        {
            // 오브젝트가 타일 위에 있는 것처럼 보이도록 y축 값을 보정해주는 값
            this.YCorrectionValue = transform.localScale.y/2;
        }

        void Update()
        {
            if(Vector2.Distance(transform.position, destination) > 0)
            {
                transform.position = Vector2.MoveTowards(
                    transform.position, 
                    destination,
                    movementSpeed * Time.deltaTime);
            }
        }
        #endregion
    }
}

