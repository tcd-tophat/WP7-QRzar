﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace QRzar
{
    public class User
    {
        public string name { get; set; }
        public string created;
        public string photo;
        public object joined_games;
        public string id { get; set; }
        public string email;

        public User(string email, string name = null, string created = null, string photo = null, object joined_games = null, string id = null)
        {
            this.name = name;
            this.email = email;
            this.created = created;
            this.photo = photo;
            this.joined_games = joined_games;
            this.id = id;
        }

        public bool Equals(User user)
        {
            return this.id == user.id;
        }
    }
}

