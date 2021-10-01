using UnityEngine;
using Utilities.Random;

namespace DialogueSystem
{
    public sealed class DialogueTester : MonoBehaviour
    {
        [SerializeField] private DialogueDataSO[] _dialogueDataSOs;
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                DialoguePlayer.Instance.SendDialogue(RandomEx.Choose(_dialogueDataSOs));
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                DialoguePlayer.Instance.SwapCameraCanvas();
            }
            
            
        }
    }
}
