using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public static class ObjectPooler
{
    private static Dictionary<GameObject, Pool> pools;

    static ObjectPooler()
    {
        pools = new Dictionary<GameObject, Pool>();
    }

    /// <summary>
    /// Use this overload if you need a simple Get<typeparam name="T"></typeparam> 
    /// and if you want to do some action on the fly you can use the Action<Gameobject>
    /// </summary>
    public static T Get<T>(GameObject prefab, Action<GameObject> action = null) where T : Component, IPoolable
    {
        if (!pools.ContainsKey(prefab))
            pools.Add(prefab, new Pool(prefab));

        if (action != null)
            action.Invoke(prefab);

        T result = pools[prefab].Get().GetComponent<T>();
        result.prefab = prefab;
        return result;
    }
    /// <summary>
    /// Use this overload if you need to specify position and rotation that doesn't depends from the parent.
    /// </summary>
    public static T Get<T>(GameObject prefab, Vector3 pos, Quaternion rot) where T : Component, IPoolable
    {
        if (!pools.ContainsKey(prefab))
            pools.Add(prefab, new Pool(prefab));


        T result = pools[prefab].Get(pos, rot).GetComponent<T>();
        result.prefab = prefab;
        return result;
    }
    /// <summary>
    /// Use this overload to parent an object to another.
    /// </summary>
    public static T Get<T>(GameObject prefab, Transform parent) where T : Component, IPoolable
    {
        if (!pools.ContainsKey(prefab))
            pools.Add(prefab, new Pool(prefab));


        T result = pools[prefab].Get(parent).GetComponent<T>();
        result.prefab = prefab;
        return result;
    }
    public static void Recycle(GameObject instance, Action<GameObject> resetter = null)
    {
        pools[instance.GetComponent<IPoolable>().prefab].Recycle(instance);

        if (resetter != null)
            resetter.Invoke(instance);
    }

    private class Pool
    {
        private Queue<GameObject> gameObjects;

        public GameObject prefab;

        public Pool(GameObject prefab)
        {
            gameObjects = new Queue<GameObject>();
            this.prefab = prefab;
        }
        public GameObject Get()
        {
            if (gameObjects.Count > 0)
            {
                GameObject go = gameObjects.Dequeue();
                go.SetActive(true);
                return go;
            }
            else
            {
                GameObject go = GameObject.Instantiate(prefab);
                return go;
            }
        }
        public GameObject Get(Vector3 pos, Quaternion rot)
        {
            if (gameObjects.Count > 0)
            {
                GameObject go = gameObjects.Dequeue();
                go.SetActive(true);
                return go;
            }
            else
            {
                GameObject go = GameObject.Instantiate(prefab, pos, rot);
                return go;
            }
        }
        public GameObject Get(Transform parent)
        {
            if (gameObjects.Count > 0)
            {
                GameObject go = gameObjects.Dequeue();
                go.SetActive(true);
                return go;
            }
            else
            {
                GameObject go = GameObject.Instantiate(prefab, parent.position, parent.rotation, parent);
                return go;
            }
        }

        public void Recycle(GameObject toRecycle)
        {
            toRecycle.SetActive(false);
            gameObjects.Enqueue(toRecycle);
        }
    }
    public interface IPoolable
    {
        GameObject prefab { get; set; }
    }
}

