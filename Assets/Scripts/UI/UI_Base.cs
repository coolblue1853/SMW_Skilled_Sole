using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Xml.Linq;
public abstract class UI_Base : MonoBehaviour
{
    public int canvasLayer;
    Dictionary<Type, UnityEngine.Object[]> objects = new Dictionary<Type, UnityEngine.Object[]>();
    public abstract void Init();
    private void Start()
    {
        Init();
    }
    // 원하는 컴포넌트를 딕셔너리에 등록
    protected void Bind<T>(Type type) where T : UnityEngine.Object
    {
        //타입 이름 배열
        string[] names = Enum.GetNames(type);
        UnityEngine.Object[] objects = new UnityEngine.Object[name.Length];
        this.objects.Add(typeof(T), objects);

        for (int i = 0; i < names.Length; i++)
        {
            // 컴포넌트 찾기
            objects[i] = FindChild<T>(gameObject, names[i], true);
            if (objects[i] == null)
                Debug.Log($"Failed to bind{names[i]}");
        }
    }
    // 원하는 컴포넌트를 딕셔너리에서 추출
    protected T Get<T>(int idx) where T : UnityEngine.Object
    {
        UnityEngine.Object[] obj = null;
        if (objects.TryGetValue(typeof(T), out obj) == false)
            return null;

        return obj[idx] as T;
    }


    // 해당 이름과 선택된 컴포넌트 T 를 가진 오브젝트를 찾는 함수
    public static T FindChild<T>(GameObject go, string name = null, bool recuresive = false) where T : UnityEngine.Object
    {
        if (go == null) return null;

        if (recuresive == false) // 자식의 자식까지 볼것인가
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    return component;
                }
            }
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        return null;
    }
}
