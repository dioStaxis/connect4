using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace Connect4Game
{
    /// <summary>
    /// A game of Connect 4
    /// </summary>
    public partial class C4GW : Window
    {
        //Player 1 is true, player 2 is false
        public bool Player { get; set; }
        private readonly ObservableCollection<ObservableCollection<int>> backBoard = new();
        private readonly List<List<int>> frontBoard = new();

        public void ChangePlayer()
        {
            Player = !Player;
            UpdateContext();
        }

        public string PlayerAsString()
        {
            if (Player)
                return "1";
            else
                return "2";
        }

        public void UpdateContext()
        {
            DataContext = Player;
        }

        public void SetBackBoard(int row, int column, int value)
        {
            backBoard[row][column] = value;
        }

        public int GetBackBoard(int row, int col)
        {
            return backBoard[row][col];
        }

        private static int GetColumn(int num)
        {
            return num % 10;
        }

        public bool CheckEmpty(int row, int column)
        {
            if (backBoard[row][column].Equals(0))
                return true;
            else 
                return false;
        }

        public bool TakeLowest(int column, bool player1)
        {
            for (int row = backBoard.Count - 1; row >= 0; row--)
            {
                if (CheckEmpty(row, column))
                {
                    if (player1)
                        SetBackBoard(row, column, 1);
                    else
                        SetBackBoard(row, column, 2);

                    if (CheckWin(backBoard, Player, row, column))
                        GameOver();
                    else if (CheckFull())
                        BoardFull();
                    else                    
                        ChangePlayer();

                    return true;
                }
            }
            return false;
        }

        public C4GW()
        {
            for (int row = 0; row < 6; row++)
            {
                backBoard.Add(new ObservableCollection<int>());
                frontBoard.Add(new List<int>());
                for (int column = 0; column < 7; column++)
                {
                    frontBoard[row].Add(row * 10 + column);
                    backBoard[row].Add(0);
                }
            }

            Player = true;
            InitializeComponent();
            BackBoard.ItemsSource = backBoard;
            FrontBoard.ItemsSource = frontBoard;
            UpdateContext();
        }

        private void ButtonClickEvent(object sender, RoutedEventArgs e)
        {
            int location = (int)((Button)sender).Tag;
            int column = GetColumn(location);
            TakeLowest(column, Player);
        }

        private void SaveClickEvent(object sender, RoutedEventArgs e)
        {
            SaveGame();
        }

        private void LoadClickEvent(object sender, RoutedEventArgs e)
        {
            LoadGame();
        }

        private void ResetClickEvent(object sender, RoutedEventArgs e)
        {
            ResetGame();
        }

        public void SaveGame()
        {
            using (StreamWriter sw = new StreamWriter(".\\savegame.txt"))
            {
                if (Player)
                    sw.Write(0);
                else
                    sw.Write(1);
                for (int row = 0; row < 6; row++)
                {
                    for (int column = 0; column < 7; column++)
                    {
                        sw.Write(GetBackBoard(row, column));
                    }
                }
                sw.Close();
            }
        }

        public void LoadGame()
        {
            using (StreamReader sr = new StreamReader(".\\savegame.txt"))
            {
                int firstVal = sr.Read();
                if (firstVal == 48)
                    Player = true;
                else
                    Player = false;
                UpdateContext();
                for (int row = 0; row < 6; row++)
                {
                    for (int column = 0; column < 7; column++)
                    {    
                        int nextVal = sr.Read();
                        if(nextVal == 48)
                            SetBackBoard(row, column, 0);
                        else if(nextVal == 49)
                            SetBackBoard(row, column, 1);
                        else if(nextVal == 50)
                            SetBackBoard(row, column, 2);
                    }
                }
                sr.Close();
            }
        }

        public void ResetGame()
        {
            for (int row = 0; row < 6; row++)
            {
                for (int column = 0; column < 7; column++)
                {
                    SetBackBoard(row,column,0);
                }
            }
            Player = true;
            UpdateContext();
        }

        public void GameOver()
        {
            MessageBox.Show("Game over! Player " + PlayerAsString() + " wins!" + Environment.NewLine + "Click OK to play again.");
            ResetGame();
        }

        public void BoardFull()
        {
            MessageBox.Show("Game over! Board is full, no one wins!" + Environment.NewLine + "Click OK to play again.");
            ResetGame();
        }

        public bool CheckFull()
        {
            return (!CheckEmpty(0, 0) && !CheckEmpty(0, 1) && !CheckEmpty(0, 2) && !CheckEmpty(0, 3) && !CheckEmpty(0, 4) && !CheckEmpty(0, 5) && !CheckEmpty(0, 6));
        }

        public static bool CheckWin(ObservableCollection<ObservableCollection<int>> backBoard, bool player, int row, int column)
        {
            int value;
            if (player)
                value = 1;
            else 
                value = 2;

            if (backBoard[row][column] == value)
                if (CheckNS(backBoard, row, column, value)   ||
                    CheckNESW(backBoard, row, column, value) ||
                    CheckEW(backBoard, row, column, value)   ||
                    CheckSENW(backBoard, row, column, value))
                    return true;
            return false;
        }

        public static bool CheckNS(ObservableCollection<ObservableCollection<int>> backBoard, int row, int column, int value)
        {
            //check north +3
            if (row >= 3)
                if ((backBoard[row - 3][column] == value) && (backBoard[row - 2][column] == value) && (backBoard[row - 1][column] == value))
                    return true;
                
            //check north +2, south +1
            if (row >= 2 && row <= 4)
                if ((backBoard[row - 2][column] == value) && (backBoard[row - 1][column] == value) && (backBoard[row + 1][column] == value))
                    return true;

            //check north +1, south +2
            if (row >= 1 && row <= 3)
                if ((backBoard[row - 1][column] == value) && (backBoard[row + 1][column] == value) && (backBoard[row + 2][column] == value))
                    return true;

            //check south +3
            if (row <= 2)
                if ((backBoard[row + 1][column] == value) && (backBoard[row + 2][column] == value) && (backBoard[row + 3][column] == value))
                    return true;

            return false;
        } 

        public static bool CheckNESW(ObservableCollection<ObservableCollection<int>> backBoard, int row, int column, int value)
        {
            //check northeast +3
            if (row >= 3  && column <= 3)
                if ((backBoard[row - 3][column + 3] == value) && (backBoard[row - 2][column + 2] == value) && (backBoard[row - 1][column + 1] == value))
                    return true;
                
            //check northeast +2, southwest +1
            if (row >= 2 && row <= 4 && column >= 1 && column <= 4)
                if ((backBoard[row - 2][column + 2] == value) && (backBoard[row - 1][column + 1] == value) && (backBoard[row + 1][column - 1] == value))
                    return true;
                
            //check northeast +1, southwest +2
            if (row >= 1 && row <= 3 && column >= 2 && column <= 5)
                if ((backBoard[row - 1][column + 1] == value) && (backBoard[row + 1][column - 1] == value) && (backBoard[row + 2][column - 2] == value))
                    return true;

            //check southwest +3
            if (row <= 2 && column >= 3)
                if ((backBoard[row + 1][column - 1] == value) && (backBoard[row + 2][column - 2] == value) && (backBoard[row + 3][column - 3] == value))
                    return true;

            return false;

        }

        public static bool CheckEW(ObservableCollection<ObservableCollection<int>> backBoard, int row, int column, int value)
        {
            //check east +3
            if (column <= 3)
                if ((backBoard[row][column + 3] == value) && (backBoard[row][column + 2] == value) && (backBoard[row][column + 1] == value))
                    return true;

            //check east +2, west +1
            if (column >= 1 && column <= 4)
                if ((backBoard[row][column + 2] == value) && (backBoard[row][column + 1] == value) && (backBoard[row][column - 1] == value))
                    return true;
                
            //check east +1, west +2
            if (column >= 2 && column <= 5)
                if ((backBoard[row][column + 1] == value) && (backBoard[row][column - 1] == value) && (backBoard[row][column - 2] == value))
                    return true;
                
            //check west +3
            if (column >= 3 )
                if ((backBoard[row][column - 3] == value) && (backBoard[row][column - 2] == value) && (backBoard[row][column - 1] == value))
                    return true;
                
            return false;
        }

        public static bool CheckSENW(ObservableCollection<ObservableCollection<int>> backBoard, int row, int column, int value)
        {
            //check southeast +3
            if (row <= 2 &&column <= 3)
                if((backBoard[row + 3][column + 3] == value) && (backBoard[row + 2][column + 2] == value) && (backBoard[row + 1][column + 1] == value))
                    return true;
                
            //check southeast +2, northwest +1
            if (row >= 1 && row <= 3 && column >= 1 && column <= 4)
                if ((backBoard[row + 2][column + 2] == value) && (backBoard[row + 1][column + 1] == value) && (backBoard[row - 1][column - 1] == value))
                    return true;
                
            //check southeast +1, northwest +2
            if (row >= 2 && row <= 4 && column >= 2 && column <= 5)
                if ((backBoard[row + 1][column + 1] == value) && (backBoard[row -1][column -1] == value) && (backBoard[row -2][column -2] == value))
                    return true;
                
            //check northwest +3
            if (row >= 3 && column >= 3)
                if ((backBoard[row - 3][column - 3] == value) && (backBoard[row - 2][column - 2] == value) && (backBoard[row - 1][column - 1] == value))
                    return true;
                
            return false;
        }
    }
}
