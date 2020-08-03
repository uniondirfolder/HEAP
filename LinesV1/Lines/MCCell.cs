using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Lines
{
    public class mcCell
    {

        public CellGame _cellBall = null;
        public int _glInd = -1;
        public Point _coord;
        public mcCellBlock _cellBlock = mcCellBlock.No;
        public BallColor _cellCurrentColor = BallColor.NoColor;
        public mcPositionBorder _cellPosBord = mcPositionBorder.NaN;
        public mcCellRelations _celRel;

        public void CheckColor(List<Point> listXYposition, Directions where,MCLog log)
        {
            listXYposition.Add(_coord); log.WriteToLog($"\r\nadd ->search in [{_coord}] dir{where} pos{_cellPosBord}\r\n");
            if (_celRel.CanGo(where))
            {
                switch (where)
                {
                    case Directions.Right:
                        if (_cellCurrentColor != _celRel.nR._cellCurrentColor)
                        { log.WriteToLog($"other color rihgt "); return; }
                        log.WriteToLog($"go rihgt ");
                        _celRel.nR.CheckColor(listXYposition, where, log);
                        break;
                    case Directions.Left:
                        if (_cellCurrentColor != _celRel.nL._cellCurrentColor)
                        { log.WriteToLog($"other color left "); return; }
                        log.WriteToLog($"go left ");
                        _celRel.nL.CheckColor(listXYposition, where, log);
                        break;
                    case Directions.Top:
                        if (_cellCurrentColor != _celRel.nT._cellCurrentColor)
                        { log.WriteToLog($"other color top "); return; }
                        log.WriteToLog($"go top ");
                        _celRel.nT.CheckColor(listXYposition, where, log);
                        break;
                    case Directions.Down:
                        if (_cellCurrentColor != _celRel.nD._cellCurrentColor)
                        { log.WriteToLog($"other color down "); return; };
                        log.WriteToLog($"go down ");
                        _celRel.nD.CheckColor(listXYposition, where, log);
                        break;
                    case Directions.RightTop:
                        if (_cellCurrentColor != _celRel.nTR._cellCurrentColor)
                        { log.WriteToLog($"other color righttop "); return; }
                        log.WriteToLog($"go righttop ");
                        _celRel.nTR.CheckColor(listXYposition, where, log);
                        break;
                    case Directions.LeftTop:
                        if (_cellCurrentColor != _celRel.nTL._cellCurrentColor)
                        { log.WriteToLog($"other color lefttop "); return; }
                        log.WriteToLog($"go lefttop ");
                        _celRel.nTL.CheckColor(listXYposition, where, log);
                        break;
                    case Directions.RightDown:
                        if (_cellCurrentColor != _celRel.nDR._cellCurrentColor)
                        { log.WriteToLog($"other color rightdown "); return; }
                        log.WriteToLog($"go rightdown ");
                        _celRel.nDR.CheckColor(listXYposition, where, log);
                        break;
                    case Directions.LeftDown:
                        if (_cellCurrentColor != _celRel.nDL._cellCurrentColor)
                        { log.WriteToLog($"other color leftdown "); return; }
                        log.WriteToLog($"go leftdown ");
                        _celRel.nDL.CheckColor(listXYposition, where, log);
                        break;
                    default:
                        break;
                }

            }
            else { log.WriteToLog($"\r\ncan't go {where} \r\n---"); return; }
        }

        public mcCell(double X = 0, double Y = 0) { _celRel = new mcCellRelations(); _coord = new Point(X, Y); }
    }
}
