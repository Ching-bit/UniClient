using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Control.Basic;
using Framework.Common;
using Plugin.AppEnv;
using Plugin.Log;
using UniClient.Models;

namespace UniClient;

public partial class TitleBarRightViewModel : UniViewModel
{
    [RelayCommand]
    private async Task Setting()
    {
        ConfirmDialogResult confirmResult = await ConfirmDialog.Show<SettingDialog, SettingDialogViewModel>();
        if (!confirmResult.IsConfirmed || confirmResult.ReturnParameter is not SettingModel settingModel)
        {
            return;
        }
        settingModel.SyncToGlobalSetting();
    }

    [RelayCommand]
    private async Task Connections()
    {
        ConfirmDialogResult confirmResult = await ConfirmDialog.Show<ConnectionsDialog, ConnectionsDialogViewModel>();
        if (!confirmResult.IsConfirmed || confirmResult.ReturnParameter is not ObservableCollection<ConnConfModel> connConfs)
        {
            return;
        }

        string? selectedConnName = connConfs.FirstOrDefault(x => x.IsSelected)?.Name;
        if (null == selectedConnName)
        {
            Global.Get<ILog>().Error(LogModule.DIALOG, "No connection selected");
            return; // impossible
        }

        if (selectedConnName != Global.Get<IGlobalSetting>().SelectedConn &&
            null != Global.Get<IAppEnv>().User &&
            !await MessageDialog.Show("R_STR_CHANGE_CONN_NOTICE", isCancelButtonVisible: true))
        {
            return;
        }

        Global.Get<IGlobalSetting>().SelectedConn = selectedConnName;
        ConnConf? selectedConn = SystemConfig.ConnConfDict[ConnConfType.Trade]
            .FirstOrDefault(x => x.Name == selectedConnName);
        if (null == selectedConn)
        {
            return; // impossible
        }
        WeakReferenceMessenger.Default.Send(new ConnectionChangedMessage(selectedConn));
    }
    
}