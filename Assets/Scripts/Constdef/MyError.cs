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
    
    public static class ConstDefine
    {
        public static readonly MyError CameraNullError = new MyError("<null err:摄像机为空!>", 10001);
        public static readonly MyError TransformNullError = new MyError("<null err:Transform为空!>", 10002);
        public static readonly MyError GenerateItemNullError = new MyError("<null error:地图预设Item为空!>", 10003);
        public static readonly MyError MapSplitNumError = new MyError("<number error:split number error!>", 10003);


        public static readonly string MapLayer = "Map";
    }
}