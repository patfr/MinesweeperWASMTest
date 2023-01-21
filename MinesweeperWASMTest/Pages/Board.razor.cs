using Microsoft.AspNetCore.Components;
using MinesweeperWASMTest.Data.Classes;
using MinesweeperWASMTest.Data.Enums;

namespace MinesweeperWASMTest.Pages
{
    public partial class Board
    {
        private static CellClass[,] _cells = new CellClass[0,0];
        private static int _columns;
        private static int _rows;
        private int _mines;
        private static bool _lost = false;
        private static bool _done = false;
        private static bool _won = false;

        public static bool Lost => _lost;
        public static bool Done => _done;

        public static CellClass[,] Cells => _cells;

        public static int Columns => _columns;
        public static int Rows => _rows;

        public static string State =>
            _lost ? "You lost" :
            _won ? "You won" :
            _done ? "Incorrect" :
            "";

        protected override void OnInitialized()
        {
            _columns = 10;
            _rows = 10;
            _mines = 10;

            GenerateBoard();

            base.OnInitialized();
        }

        private void GenerateBoard()
        {
            var random = new Random();

            _cells = new CellClass[_columns, _rows];

            for (var i = 0; i < _columns; i++)
                for (var j = 0; j < _rows; j++)
                    _cells[i, j] = new CellClass(i, j);

            for (var i = 0; i < _mines; i++)
            {
                var x = random.Next(0, _columns);
                var y = random.Next(0, _rows);

                if (IsCellMine(ref _cells, x, y))
                    i--;
                else
                    _cells[x, y].MakeMine();
            }

            foreach (var cell in _cells)
                cell.SetCount(GetNeighborMineCount(cell, ref _cells));
        }

        private int GetNeighborMineCount(CellClass cell, ref CellClass[,] cells)
        {
            var count = 0;

            for (var i = -1; i < 2; i++)
                for (var j = -1; j < 2; j++)
                    if (!(i == 0 && j == 0) && IsCellMine(ref cells, cell.X + i, cell.Y + j))
                        count++;

            return count;
        }

        private bool IsCellMine(ref CellClass[,] cells, int x, int y)
        {
            if (x < 0 || y < 0)
                return false;

            if (x >= _columns || y >= _rows) 
                return false;

            return cells[x, y].Type == CellType.Mine;
        }

        private void CellClicked(int x, int y)
        {
            if (IsCellMine(ref _cells, x, y))
                _lost = true;
            else
                _cells[x, y].Reveal();

            CheckIfWon();

            StateHasChanged();
        }

        private void CellRightClicked(int x, int y)
        {
            if (!_cells[x, y].IsRevealed)
                _cells[x, y].IsFlaged = !_cells[x, y].IsFlaged;

            CheckIfWon();

            StateHasChanged();
        }

        private void CheckIfWon()
        {
            if (!_lost)
            {
                var done = true;
                var won = true;

                for (var i = 0; i < _columns; i++)
                    for (var j = 0; j < _rows; j++)
                        if (!_cells[i, j].IsRevealed ^ _cells[i, j].IsFlaged)
                        {
                            won = false;
                            done = false;
                        }
                        else if (_cells[i, j].IsFlaged && _cells[i, j].Type != CellType.Mine)
                            won = false;

                _done = done;
                _won = won;
            }
        }
    }
}
