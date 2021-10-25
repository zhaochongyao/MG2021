using System.Collections.Generic;
using Utilities.DesignPatterns;

namespace DialogueSystem
{
    public class DialogueEventInvoker : LSingleton<DialogueEventInvoker>
    {
        private HashSet<string> _eventNameSet;

        private void Start()
        {
            _eventNameSet = new HashSet<string>();
        }

        public bool Check(string eventName)
        {
            return _eventNameSet.Contains(eventName);
        }

        public void InvokeContinueEvent(string eventName)
        {
            _eventNameSet.Add(eventName);
        }
    }
}