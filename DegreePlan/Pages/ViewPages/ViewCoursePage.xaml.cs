using DegreePlan.Classes;
using SQLite;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DegreePlan
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewCourse : ContentPage
    {

        // ---------- Variables ----------
        public MainPage mainPage;
        public Term term;
        public Course course;



        // ---------- Constructor ----------
        public ViewCourse(MainPage mainPage, Term term, Course course)
        {
            this.mainPage = mainPage;
            this.term = term;
            this.course = course;
            InitializeComponent();
            xPageTitle.Text = course.Name;
            xCourseStartDate.Text = course.StartDate.ToString("MM/dd/yy");
            xCourseEndDate.Text = course.EndDate.ToString("MM/dd/yy");
            xStatus.Text = course.Status;
            xInstName.Text = course.InstructorName;
            xInstPhone.Text = course.InstructorPhone;
            xInstEmail.Text = course.InstructorEmail;
            xNotes.Text = course.Notes;
            // Create new event handler to determine if user clicked new item in list
            xAssessList.ItemTapped += new EventHandler<ItemTappedEventArgs>(xAssessList_ItemTapped);
        }



        // ---------- Android App Events ----------
        protected override void OnAppearing()
        {
            using (SQLiteConnection sql = new SQLiteConnection(App.FilePath))
            {
                xPageTitle.Text = course.Name;
                xCourseStartDate.Text = course.StartDate.ToString("MM/dd/yy");
                xCourseEndDate.Text = course.EndDate.ToString("MM/dd/yy");
                xStatus.Text = course.Status;
                xInstName.Text = course.InstructorName;
                xInstPhone.Text = course.InstructorPhone;
                xInstEmail.Text = course.InstructorEmail;
                xNotes.Text = course.Notes;
                mainPage.assessList = sql.Query<Assess>($"SELECT * FROM tblAssessments WHERE CourseId = '{course.Id}'").ToList();
                xAssessList.ItemsSource = mainPage.assessList;
            }
            base.OnAppearing();
        }



        // ---------- Form Events ----------
        // ADD ASSESSMENT button click
        async private void xBtnAdd_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddAssess(mainPage, term, course));
        }

        // EDIT COURSE button click
        async private void xBtnEdit_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ModCourse(mainPage, term, course));
        }

        // ASSESSMENT LIST item tapped
        async private void xAssessList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Assess assess = (Assess)e.Item;
            await Navigation.PushAsync(new ModAssess(mainPage, course, assess));
        }

        // SHARE BUTTON tapped
        private async void xShare_Tapped(object sender, EventArgs e)
        {
            await doNoteShare(course.Notes);
        }



        // ---------- Functions ----------
        public async Task doNoteShare(string text)
        {
            await Share.RequestAsync(new ShareTextRequest{Text = text, Title = "Share Notes"});
        }
    }
}