using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    int _order = 10;

    [SerializeField]
    Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();
    public static UIManager instance;

    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("PopupUI_Root");
            if (root == null)
                root = new GameObject { name = "PopupUI_Root" };
            return root;
        }
    }
    private void Awake()
    {
        // 싱글톤
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);
    }
    private void Start()
    {

    }
    // 캔버스 지정함수
    public void SetCanvas(GameObject go, bool sort = true, int canvasLayer = 0) 
    {

        if(go.GetComponent<GraphicRaycaster>() == null)
            go.AddComponent<GraphicRaycaster>();
        if (go.GetComponent<GraphicRaycaster>() == null)
            go.AddComponent<Canvas>();


        Canvas canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        if (sort)
        {
            canvas.sortingOrder = _order;
            _order++;
        }
        else
        {
            if(canvasLayer == 0)
                canvas.sortingOrder = 0;
            else
                canvas.sortingOrder = canvasLayer;
        }
         
    }

    // PopUI 등장 및 Stack 삽입
    public GameObject ShowPopupUI(string name = null)
    {
        Time.timeScale = 0f;

        GameObject prefab = Resources.Load<GameObject>($"Prefabs/UI/Popup/{name}");
        GameObject ui = Instantiate(prefab);
        if (ui == null)
            return null;

        UI_Popup popup = ui.GetComponent<UI_Popup>();
        _popupStack.Push(popup);

        ui.transform.SetParent(Root.transform, false);
        return ui;
    }

    // PopUI 제거 및 Stack 제거
    //확정적으로 원하는녀석 삭제하는지 체크
    public void ClosePopupUI(UI_Popup popup)
    {
        if (_popupStack.Count == 0) return;
        if (_popupStack.Peek() != popup)
        {
            Debug.Log("Close Popup Failed");
            return;
        }
        ClosePopupUI();
    }
    // 가장 최근 제거
    public void ClosePopupUI()
    {
        if (_popupStack.Count == 0) return;

        UI_Popup popup = _popupStack.Pop();
        Destroy(popup.gameObject);
        popup = null;

        _order--;

        Time.timeScale = 1.0f;
    }
}
