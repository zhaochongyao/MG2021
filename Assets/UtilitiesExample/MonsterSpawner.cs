using System.Collections;
using UnityEngine;
using Utilities;
using Utilities.DataStructures; // 引用命名空间


namespace UtilitiesExample
{
    /// <summary>
    /// 怪物生成样例
    /// </summary>
    public class MonsterSpawner : MonoBehaviour
    {
        // 鼠标放在函数上看注释
    
        /// <summary>
        /// 对象池接口
        /// </summary>
        private void ObjectPoolAPI()
        {
            // 对频繁生成销毁的物体，使用对象池

            Transform parent = transform.parent;
            Vector3 position = transform.position;
            GameObject go = null;
        
            // 用法同Instantiate
            go = ObjectPool.Spawn(_monsterPrefab);
            go = ObjectPool.Spawn(_monsterPrefab, parent);
            go = ObjectPool.Spawn(_monsterPrefab, parent, true);
            go = ObjectPool.Spawn(_monsterPrefab, position, Quaternion.identity);
            go = ObjectPool.Spawn(_monsterPrefab, position, Quaternion.identity, parent);

            // 用法同Destroy
            ObjectPool.Return(go);
            ObjectPool.Return(go, 10f);
        }

        /// <summary>
        /// WaitCache接口
        /// </summary>
        private IEnumerator WaitCacheAPI()
        {
            // 减少垃圾回收，可缓存“Wait类对象”
        
            yield return null;
        
            yield return WaitCache.Seconds(1f);
            // 代替 yield return new WaitForSeconds(1f);

            yield return WaitCache.Frames(10);

            yield return WaitCache.FixedUpdate();

            yield return WaitCache.EndOfFrame();
        }
    
        /// <summary> 怪物对象预制体 </summary>
        [SerializeField] private GameObject _monsterPrefab;
    
        private void Start()
        {
            // 启动样例协程
            StartCoroutine(RepeatedSpawnCo());
        }

        /// <summary>
        /// 样例协程
        /// 固定频率生成物体
        /// </summary>
        private IEnumerator RepeatedSpawnCo()
        {
            while (true)
            {
                Vector3 randomPosition = new Vector3
                {
                    x = Random.Range(-10, 10), 
                    y = Random.Range(-10, 10), 
                    z = 0
                };
            
                GameObject go = ObjectPool.Spawn(_monsterPrefab, randomPosition, Quaternion.identity);
                ObjectPool.Return(go, 100f);
                // 代替 Instantiate(_monsterPrefab, randomPosition, Quaternion.identity);
                // 代替 Destroy(go, 100f);
                // 100秒后自动回收
            
                yield return WaitCache.Seconds(1f);
                // 间隔1秒生成
            }
        }
    }
}