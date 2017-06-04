using Contacts.Models;
using Contacts.Services;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Contacts.ViewModels
{
    public class ContactsViewModel : INotifyPropertyChanged
    {
        #region Attributes
        private ApiService apiService;
        private DialogService dialogService;
        private bool refreshing;
        
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #endregion

        #region Properties
        public ObservableCollection<ContactItemViewModel> MyContacts { get; set; }

        public bool Refreshing
        {
            set
            {
                if (refreshing != value)
                {
                    refreshing = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Refreshing"));
                }
            }
            get
            {
                return refreshing;
            }

        }
        #endregion

        #region Constructors
        public ContactsViewModel()
        {
            apiService = new ApiService();
            dialogService = new DialogService();
            MyContacts = new ObservableCollection<ContactItemViewModel>();
            Refreshing = false;
            LoadContacts();
        }
        #endregion

        #region Methods
        private async void LoadContacts()
        {
            Refreshing = true;
            var response = await apiService.Get<Contact>("http://contactsxamarintata.azurewebsites.net/", "/api", "/Contacts");

            if (!response.IsSuccess)
            {
                await dialogService.ShowMessage("Error", response.Message);
                return;
            }

            ReloadContacts((List<Contact>)response.Result);
            Refreshing = false;
        }
        private void ReloadContacts(List<Contact> contacts)
        {
            MyContacts.Clear();
            foreach (var contact in contacts.OrderBy(c => c.FirstName).ThenBy(c => c.LastName))
            {
                MyContacts.Add(new ContactItemViewModel
                {
                    ContactId = contact.ContactId,
                    EmailAddress = contact.EmailAddress,
                    FirstName = contact.FirstName,
                    Image = contact.Image,
                    LastName = contact.LastName,
                    PhoneNumber = contact.PhoneNumber,
                });
            }
        }
        #endregion

        #region Commands
        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(Refresh);
            }
        }

        private void Refresh()
        {
            LoadContacts();
        }

        #endregion
    }
}
