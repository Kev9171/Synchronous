using Photon.Pun;

using UnityEngine;

namespace KWY
{
    public class CharacterManager : MonoBehaviourPunCallbacks, IPunObservable
    {
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
        private ICharacter character;
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
        }
        #endregion
    }
}

