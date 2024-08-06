using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Events
{
    public class EventsStorage : MonoBehaviour
    {
        [SerializeField]
        private string storagePrefix;

        private Queue<Event> _events = new();

        private void Awake()
        {
            Load();
        }

        public void AddEvent(Event value)
        {
            _events.Enqueue(value);

            Save();
        }

        public Event[] GetEvents()
        {
            return _events.ToArray();
        }

        public void RemoveEvents(int count)
        {
            var toRemove = Mathf.Min(count, _events.Count);
            for (var i = 0; i < toRemove; i++)
            {
                _events.Dequeue();
            }

            Save();
        }

        private void Save()
        {
            var json = JsonUtility.ToJson(_events);

            var tmpFilePath = Path.Combine(Application.persistentDataPath, storagePrefix);
            Directory.CreateDirectory(Path.GetDirectoryName(tmpFilePath)!);

            if (File.Exists(tmpFilePath))
                File.Delete(tmpFilePath);

            File.WriteAllText(tmpFilePath, json);
        }

        private void Load()
        {
            var tmpFilePath = Path.Combine(Application.persistentDataPath, storagePrefix);

            if (!File.Exists(tmpFilePath))
                return;

            var json = File.ReadAllText(tmpFilePath);
            _events = JsonUtility.FromJson<Queue<Event>>(json)!;
        }
    }
}