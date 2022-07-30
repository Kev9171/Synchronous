using UnityEngine;

namespace KWY
{
    [CreateAssetMenu(fileName = "MoveAction", menuName = "MoveAction/MoveBase")]
    public class MoveBase : ActionBase
    {
        public  int distance;

        public override string ToString()
        {
            return string.Format("Distance: {0}, Cost: {1}", distance, cost);
        }
    }
}
