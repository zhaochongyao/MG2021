using System.Collections.Generic;

namespace Constdef
{
    public class MyError
    {
        private string _error_log;
        private int _error_code;

        public MyError()
        {
        }
        
        public MyError(string errorLOG, int errorCode)
        {
            _error_log = errorLOG;
            _error_code = errorCode;
        }

        public override string ToString()
        {
            return "error code: "+_error_code+"\t error log: "+_error_log;
        }
    }
    
    public class ConstDefine
    {
        public static MyError CameraNullError = new MyError("<point err:摄像机为空!>", 10001);
        public static MyError TransformNullError = new MyError("<point err:Transform为空!>", 10002);
    }
}