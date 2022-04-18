using DegreePlan.Classes;
using Plugin.LocalNotifications;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

/*
 *
 *   ******** DEGREE PLAN MOBILE APPLICATION ********
 *              by harryse7en, 07-Mar-2022
 *
 *   This application was created for a college course assignment to create a degree plan using Visual Studio 2019 with C# & Xamarin for Android
 *
 */


namespace DegreePlan
{
    public partial class MainPage : ContentPage
    {

        // ---------- Variables ----------
        public List<Assess> assessList = new List<Assess>();
        public List<Course> courseList = new List<Course>();
        public List<Term> termList = new List<Term>();
        private bool init = false;
        private bool dropOnStart = false;  // Set this to TRUE if you want to start with blank database each time



        // ---------- Constructor ----------
        public MainPage()
        {
            InitializeComponent();
            // Create new event handler to determine if user clicked new item in list
            xTermList.ItemTapped += new EventHandler<ItemTappedEventArgs>(xTermList_ItemTapped);
        }



        // ---------- Android App Events ----------
        protected override void OnAppearing()
        {
            using (SQLiteConnection sql = new SQLiteConnection(App.FilePath))
            {
                sql.CreateTable<Assess>();
                sql.CreateTable<Course>();
                sql.CreateTable<Term>();
                termList = sql.Table<Term>().ToList();
            }
            if (termList.Any() && init)
            {
                using (SQLiteConnection sql = new SQLiteConnection(App.FilePath))
                {
                    if (dropOnStart) // Set this value in variables section above
                    {
                        sql.DropTable<Assess>();
                        sql.DropTable<Course>();
                        sql.DropTable<Term>();
                    }
                    sql.CreateTable<Assess>();
                    sql.CreateTable<Course>();
                    sql.CreateTable<Term>();
                    doInitialData();
                }
                init = false;
            }
            else if (init)
            {
                doInitialData();
                init = false;
            }
            using (SQLiteConnection sql = new SQLiteConnection(App.FilePath))
            {
                termList = sql.Table<Term>().ToList();
                xTermList.ItemsSource = termList;
            }
            doNotify();
            base.OnAppearing();
        }



        // ---------- Form Events ----------
        // REFERENCES button click
        async private void xBtnRef_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new References());
        }

        // NEW TERM button click
        async private void xBtnNewTerm_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddTerm(this));
        }

        // TERM LIST item tapped
        async private void xTermList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Term term = (Term)e.Item;
            await Navigation.PushAsync(new ViewTerm(this, term));
        }



        // ---------- Functions ----------
        private void doNotify()
        {
            using (SQLiteConnection sql = new SQLiteConnection(App.FilePath))
            {
                courseList = sql.Query<Course>($"SELECT * FROM tblCourses WHERE Notify = True").ToList();
                assessList = sql.Query<Assess>($"SELECT * FROM tblAssessments WHERE Notify = True").ToList();
            }

            // Notifications for course start and end dates
            foreach (Course c in courseList)
            {
                if (c.StartDate == DateTime.Today)
                {
                    CrossLocalNotifications.Current.Show("REMINDER", $"{c.Name} starts today!");
                }
                if (c.EndDate == DateTime.Today)
                {
                    CrossLocalNotifications.Current.Show("REMINDER", $"{c.Name} ends today!");
                }
                if (c.EndDate == DateTime.Today.AddDays(1))
                {
                    CrossLocalNotifications.Current.Show("REMINDER", $"{c.Name} ends tomorrow!");
                }
            }

            // Notifications for assessment due dates
            foreach (Assess a in assessList)
            {
                if (a.DueDate == DateTime.Today)
                {
                    CrossLocalNotifications.Current.Show("REMINDER", $"{a.Name} is due today!");
                }
                if (a.DueDate == DateTime.Today.AddDays(1))
                {
                    CrossLocalNotifications.Current.Show("REMINDER", $"{a.Name} is due tomorrow!");
                }
            }
        }



        /*  Code to create a set of data for testing purposes, including:
         *    a)  one term
         *    b)  one course
         *    c)  two assessments
         */
        private void doInitialData()
        {
            int insertResult;

            // Create new term lasting 3 months
            Term newTerm = new Term
            {
                Title = "Term #1 (Spring)",
                StartDate = System.DateTime.Today,
                EndDate = System.DateTime.Today.AddMonths(3)
            };
            using (SQLiteConnection sql = new SQLiteConnection(App.FilePath))
            {
                insertResult = sql.Insert(newTerm);
                termList.Add(newTerm);
            }


            // Create new course
            Course newCourse = new Course
            {
                Name = "Basketweaving 101",
                Status = "Enrolled",
                StartDate = System.DateTime.Today.AddDays(7),
                EndDate = System.DateTime.Today.AddMonths(1),
                InstructorName = "Harvey Sanford",
                InstructorPhone = "1234567890",
                InstructorEmail = "harvsan@example.edu",
                Notes = "Example notes that can be entered to enhance your basketweaving skills",
                Notify = false,
                TermId = newTerm.Id
            };
            using (SQLiteConnection sql = new SQLiteConnection(App.FilePath))
            {
                insertResult = sql.Insert(newCourse);
                courseList.Add(newCourse);
            }

            // Create new OA and PA
            Assess newAssessOA = new Assess
            {
                Name = "BWO1",
                Type = "Objective (OA)",
                Status = "Unsubmitted",
                DueDate = newCourse.EndDate,
                Notify = true,
                CourseId = newCourse.Id
            };
            Assess newAssessPA = new Assess
            {
                Name = "BWP1",
                Type = "Performance (PA)",
                Status = "Unsubmitted",
                DueDate = newCourse.EndDate,
                Notify = true,
                CourseId = newCourse.Id
            };
            using (SQLiteConnection sql = new SQLiteConnection(App.FilePath))
            {
                insertResult = sql.Insert(newAssessOA);
                insertResult = sql.Insert(newAssessPA);
                assessList.Add(newAssessOA);
                assessList.Add(newAssessPA);
            }
        }
    }
}
