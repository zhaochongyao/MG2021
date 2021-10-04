using Constdef;
using UnityEngine;

namespace Managers.MapManager
{
    public class MapItemController : MonoBehaviour
    {
        private int _layoutX;
        private int _layoutY;

        public void Init(int x, int y)
        {
            _layoutX = x;
            _layoutY = y;
        }
        private void OnMouseDown()
        {
            EventCenter.GetInstance().EventTrigger<>(Common.PlayerMoveEvent, MapManager.GetInstance().GetPoint(_layoutX, _layoutY));
        }
    }
}
