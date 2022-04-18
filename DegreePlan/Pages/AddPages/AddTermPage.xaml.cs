using DegreePlan.Classes;
using SQLite;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DegreePlan
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddTerm : ContentPage
    {

        // ---------- Variables ----------
        public MainPage mainPage;
        public int insertResult;



        // ---------- Constructor ----------
        public AddTerm(MainPage mainPage)
        {
            this.mainPage = mainPage;
            InitializeComponent();
            xBtnCreate.IsVisible = false;
            xStartDate.MinimumDate = System.DateTime.Now;
            xEndDate.Date = xStartDate.Date.AddMonths(3).AddDays(-1);
            xEndDate.MinimumDate = xStartDate.Date.AddMonths(1).AddDays(-1);
            xEndDate.MaximumDate = xStartDate.Date.AddMonths(6).AddDays(-1);
        }



        // ---------- Android App Events ----------
        protected override void OnAppearing()
        {
            base.OnAppearing();
        }



        // ---------- Form Events ----------
        // CREATE button click
        private async void xBtnCreate_Clicked(object sender, EventArgs e)
        {
            if (doValidate())
            {
                Term newTerm = new Term
                {
                    Title = xName.Text,
                    StartDate = xStartDate.Date,
                    EndDate = xEndDate.Date
                };
                using (SQLiteConnection sql = new SQLiteConnection(App.FilePath))
                {
                    insertResult = sql.Insert(newTerm);
                    mainPage.termList.Add(newTerm);
                    await Navigation.PopAsync();
                }
            }
            else
            {
                await DisplayAlert("ERROR", "Please check all fields are entered properly", "OK");
            }
        }

        // TITLE text changed (used to make CREATE button visible)
        private void xName_Changed(object sender, FocusEventArgs e)
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
                xStartDate.Date != null && xEndDate.Date != null &&
                xEndDate.Date > xStartDate.Date)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Used to make the CREATE button visible
        private void doVisible()
        {
            if (xName.Text != null && xName.Text != "")
            {
                xBtnCreate.IsVisible = true;
            }
            else
            {
                xBtnCreate.IsVisible = false;
            }
            return;
        }
    }
}
