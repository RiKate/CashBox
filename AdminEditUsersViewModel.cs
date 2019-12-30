using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace MVVMCashbox
{
    class AdminEditUsersViewModel
    {
        private ObservableCollection<User> users;
        private User selectedUser;

        private Command addUserCommand;
        private Command deleteUserCommand;

        public AdminEditUsersViewModel()
        {
            Users = new ObservableCollection<User>();
        }

        public ObservableCollection<User> Users
        {
            get { return users; }
            set
            {
                users = value;
                OnPropertyChanged("Users");
            }
        }

        public User SelectedUser
        {
            get { return selectedUser; }
            set
            {
                selectedUser = value;
                OnPropertyChanged("SelectedUser");
            }
        }


        public Command AddUserCommand
        {
            get
            {
                return addUserCommand ?? (addUserCommand = new Command((obj) =>
                 {
                     User user = new User();
                     Users.Insert(0, user);
                     SelectedUser = user;
                 }));
            }
        }

        public Command DeleteUserCommand => deleteUserCommand ?? (deleteUserCommand = new Command((obj) =>
                                                           {
                                                               if (obj is User user)
                                                               {
                                                                   Users.Remove(user);
                                                               }
                                                           }, (obj) =>
                                                           {
                                                               if (obj is User user)
                                                               {
                                                                   return Users.Count > 0 && !user.IsAdmin;
                                                               }
                                                               else
                                                               {
                                                                   return false;
                                                               }
                                                           }));

        public void CheckUsers()
        {
            ObservableCollection<User> save = new ObservableCollection<User>();
            foreach (var user in users)
            {
                if (user.Name != null && user.Password != null) save.Add(user);
            }
            users = save;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
