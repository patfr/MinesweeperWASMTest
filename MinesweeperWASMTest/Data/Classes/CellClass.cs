using MinesweeperWASMTest.Data.Enums;
using MinesweeperWASMTest.Pages;

namespace MinesweeperWASMTest.Data.Classes
{
    public sealed class CellClass
    {
        private CellType _type = CellType.Zero;
        private bool _isFlaged = false;
        private bool _isRevealed = false;
        private int _x;
        private int _y;

        public CellType Type 
        { 
            get => _type; 
            private set
            {
                if (!Enum.IsDefined(typeof(CellType), value))
                    throw new ArgumentException("Type did not exist", nameof(value));
                else if (_type != value)
                    _type = value;
            }
        }

        public bool IsFlaged { get => _isFlaged; set => _isFlaged = value; }
        public bool IsRevealed { get => _isRevealed; set => _isRevealed = value; }

        public int X => _x;
        public int Y => _y;


        public CellClass(int x, int y)
        {
            _x = x;
            _y = y;
        }

        public void MakeMine() => _type = CellType.Mine;

        public void SetCount(int count)
        {
            if (_type == CellType.Mine)
                return;

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Count cannot be negative");
            else if (count > 8)
                throw new ArgumentOutOfRangeException(nameof(count), "Count cannot be above 8");

            _type = (CellType)count;
        }

        public void Reveal()
        {
            if (!_isFlaged)
            {
                var was = _isRevealed;
                _isRevealed = true;

                if (!was && _type == CellType.Zero)
                {
                    for (var i = -1; i < 2; i++)
                        for (var j = -1; j < 2; j++)
                            if (!(i == 0 && j == 0) && CellExists(Board.Cells, X + i, Y + j))
                                Board.Cells[X + i, Y + j].Reveal();
                }
            }
        }

        private bool CellExists(CellClass[,] cells, int x, int y)
        {
            if (x < 0 || y < 0)
                return false;

            if (x >= Board.Columns || y >= Board.Rows)
                return false;

            return true;
        }

        public string GetCellText() => 
            (Board.Lost || Board.Done) && _type == CellType.Mine ? "M" :
            _isFlaged ? "F" :
            _isRevealed && _type != CellType.Zero ? ((int)_type).ToString() : 
            "";

        public string GetTypeClass() => _type switch
        {
            CellType.Zero => "zero",
            CellType.One => "one",
            CellType.Two => "two",
            CellType.Three => "three",
            CellType.Four => "four",
            CellType.Five => "five",
            CellType.Six => "six",
            CellType.Seven => "seven",
            CellType.Eight => "eight",
            _ => "",
        };

        public string GetOtherClass()
        {
            var classes = "";

            if (_isFlaged)
                classes += " flagged";

            if (_isRevealed && _type == CellType.Mine)
                classes += " mine";

            return classes;
        }

        public override string ToString() => $"X: {X}, Y: {Y}, Type: {Enum.GetName(Type)}";
    }
}
