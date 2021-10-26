using System;
using UnityEngine;

namespace StoryLine
{
    public class PlayerDistanceChecker : MonoBehaviour
    {
        [SerializeField] private float _distance;
        [SerializeField] private Transform _player;

        [SerializeField] private string _eventName;
        
        public static event Action<string> EnterEvent = delegate { };
        public static event Action<string> StayEvent = delegate { };
        public static event Action<string> ExitEvent = delegate { };

        private bool _inside;

        private void Start()
        {
            _inside = false;
        }

        private void FixedUpdate()
        {
            if ((_player.transform.position - transform.position).magnitude < _distance)
            {
                if (_inside == false)
                {
                    _inside = true;
                    EnterEvent.Invoke(_eventName);
                }
                StayEvent.Invoke(_eventName);
            }
            else
            {
                if (_inside)
                {
                    _inside = false;
                    ExitEvent.Invoke(_eventName);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            
            Gizmos.DrawWireSphere(transform.position, _distance);
        }
    }
}