using Panacea.Controls;
using Panacea.Core;
using Panacea.Modularity;
using Panacea.Modularity.UiManager.Extensions;
using Panacea.Modules.Advertisements.ViewModels;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Panacea.Modules.Advertisements
{
    class AdvertisementsPlugin : IPlugin
    {
        PanaceaServices _core;
        public AdvertisementsPlugin(PanaceaServices core)
        {
            
            _core = core;
        }

        

        public Task BeginInit()
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {

        }

        public Task EndInit()
        {
            var adv = new AdvertisementViewModel(_core);
            _core.GetUiManager().AddNavigationBarControl(adv);
            return Task.CompletedTask;
        }

        public Task Shutdown()
        {
            return Task.CompletedTask;
        }
    }
}
