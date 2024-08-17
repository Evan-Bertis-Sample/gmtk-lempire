using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtils;

namespace Curly.Turns
{
    public class TurnManager : Singleton<TurnManager>
    {
        public struct TurnEvent
        {
            public string Name;
            public int TurnInterval;
            public int StartingTurn;
            public System.Action Action;

            public TurnEvent(string name, int turnInterval, int startingTurn, System.Action action)
            {
                Name = name;
                TurnInterval = turnInterval;
                StartingTurn = startingTurn;
                Action = action;
            }
        }

        [field: SerializeField] public float TurnDuration { get; private set; } = 10f; // The duration of a turn in seconds
        [field: SerializeField] public int TurnCount { get; private set; } = 0; // The current turn count
        [field: SerializeField] public bool IsPaused { get; private set; } = false; // Whether the turn manager is paused
        public List<TurnEvent> TurnEvents = new List<TurnEvent>();

        private float _timeSinceLastTurn = 0f; // The time since the last turn

        public void RegisterTurnEvent(string name, int turnInterval, int startingTurn, System.Action action)
        {
            TurnEvents.Add(new TurnEvent(name, turnInterval, startingTurn, action));
        }

        public void RegisterTurnEvent(string name, int turnInterval, System.Action action)
        {
            RegisterTurnEvent(name, turnInterval, TurnCount, action);
        }

        public void DisableTurnEvent(string name)
        {
            TurnEvents.RemoveAll(turnEvent => turnEvent.Name == name);
        }

        private void Update()
        {
            if (IsPaused)
            {
                return;
            }

            _timeSinceLastTurn += Time.deltaTime;

            if (_timeSinceLastTurn >= TurnDuration)
            {
                _timeSinceLastTurn = 0f;
                TurnCount++;
                OnTurn();
            }
        }

        private void OnTurn()
        {
            foreach (TurnEvent turnEvent in TurnEvents)
            {
                if (TurnCount >= turnEvent.StartingTurn && (TurnCount - turnEvent.StartingTurn) % turnEvent.TurnInterval == 0)
                {
                    turnEvent.Action.Invoke();
                }
            }
        }
    }
}