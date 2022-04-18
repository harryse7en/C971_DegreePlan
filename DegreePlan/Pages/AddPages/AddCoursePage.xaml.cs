using DegreePlan.Classes;
using SQLite;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DegreePlan
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddCourse : ContentPage
    {

        // ---------- Variables ----------
        public MainPage mainPage;
        public Term term;
        public int insertResult;



        // ---------- Constructor ----------
        public AddCourse(MainPage mainPage, Term term)
        {
            this.mainPage = mainPage;
            this.term = term;
            InitializeComponent();
            xBtnAdd.IsVisible = false;
            xTermName.Text = "Term: " + term.Title;
            xStatus.SelectedIndex = 0;
            xStartDate.MinimumDate = term.StartDate;
            xStartDate.MaximumDate = term.EndDate;
            xEndDate.Date = xStartDate.Date.AddDays(30);
            xEndDate.MinimumDate = xStartDate.MinimumDate.AddDays(1);
            xEndDate.MaximumDate = term.EndDate;
        }



        // ---------- Android App Events ----------
        protected override void OnAppearing()
        {
            base.OnAppearing();
        }



        // ---------- Form Events ----------
        // ADD button click
        private async void xBtnAdd_Clicked(object sender, EventArgs e)
        {
            if (doValidate())
            {
                Course newCourse = new Course
                {
                    Name = xName.Text,
                    Status = xStatus.SelectedItem.ToString(),
                    StartDate = xStartDate.Date,
                    EndDate = xEndDate.Date,
                    InstructorName = xInstName.Text,
                    InstructorPhone = xInstPhone.Text,
                    InstructorEmail = xInstEmail.Text,
                    Notes = xNotes.Text,
                    Notify = xNotify.IsToggled,
                    TermId = term.Id
                };
                using (SQLiteConnection sql = new SQLiteConnection(App.FilePath))
                {
                    insertResult = sql.Insert(newCourse);
                    mainPage.courseList.Add(newCourse);
                    await Navigation.PopAsync();
                }
            }
            else
            {
                await DisplayAlert("ERROR", "Please check all fields are entered properly", "OK");
            }
        }

        // NAME text changed
        private void xName_Changed(object sender, FocusEventArgs e)
        {
            doVisible();
        }

        // INSTRUCTOR NAME text changed
        private void xInstName_Changed(object sender, FocusEventArgs e)
        {
            doVisible();
        }

        // INSTRUCTOR PHONE text changed
        private void xInstPhone_Changed(object sender, FocusEventArgs e)
        {
            doVisible();
        }

        // INSTRUCTOR EMAIL text changed
        private void xInstEmail_Changed(object sender, FocusEventArgs e)
        {
            doVisible();
        }

        // START DATE selected (auto-adjust the END DATE)
        private void xStartDate_DateSelected(object sender, DateChangedEventArgs e)
        {
            xEndDate.MaximumDate = xStartDate.Date.AddMonths(6).AddDays(-1);
            xEndDate.MinimumDate = xStartDate.Date.AddMonths(1).AddDays(-1);
        }



        // ---------- Functions ----------
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

        // Used to make the ADD button visible
        private void doVisible()
        {
            if (xName.Text != null && xName.Text != "" &&
                xInstName.Text != null && xInstName.Text != "" &&
                xInstPhone.Text != null && xInstPhone.Text.Length >= 10 &&
                xInstEmail.Text != null && xInstEmail.Text != "")
            {
                xBtnAdd.IsVisible = true;
            }
            else
            {
                xBtnAdd.IsVisible = false;
            }
            return;
        }
    }
}
