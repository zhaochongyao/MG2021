using System;
using System.Collections.Generic;
using System.IO;
using Constdef;
using Newtonsoft.Json;
using UnityEngine;
using utils;

namespace Managers.AchieveManager
{
    public class AchievementMgr : BaseManager<AchievementMgr>
    {
        private static Dictionary<string, Achievement> doneAchievement;
        private static Dictionary<string, AchieveEvent> doneEvent;
        private static Dictionary<string, Achievement> totalAchievement;
        private static Dictionary<string, AchieveEvent> totalEvent;

        public void Init()
        {
            doneAchievement = new Dictionary<string, Achievement>();
            doneEvent = new Dictionary<string, AchieveEvent>();
            totalAchievement = new Dictionary<string, Achievement>();
            totalEvent = new Dictionary<string, AchieveEvent>();
            GenerateTotal();
            // GenerateDone();
            EventCenter.GetInstance().AddEventListener<string>("DoneEvent", DoneEvent);
        }

        private void GenerateDone()
        {
            var doneAchievementList = JsonConvert.DeserializeObject<List<Achievement>>(ReadJsonStr(""));
            var doneEventList = JsonConvert.DeserializeObject<List<AchieveEvent>>(ReadJsonStr(""));
            if (doneAchievementList == null || doneEventList == null)
            {
                utils.LogUtil.LogError(new MyError("achievement generate error", 3001));
                return;
            }
            foreach (var achievement in doneAchievementList)
            {
                doneAchievement.Add(achievement.AchievementName, achievement);
            }
            foreach (var achieveEvent in doneEventList)
            {
                doneEvent.Add(achieveEvent.EventName, achieveEvent);
            }
        }

        private void GenerateTotal()
        {
            var totalAchievementList = JsonConvert.DeserializeObject<List<Achievement>>(ReadJsonStr("./Assets/Scripts/Managers/GameManager/TotalAchievement.json"));
            var totalEventList = JsonConvert.DeserializeObject<List<AchieveEvent>>(ReadJsonStr("./Assets/Scripts/Managers/GameManager/TotalEvent.json"));
            if (totalAchievementList == null || totalEventList == null)
            {
                LogUtil.LogError(new MyError("achievement generate error", 3002));
                return;
            }
            foreach (var achievement in totalAchievementList)
            {
                totalAchievement.Add(achievement.AchievementName, achievement);
            }
            foreach (var achieveEvent in totalEventList)
            {
                totalEvent.Add(achieveEvent.EventName, achieveEvent);
            }
        }

        private string ReadJsonStr(string filePath)
        {
            var resultStr = "";
            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        resultStr += line;
                    }
                }
            }
            catch (Exception e)
            {
                utils.LogUtil.LogError(new MyError("read json file error, error=%s" + e.ToString(), 2000));
            }

            Debug.Log(resultStr);
            return resultStr;
        }

        public void DoneEvent(string eventName)
        {
            var _event = totalEvent[eventName];
            foreach (var achievement in _event.FrontEventList)
            {
                if (!achievement.Over)
                {
                    Debug.Log(string.Format("front event not over. cur_event = %s", eventName));
                    return;
                }
            }

            _event.Over = true;
            doneEvent.Add(eventName, _event);
            foreach (var s in _event.BindAchievemnt)
            {
                DoneAchievement(s);
            }
        }

        private void DoneAchievement(string achievementName)
        {
            var achievement = totalAchievement[achievementName];
            foreach (var _event in achievement.EventList)
            {
                if (!_event.Over)
                {
                    return;
                }
            }
            achievement.Over = true;
            doneAchievement.Add(achievementName, achievement);
        }
    }

    public class Achievement
    {
        public int achievement_id;
        public string achievement_name;
        public List<AchieveEvent> event_list;
        public bool over;
        public List<string> event_name_list;

        public Achievement(int achievementID, string achievementName, List<AchieveEvent> eventList)
        {
            achievement_id = achievementID;
            achievement_name = achievementName;
            event_list = eventList;
        }

        public int AchievementID
        {
            get => achievement_id;
            set => achievement_id = value;
        }

        public string AchievementName
        {
            get => achievement_name;
            set => achievement_name = value;
        }

        public List<AchieveEvent> EventList
        {
            get => event_list;
            set => event_list = value;
        }

        public bool Over
        {
            get => over;
            set => over = value;
        }
    }

    public class AchieveEvent
    {
        public int event_id;
        public string event_name;
        public bool over;
        public List<AchieveEvent> front_event_list;
        public List<string> front_event_name_list;
        public List<string> bind_achievement;

        public AchieveEvent(int eventID, string eventName, List<AchieveEvent> frontEventList, List<string> bindAchievement)
        {
            event_id = eventID;
            event_name = eventName;
            front_event_list = frontEventList;
            bind_achievement = bindAchievement;
        }

        public int EventID
        {
            get => event_id;
            set => event_id = value;
        }

        public string EventName
        {
            get => event_name;
            set => event_name = value;
        }

        public List<AchieveEvent> FrontEventList
        {
            get => front_event_list;
            set => front_event_list = value;
        }

        public List<string> BindAchievemnt
        {
            get => bind_achievement;
            set => bind_achievement = value;
        }

        public bool Over
        {
            get => over;
            set => over = value;
        }

        public List<string> FrontEventNameList
        {
            get => front_event_name_list;
            set => front_event_name_list = value;
        }
    }
    
}