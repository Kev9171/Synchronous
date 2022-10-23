using UnityEngine;

namespace KWY
{
    public class GameOverObserver : IObserver
    {
        public void OnNotify()
        {
            // 1: 무승부
            // 2: MasterClient 승
            // 3: OtherClient 승

            bool teamALose = MainGameData.Instance.NotBreakDownTeamA == 0;
            bool teamBLose = MainGameData.Instance.NotBreakDownTeamB == 0;

            if (!teamALose && !teamBLose)
            {
                return;
            }

            if (teamALose && teamBLose)
            {
                MainGameEvent.Instance.RaiseEventGameEnd(TICK_RESULT.DRAW);
            }
            else if (!teamALose && !teamBLose)
            {
                MainGameEvent.Instance.RaiseEventGameEnd(TICK_RESULT.MASTER_WIN);
            }
            else if (teamALose && !teamBLose)
            {
                MainGameEvent.Instance.RaiseEventGameEnd(TICK_RESULT.CLIENT_WIN);
            }
            
        }
    }
}
