/*
 * Description:     A basic PONG simulator
 * Author:          Calem McTavish         
 * Date:            Sept. 22, 2020
 */

#region libraries

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Media;

#endregion

namespace Pong
{
    public partial class Form1 : Form
    {
        #region global values

        //graphics objects for drawing
        SolidBrush blueBrush = new SolidBrush(Color.Blue);
        SolidBrush redBrush = new SolidBrush(Color.Red);
        SolidBrush whiteBrush = new SolidBrush(Color.White);
        Pen redPen = new Pen(Color.Red, 10);
        Pen bluePen = new Pen(Color.Blue, 10);
        Pen blackPen = new Pen(Color.Black, 10);
        Font drawFont = new Font("Courier New", 10);

        // Sounds for game
        SoundPlayer scoreSound = new SoundPlayer(Properties.Resources.score);
        SoundPlayer collisionSound = new SoundPlayer(Properties.Resources.collision);

        //determines whether a key is being pressed or not
        Boolean aKeyDown, zKeyDown, jKeyDown, mKeyDown;

        // check to see if a new game can be started
        Boolean newGameOk = true;

        //ball directions, speed, and rectangle
        Boolean ballMoveRight = true;
        Boolean ballMoveDown = true;
        int BALL_SPEED = 4;
        Rectangle ball;

        //paddle speeds and rectangles
        int PADDLE_SPEED = 4;
        Rectangle p1, p2;
        int speedCounter = 0;

        //player and game scores
        float player1Score = 0;
        float player2Score = 0;
        float gameWinScore = 5;  // number of points needed to win game

        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        // -- YOU DO NOT NEED TO MAKE CHANGES TO THIS METHOD
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //check to see if a key is pressed and set is KeyDown value to true if it has
            switch (e.KeyCode)
            {
                case Keys.A:
                    aKeyDown = true;
                    break;
                case Keys.Z:
                    zKeyDown = true;
                    break;
                case Keys.J:
                    jKeyDown = true;
                    break;
                case Keys.M:
                    mKeyDown = true;
                    break;
                case Keys.Y:
                case Keys.Space:
                    if (newGameOk)
                    {
                        SetParameters();
                    }
                    break;
                case Keys.N:
                    if (newGameOk)
                    {
                        Close();
                    }
                    break;
            }
        }

