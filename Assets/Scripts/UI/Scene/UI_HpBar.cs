using UnityEngine.UI;

public class UI_HpBar : UI_Scene
{
    private PlayerStatHandler _stathandler;

    enum Images
    {
        EmptyBar,
        HpBar,
    }
 
    public override void Init()
    {
        base.Init();
        _stathandler = PlayerManager.Instance.StatHandler;
        _stathandler.OnHealthChanged += UpdateHealthBar;
        _stathandler.OnMaxHealthChanged += UpdateMaxHealthBar;

        Bind<Image>(typeof(Images));
        UpdateHealthBar(_stathandler.Health);
        UpdateMaxHealthBar(_stathandler.MaxHealth);
    }


    void OnDisable()
    {
        _stathandler.OnHealthChanged -= UpdateHealthBar;
        _stathandler.OnMaxHealthChanged -= UpdateMaxHealthBar;
    }

    void UpdateHealthBar(float current)
    {
        var image = Get<Image>((int)Images.HpBar);
        if (image != null)
            image.fillAmount = (float)current / _stathandler.limitHealth;
    }
    void UpdateMaxHealthBar(float current)
    {
        var image = Get<Image>((int)Images.EmptyBar);
        if (image != null)
            image.fillAmount = (float)current / _stathandler.limitHealth;
    }
}
