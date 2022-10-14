using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KWY
{
    public class ResultData
    {
        public List<PlayableCharacter> MyTeamCharacters { get; private set; }

        public int PlayerSkillCount { get; private set; }

        public ResultData(List<PlayableCharacter> myTeamCharacters, Player player)
        {
            MyTeamCharacters = myTeamCharacters;
            PlayerSkillCount = player.SkillCount;
        }

        public void SetData(List<PlayableCharacter> myTeamCharacters, int playerSkillCount)
        {
            MyTeamCharacters = myTeamCharacters;
            PlayerSkillCount = playerSkillCount;
        }
    }
}
