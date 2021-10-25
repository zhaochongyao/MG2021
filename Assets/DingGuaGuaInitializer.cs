using Iphone;
using KeywordSystem;
using Singletons;
using UnityEngine;
using Utilities.DataStructures;

public class DingGuaGuaInitializer : MonoBehaviour
{
    private void Start()
    {
        WaitCache.Delayed(() =>
        {
            KeywordConfigSO keywordConfigSO = GameConfigProxy.Instance.KeywordConfigSO;
            foreach (string keyword in keywordConfigSO.KeywordListSO.KeywordList)
            {
                KeywordCollector.Instance.Collect(keyword);
            }

            foreach (MergeOnlyKeyword mergeOnlyKeyword in keywordConfigSO.KeywordListSO.MergeOnlyKeywordList)
            {
                DingGuaGua.Instance.AddBackground(mergeOnlyKeyword.Keyword);
            }
        }, 0.1f);
    }
}
