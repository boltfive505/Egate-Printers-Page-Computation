﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;
using AForge.Video;
using AForge.Video.DirectShow;
using CustomControls.Modal;

namespace Egate_Printers_Page_Computation.Templates
{
    /// <summary>
    /// Interaction logic for open_camera_modal.xaml
    /// </summary>
    public partial class open_camera_modal : UserControl
    {
        public static readonly DependencyProperty CapturedImageProperty = DependencyProperty.Register(nameof(CapturedImage), typeof(BitmapImage), typeof(open_camera_modal));
        public BitmapImage CapturedImage
        {
            get { return (BitmapImage)GetValue(CapturedImageProperty); }
            set { SetValue(CapturedImageProperty, value); }
        }

        private bool captureSnapshot = false;

        public open_camera_modal()
        {
            InitializeComponent();

            cameraList_cbox.ItemsSource = GetCameraDeviceList();
            cameraList_cbox.SelectedIndex = 0;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            CloseCurrentVideoSource();
        }

        public string GetCapturedImageFile()
        {
            if (this.CapturedImage == null) return string.Empty;
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(this.CapturedImage));
            string imgFile = Path.Combine(Path.GetTempPath(), string.Format("captured_{0}.png", DateTime.Now.ToString("yyyy-MM-dd-hhhmmss")));
            using (FileStream fs = new FileStream(imgFile, FileMode.Create, FileAccess.Write))
            {
                encoder.Save(fs);
            }
            //set filename value
            return imgFile;
        }

        private IEnumerable<FilterInfo> GetCameraDeviceList()
        {
            var devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            return devices.OfType<FilterInfo>();
        }

        private void cameraList_cbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var info = (sender as ComboBox).SelectedItem as FilterInfo;
            if (info != null)
            {
                var device = new VideoCaptureDevice(info.MonikerString);
                SetupVideoCaptureDevice(device);
                OpenVideoSource(device);
            }
        }

        //runs on separate thread
        private void videoSourcePlayer_NewFrame(object sender, ref System.Drawing.Bitmap image)
        {
            Size actualSize = new Size(image.Width, image.Height);
            Dispatcher.BeginInvoke(new Action(() => 
            {
                VideoSizeText.Text = string.Format("{0}x{1}", actualSize.Width, actualSize.Height);
                DoResizeVideoSourcePlayer(actualSize);
            }), System.Windows.Threading.DispatcherPriority.Background);

            if (captureSnapshot)
            {
                var img = image;
                Dispatcher.Invoke(() => DoCaptureSnapshot(img));
                captureSnapshot = false;
            }
        }

        private void DoResizeVideoSourcePlayer(Size actualSize)
        {
            Size maxSize = new Size(cameraContainer.ActualWidth, cameraContainer.ActualHeight);
            Size resize = Helpers.ImageHelper.Resize(actualSize, maxSize);
            cameraHost.Width = resize.Width;
            cameraHost.Height = resize.Height;
        }

        private void DoCaptureSnapshot(System.Drawing.Bitmap image)
        {
            BitmapImage bitmap = new BitmapImage();
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;
                bitmap.BeginInit();
                bitmap.StreamSource = ms;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
            }
            bitmap.Freeze();
            CapturedImage = bitmap;
            cameraContainer.Visibility = Visibility.Collapsed;
            snapshotContainer.Visibility = Visibility.Visible;
            capture_btn.IsEnabled = false;
            retry_btn.IsEnabled = true;
        }

        private void SetupVideoCaptureDevice(VideoCaptureDevice device)
        {
            if (device.VideoCapabilities.Length > 0)
                device.VideoResolution = device.VideoCapabilities.Last();
            //if (device.SnapshotCapabilities.Length > 0)
            //    device.SnapshotResolution = device.SnapshotCapabilities.Last();
        }

        private void OpenVideoSource(IVideoSource source)
        {
            CloseCurrentVideoSource();

            videoSourcePlayer.VideoSource = source;
            videoSourcePlayer.Start();
        }

        private void CloseCurrentVideoSource()
        {
            if (videoSourcePlayer.VideoSource != null)
            {
                videoSourcePlayer.SignalToStop();
                videoSourcePlayer.WaitForStop();
                //if (videoSourcePlayer.IsRunning)
                //    videoSourcePlayer.Stop();
                videoSourcePlayer.VideoSource = null;
            }
        }

        public void RetryCapture()
        {
            CapturedImage = null;
            cameraContainer.Visibility = Visibility.Visible;
            snapshotContainer.Visibility = Visibility.Collapsed;
            capture_btn.IsEnabled = true;
            retry_btn.IsEnabled = false;
        }

        private void capture_btn_Click(object sender, RoutedEventArgs e)
        {
            captureSnapshot = true;
        }

        private void retry_btn_Click(object sender, RoutedEventArgs e)
        {
            RetryCapture();
        }

        public void ModalClosing(ModalClosingArgs e)
        {
            if (e.Result == ModalResult.Yes)
            {
                if (this.CapturedImage == null)
                    e.Cancel = true;
            }
        }
    }
}
