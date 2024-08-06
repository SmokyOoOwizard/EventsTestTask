using System.Collections;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

namespace Events
{
    public class EventService : MonoBehaviour
    {
        [SerializeField]
        public EventsStorage eventsStorage;

        [SerializeField]
        private string serverUrl;

        [SerializeField]
        private float cooldownDuration = 1.0f;

        private bool _isCooldown;


        public void TrackEvent(string type, string data)
        {
            eventsStorage.AddEvent(new(type, data));

            if (_isCooldown)
                return;

            StartCoroutine(CooldownTimer());
        }

        private IEnumerator CooldownTimer()
        {
            _isCooldown = true;

            while (true)
            {
                yield return new WaitForSeconds(cooldownDuration);

                yield return SendEvents();
            }
        }

        private IEnumerator SendEvents()
        {
            var events = eventsStorage.GetEvents();

            var json = JsonUtility.ToJson(events);

            var request = UnityWebRequest.Post(serverUrl, json);
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.responseCode != (int)HttpStatusCode.OK)
            {
                Debug.LogError($"Request error: {request.responseCode}\n{request.error}");
                yield break;
            }

            eventsStorage.RemoveEvents(events.Length);
        }
    }
}