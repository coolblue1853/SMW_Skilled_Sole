using UnityEngine.UI;
public class UI_Stemina : UI_Scene
{
    private PlayerStatHandler _stathandler;
    enum Images
    {
        EmptyBar,
        SteminaBar,
    }

    public override void Init()
    {
        base.Init();
        _stathandler = PlayerManager.Instance.StatHandler;
        _stathandler.OnSteminaChanged += UpdateSteminaBar;
        _stathandler.OnMaxSteminaChanged += UpdateMaxSteminaBar;

        Bind<Image>(typeof(Images));
        UpdateSteminaBar(_stathandler.Stemina);
        UpdateMaxSteminaBar(_stathandler.MaxStemina);
    }


    void OnDisable()
    {
        _stathandler.OnSteminaChanged -= UpdateSteminaBar;
        _stathandler.OnMaxSteminaChanged -= UpdateMaxSteminaBar;
    }

    void UpdateSteminaBar(float current)
    {
        var image = Get<Image>((int)Images.SteminaBar);
        if (image != null)
            image.fillAmount = (float)current / _stathandler.limitStemina;
    }
    void UpdateMaxSteminaBar(float current)
    {
        var image = Get<Image>((int)Images.EmptyBar);
        if (image != null)
            image.fillAmount = (float)current / _stathandler.limitStemina;
    }

}
