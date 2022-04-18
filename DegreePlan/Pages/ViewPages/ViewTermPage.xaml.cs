using DegreePlan.Classes;
using SQLite;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DegreePlan
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewTerm : ContentPage
    {

        // ---------- Variables ----------
        public MainPage mainPage;
        public Term term;



        // ---------- Constructor ----------
        public ViewTerm(MainPage mainPage, Term term)
        {
            this.mainPage = mainPage;
            this.term = term;
            InitializeComponent();
            xPageTitle.Text = term.Title;
            xTermStartDate.Text = term.StartDate.ToString("MM/dd/yy");
            xTermEndDate.Text = term.EndDate.ToString("MM/dd/yy");
            // Create new event handler to determine if user clicked new item in list
            xCourseList.ItemTapped += new EventHandler<ItemTappedEventArgs>(xCourseList_ItemTapped);
        }



        // ---------- Android App Events ----------
        protected override void OnAppearing()
        {
            using (SQLiteConnection sql = new SQLiteConnection(App.FilePath))
            {
                xPageTitle.Text = term.Title;
                xTermStartDate.Text = term.StartDate.ToString("MM/dd/yy");
                xTermEndDate.Text = term.EndDate.ToString("MM/dd/yy");
                mainPage.courseList = sql.Query<Course>($"SELECT * FROM tblCourses WHERE TermId = '{term.Id}'").ToList();
                xCourseList.ItemsSource = mainPage.courseList;
            }
            base.OnAppearing();
        }



        // ---------- Form Events ----------
        // ADD COURSE button click
        async private void xBtnAdd_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddCourse(mainPage, term));
        }

        // EDIT TERM button click
        async private void xBtnEdit_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ModTerm(mainPage, term));
        }

        // COURSE LIST item tapped
        async private void xCourseList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Course course = (Course)e.Item;
            await Navigation.PushAsync(new ViewCourse(mainPage, term, course));
        }
    }
}