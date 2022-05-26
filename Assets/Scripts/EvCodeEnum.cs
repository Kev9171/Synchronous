using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KWY 
{
    public enum EvCode : byte
    {
        // for lobby
        LobbyReady = 10, // �κ񿡼� �غ� �Ϸ� ���� ���� content: bool (only true) - null������ �ٲ㵵 ���� Ȯ�� �ʿ�
        ResLobbyReady = 110, // LobbyReady�� ���� res �� data: [ userId: string, ok: bool, startgame: bool ]

        // for main
        TurnReady = 11, // �� ���� �غ� �Ϸ� ���� ���� content: actionData: Dictionary<int, int>
        SimulEnd = 12, // �ùķ��̼��� �����ٴ� �ñ׳� ���� content: bool (only true) - null ������ �ٲ㵵 ���� Ȯ�� �ʿ�
        GameEnd = 13, // ������ �����ٴ� �ñ׳� ���� content: winnerId: string


        PlayerSkill1 = 21,
        PlayerSkill2 = 22,
        PlayerSkill3 = 23,

        // TurnReady�� ���� res �� data: [ userId: string, ok: bool, startSimul: bool, simulData: Dictionary<int, int> ];
        // if startSimul is false, there is not simulData
        ResTurnReady = 111, 
        ResSimulEnd = 112, // SimulEnd�� ���� res �� data: null
        ResGameEnd = 113, // GameEnd�� ���� res �� data: winnerId: string
    }
}

