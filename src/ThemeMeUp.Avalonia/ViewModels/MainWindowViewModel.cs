﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using ReactiveUI;
using ThemeMeUp.Avalonia.Models;
using ThemeMeUp.Avalonia.Utilities;
using ThemeMeUp.Core.Boundaries.GetLatestWallpapers;
using ThemeMeUp.Core.Entities;

namespace ThemeMeUp.Avalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IGetLatestWallpapersUseCase _useCase;
        private readonly LatestWallpapersPresenter _output;
        private readonly UrlImageConverter _imgConverter;
        private readonly WallpaperSetter _wallpaperSetter;
        private string _searchTerm;
        private bool _includeSfw;
        private bool _includeSketchy;
        private bool _includeNsfw;
        private bool _fetchButtonEnabled = true;
        private IEnumerable<WallpaperModel> _wallpapers;
        private WallpaperModel _selectedWallpaper;

        public MainWindowViewModel(IGetLatestWallpapersUseCase useCase, IGetLatestWallpapersOutputPort output, UrlImageConverter imgConverter, WallpaperSetter wallpaperSetter)
        {
            _useCase = useCase;
            _output = (LatestWallpapersPresenter)output;
            _imgConverter = imgConverter;
            _wallpaperSetter = wallpaperSetter;
        }

        public string Greeting => "Hello World!";

        public string SearchTerm
        {
            get => _searchTerm;
            set => _ = this.RaiseAndSetIfChanged(ref _searchTerm, value);
        }

        public bool IncludeSfw
        {
            get => _includeSfw;
            set => _ = this.RaiseAndSetIfChanged(ref _includeSfw, value);
        }

        public bool IncludeSketchy
        {
            get => _includeSketchy;
            set => _ = this.RaiseAndSetIfChanged(ref _includeSketchy, value);
        }

        public bool IncludeNsfw
        {
            get => _includeNsfw;
            set => _ = this.RaiseAndSetIfChanged(ref _includeNsfw, value);
        }

        public bool FetchButtonEnabled
        {
            get => _fetchButtonEnabled;
            set => _ = this.RaiseAndSetIfChanged(ref _fetchButtonEnabled, value);
        }

        public IEnumerable<WallpaperModel> Wallpapers
        {
            get => _wallpapers;
            set => _ = this.RaiseAndSetIfChanged(ref _wallpapers, value);
        }

        public WallpaperModel SelectedWallpaper
        {
            get => _selectedWallpaper;
            set => _ = this.RaiseAndSetIfChanged(ref _selectedWallpaper, value);
        }

        public async void OnGetWallpapers()
        {
            Wallpapers = new WallpaperModel[0];
            _output.Clear();            

            FetchButtonEnabled = false;

            await _useCase.Execute(new GetLatestWallpapersInput
            {
                SearchTerm = SearchTerm,
                Sfw = IncludeSfw,
                Sketchy = IncludeSketchy,
                Nsfw = IncludeNsfw
            });

            if(_output.noApiKey || _output.noConnection)
            {
                // TODO: Create an error popup
                FetchButtonEnabled = true;
                return;
            }

            var result = new List<WallpaperModel>();

            foreach(var wallpaper in _output.wallpapers)
            {
                result.Add(await ToModel(wallpaper));
            }

            Wallpapers = result;

            FetchButtonEnabled = true;
        }

        public async void SetSelectedWallpaper()
        {
            if(SelectedWallpaper is null) { return; }
            
            await _wallpaperSetter.SetFromUrlAsync(SelectedWallpaper.FullImageUrl);
        }

        private async Task<WallpaperModel> ToModel(Wallpaper w)
        {
            var model = new WallpaperModel
            {
                FullImageUrl = w.FullImageUrl
            };

            model.ThumbnailBitmap = await _imgConverter.UrlToBitmapAsync(w.SmallThumbnailUrl);
            
            return model;
        }
    }
}