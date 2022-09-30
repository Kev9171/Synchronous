using System.Collections;
using UnityEngine;

namespace KWY
{
    public class PlayableCharacter
    {
        private readonly int _id;
        private readonly Team _team;

        private readonly GameObject _charaObject;
        private readonly Character _baseCharacter = null;

        public GameObject CharaObject
        {
            get
            {
                return _charaObject;
            }
        }

        public Character Chara
        {
            get
            {
                return _baseCharacter;
            }
        }

        public int Id
        {
            get
            {
                return _id;
            }
        }

        public PlayableCharacter(GameObject charaObject, int id, Team team)
        {
            _charaObject = charaObject;
            _id = id;
            _team = team;

            _baseCharacter = _charaObject.GetComponent<Character>();

            if (!_baseCharacter)
            {
                Debug.Log($"Can not find the component : Character in GameObject - {_charaObject}");
            }
        }

        public override string ToString()
        {
            return $"[id: {_id}, team: {_team}, character: {_baseCharacter}]";
        }
    }
}