using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace KWY
{
    public class CharacterManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        public Tilemap map;
        MouseInput mouseInput;
        [SerializeField] private float movementSpeed;

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

        #region Private Fields
        public ICharacter character;
        private bool moveState, atkState = true;
        private int selOption = 1;
        //private int index = 0; 
        //private Vector3 destination;
        #endregion

        #region MonoBehaviourCallbacks
        void Awake()
        {
            character = this.GetComponent<ICharacter>();
            if (character != null)
            {
                Debug.LogFormat("Character assigned: {0}", character.CLASS);
            }
            else
            {
                Debug.LogFormat("Character  not assigned");
            }
            mouseInput = new MouseInput();
        }

        void Start()
        {
            mouseInput.Mouse.MouseClick.performed += OnClick;
        }
        #endregion

        public void MoveSelectBtn()
        {
            if(character != null)
            {
                mouseInput.Mouse.MouseClick.performed -= OnClick;
                mouseInput.Mouse.MouseClick.performed += CharMoveSelect;
                Debug.Log("movesel");
            }
            else
                Debug.Log("no character selected");


            //if (!moveState)
            //{
            //    mouseInput.Mouse.MouseClick.performed -= CharMoveSelect;
            //    mouseInput.Mouse.MouseClick.performed += OnClick;
            //    moveState = !moveState;
            //    character = null;
            //    Debug.Log("onclick :");
            //}
            //else
            //{
            //    mouseInput.Mouse.MouseClick.performed -= OnClick;
            //    mouseInput.Mouse.MouseClick.performed += CharMoveSelect;
            //    moveState = !moveState;

            //}
        }

        public void AttackSelectBtn()
        {
            if(character != null)
            {
                mouseInput.Mouse.MouseClick.performed -= OnClick;
                mouseInput.Mouse.MouseClick.performed += CharAttackSelect;
                Debug.Log("atksel");
            }
            else
                Debug.Log("no character selected");
            

            //if (!atkState)
            //{
            //    mouseInput.Mouse.MouseClick.performed -= CharAttackSelect;
            //    mouseInput.Mouse.MouseClick.performed += OnClick;
            //    atkState = !atkState;
            //    character = null;
            //}
            //else
            //{
            //    mouseInput.Mouse.MouseClick.performed -= OnClick;
            //    mouseInput.Mouse.MouseClick.performed += CharAttackSelect;
            //    atkState = !atkState;
            //}
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            Vector2 mousePosition = mouseInput.Mouse.MousePosition.ReadValue<Vector2>();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

            if (hit.collider != null)
            {
                character = hit.collider.gameObject.GetComponent<ICharacter>();

                Debug.Log("Character :" + character);
                //Debug.Log("moves :" + character.Moves.Count);
                //Debug.Log("Target Name: " + hit.collider.gameObject.name);
            }
            //Debug.Log("click");
        }

        private new void OnEnable()
        {
            mouseInput.Enable();
        }
        private new void OnDisable()
        {
            mouseInput.Disable();
        }


        void CharMoveSelect(InputAction.CallbackContext context)
        {

            Vector2 mousePosition = mouseInput.Mouse.MousePosition.ReadValue<Vector2>();
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            Vector3Int gridPosition = map.WorldToCell(mousePosition);
            //Debug.Log("map :" + map.HasTile(gridPosition));
            //Debug.Log("moves :" + character.Moves.Count);
            if (map.HasTile(gridPosition) && character.Moves.Count < 3)
            {
                Vector2 deltaXY = ((Vector2Int)gridPosition) - character.position;
                object[] arr = { "move", deltaXY.x, deltaXY.y };
                //Debug.Log(arr[1] + ", " + arr[2]);
                character.Moves.Add(character.Moves.Count, arr);
                character.position = (Vector2Int)gridPosition;
                Debug.Log("tile selected");
            }
            else if (!map.HasTile(gridPosition))
                Debug.Log("no tile selected");
            else
            {
                foreach (KeyValuePair<int, object[]> item in character.Moves)
                {
                    Debug.Log(item.Key + ", " +item.Value[0] + ", " + item.Value[1] + ", " + item.Value[2]);
                }
            }


            //switch (selOption)
            //{
            //    case 0:
            //        Debug.Log("nothing selected");
            //        break;
            //    case 1:
            //        Debug.Log("movement selected");

                    

            //        break;
            //    case 2:
            //        Debug.Log("skill selected");
            //        break;
            //}
            //}
            
        }

        void CharAttackSelect(InputAction.CallbackContext context)
        {
            Vector2 mousePosition = mouseInput.Mouse.MousePosition.ReadValue<Vector2>();
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            Vector3Int gridPosition = map.WorldToCell(mousePosition);
            //Debug.Log("map :" + map.HasTile(gridPosition));
            //Debug.Log("moves :" + character.Moves.Count);
            Vector2 deltaXY = ((Vector2Int)gridPosition) - character.position;
            if (map.HasTile(gridPosition) && deltaXY.sqrMagnitude <= 2 && character.Moves.Count < 3)
            {
                object[] arr = { "skill", deltaXY.x, deltaXY.y, selOption };
                //Debug.Log(arr[1] + ", " + arr[2]);
                character.Moves.Add(character.Moves.Count, arr);
                //character.position = (Vector2Int)gridPosition;
                Debug.Log("attack selected");
            }
            else if (deltaXY.sqrMagnitude > 2)
                Debug.Log("outside the atk range");
            else
            {
                foreach (KeyValuePair<int, object[]> item in character.Moves)
                {
                    Debug.Log(item.Key + ", " + item.Value[0] + ", " + item.Value[1] + ", " + item.Value[2]);
                }
            }
        }


        void Update()
        {
            if (mouseInput.Mouse.MouseClick.IsPressed())
            {
                mouseInput.Mouse.MouseClick.performed -= CharMoveSelect;
                mouseInput.Mouse.MouseClick.performed -= CharAttackSelect;
                mouseInput.Mouse.MouseClick.performed += OnClick;
            }
        }
    }
}

