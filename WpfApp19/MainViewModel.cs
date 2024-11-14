using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace TicTacToe
{
    public class MainViewModel : ViewModelBase
    {
        private const int BoardSize = 3;
        private MarkType[,] board;
        private bool isPlayerTurn;
        private Random random;
        public ObservableCollection<CellViewModel> Board { get; set; }
        public ICommand CellClickCommand { get; set; }
        public ICommand NewGameCommand { get; set; }
        public bool IsCrossSelected { get; set; }
        public bool IsCircleSelected { get; set; }
        public bool IsCellEnabled { get; set; }

        public MainViewModel()
        {
            InitializeGame();
            CellClickCommand = new RelayCommand<CellViewModel>(HandleCellClick);
            NewGameCommand = new RelayCommand(InitializeGame);
            IsCrossSelected = true;
            IsCircleSelected = false;
            random = new Random();
        }

        private void InitializeGame()
        {
            board = new MarkType[BoardSize, BoardSize];
            isPlayerTurn = true;
            IsCellEnabled = true;
            Board = new ObservableCollection<CellViewModel>();
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    Board.Add(new CellViewModel { Row = i, Column = j, Mark = MarkType.Empty, MarkDisplay = " " });
                }
            }
            OnPropertyChanged(nameof(Board));
        }

        private void HandleCellClick(CellViewModel cell) // обработка нажатия
        {
            if (!IsCellEnabled || cell.Mark != MarkType.Empty)
                return;
            if (IsCrossSelected)
            {
                cell.Mark = MarkType.Cross;
                cell.MarkDisplay = "X";
            }
            else if (IsCircleSelected)
            {
                cell.Mark = MarkType.Circle;
                cell.MarkDisplay = "O";
            }
            board[cell.Row, cell.Column] = cell.Mark;
            isPlayerTurn = !isPlayerTurn;

            IsCellEnabled = false; // невозможность хода, пока ходит компьютер
            OnPropertyChanged(nameof(IsCellEnabled));
            if (CheckForWin(cell.Mark))
            {
                MessageBox.Show("ТЫ ВЫИГРАЛ!");
                InitializeGame();
                return;
            }
            if (IsBoardFull())
            {
                MessageBox.Show("НИЧЬЯ!");
                InitializeGame();
                return;
            }

            MakeComputerMove();

            IsCellEnabled = true; // ход игрока
            OnPropertyChanged(nameof(IsCellEnabled));
            if (CheckForWin(MarkType.Circle))
            {
                MessageBox.Show("КОМПЬЮТЕР ВЫИГРАЛ!");
                InitializeGame();
                return;
            }
            if (IsBoardFull())
            {
                MessageBox.Show("НИЧЬЯ!");
                InitializeGame();
                return;
            }
        }

        private void MakeComputerMove()
        {
            MarkType computerMoveMark = IsCrossSelected ? MarkType.Circle : MarkType.Cross; // определение знака компьютера
            // реализация логики компьютера
            while (true)
            {
                int row = random.Next(0, BoardSize);
                int col = random.Next(0, BoardSize);
                if (board[row, col] == MarkType.Empty)
                {
                    board[row, col] = computerMoveMark;
                    Board[row * BoardSize + col].Mark = computerMoveMark;
                    Board[row * BoardSize + col].MarkDisplay = computerMoveMark == MarkType.Cross ? "X" : "O";
                    isPlayerTurn = true;
                    break;
                }
            }
        }

        private bool CheckForWin(MarkType mark)
        {
            // проверка строк (столбцы и диагонали)
            for (int i = 0; i < BoardSize; i++)
            {
                if (board[i, 0] == mark && board[i, 1] == mark && board[i, 2] == mark)
                    return true;
                if (board[0, i] == mark && board[1, i] == mark && board[2, i] == mark)
                    return true;
            }
            if (board[0, 0] == mark && board[1, 1] == mark && board[2, 2] == mark)
                return true;
            if (board[0, 2] == mark && board[1, 1] == mark && board[2, 0] == mark)
                return true;
            return false;
        }

        private bool IsBoardFull()
        {
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    if (board[i, j] == MarkType.Empty)
                        return false;
                }
            }
            return true;
        }
    }

    public enum MarkType // тип метки
    {
        Empty,
        Cross,
        Circle
    }

    public class CellViewModel : ViewModelBase
    {
        private string markDisplay;
        public int Row { get; set; }
        public int Column { get; set; }
        public MarkType Mark { get; set; }
        public string MarkDisplay
        {
            get { return markDisplay; }
            set
            {
                markDisplay = value;
                OnPropertyChanged(nameof(MarkDisplay));
            }
        }
    }
}