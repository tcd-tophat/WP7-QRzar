using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;


namespace QRzar
{
    public partial class PhonePage1 : PhoneApplicationPage
    {
        public PhonePage1()
        {
            InitializeComponent();
        }

        public void Logout(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
		
		string scannedCode = "";
		bool isValidGame;
		protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e) {
	 		base.OnNavigatedTo(e);
         	IDictionary<string, string> parameters = this.NavigationContext.QueryString;
         	if (parameters.ContainsKey("Code"))
            {
             	PlayerCode.Text = parameters["Code"];
				scannedCode = parameters["Code"];
                playerInformation(scannedCode);
         	}
		}

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
			if (isValidGame == true)
			{
				NavigationService.Navigate(new Uri("/PanoramaPage1.xaml", UriKind.Relative));
			}
			else 
			{
				NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
			}
        }
        
        private void playerInformation(string playerCode)
        {
            QRparser QRpars = new QRparser(scannedCode);
			bool validGame = QRpars.isValid();
			if (validGame == true)
			{
				//displays player ID
				int playerNumberInt = QRpars.getPlayerId();
				playerNumberText.Text = playerNumberInt.ToString();
				//displays game ID
				int gameNumberInt = QRpars.getGameId();
				gameNumberText.Text = gameNumberInt.ToString();
				//displays team colour
				teamColorText.Text = QRpars.getTeam();
				isValidGame = true;
				nextScreen.Content= "Go to Game Screen";
				
			}
			
			else
			{
				playerNumberText.Text = "invalid game";
				gameNumberText.Text = "invalid game";
				teamColorText.Text = "invalid game";
				isValidGame = false;
				nextScreen.Content= "Go Back to Scanning Screen";
				
			}


        }

		
		
		
		public class QRparser
		{
    		private string data = "";
    
    		/**
			* So that the team only needs to be extracted once in order to be used, it is saved in this variable after one
			*/
    		private string team;
    
    		/**
			* The game Id is saved in here after one in order to ensure that it does not have to be recalculated.
			*/
    		private int gameId;
    
    		/**
			* This playerId
			*/
   			private int playerId;
    
    		private bool valid = false;

    		public QRparser(String QRinput)
    		{
     			//Clone the string
       			data = QRinput;
        
        		if ( data.Length != 6)
        		{
         			this.valid = false;
        		}
				
				else if (data[0] != '\x47' && data[0] != '\x42' && data[0] != '\x52' && data[0] != '\x59' )
				{
					this.valid = false;
				}
				
        		else
        		{
        		this.valid = true;
        		}
    		}

			public String getTeam()
			{
				if( team == null )
			
				{
					if(data[0] == 'R')
					{
						team = "Red";
					}
					else if(data[0] == 'B')
					{
						team = "Blue";
					}
					else if(data[0] == 'G')
					{
						team = "Green";
					}
					else if(data[0] == 'Y')
					{
						team = "Yellow";
					}
										
				}
        
			return team;
    		}

			public int getGameId()
			{
				

				gameId = 777;
				gameId += (int)data[1];	
				gameId = gameId << 256;
				gameId += (int)data[2];
				gameId = gameId << 256;
				gameId += (int)data[3];
				
			
				return gameId;
			}
	
			public int getPlayerId()
			{
				int res = 0;
				res += (int)data[4];
				res = res << 256;
				res += (int)data[5];
				return res;
			}
		
			public bool isValid()
			{
				return this.valid;
			}
		}
	
		
		
	}
    	
}
