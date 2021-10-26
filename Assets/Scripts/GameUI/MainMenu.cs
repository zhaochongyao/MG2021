using System;
using UnityEngine;
using Utilities;
using Utilities.DesignPatterns;

namespace GameUI
{
    public class MainMenu : LSingleton<MainMenu>
    {
        [SerializeField] private string _startChapterName;

        [SerializeField] private GameObject _mainMenu;
        [SerializeField] private GameObject _achievement;
        [SerializeField] private GameObject _outTeam;

        private void Start()
        {
            ToMainMenu();
        }

        public void StartGame()
        {
            SceneLoader.LoadScene(_startChapterName);
        }

        public void ContinueGame()
        {
            
        }

        public void ToAchievement()
        {
            _mainMenu.SetActive(false);
            _achievement.SetActive(true);
            _outTeam.SetActive(false);
        }
        
        public void ToOurTeam()
        {
            _mainMenu.SetActive(false);
            _achievement.SetActive(false);
            _outTeam.SetActive(true);
        }

        public void ToMainMenu()
        {
            _mainMenu.SetActive(true);
            _achievement.SetActive(false);
            _outTeam.SetActive(false);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
