using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Ascensor.Utils
{
    [AddComponentMenu("Event/Lifecycle Trigger")]
    public class LifecycleTrigger : MonoBehaviour
    {
        public enum LifecycleEventType
        {
            Awake,
            OnEnable,
            Start,
            Update,
            FixedUpdate,
            LateUpdate,
            OnDisable
        }

        [SerializeField] private List<Entry> entries;

        public List<Entry> Triggers
        {
            get
            {
                if (entries == null)
                    entries = new List<Entry>();

                return entries;
            }
            set => entries = value;
        }

        private void Awake()
        {
            Execute(LifecycleEventType.Awake);
        }

        private void Start()
        {
            Execute(LifecycleEventType.Start);
        }

        private void Update()
        {
            Execute(LifecycleEventType.Update);
        }

        private void FixedUpdate()
        {
            Execute(LifecycleEventType.FixedUpdate);
        }

        private void LateUpdate()
        {
            Execute(LifecycleEventType.LateUpdate);
        }

        private void OnEnable()
        {
            Execute(LifecycleEventType.OnEnable);
        }

        private void OnDisable()
        {
            Execute(LifecycleEventType.OnDisable);
        }

        private void Execute(LifecycleEventType id)
        {
            for (var i = 0; i < Triggers.Count; i++)
            {
                var entry = Triggers[i];

                if (entry.eventID == id && entry.callback != null)
                    entry.callback.Invoke();
            }
        }

        [Serializable]
        public class LifecycleEvent : UnityEvent
        {
        }

        [Serializable]
        public class Entry
        {
            public LifecycleEventType eventID = LifecycleEventType.Start;
            public LifecycleEvent callback = new();
        }
    }
}