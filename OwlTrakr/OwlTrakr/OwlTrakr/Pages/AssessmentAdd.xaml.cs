﻿using OwlTrakr.Models;
using OwlTrakr.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OwlTrakr.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AssessmentAdd : ContentPage
    {
        public Term _term;
        public Course _course;
        public string PageSubtitle { get; set; }

        public AssessmentAdd(Term term, Course course)
        {
            _term = term;
            _course = course;
            PageSubtitle = "Term: " + term.Title + ", Course: " + course.Title;
            BindingContext = this;
            InitializeComponent();

            List<string> TypeOptions = new List<string>();
            TypeOptions.Add("Objective Assessment");
            TypeOptions.Add("Performance Assessment");

            Picker type = (Picker)FindByName("NewAssessment_Type");
            type.ItemsSource = TypeOptions;
        }

        async public void SaveAssessment_Clicked(object sender, EventArgs e)
        {
            Assessment assessment = new Assessment()
            {
                TermId = this._term.Id,
                CourseId = this._course.Id,
                Title = ((Entry)FindByName("NewAssessment_Title")).Text,
                Start = ((DatePicker)FindByName("NewAssessment_StartDate")).Date,
                End = ((DatePicker)FindByName("NewAssessment_EndDate")).Date,
                Type = (string)((Picker)FindByName("NewAssessment_Type")).SelectedItem,
                NotificationsEnabled = ((Switch)FindByName("NewAssessment_NotificationsEnabled")).IsToggled
            };

            if (String.IsNullOrEmpty(assessment.Title))
            {
                await DisplayAlert("Error", "You must provide the assessment title", "OK");
                return;
            }

            if (String.IsNullOrEmpty(assessment.Type))
            {
                await DisplayAlert("Error", "You must provide the assessment type", "OK");
                return;
            }

            if (assessment.Start > assessment.End)
            {
                await DisplayAlert("Error", "Start date must be before end date", "OK");
                return;
            }

            ObservableCollection<Assessment> assessments = await Data.FetchAssessments(this._term, this._course);
            if (assessments.Any(a => a.Type == assessment.Type))
            {
                await DisplayAlert("Error", "This course already contains this type of assessment", "OK");
                return;
            }

            await Data.NewAssessment(assessment);
            AssessmentListViewModel.instance.Assessments.Add(assessment);
            await Navigation.PopAsync();
        }

        private void NewAssessment_StartDate_DateSelected(object sender, DateChangedEventArgs e)
        {
            ((DatePicker)FindByName("NewAssessment_EndDate")).MinimumDate = e.NewDate;
        }
    }
}