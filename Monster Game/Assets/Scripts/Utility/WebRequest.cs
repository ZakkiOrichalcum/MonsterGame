using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.Networking;
using System.Collections.Specialized;

public class WebRequest
{
    public IEnumerator GetFromAirtable(string url, NameValueCollection headers, UnityAction<string> onSuccess, UnityAction<string> onUpdate)
    {
        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            if (headers.Get("Authorization") != null)
            {
                req.SetRequestHeader("Authorization", headers.Get("Authorization"));
            }
            yield return req.SendWebRequest();
            while (!req.isDone)
                yield return null;
            byte[] result = req.downloadHandler.data;
            string str = System.Text.Encoding.Default.GetString(result);
            onUpdate(string.Format("Downloaded from {0}. Response {1}", url, str));
            onSuccess(str);
        }
    }
}
