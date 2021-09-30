using Constdef;
using UnityEngine;

namespace utils
{
    public class LogUtil
    {
        

        public static void LogError(MyError error)
        {
            Debug.LogError(error.ToString());
        }
        
    }
}