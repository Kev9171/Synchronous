using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KWY 
{
    public enum EvCode : byte
    {
        // for lobby
        LobbyReady = 10, // 로비에서 준비 완료 상태 전송 content: bool (only true) - null값으로 바꿔도 될지 확인 필요
        ResLobbyReady = 110, // LobbyReady에 대한 res 값 data: [ userId: string, ok: bool, startgame: bool ]

        // for main
        TurnReady = 11, // 턴 시작 준비 완료 상태 전송 content: actionData: Dictionary<int, int>
        SimulEnd = 12, // 시뮬레이션이 끝나다는 시그널 전송 content: bool (only true) - null 값으로 바꿔도 될지 확인 필요
        GameEnd = 13, // 게임이 종료됬다는 시그널 전송 content: winnerId: string


        PlayerSkill1 = 21,
        PlayerSkill2 = 22,
        PlayerSkill3 = 23,

        // TurnReady에 대한 res 값 data: [ userId: string, ok: bool, startSimul: bool, simulData: Dictionary<int, int> ];
        // if startSimul is false, there is not simulData
        ResTurnReady = 111, 
        ResSimulEnd = 112, // SimulEnd에 대한 res 값 data: null
        ResGameEnd = 113, // GameEnd에 대한 res 값 data: winnerId: string
    }
}

