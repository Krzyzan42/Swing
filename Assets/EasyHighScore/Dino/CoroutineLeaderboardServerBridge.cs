using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using UnityEngine;
using UnityEngine.Networking;

namespace EasyHighScore.Dino
{
    public class CoroutineLeaderboardServerBridge : MonoBehaviour
    {
        public string serverEndpoint = "https://exploitavoid.com/leaderboards/v1/api";
        public int leaderboardID;
        public string leaderboardSecret;

        public Coroutine RequestEntries(int start, int count, Action<List<LeaderboardEntry>> successCallback,
            Action errorCallback)
        {
            return StartCoroutine(RequestEntriesCoroutine(start, count, successCallback, errorCallback));
        }

        private IEnumerator RequestEntriesCoroutine(int start, int count,
            Action<List<LeaderboardEntry>> successCallback, Action errorCallback)
        {
            var url = serverEndpoint + $"/get_entries?leaderboard_id={leaderboardID}&start={start}&count={count}";
            using var unityWebRequest = UnityWebRequest.Get(url);

            yield return unityWebRequest.SendWebRequest();

            switch (unityWebRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(unityWebRequest.error);
                    Debug.LogError(unityWebRequest.downloadHandler.text);
                    errorCallback.Invoke();
                    break;
                case UnityWebRequest.Result.Success:
                    var scores = DeserializeJson<List<LeaderboardEntry>>(unityWebRequest.downloadHandler.text);
                    successCallback.Invoke(scores);
                    break;
            }
        }

        public Coroutine RequestUserEntry(string userName, Action<LeaderboardEntry> successCallback,
            Action errorCallback)
        {
            return StartCoroutine(RequestUserEntryCoroutine(userName, successCallback, errorCallback));
        }

        private IEnumerator RequestUserEntryCoroutine(string userName, Action<LeaderboardEntry> successCallback,
            Action errorCallback)
        {
            var urlEncodedName = HttpUtility.UrlEncode(userName);
            var url = serverEndpoint +
                      $"/get_entries?leaderboard_id={leaderboardID}&start=1&count=1&search={urlEncodedName}";
            using var unityWebRequest = UnityWebRequest.Get(url);

            yield return unityWebRequest.SendWebRequest();

            switch (unityWebRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(unityWebRequest.error);
                    Debug.LogError(unityWebRequest.downloadHandler.text);
                    errorCallback.Invoke();
                    break;
                case UnityWebRequest.Result.Success:
                    var scores = DeserializeJson<List<LeaderboardEntry>>(unityWebRequest.downloadHandler.text);
                    if (scores is { Count: > 0 })
                        successCallback.Invoke(scores[0]);
                    else
                        errorCallback.Invoke();
                    break;
            }
        }

        public Coroutine SendUserValue(string userName, int value, Action successCallback, Action errorCallback)
        {
            return StartCoroutine(SendUserValueCoroutine(userName, value, successCallback, errorCallback));
        }

        public Coroutine SendUserValue(string userName, float value, Action successCallback, Action errorCallback)
        {
            return StartCoroutine(SendUserValueCoroutine(userName, value, successCallback, errorCallback));
        }

        public Coroutine SendUserValue(string userName, double value, Action successCallback, Action errorCallback)
        {
            return StartCoroutine(SendUserValueCoroutine(userName, value, successCallback, errorCallback));
        }

        private IEnumerator SendUserValueCoroutine(string userName, IConvertible value, Action successCallback,
            Action errorCallback)
        {
            var url = serverEndpoint + "/update_entry";
            var valueString = value.ToString(CultureInfo.InvariantCulture);
            var uploadJson = SerializeJson(new EntryUpdate(userName, valueString, leaderboardID));
            var toHash = "/update_entry" + uploadJson + leaderboardSecret;

            var utfBytes = Encoding.UTF8.GetBytes(toHash);
            SHA256 shaM = new SHA256Managed();
            var result = shaM.ComputeHash(utfBytes);

            var hashString = BitConverter.ToString(result).Replace("-", "");

            var rawBytes = Encoding.UTF8.GetBytes(uploadJson + hashString);

            var d = new DownloadHandlerBuffer();
            var u = new UploadHandlerRaw(rawBytes);
            using var unityWebRequest = new UnityWebRequest(url, "POST", d, u);

            yield return unityWebRequest.SendWebRequest();

            switch (unityWebRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(unityWebRequest.error);
                    Debug.LogError(unityWebRequest.downloadHandler.text);
                    errorCallback.Invoke();
                    break;
                case UnityWebRequest.Result.Success:
                    successCallback.Invoke();
                    break;
            }
        }

        private static T DeserializeJson<T>(string result)
        {
            var jsonSer = new DataContractJsonSerializer(typeof(T));
            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            ms.Position = 0;
            return (T)jsonSer.ReadObject(ms);
        }

        private static string SerializeJson<T>(T value)
        {
            var jsonSer = new DataContractJsonSerializer(typeof(T));
            using var ms = new MemoryStream();
            jsonSer.WriteObject(ms, value);
            ms.Position = 0;
            return new StreamReader(ms).ReadToEnd();
        }
    }

    [DataContract]
    public class EntryUpdate
    {
        [DataMember(Name = "leaderboard_id")] public readonly int LeaderboardID;

        [DataMember(Name = "name")] public readonly string Name;

        [DataMember(Name = "value")] public readonly string Value;

        public EntryUpdate(string name, string value, int leaderboardID)
        {
            Name = name;
            Value = value;
            LeaderboardID = leaderboardID;
        }
    }

    [DataContract]
    public class LeaderboardEntry
    {
        [DataMember(Name = "value")] private readonly string _value;

        [DataMember(Name = "name")] public readonly string Name;

        [DataMember(Name = "position")] public readonly int Position;

        public string GetValueAsString()
        {
            return _value;
        }

        public int GetValueAsInt()
        {
            if (int.TryParse(_value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result)) return result;
            return (int)Math.Round(GetValueAsDouble());
        }

        public float GetValueAsFloat()
        {
            return float.Parse(_value, CultureInfo.InvariantCulture);
        }

        public double GetValueAsDouble()
        {
            return double.Parse(_value, CultureInfo.InvariantCulture);
        }
    }
}