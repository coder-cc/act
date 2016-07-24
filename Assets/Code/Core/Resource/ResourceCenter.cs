using UnityEditor;
using UnityEngine;

namespace Aqua.Resource
{

    public class ResourceCenter
    {
        public static readonly ResourceCenter Instance = new ResourceCenter();


        public static UnityEngine.Object LoadAsset(string path)
        {
            return AssetDatabase.LoadAssetAtPath<Object>(path); //Resources.Load<UnityEngine.Object>(path);
        }

    }

}