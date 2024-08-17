using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtils;

namespace Curly.Turns
{
    public class DayNightTurnEvent : Singleton<DayNightTurnEvent>
    {
        [field: SerializeField] public int TurnsPerDay { get; private set; } = 10;
        [field: SerializeField] public int TurnsPerNight { get; private set; } = 10;

        public int NumberOfDays { get; private set; } = 0;
        public int NumberOfNights { get; private set; } = 0;

        public static readonly string DayEventName = "StartDay";
        public static readonly string NightEventName = "StartNight";

        private void Start()
        {
            TurnManager.Instance.RegisterTurnEvent(DayEventName, TurnsPerDay, 0, OnDayStart);
            TurnManager.Instance.RegisterTurnEvent(NightEventName, TurnsPerNight, TurnsPerDay, OnNightStart);
        }

        private void OnDayStart()
        {
            Debug.Log("Day " + NumberOfDays + " has started");
            NumberOfDays++;
        }

        private void OnNightStart()
        {
            Debug.Log("Night " + NumberOfNights + " has started");
            NumberOfNights++;
        }
    }
}