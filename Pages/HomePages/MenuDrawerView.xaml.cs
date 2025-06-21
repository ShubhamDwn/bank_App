using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace bank_demo.Pages.HomePages
{
    public partial class MenuDrawerView : ContentView
    {
        public MenuDrawerView()
        {
            InitializeComponent();
        }

        

        public static readonly BindableProperty ProfileCommandProperty =
            BindableProperty.Create(nameof(ProfileCommand), typeof(ICommand), typeof(MenuDrawerView));

        public ICommand ProfileCommand
        {
            get => (ICommand)GetValue(ProfileCommandProperty);
            set => SetValue(ProfileCommandProperty, value);
        }

        public static readonly BindableProperty TransactionHistoryCommandProperty =
            BindableProperty.Create(nameof(TransactionHistoryCommand), typeof(ICommand), typeof(MenuDrawerView));

        public ICommand TransactionHistoryCommand
        {
            get => (ICommand)GetValue(TransactionHistoryCommandProperty);
            set => SetValue(TransactionHistoryCommandProperty, value);
        }

        public static readonly BindableProperty BeneficiaryStatusCommandProperty =
            BindableProperty.Create(nameof(BeneficiaryStatusCommand), typeof(ICommand), typeof(MenuDrawerView));

        public ICommand BeneficiaryStatusCommand
        {
            get => (ICommand)GetValue(BeneficiaryStatusCommandProperty);
            set => SetValue(BeneficiaryStatusCommandProperty, value);
        }

        public static readonly BindableProperty ContactSupportCommandProperty =
            BindableProperty.Create(nameof(ContactSupportCommand), typeof(ICommand), typeof(MenuDrawerView));

        public ICommand ContactSupportCommand
        {
            get => (ICommand)GetValue(ContactSupportCommandProperty);
            set => SetValue(ContactSupportCommandProperty, value);
        }

        public static readonly BindableProperty SecuritySettingsCommandProperty =
            BindableProperty.Create(nameof(SecuritySettingsCommand), typeof(ICommand), typeof(MenuDrawerView));

        public ICommand SecuritySettingsCommand
        {
            get => (ICommand)GetValue(SecuritySettingsCommandProperty);
            set => SetValue(SecuritySettingsCommandProperty, value);
        }

        public static readonly BindableProperty TermsCommandProperty =
            BindableProperty.Create(nameof(TermsCommand), typeof(ICommand), typeof(MenuDrawerView));

        public ICommand TermsCommand
        {
            get => (ICommand)GetValue(TermsCommandProperty);
            set => SetValue(TermsCommandProperty, value);
        }

        public static readonly BindableProperty LogoutCommandProperty =
            BindableProperty.Create(nameof(LogoutCommand), typeof(ICommand), typeof(MenuDrawerView));

        public ICommand LogoutCommand
        {
            get => (ICommand)GetValue(LogoutCommandProperty);
            set => SetValue(LogoutCommandProperty, value);
        }
    }
}
