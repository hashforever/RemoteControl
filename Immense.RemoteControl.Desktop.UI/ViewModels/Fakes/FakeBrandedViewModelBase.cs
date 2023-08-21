﻿using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Immense.RemoteControl.Desktop.UI.Controls.Dialogs;
using Immense.RemoteControl.Shared.Models;
using System.Diagnostics;
using System.IO;

namespace Immense.RemoteControl.Desktop.UI.ViewModels.Fakes;

public class FakeBrandedViewModelBase : IBrandedViewModelBase
{
    private readonly BrandingInfoBase _brandingInfo;
    private Bitmap? _icon;

    public FakeBrandedViewModelBase()
    {
        _brandingInfo = new BrandingInfoBase();
        _icon = GetBitmapImageIcon(_brandingInfo);

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
    }
    public Bitmap? Icon
    {
        get => _icon;
        set => _icon = value;
    }
    public string ProductName { get; set; } = "Test Product";
    public SolidColorBrush? TitleBackgroundColor { get; set; }
    public SolidColorBrush? TitleButtonForegroundColor { get; set; }
    public SolidColorBrush? TitleForegroundColor { get; set; }
    public WindowIcon? WindowIcon { get; set; }

    public Task ApplyBranding()
    {
        return Task.CompletedTask;
    }

    private Bitmap? GetBitmapImageIcon(BrandingInfoBase bi)
    {
        try
        {
            using var imageStream = typeof(Shared.Services.AppState)
                .Assembly
                .GetManifestResourceStream("Immense.RemoteControl.Desktop.Shared.Assets.DefaultIcon.png") ?? new MemoryStream();

            return new Bitmap(imageStream);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return null;
        }
    }
}
