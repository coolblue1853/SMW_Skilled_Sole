using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Hover : UI_Scene
{
    private Camera mainCamera;
    [SerializeField] private GameObject hoverUI;
    private TextMeshProUGUI hoverTxt;
    [SerializeField] private LayerMask interactableLayer;
    enum Texts
    {
        Txt,
    }

    public override void Init()
    {
        hoverUI.SetActive(true);

        Bind<TextMeshProUGUI>(typeof(Texts));
        hoverTxt = Get<TextMeshProUGUI>((int)Texts.Txt);
        mainCamera = Camera.main;

        hoverUI.SetActive(false);
    }

    private void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, interactableLayer))
        {
            // 마우스 근처에 UI 표시
            hoverUI.SetActive(true);
            hit.transform.TryGetComponent<ItemObject>(out ItemObject item);
            hoverTxt.text = item.Data.DisplayName;

            Vector2 screenPos = Input.mousePosition;
            hoverUI.transform.position = screenPos + new Vector2(20f, -20f); // 마우스 옆에 띄움
        }
        else
        {
            hoverUI.SetActive(false);
        }
    }
}

