using UnityEngine;

namespace Managers.MapManager
{
    public class MapManager : BaseManager<MapManager>
    {
        private StarA _start_a_map;

        public MapManager()
        {
            _start_a_map = new StarA();
        }
        public void Init(int w, int h)
        {
            _start_a_map.Init(w, h);
        }

        public void FindResult(StarA.Point a, StarA.Point b)
        {
            Debug.Log("寻找路径: "+a.x+","+a.y+"--->"+b.x+","+b.y);
            _start_a_map.StartAs(a, b);
            _start_a_map.Link(b);
        }
        public StarA.Point GetPoint(int x, int y)
        {
            return _start_a_map.points[x, y];
        }

        public void ReInit()
        {
            _start_a_map.ReInitMap();
        }

        public void SetControllers(MapItemController[,] controllers)
        {
            _start_a_map.Controllers = controllers;
        }
    }
}
