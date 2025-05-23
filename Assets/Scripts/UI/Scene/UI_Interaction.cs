using TMPro;
using UnityEngine.UI;

public class UI_Interaction : UI_Scene
{
    private PlayerInteractController _interaction;
    enum Texts
    {
        Name,
        Description,
    }
    enum Panels
    {
        Background
    }
    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Panels));
        _interaction = PlayerManager.Instance.InteractController;
        _interaction.OnInteractionChanged += UpdateInteraction;

        Get<Image>((int)Panels.Background).gameObject.SetActive(false);
    }


    void OnDisable()
    {
        _interaction.OnInteractionChanged -= UpdateInteraction;
    }

    void UpdateInteraction(ItemObject current)
    {
        var nameTxt = Get<TextMeshProUGUI>((int)Texts.Name);
        var descriptionTxt = Get<TextMeshProUGUI>((int)Texts.Description);
        var panel = Get<Image>((int)Panels.Background);
        if (current != null)
        {
            nameTxt.text = current.Data.DisplayName;
            descriptionTxt.text = current.Data.Descrition.Replace("\\n", "\n"); 
            panel.gameObject.SetActive(true);
        }
        else
        {
            nameTxt.text = "";
            descriptionTxt.text = "";
            panel.gameObject.SetActive(false);
        }
    }
}
