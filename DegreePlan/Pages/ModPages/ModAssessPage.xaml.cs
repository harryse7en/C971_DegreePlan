using DegreePlan.Classes;
using SQLite;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DegreePlan
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ModAssess : ContentPage
    {


        // ---------- Variables ----------
        public MainPage mainPage;
        public int updateResult, deleteResult;
        public Course course;
        public Assess assess;



        // ---------- Constructor ----------
        public ModAssess(MainPage mainPage, Course course, Assess assess)
        {
            this.mainPage = mainPage;
            this.course = course;
            this.assess = assess;
            InitializeComponent();
            xName.Text = assess.Name;
            if (assess.Type == "Objective (OA)")
            {
                xType.SelectedIndex = 0;
            }
            if (assess.Type == "Performance (PA)")
            {
                xType.SelectedIndex = 1;
            }
            if (assess.Status == "Unsubmitted")
            {
                xStatus.SelectedIndex = 0;
            }
            if (assess.Status == "In Queue")
            {
                xStatus.SelectedIndex = 1;
            }
            if (assess.Status == "Passed")
            {
                xStatus.SelectedIndex = 2;
            }
            if (assess.Status == "Returned")
            {
                xStatus.SelectedIndex = 3;
            }
            xDueDate.Date = assess.DueDate;
            xDueDate.MinimumDate = course.StartDate;
            xDueDate.MaximumDate = course.EndDate;
            xNotify.IsToggled = assess.Notify;
            xBtnSave.IsVisible = false;
        }



        // ---------- Android App Events ----------
        protected override void OnAppearing()
        {
            xName.Text = assess.Name;
            if (assess.Type == "Objective (OA)")
            {
                xType.SelectedIndex = 0;
            }
            if (assess.Type == "Performance (PA)")
            {
                xType.SelectedIndex = 1;
            }
            if (assess.Status == "Unsubmitted")
            {
                xStatus.SelectedIndex = 0;
            }
            if (assess.Status == "In Queue")
            {
                xStatus.SelectedIndex = 1;
            }
            if (assess.Status == "Passed")
            {
                xStatus.SelectedIndex = 2;
            }
            if (assess.Status == "Returned")
            {
                xStatus.SelectedIndex = 3;
            }
            xDueDate.Date = assess.DueDate;
            xDueDate.MinimumDate = course.StartDate;
            xDueDate.MaximumDate = course.EndDate;
            xNotify.IsToggled = assess.Notify;
            base.OnAppearing();
        }



        // ---------- Form Events ----------
        // SAVE button click
        private async void xBtnSave_Clicked(object sender, EventArgs e)
        {
            if (doValidate())
            {
                if (doDupCheck())
                {
                    assess.Name = xName.Text;
                    assess.Status = xStatus.SelectedItem.ToString();
                    assess.Type = xType.SelectedItem.ToString();
                    assess.DueDate = xDueDate.Date;
                    assess.Notify = xNotify.IsToggled;
                    using (SQLiteConnection sql = new SQLiteConnection(App.FilePath))
                    {
                        updateResult = sql.Update(assess);
                        await Navigation.PopAsync();
                    }
                }
                else
                {
                    await DisplayAlert("ERROR", "You are only allowed 1 OA and 1 PA.", "OK");
                }
            }
            else
            {
                await DisplayAlert("ERROR", "Please check all fields are entered properly", "OK");
            }
        }

        // DELETE button click
        private async void xBtnDelete_Clicked(object sender, EventArgs e)
        {
            if (await DisplayAlert("CONFIRM", "Are you sure you want to delete this course?", "YES", "CANCEL"))
            {
                using (SQLiteConnection sql = new SQLiteConnection(App.FilePath))
                {
                    deleteResult = sql.Delete(assess);
                    await Navigation.PopAsync();
                }
            }
        }

        // NAME text changed (used to make SAVE button visible)
        private void xName_Changed(object sender, FocusEventArgs e)
        {
            doEditCheck();
        }

        // DUE DATE selected (used to make SAVE button visible)
        private void xDueDate_DateSelected(object sender, DateChangedEventArgs e)
        {
            doEditCheck();
        }

        // TYPE selected (used to make SAVE button visible)
        private void xType_Changed(object sender, EventArgs e)
        {
            doEditCheck();
        }

        // STATUS selected (used to make SAVE button visible)
        private void xStatus_Changed(object sender, EventArgs e)
        {
            doEditCheck();
        }

        // NOTIFICATIONS toggled (used to make SAVE button visible)
        private void xNotify_Toggled(object sender, ToggledEventArgs e)
        {
            doEditCheck();
        }



        // ---------- Functions ----------
        private bool doDupCheck()
        {
            using (SQLiteConnection sql = new SQLiteConnection(App.FilePath))
            {
                var numOA = sql.Query<Assess>($"SELECT * FROM tblAssessments WHERE CourseId = '{course.Id}' AND Type = 'Objective (OA)'");
                var numPA = sql.Query<Assess>($"SELECT * FROM tblAssessments WHERE CourseId = '{course.Id}' AND Type = 'Performance (PA)'");

                if (xType.SelectedItem.ToString() == "Objective (OA)" && (numOA.Count == 0 || assess.Type == "Objective (OA)"))
                {
                    return true;
                }
                else if (xType.SelectedItem.ToString() == "Performance (PA)" && (numPA.Count == 0 || assess.Type == "Performance (PA)"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private void doEditCheck()
        {
            if (xName.Text != assess.Name ||
                xDueDate.Date != assess.DueDate ||
                xType.SelectedItem.ToString() != assess.Type ||
                xNotify.IsToggled != assess.Notify ||
                xStatus.SelectedItem.ToString() != assess.Status)
            {
                xBtnSave.IsVisible = true;
            }
            else
            {
                xBtnSave.IsVisible = false;
            }
        }

        // Used to make the ADD button visible
        private bool doValidate()
        {
            if (xName.Text != null && xName.Text != "" &&
                xDueDate.Date != null && xType.SelectedItem.ToString() != null &&
                xStatus.SelectedItem.ToString() != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
