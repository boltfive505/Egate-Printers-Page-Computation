using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.IO;
using Egate_Printers_Page_Computation.Objects.Tutorials;
using Egate_Printers_Page_Computation.Helpers;
using Egate_Printers_Page_Computation.SubModals.Tutorials;
using bolt5.ModalWpf;
using bolt5.CloneCopy;

namespace Egate_Printers_Page_Computation.Pages
{
    /// <summary>
    /// Interaction logic for tutorials_page.xaml
    /// </summary>
    public partial class tutorials_page : Page
    {
        public class FilterGroup : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public string FilterDescription { get; set; }
            public int FilterSelectedCategory { get; set; }
            public bool ShowInactiveCategory { get; set; }
        }

        public static readonly DependencyProperty FiltersProperty = DependencyProperty.Register(nameof(Filters), typeof(FilterGroup), typeof(tutorials_page));
        public FilterGroup Filters
        { 
            get { return (FilterGroup)GetValue(FiltersProperty); }
            set { SetValue(FiltersProperty, value); }
        }

        private ICollectionView categoryView;
        private ICollectionView videoView;
        private List<TutorialCategoryViewModel> categoryList = new List<TutorialCategoryViewModel>();
        private List<TutorialVideoViewModel> videoList = new List<TutorialVideoViewModel>();
        private FilterGroup filters2;

        public tutorials_page()
        {
            InitializeComponent();

            categoryView = new CollectionViewSource() { Source = categoryList }.View;
            categoryView.SortDescriptions.Add(new SortDescription("PathName", ListSortDirection.Ascending));
            categoryView.Filter = x => DoFilterCategory(x as TutorialCategoryViewModel);
            categoryDataGrid.ItemsSource = categoryView;

            videoView = new CollectionViewSource() { Source = videoList }.View;
            videoView.SortDescriptions.Add(new SortDescription("ShortDescriptionSimpleText", ListSortDirection.Ascending));
            videoView.Filter = x => DoFilterVideo(x as TutorialVideoViewModel);
            videoDataGrid.ItemsSource = videoView;

            Filters = new FilterGroup();
            Filters.PropertyChanged += Filters_PropertyChanged;
            filtersGroup.DataContext = Filters;

            filters2 = new FilterGroup();
            filters2.PropertyChanged += Filters2_PropertyChanged;
            filtersCategoryGroup.DataContext = filters2;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
            TutorialsHelper.SetCategoryHierarchy(categoryList);
            TutorialsHelper.SetAllCategoryListVideoCounting(categoryList, videoList);

            ExpandAllCategoryList(true);
        }

        private void LoadData()
        {
            List<Task> dataTasks = new List<Task>();
            dataTasks.Add(LoadDataAsync(categoryList, TutorialsHelper.GetCategoryListAsync(), categoryView));
            dataTasks.Add(LoadDataAsync(videoList, TutorialsHelper.GetVideoListAsync(), videoView));
            Task.WaitAll(dataTasks.ToArray());
        }

        private async Task LoadDataAsync<T>(List<T> list, Task<IEnumerable<T>> task, ICollectionView view)
        {
            list.Clear();
            var result = await task;
            list.AddRange(result);
            _ = Dispatcher.BeginInvoke(new Action(() => view.Refresh()), System.Windows.Threading.DispatcherPriority.Background);
        }

        private bool DoFilterVideo(TutorialVideoViewModel i)
        {
            bool flag = true;

            //selected category
            flag &= i.Category.Id == Filters.FilterSelectedCategory;

            //short and long description
            if (!string.IsNullOrWhiteSpace(Filters.FilterDescription))
            {
                string keyword = Filters.FilterDescription.Trim();
                string description = i.ShortDescriptionSimpleText + " " + i.LongDescriptionSimpleText;
                flag &= description.IndexOf(keyword, 0, StringComparison.InvariantCultureIgnoreCase) != -1;
            }

            return flag;
        }

        private bool DoFilterCategory(TutorialCategoryViewModel i)
        {
            bool flag = true;
            if (!filters2.ShowInactiveCategory)
                flag &= i.IsActive && i.IsActiveHierarchy;
            return flag;
        }

