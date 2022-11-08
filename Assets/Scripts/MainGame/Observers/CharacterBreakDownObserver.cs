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
            if (!chara.BreakDownNotice && chara.BreakDown)
            {
                Team team = chara.Pc.Team;

                if (team == Team.A)
                {
                    MainGameData.Instance.NotBreakDownTeamA--;
                    if (MainGameData.Instance.TryGetActionData(chara.Pc.Id, out var data))
                    {
                        data.ClearActionData();
                    }
                    MainGameData.Instance.CharaActionData.Remove(chara.Pc.Id);
                    chara.BreakDownNotice = true;
                }
                else if (team == Team.B)
                {
                    MainGameData.Instance.NotBreakDownTeamB--;
                    if (MainGameData.Instance.TryGetActionData(chara.Pc.Id, out var data))
                    {
                        data.ClearActionData();
                    }
                    MainGameData.Instance.CharaActionData.Remove(chara.Pc.Id);
                    chara.BreakDownNotice = true;
                }
            }
        }
    }
}
