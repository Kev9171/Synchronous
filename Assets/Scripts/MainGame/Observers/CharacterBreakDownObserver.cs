using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KWY
{
    public class CharacterBreakDownObserver : IObserver<Character>
    {
        public void OnNotify(Character chara)
        {
            if (chara.BreakDown)
            {
                Team team = chara.Pc.Team;

                if (team == Team.A)
                {
                    MainGameData.Instance.NotBreakDownTeamA--;
                }
                else if (team == Team.B)
                {
                    MainGameData.Instance.NotBreakDownTeamB--;
                }
            }
        }
    }
}