        private void Filters_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            videoView.Refresh();
        }

        private void Filters2_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            categoryView.Refresh();
        }

        private void OpenVideo_btn_Click(object sender, RoutedEventArgs e)
        {
            TutorialVideoViewModel video = (sender as FrameworkElement).DataContext as TutorialVideoViewModel;
            string file = FileHelper.GetFile(video.VideoFile, TutorialsHelper.TUTORIAL_VIDEO_FOLDER);
            if (File.Exists(file))
            {
                FileHelper.Open(file);
            }
            else
            {
                MessageBox.Show("ERROR: File not found.", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenRawVideo_btn_Click(object sender, RoutedEventArgs e)
        {
            string fileName = Convert.ToString((sender as FrameworkElement).Tag);
            string file = FileHelper.GetFile(fileName, TutorialsHelper.TUTORIAL_RAW_VIDEO_FOLDER);
            if (File.Exists(file))
            {
                FileHelper.Open(file);
            }
            else
            {
                MessageBox.Show("ERROR: File not found.", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenFileAttachment_btn_Click(object sender, RoutedEventArgs e)
        {
            string fileName = Convert.ToString((sender as FrameworkElement).Tag);
            string file = FileHelper.GetFile(fileName, TutorialsHelper.TUTORIAL_FILES_FOLDER);
            if (File.Exists(file))
            {
                FileHelper.Open(file);
            }
            else
            {
                MessageBox.Show("ERROR: File not found.", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenYoutubeLink_btn_Click(object sender, RoutedEventArgs e)
        {
            string link = Convert.ToString((sender as FrameworkElement).Tag);
            System.Diagnostics.Process.Start(link);
        }

        private void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            bool isEdit = true;
            string title = "Edit Category";
            var category = (sender as FrameworkElement).DataContext as TutorialCategoryViewModel;
            if (category == null)
            {
                isEdit = false;
                title = "Add Category";
                category = new TutorialCategoryViewModel();
            }
            var modal = new category_add_modal();
            var clone = category.DeepClone();
            modal.DataContext = clone;
            if (ModalForm.ShowModal(modal, title, ModalButtons.SaveCancel) == ModalResult.Save)
            {
                clone.DeepCopyTo(category);
                var task = TutorialsHelper.AddUpdateCategoryAsync(category);
                task.ContinueWith(t =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        if (!isEdit)
                            categoryList.Add(category);
                        DoPostAddUpdateCategory(category);
                    });
                });
            }
        }

        private void AddVideo_Click(object sender, RoutedEventArgs e)
        {
            bool isEdit = true;
            string title = "Edit Video";
            ModalButtons buttons = ModalButtons.SaveCancelDelete;
            var video = (sender as FrameworkElement).DataContext as TutorialVideoViewModel;
            if (video == null)
            {
                isEdit = false;
                title = "Add Video";
                buttons = ModalButtons.SaveCancel;
                video = new TutorialVideoViewModel();
                video.Category = (sender as FrameworkElement).DataContext as TutorialCategoryViewModel;
            }
            var modal = new video_add_modal();
            var clone = video.DeepClone();
            modal.DataContext = clone;
            ModalResult result = ModalForm.ShowModal(modal, title, buttons);
            if (result == ModalResult.Save)
            {
                clone.DeepCopyTo(video);
                _ = TutorialsHelper.AddUpdateVideoAsync(video);
                if (!isEdit)
                    videoList.Add(video);
                videoView.Refresh();
                TutorialsHelper.SetAllCategoryListVideoCounting(categoryList, videoList);
            }
            else if (result == ModalResult.Delete)
            {
                _ = TutorialsHelper.DeleteVideo(video);
                videoList.Remove(video);
                videoView.Refresh();
                videoDataGrid.UnselectAll();
                TutorialsHelper.SetAllCategoryListVideoCounting(categoryList, videoList);
            }
        }

        private void DoPostAddUpdateCategory(TutorialCategoryViewModel categoryVm)
        {
            TutorialsHelper.SetCategoryHierarchy(categoryList);
            if (categoryVm.ParentCategory != null)
                categoryVm.ParentCategory.IsCategoryExpanded = true;
            categoryView.Refresh();
            categoryDataGrid.SelectedValue = categoryVm.Id;
        }

        private void ExpandAll_btn_Click(object sender, RoutedEventArgs e)
        {
            ExpandAllCategoryList(true);
        }

        private void CollapseAll_btn_Click(object sender, RoutedEventArgs e)
        {
            ExpandAllCategoryList(false);
        }

        private void ExpandAllCategoryList(bool expand)
        {
            foreach (var cat in categoryList)
                cat.IsCategoryExpanded = expand;
        }

        private void FullExpandCategory(TutorialCategoryViewModel category)
        {
            var cat = categoryList.FirstOrDefault(i => i.Id == category.Id);
            if (cat != null)
            {
                cat.IsCategoryExpanded = true;
                if (cat.ParentCategory != null)
                    FullExpandCategory(cat.ParentCategory);
            }
        }

        public void SelectSchedule(TutorialVideoViewModel video)
        {
            FullExpandCategory(video.Category);
            categoryDataGrid.SelectedItem = video.Category;
            videoDataGrid.SelectedItem = video;
        }
    }
}
