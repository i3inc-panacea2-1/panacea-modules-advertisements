using Panacea.Controls;
using Panacea.Core;
using Panacea.Modularity;
using Panacea.Modularity.UiManager.Extensions;
using Panacea.Modules.Advertisements.ViewModels;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Panacea.Modules.Advertisements
{
    class AdvertisementsPlugin : IPlugin
    {
        PanaceaServices _core;
        public AdvertisementsPlugin(PanaceaServices core)
        {
            CacheImage.ImageUrlChanged += CacheImage_ImageUrlChanged;
            _core = core;
        }

        private void CacheImage_ImageUrlChanged(object sender, string e)
        {
            var image = sender as CacheImage;
            if (!image.IsLoaded)
            {
                image.Loaded += Image_Loaded;
                return;
            }
            var task = SetImage(image, e);

        }

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            var image = sender as CacheImage;
            image.Loaded -= Image_Loaded;
            var task = SetImage(image, image.ImageUrl);
        }

        async Task SetImage(CacheImage image, string url)
        {
            var download = await _core.HttpClient.DownloadDataAsync(url);
            var img2 = new BitmapImage();
            img2.BeginInit();
            img2.CreateOptions |= BitmapCreateOptions.IgnoreColorProfile;
            img2.CacheOption = BitmapCacheOption.OnLoad;
            img2.StreamSource = new MemoryStream(download);

            img2.EndInit();
            img2.Freeze();
            image.Source = img2;
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
