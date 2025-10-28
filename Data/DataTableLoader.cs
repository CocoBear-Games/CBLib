using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace CBLib
{
    public class DataTableLoader : MonoBehaviour
    {
        [SerializeField] private string baseUrl;

        public void SetUrl(string url) => baseUrl = url;

        public IEnumerator LoadTable<T>(string sheetName, Action<T> onLoaded) where T : class
        {
            string url = $"{baseUrl}?sheet={sheetName}";
            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                string wrapped = "{\"items\":" + json + "}";
                T data = JsonUtility.FromJson<T>(wrapped);
                onLoaded?.Invoke(data);
            }
            else
            {
                Debug.LogError($"❌ GoogleSheet 요청 실패 : {request.error}");
            }
        }
    }
}