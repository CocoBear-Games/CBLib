using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CBLib
{
    public class GameResource<T> where T : Object
    {
        protected List<string> pathNames = new List<string>();
        protected List<string> extensions = new List<string>();
        protected Dictionary<string, T> resources = new Dictionary<string, T>();
        protected bool isAllLoaded = false;

        public GameResource(string pathName, string extension = "")
        {
            pathNames.ParsingStringSplit(pathName, ',');
            extensions.Add("prefab");
            extensions.ParsingStringSplit(extension, ',');
        }

        public void AddPathName(string pathName)
        {
            pathNames.ParsingStringSplit(pathName, ',');
        }

        public void AddExtension(string extension)
        {
            extensions.ParsingStringSplit(extension, ',');
        }

        public virtual T GetResource(string strFileName)
        {
            return string.IsNullOrEmpty(strFileName) ? default(T) : LoadResource(strFileName);
        }

        public void DelResource(string fileName)
        {
            Debug.Log(fileName);
            if (string.IsNullOrEmpty(fileName)) return;
            
            Destroy(fileName);
        }

        protected virtual T[] ResourceLoadAll()
        {
            List<T> objList = new List<T>();
            if (pathNames.Count > 0)
                objList.AddRange(Resources.LoadAll<T>(pathNames[0]));
            return objList.ToArray();
        }

        protected virtual T ResourceLoadAtPath(string pathAndFileName) => default;

        public void AllResourceLoad()
        {
            if (isAllLoaded)
                return;
            isAllLoaded = true;
            foreach (T obj in ResourceLoadAll())
                AddResource(obj.name, obj);
        }

        private T LoadResource(string fileName)
        {
            T resource = FindResource(fileName);
            if (resource != null) return resource;
            
            int count = pathNames.Count;
            for (int index = 0; index < count; ++index)
            {
                string fullPath = $"{pathNames[index]}/{fileName}";
                
                T obj = Resources.Load<T>(fullPath);
                if (obj != null)
                {
                    AddResource(fileName, obj);
                    return obj;
                }
            }

            Debug.LogWarning($"[Resource - {typeof(T)}] LoadResource Fail : {fileName}");
            
            return default;
        }

        public void AddResource(string key, T obj)
        {
            if (ContainsKey(key))
                return;
            resources.Add(key, obj);
        }

        protected T FindResource(string _key)
        {
            return ContainsKey(_key) ? resources[_key] : default(T);
        }

        private bool ContainsKey(string _key) => _key != null && resources.ContainsKey(_key);

        public void AllDestroy()
        {
            isAllLoaded = false;
            Dictionary<string, T>.Enumerator enumerator = resources.GetEnumerator();
            while (enumerator.MoveNext())
                Object.DestroyImmediate(enumerator.Current.Value);
            
            resources.Clear();
        }

        private void Destroy(string _strName)
        {
            T resource = FindResource(_strName);
            if (resource == null)
                return;
            Object.DestroyImmediate(resource);
            Debug.Log(("Destroy " + _strName));
            resources.Remove(_strName);
        }

        protected Dictionary<string, T> GetAllResource() => resources;
        public string[] GetResourceNames() => resources.Keys.ToArray<string>();
        public List<string> GetResourcePathName() => pathNames;
        public List<string> GetResourceExtensionName() => extensions;
        public int GetCount() => resources.Count;
    }
}
