using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using yari.Models;

namespace yari.ViewModels
{
    public class UsuariosViewModel : NotificationEnabledObject
    {
        private ObservableCollection<Usuario> myUsersList;

        public ObservableCollection<Usuario> MyUsersList
        {
            get
            {
                if (myUsersList == null)
                    myUsersList = new ObservableCollection<Usuario>();
                return myUsersList;
            }
            set
            {
                myUsersList = value;
                OnPropertyChanged();
            }
        }

        private bool userLogin;

        public bool UserLogin
        {
            get
            {
                if (userLogin == null)
                    userLogin = new bool();
                return userLogin;
            }
            set
            {
                userLogin = value;
                OnPropertyChanged();
            }
        }


        ServiceModel serviceModel = new ServiceModel();

        public UsuariosViewModel()
        {
            //serviceModel.GetUsuariosCompleted += (s, a) =>
            //    {
            //        MyUsersList = new ObservableCollection<Usuario>(a.Results);
            //    };
            serviceModel.GetLoginCompleted += (s, a) =>
                {
                    UserLogin = a.Results;
                };
        }

        private ActionCommand getUsersCommand;

        public ActionCommand GetUsersCommand
        {
            get
            {
                if (getUsersCommand == null)
                {
                    getUsersCommand = new ActionCommand(() =>
                    {
                        serviceModel.GetUsers();
                    });
                }
                return getUsersCommand;
            }
        }

        private ActionCommand getLoginCommand;

        public ActionCommand GetLoginCommand
        {
            get
            {
                if (getLoginCommand == null)
                {
                    getLoginCommand = new ActionCommand(() =>
                    {
                        serviceModel.GetLogin(MyUsersList);
                    });
                }
                return getLoginCommand;
            }
        }


    }
}
