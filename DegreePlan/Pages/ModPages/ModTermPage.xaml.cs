using DegreePlan.Classes;
using SQLite;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DegreePlan
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ModTerm : ContentPage
    {

        // ---------- Variables ----------
        public MainPage mainPage;
        public int updateResult, deleteResult;
        public Term term;



        // ---------- Constructor ----------
        public ModTerm(MainPage mainPage, Term term)
        {
            this.mainPage = mainPage;
            this.term = term;
            InitializeComponent();
            xName.Text = term.Title;
            xStartDate.Date = term.StartDate;
            xEndDate.Date = term.EndDate;
            xBtnSave.IsVisible = false;
        }



        // ---------- Android App Events ----------
        protected override void OnAppearing()
        {
            xName.Text = term.Title;
            xStartDate.Date = term.StartDate;
            xEndDate.Date = term.EndDate;
            base.OnAppearing();
        }



        // ---------- Form Events ----------
        // SAVE button click
        private async void xBtnSave_Clicked(object sender, EventArgs e)
        {
            if (doValidate())
            {
                term.Title = xName.Text;
                term.StartDate = xStartDate.Date;
                term.EndDate = xEndDate.Date;
                using (SQLiteConnection sql = new SQLiteConnection(App.FilePath))
                {
                    updateResult = sql.Update(term);
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
            if (await DisplayAlert("CONFIRM", "Are you sure you want to delete this term?", "YES", "CANCEL"))
            {
                using (SQLiteConnection sql = new SQLiteConnection(App.FilePath))
                {
                    deleteResult = sql.Delete(term);
                    Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);
                    await Navigation.PopAsync();
                }
            }
        }

        // TITLE text changed (used to make SAVE button visible)
        private void xName_Changed(object sender, FocusEventArgs e)
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



        // ---------- Functions ----------
        private void doEditCheck()
        {
            if (xName.Text != null && xName.Text != "")
            {
                if (xName.Text != term.Title || xStartDate.Date != term.StartDate || xEndDate.Date != term.EndDate)
                {
                    xBtnSave.IsVisible = true;
                } else
                {
                    xBtnSave.IsVisible = false;
                }
            } else
            {
                xBtnSave.IsVisible = false;
            }
        }

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
    }
}
