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
        private bool state = true;
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
            
            //CharMoveSelect();
            if (!state)
            {
                mouseInput.Mouse.MouseClick.performed -= CharMoveSelect;
                mouseInput.Mouse.MouseClick.performed += OnClick;
                state = !state;
                character = null;
                Debug.Log("onclick :");
            }
            else
            {
                mouseInput.Mouse.MouseClick.performed -= OnClick;
                mouseInput.Mouse.MouseClick.performed += CharMoveSelect;
                state = !state;
                Debug.Log("movesel :");
            }
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            Vector2 mousePosition = mouseInput.Mouse.MousePosition.ReadValue<Vector2>();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

            if (hit.collider != null)
            {
                character = hit.collider.gameObject.GetComponent<ICharacter>();

                //CManager.character = this;
                Debug.Log("Character :" + character);
                Debug.Log("moves :" + character.Moves.Count);
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
            int loop = 6;
            //while (loop != 0)
            //{


            switch (selOption)
            {
                case 0:
                    Debug.Log("nothing selected");
                    break;
                case 1:
                    Debug.Log("movement selected");

                    Vector2 mousePosition = mouseInput.Mouse.MousePosition.ReadValue<Vector2>();
                    mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
                    Vector3Int gridPosition = map.WorldToCell(mousePosition);
                    //Debug.Log("map :" + map.HasTile(gridPosition));
                    //Debug.Log("moves :" + character.Moves.Count);
                    if (map.HasTile(gridPosition) && character.Moves.Count < 3)
                    {
                        character.Moves.Add(character.Moves.Count, gridPosition.ToString());
                        Debug.Log("tile selected");
                        //character.moveCnt--;
                    }
                    else if(!map.HasTile(gridPosition))
                        Debug.Log("no tile selected");
                    else
                    {
                        foreach (KeyValuePair<int, string> item in character.Moves)
                        {
                            Debug.Log(item.Key +", " +item.Value);
                        }
                    }

                    break;
                case 2:
                    Debug.Log("skill seleceted");
                    break;
            }
            //}
            
        }


        void Update()
        {

        }
    }
}

