using Panacea.Controls;
using Panacea.Core;
using Panacea.Mvvm;
using Panacea.Modularity.UiManager.Extensions;
using System.Windows.Input;

namespace Panacea.Modules.Advertisements.ViewModels
{
    [View(typeof(AdNotification))]
    public class AdNotificationViewModel : ViewModelBase
    {
        public AdvertisementPresenterViewModel Presenter;
        public ICommand Close { get; private set; }
        public ICommand Click { get; private set; }
        private readonly PanaceaServices _core;
        public AdNotificationViewModel(AdvertisementPresenterViewModel presenter, PanaceaServices core)
        {
            _core = core;
            Presenter = presenter;
            Close = new RelayCommand(async (arg) =>
            {
                _core.GetUiManager().Refrain(this);
            });
            Click = new RelayCommand(async (arg) => {
                _core.GetUiManager().Refrain(this);
            });
        }
    }
}
