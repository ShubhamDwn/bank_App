using System;
using Microsoft.Maui.Controls;
using bank_demo.Services;
using bank_demo.Services.API;
using bank_demo.ViewModels.FeaturesPages.FundTransfer;

namespace bank_demo.Pages.Fund_Transfer;

    public partial class TransactionListPage : ContentPage
    {

        public TransactionListPage()
        {
            InitializeComponent();
            BindingContext = new TransactionListViewModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var vm = BindingContext as TransactionListViewModel;

        }
    }