        // -- YOU DO NOT NEED TO MAKE CHANGES TO THIS METHOD
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            //check to see if a key has been released and set its KeyDown value to false if it has
            switch (e.KeyCode)
            {
                case Keys.A:
                    aKeyDown = false;
                    break;
                case Keys.Z:
                    zKeyDown = false;
                    break;
                case Keys.J:
                    jKeyDown = false;
                    break;
                case Keys.M:
                    mKeyDown = false;
                    break;
            }
        }

        /// <summary>
        /// sets the ball and paddle positions for game start
        /// </summary>
        private void SetParameters()
        {
            if (newGameOk)
            {
                Thread.Sleep(500);
                player1Score = player2Score = 0;
                // resetting labels
                p1ScoreLabel.Text = "PLAYER 1: 0";
                p2ScoreLabel.Text = "PLAYER 2: 0";
                p1ScoreLabel.Visible = true;
                p2ScoreLabel.Visible = true;
                nLabel.Visible = false;
                yLabel.Visible = false;

                newGameOk = false;
                startLabel.Visible = false;
                gameUpdateLoop.Start();


            }
            #region paddle dimensions
            //set starting position for paddles on new game and point scored 
            const int PADDLE_EDGE = 20;  // buffer distance between screen edge and paddle            

            p1.Width = p2.Width = 10;    //width for both paddles set the same
            p1.Height = p2.Height = 65;  //height for both paddles set the same

            //p1 starting position
            p1.X = PADDLE_EDGE;
            p1.Y = this.Height / 2 - p1.Height / 2;

            //p2 starting position
            p2.X = this.Width - PADDLE_EDGE - p2.Width;
            p2.Y = this.Height / 2 - p2.Height / 2;


            // TODO set Width and Height of ball
            ball.Width = 15;
            ball.Height = 15;
            // TODO set starting X position for ball to middle of screen, (use this.Width and ball.Width)
            ball.X = this.Width / 2 - ball.Width;
            // TODO set starting Y position for ball to middle of screen, (use this.Height and ball.Height)
            ball.Y = this.Height / 2 - ball.Height;

            #endregion
        }


        /// <summary>
        /// This method is the game engine loop that updates the position of all elements
        /// and checks for collisions.
        /// </summary>
        private void gameUpdateLoop_Tick(object sender, EventArgs e)
        {
            #region update ball position

            // TODO create code to move ball either left or right based on ballMoveRight and using BALL_SPEED
            if (ballMoveRight == true)
            {
                ball.X = ball.X + BALL_SPEED;
            }
            else
            {
                ball.X = ball.X - BALL_SPEED;
            }

            // TODO create code move ball either down or up based on ballMoveDown and using BALL_SPEED
            if (ballMoveDown == true)
            {
                ball.Y = ball.Y + BALL_SPEED;
            }
            else
            {
                ball.Y = ball.Y - BALL_SPEED;
            }

            #endregion

            #region update paddle positions

            if (aKeyDown == true && p1.Y >= 0)
            {
                // TODO create code to move player 1 paddle up using p1.Y and PADDLE_SPEED
                p1.Y = p1.Y - PADDLE_SPEED;
            }

            // TODO create an if statement and code to move player 1 paddle down using p1.Y and PADDLE_SPEED
            if (zKeyDown == true && p1.Y <= this.Height - p1.Height)
            {
                p1.Y = p1.Y + PADDLE_SPEED;
            }
            // TODO create an if statement and code to move player 2 paddle up using p2.Y and PADDLE_SPEED
            if (jKeyDown == true && p2.Y >= 0)
            {
                // TODO create code to move player 1 paddle up using p1.Y and PADDLE_SPEED
                p2.Y = p2.Y - PADDLE_SPEED;
            }
            // TODO create an if statement and code to move player 2 paddle down using p2.Y and PADDLE_SPEED
            if (mKeyDown == true && p2.Y <= this.Height - p2.Height)
            {
                p2.Y = p2.Y + PADDLE_SPEED;
            }
            #endregion

            #region ball collision with top and bottom lines

            if (ball.Y < 0) // if ball hits top line
            {
                ballMoveDown = true;
                // TODO use ballMoveDown boolean to change direction
                // TODO play a collision sound
                collisionSound.Play();


            }

            if (ball.Y > this.Height - ball.Height)
            {
                ballMoveDown = false;
                collisionSound.Play();
            }
            // TODO In an else if statement use ball.Y, this.Height, and ball.Width to check for collision with bottom line
            // If true use ballMoveDown down boolean to change direction

            #endregion

            #region ball collision with paddles

            // TODO create if statment that checks p1 collides with ball and if it does
            // --- play a "paddle hit" sound and
            // --- use ballMoveRight boolean to change direction
            if (ball.IntersectsWith(p1) || ball.IntersectsWith(p2))
            {
                ballMoveRight = !ballMoveRight;
                collisionSound.Play();
            }

            #endregion

            #region ball collision with side walls (point scored)

            if (ball.X < 0)  // ball hits left wall logic
            {
                // TODO
                // --- play SCORE SOUND
                // --- update player 2 score
                player2Score = player2Score + 1;
                p2ScoreLabel.Text = "PLAYER 2: " + player2Score;
                scoreSound.Play();

                //resetting ball speed when point scored
                speedCounter = 0;

                // TODO use if statement to check to see if player 2 has won the game. If true run 
                // GameOver method. Else change direction of ball and call SetParameters method.
                if (player2Score == gameWinScore)
                {
                    GameOver("player 2");
                }
                else
                {
                    SetParameters();
                }
            }

            if (ball.X > this.Width)  // ball hits right wall logic
            {
                // TODO
                // --- play score sound
                // --- update player 1 score
                player1Score = player1Score + 1;
                p1ScoreLabel.Text = "PLAYER 1: " + player1Score;
                scoreSound.Play();

                // TODO use if statement to check to see if player 2 has won the game. If true run 
                // GameOver method. Else change direction of ball and call SetParameters method.
                if (player1Score == gameWinScore)
                {
                    GameOver("player 1");
                }
                else
                {
                    SetParameters();
                }

            }

            // TODO same as above but this time check for collision with the right wall

            #endregion

            //refresh the screen, which causes the Form1_Paint method to run
            this.Refresh();
        }

        /// <summary>
        /// Displays a message for the winner when the game is over and allows the user to either select
        /// to play again or end the program
        /// </summary>
        /// <param name="winner">The player name to be shown as the winner</param>
        private void GameOver(string winner)
        {

            newGameOk = true;

            // TODO create game over logic

            // --- stop the gameUpdateLoop
            gameUpdateLoop.Stop();

            // --- show a message on the startLabel to indicate a winner, (need to Refresh).
            Refresh();
            startLabel.Visible = true;
            startLabel.Text = $"{winner} wins";
            startLabel.Refresh();

            // --- pause for two seconds 
            Thread.Sleep(2000);

            // --- use the startLabel to ask the user if they want to play again
            startLabel.Text = "Would you like to destroy your opponent once more?";
            yLabel.Visible = true;
            nLabel.Visible = true;

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            // TODO draw paddles using FillRectangle
            e.Graphics.FillRectangle(redBrush, p1);
            e.Graphics.FillRectangle(blueBrush, p2);

            //make course look interesting
            e.Graphics.DrawLine(redPen, this.Width / 2 - 5, 0, this.Width / 2 - 5, this.Height);
            e.Graphics.DrawLine(bluePen, this.Width / 2 + 5, 0, this.Width / 2 + 5, this.Height);

            e.Graphics.DrawLine(redPen, 0, 3, this.Width / 2, 3);
            e.Graphics.DrawLine(redPen, 0, this.Height - 5, this.Width / 2, this.Height - 5);

            e.Graphics.DrawLine(bluePen, this.Width / 2, 5, this.Width, 5);
            e.Graphics.DrawLine(bluePen, this.Width / 2, this.Height - 5, this.Width, this.Height - 5);

            e.Graphics.DrawEllipse(blackPen, this.Width / 2 - 25, this.Height / 2 - 25, 50, 50);

            //changes ball colour depending on which side its on
            if (ball.X < this.Width / 2 - ball.Width)
            {
                e.Graphics.FillEllipse(blueBrush, ball);
            }
            else
            {
                e.Graphics.FillEllipse(redBrush, ball);
            }

        }

    }
}
