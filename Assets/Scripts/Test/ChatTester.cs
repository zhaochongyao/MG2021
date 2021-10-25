﻿using GameUI;
using Iphone.ChatSystem;
using UnityEngine;

namespace Test
{
    public class ChatTester : MonoBehaviour
    {
        [SerializeField] private ChatLineListSO _chatLineListSO;
        [SerializeField] private ChatLineListSO _groupTest;
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                ChatPlayer.Instance.SendChat(_chatLineListSO);
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                ChatPlayer.Instance.SendChat(_groupTest);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                SelfTalkManager.Instance.PlaySelfTalk(
                    "得了ICPC金牌真的好爽啊！嘿嘿", 2f);  
            }
        }
    }
}