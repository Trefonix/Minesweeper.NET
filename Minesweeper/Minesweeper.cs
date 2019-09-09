using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Minesweeper
{
    public partial class Minesweeper : Form
    {
        public struct MinesweeperSquare
        {
            // The struct that will be linked to the array of buttons to give extra parameters
            public int numBombsTouching { get; set; }
            public bool clicked { get; set; }
            public bool hasMine { get; set; }
            public bool hasFlag { get; set; }
        }

        public int size_x = 8;
        public int size_y = 8;
        public int bombs;
        public int numCorrect = 0;
        public int clockTime = 0;
        public bool firstClick = true;

        public Minesweeper()
        {
            InitializeComponent();
        }
        //2 dimenstional array of buttons
        public Button[,] square;
        public MinesweeperSquare[,] squareItem;


        private void genMines(int startX, int startY)
        {
            //Place the mines onto the screen
            int bombsPlaced = 0;
            int randX = 0;
            int randY = 0;
            int chance = 0;
            Random rnd = new Random();
            clockTime = 0;

            while (bombsPlaced < bombs)
            {
                //Generate a 'random' number for both the X and Y co-ordinate.  
                randX = rnd.Next(0, size_x);
                randY = rnd.Next(0, size_y);
                if(Squares_Touching(randX,randY,size_x-1,size_y-1) == 0)
                {
                    // This part was added to give a 30% chance of regen when a bomb is placed in isolation
                    chance = rnd.Next(0, 100);
                    if(chance >= 30)
                    {
                        if (!squareItem[randX, randY].hasMine && !(randX == startX && randY == startY))
                        {
                            squareItem[randX, randY].hasMine = true;
                            bombsPlaced += 1;
                        }
                    }
                } 
                //Check if that square already has a mine on it
                else if (!squareItem[randX, randY].hasMine && !(randX == startX && randY == startY))
                {
                    squareItem[randX, randY].hasMine = true;
                    bombsPlaced += 1;
                }
            }

            for (int x = 0; x < size_x; x++)
            {
                for(int y = 0; y < size_y; y++)
                {
                    squareItem[x, y].numBombsTouching = Squares_Touching(x, y, size_x - 1, size_y - 1);
                }
            }
            // Work out how many bombs it is touching
            time.Enabled = true;
        }

        private void initGame(int sizeX, int sizeY, int numBombs)
        {
            int xOffset = 50;
            int yOffset = 70;
            clockTime = 0;
            // Generate the buttons at runtime. Buttons stored in a 2D array.
            square = new Button[sizeX + 1, sizeY + 1];

            // Generate a two dimensional array of strings to mirror the button array, set properies etc.
            squareItem = new MinesweeperSquare[sizeX, sizeY];
            
            // Load the buttons to the form.
            for (int column = 0; column < sizeY; column++)
            {
                xOffset = 50;
                for(int row = 0; row < sizeX; row++)
                {
                    // Give the buttons properties
                    square[row, column] = new Button();
                    square[row, column].Name = row.ToString() + "," + column.ToString();
                    square[row, column].Location = new Point(xOffset, yOffset);
                    square[row, column].Height = 40;
                    square[row, column].Width = 40;
                    square[row, column].BackColor = Color.Green;
                    square[row, column].ForeColor = Color.Black ;
                    square[row, column].Text = "";
                    
                    // Create event handlers
                    square[row, column].MouseDown += new MouseEventHandler(btn_mouseclick);

                    // Push the buttons to the form
                    Controls.Add(square[row, column]);

                    // Spacing between the buttons
                    xOffset += 44;
                }
                yOffset += 44;
            }
            this.Width = xOffset + 60;
            this.Height = yOffset + 60;
            lblTimer.Location = new Point(xOffset / 2, 30);
        }

        // Event for clicking one of the buttons
        private void btn_mouseclick(object sender, MouseEventArgs e)
        {
            var clickedSquare = (Button)sender;
            
            //Seperate out X and Y values from the name
            string[] coords = clickedSquare.Name.Split(',');
            int x = int.Parse(coords[0]);
            int y = int.Parse(coords[1]);
            if (firstClick)
            {
                genMines(x, y);
                firstClick = false;
            }
            if (e.Button == MouseButtons.Right && !squareItem[x, y].clicked)
            {
                if (squareItem[x, y].hasFlag)
                {
                    squareItem[x, y].hasFlag = false;
                    square[x, y].BackColor = Color.Green;
                    square[x, y].Text = "";
                    if (squareItem[x, y].hasMine)
                    {
                        numCorrect -= 1;
                    }
                }
                else
                {
                    squareItem[x, y].hasFlag = true;
                    square[x, y].BackColor = Color.OrangeRed;
                    square[x, y].Text = "X";
                    if (squareItem[x, y].hasMine)
                    {
                        numCorrect += 1;
                    }
                    if(numCorrect == bombs)
                    {
                        time.Enabled = false;
                        MessageBox.Show("You Win!");
                        
                    }
                }
            }
            else if (!squareItem[x, y].clicked && e.Button == MouseButtons.Left && !squareItem[x,y].hasFlag)
            {
                Mine_Sweep(x, y);
            }       
        }

        private void Mine_Sweep(int x, int y)
        {
            // Check how many bombs the square is touching / if they have just clicked on a mine.
            int bombsTouching = 0;

            if (squareItem[x, y].hasMine)
            {
                square[x,y].BackColor = Color.Red;
                time.Enabled = false;
                MessageBox.Show("Game Over!");
               
            }
            else
            {
                bombsTouching = squareItem[x, y].numBombsTouching;
                if (bombsTouching == 0)
                {
                    ClearEmpty(x, y);
                }
                square[x,y].Text = bombsTouching.ToString();
                squareItem[x, y].clicked = true;
                square[x,y].BackColor = Color.LightGreen;
            }
        }

        private void ClearEmpty(int x, int y)
        {
            // Define some variables to save lots of confusing numbers later on
            int right = x + 1;
            int left = x - 1;
            int up = y - 1;
            int down = y + 1;
            square[x, y].Text = "0";
            square[x, y].BackColor = Color.LightGreen;
            squareItem[x, y].clicked = true;


            // This algorithm will use recursion to clear out all the conencting '0' tiles
            if (up >= 0)
            {
                if (!squareItem[x,up].clicked)
                {
                    Mine_Sweep(x, up);
                }
                if (right <= size_x - 1)
                {
                    if (!squareItem[right, up].clicked )
                    {
                        Mine_Sweep(right, up);
                    }
                }
            }
            if (right <= size_x - 1)
            {
                if (!squareItem[right, y].clicked)
                {
                    Mine_Sweep(right, y);
                }
            }
            if (down <= size_y - 1)
            {
                if (right <= size_y - 1)
                {
                    if (!squareItem[right, down].clicked)
                    {
                        Mine_Sweep(right, down);
                    }
                }
                if (!squareItem[x, down].clicked)
                {
                    Mine_Sweep(x, down);
                }
            }
            if (left >= 0)
            {
                if (down <= size_y - 1)
                {
                    if (!squareItem[left, down].clicked)
                    {
                        Mine_Sweep(left, down);
                    }
                }
                if (!squareItem[left, y].clicked)
                {
                    Mine_Sweep(left, y);

                }
                if (up >= 0)
                {
                    if (!squareItem[left, up].clicked)
                    {
                        Mine_Sweep(left, up);
                    }
                }
            }
        }


        private void Minesweeper_Load(object sender, EventArgs e)
        {
            time.Enabled = false;
            bombs = 12;
            initGame(8, 8, 12);
           
        }

        private int Squares_Touching(int x, int y, int sizeX, int sizeY)
        {
            // Define some variables to save lots of confusing numbers later on
            int right = x + 1;
            int left = x - 1;
            int up = y - 1;
            int down = y + 1;
            int squaresTouching = 0;

            /* We need to check that by doing these calculations we are not going to go outside
             * the bounds of the array. */
            
            if(up >= 0)
            {
                if (squareItem[x,up].hasMine)
                {
                    squaresTouching += 1;
                }
                if(right <= sizeX)
                {
                    if (squareItem[right, up].hasMine)
                    {
                        squaresTouching += 1;
                    }
                }
            }
            if (right <= sizeX)
            {
                if (squareItem[right, y].hasMine)
                {
                    squaresTouching += 1;
                }
            }
            if(down <= sizeY)
            {
                if(right <= sizeX)
                {
                    if (squareItem[right, down].hasMine)
                    {
                        squaresTouching += 1;
                    }
                }
                if (squareItem[x, down].hasMine)
                {
                    squaresTouching += 1;
                }
            }
            if(left >= 0)
            {
                if(down <= sizeY)
                {
                    if (squareItem[left, down].hasMine)
                    {
                        squaresTouching += 1;
                    }
                }
                if (squareItem[left, y].hasMine)
                {
                    squaresTouching += 1;
                    
                }
                if(up >= 0)
                {
                    if (squareItem[left, up].hasMine)
                    {
                        squaresTouching += 1;
                    }
                }
            }
            return squaresTouching;


        }

        private void smallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearBoard();
            size_x = 8;
            size_y = 8;
            bombs = 12;
            numCorrect = 0;
            initGame(size_x, size_y, bombs);
        }

        private void ClearBoard()
        {
            firstClick = true;
            time.Enabled = false;
            // This subroutine will remove all the old buttons ready for the new ones.
            for(int i = 0; i < size_x; i++)
            {
                for(int c = 0; c< size_y; c++)
                {
                    Controls.Remove(square[i, c]);
                }
            }
        }

        private void mediumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearBoard();
            size_x = 12;
            size_y = 12;
            bombs = 32;
            numCorrect = 0;
            initGame(size_x, size_y, bombs);
        }

        private void largeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearBoard();
            size_x = 24;
            size_y = 14;
            bombs = 80;
            numCorrect = 0;
            initGame(size_x, size_y, bombs);
        }

        private void time_Tick(object sender, EventArgs e)
        {
            clockTime++;
            lblTimer.Text = clockTime.ToString();
        }
    }
}
