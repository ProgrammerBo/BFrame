using System.Collections.Generic;
using UnityEngine.Events;

public class GameEventListener
{

    private static Dictionary<string, List<UnityAction<object>>> _dicListener = new Dictionary<string, List<UnityAction<object>>>();

    public static void Clear()
    {
        _dicListener.Clear();
    }

    public static void Add(string key, UnityAction<object> action)
    {
        if (_dicListener.ContainsKey(key))
        {
            var list = _dicListener[key];
            int index = -1;
            for (int i = 0; i < list.Count; i++)
            {
                if (action == list[i])
                {
                    index = i;
                    break;
                }
            }
            if (-1 == index)
            {
                list.Add(action);
            }
            _dicListener[key] = list;
        }
        else
        {
            var list = new List<UnityAction<object>>();
            list.Add(action);
            _dicListener[key] = list;
        }

    }

    public static void Remove(string key, UnityAction<object> action)
    {
        if (_dicListener.ContainsKey(key))
        {
            var list = _dicListener[key];
            int index = -1;
            for (int i = 0; i < list.Count; i++)
            {
                if (action == list[i])
                {
                    index = i;
                    break;
                }
            }
            if (-1 != index)
            {
                list.RemoveAt(index);
            }
            _dicListener[key] = list;
        }
    }

    public static void Dispatch(string key)
    {
        Dispatch(key, null);
    }

    public static void Dispatch(string key, object args)
    {
        if (_dicListener.ContainsKey(key))
        {
            var list = _dicListener[key];
            for (int i = 0; i < list.Count; i++)
            {
                var action = list[i];
                action(args);
            }
        }
    }
}
