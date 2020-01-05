using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Memory_Matching_Game
{
    public partial class mainWindow : Form
    {
        //Variables
        int attempts; //Player attempts
        int gameDuration; //Counts the duration of the game in seconds, after the cards are hidden.
        static int dataset = 1; //The dataset will be selected through the radiobuttons. The default is the Mario dataset.
        
        PictureBox firstCardOpen; //The first card the player has selected
        PictureBox secondCardOpen; //The second card the player has selected
        Random rndIndex = new Random();
        List<PictureBox> unPairedPictureBoxes = new List<PictureBox>(); //A List that contains all the PictureBoxes that are not paired. 

        string audio = "off";

        public mainWindow()
        {
            InitializeComponent();
        }

        private PictureBox[] NumOfPictureBoxes  
        {
            get { return Controls.OfType<PictureBox>().ToArray(); }
        }


        #region IEnumerable<Image>        
        private static IEnumerable<Image> DatasetImages 
        {
            get
            {
                if (dataset == 1) //If the Mario radiobutton is selected
                {
                    return new Image[]
                    {
                                Properties.Resources.Mario0,
                                Properties.Resources.Mario1,
                                Properties.Resources.Mario2,
                                Properties.Resources.Mario3,
                                Properties.Resources.Mario4,
                                Properties.Resources.Mario5,
                                Properties.Resources.Mario6,
                                Properties.Resources.Mario7,
                                Properties.Resources.Mario8,
                                Properties.Resources.Mario9
                    };
                }
                else if (dataset == 2) //If the Crash radiobutton is selected
                {
                    return new Image[]
                    {
                                Properties.Resources.Crash0,
                                Properties.Resources.Crash1,
                                Properties.Resources.Crash2,
                                Properties.Resources.Crash3,
                                Properties.Resources.Crash4,
                                Properties.Resources.Crash5,
                                Properties.Resources.Crash6,
                                Properties.Resources.Crash7,
                                Properties.Resources.Crash8,
                                Properties.Resources.Crash9
                    };
                }

                else
                {
                    return new Image[] //If the Pokemon radiobutton is selected
                    {
                                Properties.Resources.Pokemon0,
                                Properties.Resources.Pokemon1,
                                Properties.Resources.Pokemon2,
                                Properties.Resources.Pokemon3,
                                Properties.Resources.Pokemon4,
                                Properties.Resources.Pokemon5,
                                Properties.Resources.Pokemon6,
                                Properties.Resources.Pokemon7,
                                Properties.Resources.Pokemon8,
                                Properties.Resources.Pokemon9
                    };
                }
            }
        }        
        #endregion

        #region RadioButtons
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            dataset = 1;
        }

        private void crashButton_CheckedChanged(object sender, EventArgs e)
        {
            dataset = 2;
        }

        private void pokemonButton_CheckedChanged(object sender, EventArgs e)
        {
            dataset = 3;
        }
        #endregion

        private PictureBox FindUntagged()  //Returns a PictureBox that has not Image.Tag.
        {
            int num;
            do
            {
                num = rndIndex.Next(0, NumOfPictureBoxes.Count()); 
            }
            while (NumOfPictureBoxes[num].Tag != null);
            return NumOfPictureBoxes[num];
        }

        private void ShuffleCards()  //Imports each dataset image on two randomly selected PictureBoxes.
            {
                foreach (var image in DatasetImages)
                {
                    FindUntagged().Tag = image;  //Import each image of the dataset twice, in order to create pairs.
                    FindUntagged().Tag = image;  //Calls the FindUnTagged() again and imports the same image to a another PictureBox.
                }
                DisplayCards();
            }

        private void DisplayCards()
        {
            foreach (var pic in NumOfPictureBoxes)
            {
                pic.Image = (Image)pic.Tag;
                pic.Enabled = false;
                unPairedPictureBoxes.Add(pic);  
            }
        }

        private void ResetCards() //Remove (if exist) all the image tags.
        {
            foreach (var pic in NumOfPictureBoxes)
            {
                pic.Tag = null; 
                pic.Visible = true;
            }
        }

        private void reverseTimer_Tick(object sender, EventArgs e)
        {            
            int labelTimer = Convert.ToInt32(reverseTimeLabel.Text) - 1;
            reverseTimeLabel.Text = labelTimer.ToString();             

            if (labelTimer == 0)  //Time to hide the cards and start the game.
            {
                reverseTimeLabel.Text = "Go!";
                HideCards();
                gameTimer.Start();
                reverseTimer.Stop();
            }
        }
               
        private void HideCards()
        {
            foreach (var pic in NumOfPictureBoxes)
            {
                pic.Image = Properties.Resources.Cover;
                pic.Enabled = true;
                pic.Cursor = Cursors.Hand;
            }
        }

        private void StartButton(object sender, EventArgs e)
        {
            ResetCards();
            ShuffleCards();
            attempts = 0;
            gameDuration = 0;
            reverseTimeLabel.Visible = true;
            reverseTimeLabel.Text = "5";
            reverseTimer.Start();
            startButton.Enabled = false;  //have in a loop?
            marioButton.Enabled = false;
            crashButton.Enabled = false;
            pokemonButton.Enabled = false;
            audio = "on";
            gameAudio();
        }

        private void gameAudio()
        {
            if (dataset == 1)
            {
                System.Media.SoundPlayer simpleSound = new System.Media.SoundPlayer(Properties.Resources.MarioTheme);
                if (audio == "on")
                {
                    simpleSound.Play();
                }
                else
                {
                    simpleSound.Stop();
                }
                
            }
            else if (dataset == 2)
            {
                System.Media.SoundPlayer simpleSound = new System.Media.SoundPlayer(Properties.Resources.CrashTheme);
                if (audio == "on")
                {
                    simpleSound.Play();
                }
                else
                {
                    simpleSound.Stop();
                }

            }
            else
            {
                System.Media.SoundPlayer simpleSound = new System.Media.SoundPlayer(Properties.Resources.PokemonTheme);
                if (audio == "on")
                {
                    simpleSound.Play();
                }
                else
                {
                    simpleSound.Stop();
                }
            }
            
        }

        private void ClickOnImage(object sender, EventArgs e)
        {
            var pic = (PictureBox)sender;
            
            pic.Image = (Image)pic.Tag;
            if (firstCardOpen == null)
            {
                firstCardOpen = pic;
            }
            else
            {
                attempts = attempts + 1;
                secondCardOpen = pic;

                if ((pic.Tag == firstCardOpen.Tag) && (pic != firstCardOpen)) // && pic != firstCardOpen
                {
                    unPairedPictureBoxes.Remove(firstCardOpen); //Remove from the unpaired List.
                    unPairedPictureBoxes.Remove(secondCardOpen);

                    if (unPairedPictureBoxes.Count == 0) //All pairs are found
                    {
                        Winning();
                    }

                    firstCardOpen.Enabled = false; //Will not be able to click on these two paired PictureBoxes anymore in this game.
                    secondCardOpen.Enabled = false;
                    firstCardOpen = null;
                    secondCardOpen = null;
                }
                else
                { 
                    foreach (var pic2 in unPairedPictureBoxes)
                    {
                        pic2.Enabled = false; //Prevent to click on any other cards while the two cards are open.
                    }
                    waitTimer.Start(); //Wait 0.750 seconds till you hide the two unpaired cards.
                }
            }
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            gameDuration = gameDuration + 1;
            reverseTimeLabel.Hide();
        }

        private void waitTimer_Tick(object sender, EventArgs e)
        {
            waitTimer.Stop();
            firstCardOpen.Image = Properties.Resources.Cover; //Hide the two unpaired cards.
            secondCardOpen.Image = Properties.Resources.Cover;
            foreach (var pic2 in unPairedPictureBoxes)
            {
                pic2.Enabled = true; //Allow to click on hidden cards again.
            }
            firstCardOpen = null;
            secondCardOpen = null;
        }

        private void Winning()
        {
            gameTimer.Stop();
            MessageBox.Show("You won!" + Environment.NewLine + Environment.NewLine + "Attempts: " + attempts.ToString() + Environment.NewLine + "Game duration: " + gameDuration.ToString() + "s");
            startButton.Enabled = true;
            marioButton.Enabled = true;
            crashButton.Enabled = true;
            pokemonButton.Enabled = true;
            audio = "off";
            gameAudio();
        }

        private void reverseTimeLabel_Click(object sender, EventArgs e)
        {

        }
    }
}
