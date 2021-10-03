using UnityEngine;

namespace Managers.MapManager
{
    public class MapItemController : MonoBehaviour
    {
        private int _layout_x;
        private int _layout_y;

        public void Init(int x, int y)
        {
            _layout_x = x;
            _layout_y = y;
        }
        private void OnMouseDown()
        {
            EventCenter.GetInstance().EventTrigger<StarA.Point>("player_move_event", MapManager.GetInstance().GetPoint(_layout_x, _layout_y));
            Debug.Log("move to "+_layout_x+", "+_layout_y);
            
        }
    }
}
