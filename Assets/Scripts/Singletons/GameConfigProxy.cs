using ScriptableObjects;
using UnityEngine;
using Utilities.DesignPatterns;

namespace Singletons
{
    public class GameConfigProxy : GSingleton<GameConfigProxy>
    {
        [SerializeField] private GizmosColorSettingSO _gizmosColorSettingSO;

        public GizmosColorSettingSO GizmosColorSetting => _gizmosColorSettingSO;

    }
}
