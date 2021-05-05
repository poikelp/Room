

namespace  urlopener
{
#if UNITY_EDITOR

    using UnityEngine;

    public class URLOpener
    {
        public static void OpenURL(string s)
        {
            Debug.Log($"Clicked to open URL: {s}");
        }
        public static string GetQuery()
        {
            Debug.Log("getquery");
            return "AaQ";
        }
    }

#elif UNITY_WEBGL

    using System.Runtime.InteropServices;

    public class URLOpener
    {
        [DllImport("__Internal")]
        public static extern void OpenURL(string s);

        [DllImport("__Internal")]
        public static extern string GetQuery();
    }

#else

    public class URLOpener
    {
        public static void OpenURL(string s)
        {
        }
        public static string GetQuery(){return ""};
    }
#endif
}
