using UnityEngine;

namespace Iphone
{
    [CreateAssetMenu(fileName = "IphoneConfigSO", menuName = "Iphone/Iphone Config")]
    public class IphoneConfigSO : ScriptableObject
    {
        [SerializeField] private string _password;

        public string Password => _password;
    }
}