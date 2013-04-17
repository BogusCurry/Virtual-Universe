﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Aurora.Framework;
using OpenSim.Services.Interfaces;

namespace Aurora.Management
{
    public partial class UserChooser : Form
    {
        private IRegionManagement _regionManagement;
        public string UserName { get; private set; }

        public UserChooser(string userName, IRegionManagement regionManagement)
        {
            _regionManagement = regionManagement;
            InitializeComponent();
        }

        private void search_Click(object sender, EventArgs e)
        {
            List<UserAccount> accounts = _regionManagement.GetUserAccounts(user_name.Text);
            if (accounts == null)
                return;

            listBox.Items.Clear();
            foreach (UserAccount acc in accounts)
                listBox.Items.Add(acc.Name);
        }

        private void select_user_Click(object sender, EventArgs e)
        {
            if (listBox.SelectedItem == null)
                return;
            UserName = listBox.SelectedItem.ToString();
            Close();
        }

        private void create_user_Click(object sender, EventArgs e)
        {
            CreateUser userCreate = new CreateUser(_regionManagement);
            userCreate.ShowDialog();
            user_name.Text = userCreate.UserName;
        }
    }
}
