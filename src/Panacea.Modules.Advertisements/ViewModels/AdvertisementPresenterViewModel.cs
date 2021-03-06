﻿using System.Windows.Input;
using System;
using Panacea.Modules.Advertisements.Models;
using Panacea.Core;
using Panacea.Controls;
using Panacea.Modularity.UiManager;
using Panacea.Modularity.MediaPlayerContainer;
using Panacea.Modularity.Media;
using System.Linq;
using Panacea.Mvvm;
using Panacea.Modularity.WebBrowsing;

namespace Panacea.Modules.Advertisements.ViewModels
{
    [View(typeof(AdvertisementPresenter))]
    public class AdvertisementPresenterViewModel : ViewModelBase
    {

        AdvertisementEntry _ad;
        public AdvertisementEntry Ad
        {
            get => _ad;
            set
            {
                _ad = value;
                OnPropertyChanged();
            }
        }

        private readonly PanaceaServices _core;

        public PanaceaServices Core
        {
            get => _core;
        }

        public AdvertisementPresenterViewModel(PanaceaServices core)
        {
            _core = core;
            PreviewMouseLeftButtonUpCommand = new RelayCommand(async (arg) =>
            {
                if (_core.TryGetUiManager(out IUiManager ui))
                {
                    ui.HideAllPopups();
                }
                var ad = arg as AdvertisementEntry;
                if (string.IsNullOrEmpty(ad.DataUrl)) return;
                switch (ad.AdType)
                {
                    case AdvertisementType.Webpage:
                        if(_core.TryGetWebBrowser(out IWebBrowserPlugin web))
                        {
                            web.OpenUnmanaged(_core.HttpClient.GetApiEndpoint("/ad/adClick/" + ad.Id + "/"));
                        }
                        break;
                    case AdvertisementType.Video:
                        if(_core.TryGetMediaPlayerContainer(out IMediaPlayerContainer player))
                        {
                            player.Play(new MediaRequest(new IPTVChannel() { URL = ad.DataUrl }));
                        }
                        break;
                }
                //TODO: _websocket.PopularNotify("Advertisements", "AdBanner", ad.Id);
            });

        }
        public ICommand PreviewMouseLeftButtonUpCommand { get; private set; }
    }
}

internal class IPTVChannel : MediaItem
{
    public string URL { get; set; }

    public override string GetMRL()
    {
        return URL.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0];
    }

    public override string GetExtras()
    {
        var spl = URL.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        if (spl.Count() > 1)
        {
            var list = spl.ToList();
            return string.Join(" ", list);
        }
        return "";
    }
}