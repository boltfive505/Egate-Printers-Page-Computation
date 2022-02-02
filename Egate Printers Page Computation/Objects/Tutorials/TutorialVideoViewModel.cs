﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tutorials.Model;
using Egate_Printers_Page_Computation.Helpers;
using Egate_Printers_Page_Computation.Objects.Calendar;

namespace Egate_Printers_Page_Computation.Objects.Tutorials
{
    public class TutorialVideoViewModel : INotifyPropertyChanged, IPeriodGetter
    {
        private Converters.HtmlToPlainTextConverter htmlToText = new Converters.HtmlToPlainTextConverter();

        public event PropertyChangedEventHandler PropertyChanged;
        public int Id { get; set; } //database reference
        public string LongDescription { get; set; }
        public string ShortDescription { get; set; }
        public TutorialCategoryViewModel Category { get; set; }
        public EntryLevel EntryLevel { get; set; }
        public string VideoFile { get; set; }
        public string ThumbnailFile { get; set; }
        public string FileAttachment { get; set; }
        public string YoutubeUrlLink { get; set; }
        public string RawVideoFile1 { get; set; }
        public string RawVideoFile2 { get; set; }
        public string RawVideoFile3 { get; set; }
        public TutorialPeriodType PeriodType { get; set; }
        public int? PeriodDay { get; set; }
        public string PeriodName { get; set; }
        public bool IsPeriodNotApplicable { get; set; } = true;
        public TutorialEmployeeViewModel EmployeeAssignedTo { get; set; }

        public string ShortDescriptionSimpleText
        {
            get { return (string)htmlToText.Convert(ShortDescription, null, null, null); }
        }

        public string LongDescriptionSimpleText
        {
            get { return (string)htmlToText.Convert(LongDescription, null, null, null); }
        }

        public string PeriodDescription
        {
            get
            {
                if (IsPeriodNotApplicable) return string.Empty;
                switch (this.PeriodType)
                {
                    case TutorialPeriodType.None:
                    case TutorialPeriodType.Daily:
                        return string.Format("{0}", this.PeriodType);
                    case TutorialPeriodType.Monthly:
                    case TutorialPeriodType.Weekly:
                        return string.Format("{0} ({1})", this.PeriodType, this.PeriodDay);
                    default:
                        return string.Empty;
                }
            }
        }

        public string ThumbnailFullPath
        {
            get
            {
                if (!string.IsNullOrEmpty(ThumbnailFile))
                    return FileHelper.GetFile(ThumbnailFile, TutorialsHelper.TUTORIAL_THUMBNAIL_FOLDER);
                return string.Empty;
            }
        }

        public TutorialVideoViewModel()
        { }

        public TutorialVideoViewModel(video entity, category categoryEntity, employee employeeEntity)
        {
            if (entity == null) return;
            this.Id = (int)entity.Id;
            this.LongDescription = entity.LongDescription;
            this.ShortDescription = entity.ShortDescription;
            this.Category = new TutorialCategoryViewModel(categoryEntity);
            this.EntryLevel = entity.EntryLevel.ToEnum<EntryLevel>();
            this.VideoFile = entity.VideoFile;
            this.ThumbnailFile = entity.ThumbnailFile;
            this.FileAttachment = entity.FileAttachment;
            this.YoutubeUrlLink = entity.YoutubeUrlLink;
            this.RawVideoFile1 = entity.RawVideoFile1;
            this.RawVideoFile2 = entity.RawVideoFile2;
            this.RawVideoFile3 = entity.RawVideoFile3;
            this.PeriodType = entity.PeriodType.ToEnum<TutorialPeriodType>();
            this.PeriodDay = entity.PeriodDay;
            this.PeriodName = entity.PeriodName;
            this.IsPeriodNotApplicable = entity.IsPeriodNotApplicable.ToBool();
            this.EmployeeAssignedTo = employeeEntity == null ? null : new TutorialEmployeeViewModel(employeeEntity);
        }

        public IEnumerable<DateTime> GetPeriodDates(int year, int month)
        {
            List<DateTime> dates = new List<DateTime>();
            if (!IsPeriodNotApplicable)
            {
                try
                {
                    switch (this.PeriodType)
                    {
                        case TutorialPeriodType.Daily:
                            dates.AddRange(GetDailyDates(year, month));
                            break;
                        case TutorialPeriodType.Weekly:
                            dates.AddRange(GetWeeklyDates(year, month, this.PeriodDay.Value));
                            break;
                        case TutorialPeriodType.Monthly:
                            dates.Add(new DateTime(year, month, this.PeriodDay.Value));
                            break;
                    }
                }
                catch
                { }
            }
            return dates;
        }

        private IEnumerable<DateTime> GetDailyDates(int year, int month)
        {
            int daysInMonth = DateTime.DaysInMonth(year, month);
            return Enumerable.Range(1, daysInMonth).Select(x => new DateTime(year, month, x));
        }

        private IEnumerable<DateTime> GetWeeklyDates(int year, int month, int day)
        {
            int daysInMonth = DateTime.DaysInMonth(year, month);
            int weeksInMonth = (daysInMonth / 7);
            return Enumerable.Range(0, weeksInMonth).Select(x => new DateTime(year, month, (x * 7) + day));
        }

        public override bool Equals(object obj)
        {
            if (obj is TutorialVideoViewModel)
            {
                var o = obj as TutorialVideoViewModel;
                return this.Id == o.Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}
