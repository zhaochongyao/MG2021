using Constdef;
using UnityEngine;
using utils;

namespace Managers.MapManager
{
    public class MapGenerate : MonoBehaviour
    {
        public GameObject generateItem; // 生成时用于拼接的item
        
        public int splitColumn;      // 地图分割成多少行
        public int splitRow;         // 地图分割成多少列
        
        private float _width;        // 整个地图的宽度
        private float _height;       // 整个地图的长度
        
        private void Start()
        {
            var localTransform = transform;
            var renderSize = localTransform.GetComponent<SpriteRenderer>().size;
            var localScale = localTransform.localScale;
            _width = renderSize.x * localScale.x;
            _height = renderSize.y * localScale.y;
            GenerateMap();
        }

        /// <summary>
        /// 根据预设参数生成地图
        /// </summary>
        private void GenerateMap()
        {
            if (generateItem == null)
            {
                LogUtil.LogError(ConstDefine.TransformNullError);
                return;
            }
            if (splitColumn <= 0 || splitRow <= 0)
            {
                LogUtil.LogError(ConstDefine.MapSplitNumError);
                return;
            }
            var parentPosition = transform.position - new Vector3(_width/2f, 0,  _height/2f);
            var parentRotation = gameObject.transform.rotation;
            var itemWidth = _width / splitRow;
            var itemHeight = _height / splitColumn;
            MapManager.GetInstance().Init(splitRow, splitColumn);
            MapItemController[,] controllers = new MapItemController[splitColumn, splitRow];
            for (var column = 0; column < splitColumn; column++)
            {
                for (var row = 0; row < splitRow; row++)
                {
                    var item = GameObject.Instantiate(generateItem,  parentPosition + new Vector3(row*itemWidth, 0, column*itemHeight), parentRotation, null);
                    var itemLocalScale = item.transform.localScale;
                    itemLocalScale = new Vector3(itemLocalScale.x / splitRow, itemLocalScale.y / splitColumn, itemLocalScale.z);
                    item.transform.localScale = itemLocalScale;
                    item.layer = LayerMask.NameToLayer(ConstDefine.MapLayer);
                    item.AddComponent<BoxCollider>();
                    var mapItemManager = item.GetComponent<MapItemController>();
                    if (mapItemManager != null)
                    {
                        mapItemManager.Init(column, row);
                        controllers[column, row] = mapItemManager;
                    }
                    MapManager.GetInstance().GetPoint(column, row).Layout = item.transform.position ;
                }
            }
            MapManager.GetInstance().SetControllers(controllers);
            Destroy(gameObject);
        }
    }
}