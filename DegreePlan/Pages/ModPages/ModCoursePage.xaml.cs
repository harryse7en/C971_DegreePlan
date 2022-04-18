using DegreePlan.Classes;
using SQLite;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DegreePlan
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ModCourse : ContentPage
    {

        // ---------- Variables ----------
        public MainPage mainPage;
        public int updateResult, deleteResult;
        public Term term;
        public Course course;



        // ---------- Constructor ----------
        public ModCourse(MainPage mainPage, Term term, Course course)
        {
            this.mainPage = mainPage;
            this.term = term;
            this.course = course;
            InitializeComponent();
            xName.Text = course.Name;
            if (course.Status == "Registered")
            {
                xStatus.SelectedIndex = 0;
            }
            if (course.Status == "Enrolled")
            {
                xStatus.SelectedIndex = 1;
            }
            if (course.Status == "In Progress")
            {
                xStatus.SelectedIndex = 2;
            }
            if (course.Status == "Completed")
            {
                xStatus.SelectedIndex = 3;
            }
            if (course.Status == "Withdrawn")
            {
                xStatus.SelectedIndex = 4;
            }
            xStartDate.Date = course.StartDate;
            xStartDate.MinimumDate = term.StartDate;
            xStartDate.MaximumDate = term.EndDate;
            xEndDate.Date = course.EndDate;
            xEndDate.MinimumDate = xStartDate.MinimumDate.AddDays(1);
            xEndDate.MaximumDate = term.EndDate;
            xInstName.Text = course.InstructorName;
            xInstPhone.Text = course.InstructorPhone;
            xInstEmail.Text = course.InstructorEmail;
            xNotify.IsToggled = course.Notify;
            xNotes.Text = course.Notes;
            xBtnSave.IsVisible = false;
        }



        // ---------- Android App Events ----------
        protected override void OnAppearing()
        {
            xName.Text = course.Name;
            if (course.Status == "Registered")
            {
                xStatus.SelectedIndex = 0;
            }
            if (course.Status == "Enrolled")
            {
                xStatus.SelectedIndex = 1;
            }
            if (course.Status == "In Progress")
            {
                xStatus.SelectedIndex = 2;
            }
            if (course.Status == "Completed")
            {
                xStatus.SelectedIndex = 3;
            }
            if (course.Status == "Withdrawn")
            {
                xStatus.SelectedIndex = 4;
            }
            xStartDate.Date = course.StartDate;
            xStartDate.MinimumDate = term.StartDate;
            xStartDate.MaximumDate = term.EndDate;
            xEndDate.Date = course.EndDate;
            xEndDate.MinimumDate = xStartDate.MinimumDate.AddDays(1);
            xEndDate.MaximumDate = term.EndDate;
            xInstName.Text = course.InstructorName;
            xInstPhone.Text = course.InstructorPhone;
            xInstEmail.Text = course.InstructorEmail;
            xNotify.IsToggled = course.Notify;
            xNotes.Text = course.Notes;
            base.OnAppearing();
        }



        // ---------- Form Events ----------
        // SAVE button click
        private async void xBtnSave_Clicked(object sender, EventArgs e)
        {
            if (doValidate())
            {
                course.Name = xName.Text;
                course.Status = xStatus.SelectedItem.ToString();
                course.StartDate = xStartDate.Date;
                course.EndDate = xEndDate.Date;
                course.InstructorName = xInstName.Text;
                course.InstructorPhone = xInstPhone.Text;
                course.InstructorEmail = xInstEmail.Text;
                course.Notify = xNotify.IsToggled;
                course.Notes = xNotes.Text;
                using (SQLiteConnection sql = new SQLiteConnection(App.FilePath))
                {
                    updateResult = sql.Update(course);
                    await Navigation.PopAsync();
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
                    deleteResult = sql.Delete(course);
                    Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);
                    await Navigation.PopAsync();
                }
            }
        }

        // NAME text changed (used to make SAVE button visible)
        private void xName_Changed(object sender, FocusEventArgs e)
        {
            doEditCheck();
        }

        // STATUS selected (used to make SAVE button visible)
        private void xStatus_Changed(object sender, EventArgs e)
        {
            doEditCheck();
        }

        // INSTRUCTOR NAME text changed
        private void xInstName_Changed(object sender, FocusEventArgs e)
        {
            doEditCheck();
        }

        // INSTRUCTOR PHONE text changed
        private void xInstPhone_Changed(object sender, FocusEventArgs e)
        {
            doEditCheck();
        }

        // INSTRUCTOR EMAIL text changed
        private void xInstEmail_Changed(object sender, FocusEventArgs e)
        {
            doEditCheck();
        }

        // START DATE selected (auto-adjust the END DATE and make SAVE button visible)
        private void xStartDate_DateSelected(object sender, DateChangedEventArgs e)
        {
            xEndDate.MaximumDate = xStartDate.Date.AddMonths(6).AddDays(-1);
            xEndDate.MinimumDate = xStartDate.Date.AddMonths(1).AddDays(-1);
            doEditCheck();
        }

        // STOP DATE selected (used to make SAVE button visible)
        private void xEndDate_DateSelected(object sender, DateChangedEventArgs e)
        {
            doEditCheck();
        }

        // NOTIFICATIONS toggled (used to make SAVE button visible)
        private void xNotify_Toggled(object sender, ToggledEventArgs e)
        {
            doEditCheck();
        }

        private void xNotes_TextChanged(object sender, TextChangedEventArgs e)
        {
            doEditCheck();
        }



        // ---------- Functions ----------
        private void doEditCheck()
        {
            if (xName.Text != course.Name ||
                xStartDate.Date != course.StartDate ||
                xEndDate.Date != course.EndDate ||
                xInstName.Text != course.InstructorName ||
                xInstPhone.Text != course.InstructorPhone ||
                xInstEmail.Text != course.InstructorEmail ||
                xNotes.Text != course.Notes ||
                xNotify.IsToggled != course.Notify ||
                xStatus.SelectedItem.ToString() != course.Status)
            {
                xBtnSave.IsVisible = true;
            }
            else
            {
                xBtnSave.IsVisible = false;
            }
        }

        // Validate all data entered is valid before doing SQL transaction
        private bool doValidate()
        {
            if (xName.Text != null && xName.Text != "" &&
                xInstName.Text != null && xInstName.Text != "" &&
                xInstPhone.Text != null && xInstPhone.Text.Length >= 10 &&
                xInstEmail.Text != null && xInstEmail.Text != "" &&
                xStartDate.Date != null && xEndDate.Date != null &&
                xEndDate.Date > xStartDate.Date &&
                xStatus.SelectedItem.ToString() != null)
            {
                try
                {
                    System.Net.Mail.MailAddress email = new System.Net.Mail.MailAddress(xInstEmail.Text);
                    return true;
                }
                catch (FormatException)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
