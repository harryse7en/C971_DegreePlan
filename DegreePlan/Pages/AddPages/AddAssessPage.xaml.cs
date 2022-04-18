using DegreePlan.Classes;
using SQLite;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DegreePlan
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddAssess : ContentPage
    {

        // ---------- Variables ----------
        public MainPage mainPage;
        public Term term;
        public Course course;
        public int insertResult;



        // ---------- Constructor ----------
        public AddAssess(MainPage mainPage, Term term, Course course)
        {
            this.mainPage = mainPage;
            this.term = term;
            this.course = course;
            InitializeComponent();
            xBtnAdd.IsVisible = false;
            xCourseName.Text = "Course: " + course.Name;
            xStatus.SelectedIndex = 0;
            xType.SelectedIndex = 0;
            xDueDate.MinimumDate = course.StartDate;
            xDueDate.MaximumDate = course.EndDate;
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
                if (doDupCheck())
                {
                    Assess newAssess = new Assess
                    {
                        Name = xName.Text,
                        Type = xType.SelectedItem.ToString(),
                        Status = xStatus.SelectedItem.ToString(),
                        DueDate = xDueDate.Date,
                        Notify = xNotify.IsToggled,
                        CourseId = course.Id
                    };
                    using (SQLiteConnection sql = new SQLiteConnection(App.FilePath))
                    {
                        insertResult = sql.Insert(newAssess);
                        mainPage.assessList.Add(newAssess);
                        await Navigation.PopAsync();
                    }
                }
                else
                {
                    await DisplayAlert("ERROR", "You are only allowed 1 OA and 1 PA", "OK");
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



        // ---------- Functions ----------
        private bool doDupCheck()
        {
            using (SQLiteConnection sql = new SQLiteConnection(App.FilePath))
            {
                var numOA = sql.Query<Assess>($"SELECT * FROM tblAssessments WHERE CourseId = '{course.Id}' AND Type = 'Objective (OA)'");
                var numPA = sql.Query<Assess>($"SELECT * FROM tblAssessments WHERE CourseId = '{course.Id}' AND Type = 'Performance (PA)'");

                if (xType.SelectedItem.ToString() == "Objective (OA)" && numOA.Count == 0)
                {
                    return true;
                }
                else if (xType.SelectedItem.ToString() == "Performance (PA)" && numPA.Count == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        // Validate all data entered is valid before doing SQL transaction
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

        // Used to make the ADD button visible
        private void doVisible()
        {
            if (xName.Text != null && xName.Text != "")
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
