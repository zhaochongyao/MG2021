namespace Utilities.DesignPatterns
{
    /// <summary>
    /// 命令接口
    /// </summary>
    public interface ICommand
    {
        /// <summary> 执行 </summary>
        void Execute();

        /// <summary> 撤销 </summary>
        void Revoke();
    }
}
