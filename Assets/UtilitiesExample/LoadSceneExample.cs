using UnityEngine;
using UnityEngine.UI;
using Utilities; // 引用命名空间

namespace UtilitiesExample
{
    public class LoadSceneExample : MonoBehaviour
    {
        private void SceneLoaderAPI()
        {
            SceneLoader.LoadScene("sceneName");
            SceneLoader.LoadScene("sceneName", 1f);
        }

        [SerializeField] private Slider _loadingBar;

        private void UpdateLoadingBar(float progress)
        {
            _loadingBar.value = progress;
        }
    
        private void Start()
        {
            SceneLoader.ProgressUpdate += UpdateLoadingBar;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                SceneLoader.LoadScene("LoadingTargetScene", 1f);
                // 加载完毕后，延迟1秒再进入目标场景
                // 避免进度条一满瞬间进入目标场景，给玩家留下反应的时间
            }
        }

        private void OnDestroy()
        {
            SceneLoader.ProgressUpdate -= UpdateLoadingBar;
        }
    }
}
