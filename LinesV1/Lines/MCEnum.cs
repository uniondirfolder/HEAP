using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lines
{
    public enum Directions
    {
        Right, Left, Top, Down, RightTop, LeftTop, RightDown, LeftDown
    }
    public enum BallColor
    {
        White, Black, Green, Red, Yellow, Blue, Magenta, NoColor
    }
    public enum mcPositionBorder
    {
        RT, LT, RD, LD, RB, TB, LB, DB, C, NaN
    }
    public enum mcWhatToDo
    {
        SelectEmptyCells, SetAllBallsNaN, SetDeskCoord, SetNeigboursSNs
    }
    public enum mcCellBlock { Yes, No }
}
