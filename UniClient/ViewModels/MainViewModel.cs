using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Framework.Common;

namespace UniClient;

public partial class MainViewModel : UniViewModel
{
    #region Constructors
    public MainViewModel()
    {
        // init menus
        _menus = [];
        foreach (MenuConf menuConf in SystemConfig.MenuConfList)
        {
            _menus.Add(menuConf);
        }
        
        // register language changed
        WeakReferenceMessenger.Default.Register<LanguageChangedMessage>(this, (_, _) =>
        {
            foreach (MenuConf menuConf in Menus)
            {
                menuConf.RefreshMenuName();
            }
        });
    }
    #endregion


    #region Properties
    [ObservableProperty] private ObservableCollection<MenuConf> _menus;
    [ObservableProperty] private MenuConf? _selectedMenu;

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (nameof(SelectedMenu) == e.PropertyName)
        {
            OnMenuSelected();
            SelectedMenu = null;
        }
    }
    #endregion


    private void OnMenuSelected()
    {
        if (null == SelectedMenu ||
            null == View)
        {
            return;
        }

        ((MainView)View).OpenMenu(SelectedMenu);
    }
    
}