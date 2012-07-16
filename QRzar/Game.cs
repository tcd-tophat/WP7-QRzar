using System;
using System.Net;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Text;
using Newtonsoft.Json;

namespace QRzar
{
    public class Game
    {
        public int ID { get; private set; }

        public User[] players { get; private set; }


        public Game(int ID, User[] players)
        {
            this.ID = ID;
            this.players = players;
        }



    }

}