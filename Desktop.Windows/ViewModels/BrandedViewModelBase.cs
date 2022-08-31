﻿using CommunityToolkit.Mvvm.ComponentModel;
using Immense.RemoteControl.Desktop.Shared.Abstractions;
using Immense.RemoteControl.Desktop.Windows.Services;
using Immense.RemoteControl.Shared.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;

namespace Immense.RemoteControl.Desktop.Windows.ViewModels
{
    [ObservableObject]
    public partial class BrandedViewModelBase
    {
        private static BrandingInfo? _brandingInfo;
        private readonly IBrandingProvider _brandingProvider;
        private readonly ILogger<BrandedViewModelBase> _logger;
        private readonly IWpfDispatcher _wpfDispatcher;

#nullable disable
        [Obsolete("Parameterless constructor used only for WPF design-time DataContext")]
        public BrandedViewModelBase() { }
#nullable enable

        public BrandedViewModelBase(
            IBrandingProvider brandingProvider,
            IWpfDispatcher wpfDispatcher,
            ILogger<BrandedViewModelBase> logger)
        {
            _brandingProvider = brandingProvider;
            _wpfDispatcher = wpfDispatcher;
            _logger = logger;
            _ = Task.Run(ApplyBranding);
        }

        public BitmapImage? Icon { get; set; }

        public string? ProductName { get; set; }

        public SolidColorBrush? TitleBackgroundColor { get; set; }

        public SolidColorBrush? TitleButtonForegroundColor { get; set; }

        public SolidColorBrush? TitleForegroundColor { get; set; }

        public async Task ApplyBranding()
        {
            await _wpfDispatcher.InvokeAsync(async () =>
            {
                try
                {
                    _brandingInfo ??= await _brandingProvider.GetBrandingInfo();

                    ProductName = "Remotely";

                    if (!string.IsNullOrWhiteSpace(_brandingInfo.Product))
                    {
                        ProductName = _brandingInfo.Product;
                    }

                    TitleBackgroundColor = new SolidColorBrush(Color.FromRgb(
                        _brandingInfo.TitleBackgroundRed,
                        _brandingInfo.TitleBackgroundGreen,
                        _brandingInfo.TitleBackgroundBlue));

                    TitleForegroundColor = new SolidColorBrush(Color.FromRgb(
                       _brandingInfo.TitleForegroundRed,
                       _brandingInfo.TitleForegroundGreen,
                       _brandingInfo.TitleForegroundBlue));

                    TitleButtonForegroundColor = new SolidColorBrush(Color.FromRgb(
                       _brandingInfo.ButtonForegroundRed,
                       _brandingInfo.ButtonForegroundGreen,
                       _brandingInfo.ButtonForegroundBlue));

                    Icon = GetBitmapImageIcon(_brandingInfo);

                    OnPropertyChanged(nameof(ProductName));
                    OnPropertyChanged(nameof(TitleBackgroundColor));
                    OnPropertyChanged(nameof(TitleForegroundColor));
                    OnPropertyChanged(nameof(TitleButtonForegroundColor));
                    OnPropertyChanged(nameof(Icon));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error applying branding.");
                }
            });
          
        }
        private BitmapImage GetBitmapImageIcon(BrandingInfo bi)
        {
            try
            {
                Stream imageStream;
                if (bi.Icon?.Any() == true)
                {
                    imageStream = new MemoryStream(bi.Icon);
                }
                else
                {
                    imageStream = typeof(RemoteControl.Shared.Result)
                        .Assembly
                        .GetManifestResourceStream("Immense.RemoteControl.Shared.Assets.DefaultIcon.png") ?? new MemoryStream();
                }

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = imageStream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                imageStream.Close();

                return bitmap;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting app icon.");
                return new BitmapImage();
            }
        }
    }

}
