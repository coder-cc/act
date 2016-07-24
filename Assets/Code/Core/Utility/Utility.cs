using UnityEngine;

namespace Aqua.Util
{
    public static class Utility
    {


        public static GameObject AddChild(GameObject parent, GameObject prefab)
        {
            GameObject go = Object.Instantiate(prefab) as GameObject;

#if UNITY_EDITOR && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2
            UnityEditor.Undo.RegisterCreatedObjectUndo(go, "Create Object");
#endif

            if (go != null && parent != null)
            {
                Transform t = go.transform;
                t.parent = parent.transform;
                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.identity;
                t.localScale = Vector3.one;
                go.layer = parent.layer;
            }
            return go;
        }


        public static GameObject AddChild(Transform transform, GameObject prefab)
        {
            if (transform == null)
                return AddChild((Transform)null, prefab);

            return AddChild(transform.gameObject, prefab);
        }


    }
}