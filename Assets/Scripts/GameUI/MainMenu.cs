using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Utilities.DesignPatterns;

namespace GameUI
{
    public class MainMenu : LSingleton<MainMenu>
    {
        [SerializeField] private string _startChapter;
        
        public void StartGame()
        {
            SceneLoader.LoadScene("");
        }

        public void ContinueGame()
        {
            
        }

        public void ToOurTeam()
        {
            
            
        }
        
        public void ToSetting()
        {
            
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
