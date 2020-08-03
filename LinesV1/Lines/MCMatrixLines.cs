using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Lines 
{ 
    public class MCMatrixLines
    {
        MainWindow _MW;
        public MCLog log;
        //---------------
        Point _boardHW;
        mcCell[,] _grid;
        public Point _mousePosOnCell;
        //----------------------------------
        public Point _cellPress;
        public void _SetNANCellPress() { _cellPress.X = -1; _cellPress.Y = -1; }
        public bool _IsCellPress() { if (_cellPress.Y == -1 && _cellPress.X == -1) return false; return true; ; }
        public void _SetCellPress(Point coord) { _cellPress.X = coord.X; _cellPress.Y = coord.Y; }
        //----------------------------------

        DispatcherTimer timer;
        int _myTickTime = 2;
        Random _glRand;
        int _countNextBalls =3;
        int _countColorBalls =7;
        int _gameScore = 0;
        int _ballsCount = 0;
        bool gameOver = false;

        List<BallColor> _listColorNext;
        List<Point> _listBallsNext;
        List<Point> _listCellEmpty;
        List<Point> _listReBuild;
        Point _NBallOne;
        Point _NBallTwo;
        Point _NBallThree;

        public MCMatrixLines(int width, int hight, MainWindow mainWindow) 
        {
            
            _MW = mainWindow;
            _mousePosOnCell = new Point(-1, -1);
            _cellPress = new Point(-1, -1);
            _boardHW = new Point(width, hight);
            _grid = new mcCell[hight, width];
            _glRand = new Random();
            _listColorNext = new List<BallColor>();
            _listBallsNext = new List<Point>();
            _listCellEmpty = new List<Point>();
            _listReBuild = new List<Point>();
            _NBallOne = new Point();
            _NBallTwo = new Point();
            _NBallThree = new Point();

            FillGameDesk();

            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
            log = new MCLog();
            _MW.lblScore.Content = "0";

            
        }

        private void timer_Tick(object sender, EventArgs e)
        {

            if (this._myTickTime != 5) 
            {
                _myTickTime++;
                FirstLoadScreen();
            }
            else
            {
                timer.Stop();
                int i =_MW.Desk.Children.IndexOf(_MW.lblStartGame);
                _MW.Desk.Children.RemoveAt(i);
                ForMyFor(mcWhatToDo.SetAllBallsNaN);
                BallsNextStep();
                BallsNextStepActivate();

            }
        }

        public void FirstLoadScreen()
        {
            int r;
            for (int y = 0; y < _boardHW.Y; y++)
            {
                for (int x = 0; x < _boardHW.X; x++)
                {
                    r = _glRand.Next(0, 8);
                    _grid[y, x]._cellBall._BallChangeColor((BallColor)r);
                }
            }
        }
      
        void FillGameDesk()
        {
            int temp = 1;
            for (int y = 0; y < _boardHW.Y; y++)
            {
                for (int x = 0; x < _boardHW.X; x++)
                {
                    _grid[y, x] = new mcCell(x,y);
                    CellGame T = new CellGame(_grid[y, x]);
                    T._cellBall._celRel.MB = this;
                    T._BallChangeColor((BallColor)_glRand.Next(0, _countColorBalls));
                    T._cellBall._celRel.SN = temp; temp++;
                    _MW.Desk.Children.Add(T);
                    _grid[y, x]._cellBall = (CellGame)_MW.Desk.Children[_MW.Desk.Children.Count - 1];
                }
            }

            for (int y = 0; y < _boardHW.Y; y++)
            {
                for (int x = 0; x < _boardHW.X; x++)
                {

                    if (x > 0 && y > 0 && x < _boardHW.X - 1 && y < _boardHW.Y - 1)
                    {
                        _grid[y, x]._celRel.nT = _grid[y - 1, x];
                        _grid[y, x]._celRel.nTL = _grid[y - 1, x - 1];
                        _grid[y, x]._celRel.nTR = _grid[y - 1, x + 1];

                        _grid[y, x]._celRel.nD = _grid[y + 1, x];
                        _grid[y, x]._celRel.nDL = _grid[y + 1, x - 1];
                        _grid[y, x]._celRel.nDR = _grid[y + 1, x + 1];

                        _grid[y, x]._celRel.nL = _grid[y, x - 1];
                        _grid[y, x]._celRel.nR = _grid[y, x + 1];

                        _grid[y, x]._cellPosBord = mcPositionBorder.C;

                    }
                    if (x == 0 && y > 0 && y < _boardHW.Y - 1)
                    {
                        _grid[y, x]._celRel.nT = _grid[y - 1, x];
                        _grid[y, x]._celRel.nTR = _grid[y - 1, x + 1];
                        _grid[y, x]._celRel.nR = _grid[y, x + 1];

                        _grid[y, x]._celRel.nDR = _grid[y + 1, x + 1];
                        _grid[y, x]._celRel.nD = _grid[y + 1, x];

                        _grid[y, x]._cellPosBord = mcPositionBorder.LB;
                    }
                    if (x == _boardHW.X - 1 && y > 0 && y < _boardHW.Y - 1)
                    {
                        _grid[y, x]._celRel.nT = _grid[y - 1, x];
                        _grid[y, x]._celRel.nTL = _grid[y - 1, x - 1];
                        
                        _grid[y, x]._celRel.nL = _grid[y, x - 1];

                        _grid[y, x]._celRel.nDL = _grid[y + 1, x - 1];
                        _grid[y, x]._celRel.nD = _grid[y + 1, x];

                        _grid[y, x]._cellPosBord = mcPositionBorder.RB;
                    }
                    if (y == 0 && x > 0 && x < _boardHW.X - 1)
                    {
                        _grid[y, x]._celRel.nL = _grid[y, x - 1];
                        _grid[y, x]._celRel.nR = _grid[y, x + 1];
                        _grid[y, x]._celRel.nD = _grid[y + 1, x];
                        _grid[y, x]._celRel.nDL = _grid[y + 1, x - 1];
                        _grid[y, x]._celRel.nDR = _grid[y + 1, x + 1];

                        _grid[y, x]._cellPosBord = mcPositionBorder.TB;
                    }
                    if (y == _boardHW.Y - 1 && x > 0 && x < _boardHW.X - 1)
                    {
                        _grid[y, x]._celRel.nL = _grid[y, x - 1];
                        _grid[y, x]._celRel.nR = _grid[y, x + 1];
                        _grid[y, x]._celRel.nT = _grid[y - 1, x];
                        _grid[y, x]._celRel.nTL = _grid[y - 1, x - 1];
                        _grid[y, x]._celRel.nTR = _grid[y - 1, x + 1];

                        _grid[y, x]._cellPosBord = mcPositionBorder.DB;
                    }

                    if (x == 0 && y == 0)
                    {
                        _grid[y, x]._celRel.nR = _grid[y, x + 1];
                        _grid[y, x]._celRel.nDR = _grid[y + 1, x + 1];
                        _grid[y, x]._celRel.nD = _grid[y + 1, x];

                        _grid[y, x]._cellPosBord = mcPositionBorder.LT;
                    }
                    if (x == _boardHW.X - 1 && y == 0)
                    {
                        _grid[y, x]._celRel.nL = _grid[y, x - 1];
                        _grid[y, x]._celRel.nD = _grid[y + 1, x];
                        _grid[y, x]._celRel.nDL = _grid[y + 1, x - 1];

                        _grid[y, x]._cellPosBord = mcPositionBorder.RT;
                    }
                    if (x == 0 && y == _boardHW.Y - 1)
                    {
                        _grid[y, x]._celRel.nT = _grid[y - 1, x];
                        _grid[y, x]._celRel.nR = _grid[y, x + 1];
                        _grid[y, x]._celRel.nTR = _grid[y - 1, x + 1];

                        _grid[y, x]._cellPosBord = mcPositionBorder.LD;
                    }
                    if (x == _boardHW.X - 1 && y == _boardHW.Y - 1)
                    {
                        _grid[y, x]._celRel.nT = _grid[y - 1, x];
                        _grid[y, x]._celRel.nL = _grid[y, x - 1];
                        _grid[y, x]._celRel.nTL = _grid[y - 1, x - 1];

                        _grid[y, x]._cellPosBord = mcPositionBorder.RD;
                    }
                }
            }

            ForMyFor(mcWhatToDo.SetDeskCoord);
            ForMyFor(mcWhatToDo.SetNeigboursSNs);
        }
       
        void BallsNextStep() 
        {
            if (gameOver) return;
            if (_listCellEmpty.Count != 0) { _listCellEmpty.Clear(); }

            ForMyFor(mcWhatToDo.SelectEmptyCells);

            if (_listCellEmpty.Count == 0) { gameOver = true; ; }

            _listColorNext.Clear();

            int r;
            int i;  
            {
                r = _glRand.Next(0, _countColorBalls);
                _listColorNext.Add((BallColor)r);
                i = 1;
                while (i != _countNextBalls)
                {
                    r = _glRand.Next(0, _countColorBalls);
                    if (!_listColorNext.Contains((BallColor)r)) { _listColorNext.Add((BallColor)r); i++; }
                }
            }
            {
                //if (_listBallsNext.Count != 0) { _listBallsNext.Clear(); }

                r = _glRand.Next(0, _listCellEmpty.Count-1);
                _listBallsNext.Add(_listCellEmpty.ElementAt(r));
                i = 1;
                while (i != _countNextBalls)
                {
                    r = _glRand.Next(0, _listCellEmpty.Count-1);
                    
                    if (!_listBallsNext.Contains(_listCellEmpty.ElementAt(r))) 
                    { 
                        _listBallsNext.Add(_listCellEmpty.ElementAt(r));
                        i++;
                    }
                }
                 _listCellEmpty.Clear();

                _NBallOne = _listBallsNext[0];
                _NBallTwo = _listBallsNext[1];
                _NBallThree = _listBallsNext[2];
            }
            {
                i = 0;
                foreach (var item in _listBallsNext)
                {
                    _grid[(int)item.Y, (int)item.X]._cellBall._BallSetLittleHW();
                    _grid[(int)item.Y, (int)item.X]._cellCurrentColor = _listColorNext[i];
                    _grid[(int)item.Y, (int)item.X]._cellBall._BallChangeColor(_listColorNext[i]);
                    if (i == 0) { _MW.BallNextColor1.Fill = _grid[(int)item.Y, (int)item.X]._cellBall.Ball.Fill; }
                    if (i == 1) { _MW.BallNextColor2.Fill = _grid[(int)item.Y, (int)item.X]._cellBall.Ball.Fill; }
                    if (i == 2) { _MW.BallNextColor3.Fill = _grid[(int)item.Y, (int)item.X]._cellBall.Ball.Fill; }
                    i++;
                }
                
            }
            _ballsCount += 3;
        }
        public void BallsNextStepActivate() 
        {
            foreach (var item in _listBallsNext)
            {
                _grid[(int)item.Y,(int)item.X]._cellBall._BallSetActive();
            }
            _listBallsNext.Clear();

            BallsNextStep();          
        }
        void ForMyFor(mcWhatToDo what) 
        {
            for (int y = 0; y < _boardHW.Y; y++)
            {
                for (int x = 0; x < _boardHW.X; x++)
                {
                    switch (what)
                    {
                        case mcWhatToDo.SelectEmptyCells:
                            if (_grid[y, x]._cellCurrentColor == BallColor.NoColor)
                            { _listCellEmpty.Add(_grid[y, x]._coord); }
                            break;
                        case mcWhatToDo.SetAllBallsNaN:
                            _grid[y, x]._cellBall._BallSetNullWH();
                            _SetNANCellPress();
                            break;
                        case mcWhatToDo.SetDeskCoord:
                            _grid[y, x]._coord.Y = y;
                            _grid[y, x]._coord.X = x;
                            _grid[y, x]._celRel.cell = _grid[y, x];
                            break;
                        case mcWhatToDo.SetNeigboursSNs:
                            _grid[y, x]._celRel.SetNeigboursSN();
                            break;
                        default:
                            break;
                    }

                }
            }
        }

        public void Rearrenge(mcCell asker, bool empty) 
        {
            BallsNextStepActivate();

            Point P = new Point(_cellPress.X, _cellPress.Y);

            BallColor colrAsker = _grid[(int)asker._coord.Y, (int)asker._coord.X]._cellCurrentColor;
            BallColor colrPress = _grid[(int)_cellPress.Y, (int)_cellPress.X]._cellCurrentColor;

            _grid[(int)asker._coord.Y, (int)asker._coord.X]._cellBall._BallChangeColor(colrPress);

            _grid[(int)P.Y, (int)P.X]._cellBall._BallChangeColor(colrAsker);
            _grid[(int)P.Y, (int)P.X]._cellBall._BallSetUnPressWH();

            if (empty)
            {
                _grid[(int)asker._coord.Y, (int)asker._coord.X]._cellBall._BallSetNormalWH();
                _grid[(int)P.Y, (int)P.X]._cellBall._BallSetNullWH();                
            }
            PrintLines();
            Thread.Sleep(100);
            
            //_SetNANCellPress();
        }

        void GameOver() 
        {
            if (IsFullDesk())
            {
                gameOver = true;
                MessageBox.Show("GAME OVER!!!");

                if (MessageBox.Show("START NEW GAME?", "LINES", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    ForMyFor(mcWhatToDo.SetAllBallsNaN);
                    gameOver = false;
                    _listBallsNext.Clear();
                    BallsNextStep();
                    _gameScore = 0;
                    _MW.lblScore.Content = "0";
                    BallsNextStepActivate();
                }
                else 
                {
                    _MW.Close();
                }
            }
        }

        void CheckLine(Point P) 
        {
            log.WriteToLog($"Start search [{P.Y},{P.X}]\r\n-------------");
            if (P.X == -1) return;
            List<Point> line0 = new List<Point>();//right
            List<Point> line1 = new List<Point>();//left
            List<Point> line2 = new List<Point>();//top
            List<Point> line3 = new List<Point>();//down
            List<Point> line4 = new List<Point>();//RightTop
            List<Point> line5 = new List<Point>();//LeftTop
            List<Point> line6 = new List<Point>();//RightDown
            List<Point> line7 = new List<Point>();//LeftDown
            int i = 7;
            while (i!=-1)
            {
                switch ((Directions)i)
                {
                    case Directions.Right:

                        _grid[(int)P.Y, (int)P.X].CheckColor(line0, Directions.Right,log);
                        break;
                    case Directions.Left:
                        _grid[(int)P.Y, (int)P.X].CheckColor(line1, Directions.Left,log);
                        break;
                    case Directions.Top:
                        _grid[(int)P.Y, (int)P.X].CheckColor(line2, Directions.Top,log);
                        break;
                    case Directions.Down:
                        _grid[(int)P.Y, (int)P.X].CheckColor(line3, Directions.Down,log);
                        break;
                    case Directions.RightTop:
                        _grid[(int)P.Y, (int)P.X].CheckColor(line4, Directions.RightTop,log);
                        break;
                    case Directions.LeftTop:
                        _grid[(int)P.Y, (int)P.X].CheckColor(line5, Directions.LeftTop,log);
                        break;
                    case Directions.RightDown:
                        _grid[(int)P.Y, (int)P.X].CheckColor(line6, Directions.RightDown,log);
                        break;
                    case Directions.LeftDown:
                        _grid[(int)P.Y, (int)P.X].CheckColor(line7, Directions.LeftDown,log);
                        break;
                    default:
                        break;
                }
                i--;
            }

            log.WriteToLog($"\r\n--------\r\n");
            if (line0.Count > 0 && line1.Count > 0) { LinesMerge(line0, line1); line1.Clear(); log.WriteToLog($"++ balls left and right\r\n"); }
            if (line2.Count > 0 && line3.Count > 0) { LinesMerge(line2, line3); line3.Clear(); log.WriteToLog($"++ balls top and down\r\n"); }
            if (line4.Count > 0 && line7.Count > 0) { LinesMerge(line4, line7); line7.Clear(); log.WriteToLog($"++ balls rightTop and leftDown\r\n"); }
            if (line5.Count > 0 && line6.Count > 0) { LinesMerge(line5, line6); line6.Clear(); log.WriteToLog($"++ balls leftTop and rightDown\r\n"); }

            log.WriteToLog($"\r\n--------\r\n");
            if (line0.Count >= 5) { _listReBuild.AddRange(line0); log.WriteToLog($"L0 -{line0.Count}  addLR \r\n"); }
            if (line1.Count >= 5) { _listReBuild.AddRange(line1); log.WriteToLog($"L1 -{line1.Count}  addLR \r\n"); }
            if (line2.Count >= 5) { _listReBuild.AddRange(line2); log.WriteToLog($"L2 -{line2.Count}  addLR \r\n"); }
            if (line3.Count >= 5) { _listReBuild.AddRange(line3); log.WriteToLog($"L3 -{line3.Count}  addLR \r\n"); }
            if (line4.Count >= 5) { _listReBuild.AddRange(line4); log.WriteToLog($"L4 -{line4.Count}  addLR \r\n"); }
            if (line5.Count >= 5) { _listReBuild.AddRange(line5); log.WriteToLog($"L5 -{line5.Count}  addLR \r\n"); }
            if (line6.Count >= 5) { _listReBuild.AddRange(line6); log.WriteToLog($"L6 -{line6.Count}  addLR \r\n"); }
            if (line7.Count >= 5) { _listReBuild.AddRange(line7); log.WriteToLog($"L7 -{line7.Count}  addLR \r\n"); }
            
            log.WriteToLog($"List rebuild added: {_listReBuild.Count} balls\r\n--------\r\n");

        }
        void PrintLines() 
        {
            if (_cellPress.X == -1) return;
            log.ClearLog();

            if(_listReBuild.Count>0) _listReBuild.Clear();

            log.WriteToLog($"\r\nStart CheckLine(_NBallOne) [{_NBallOne}];{_grid[(int)_NBallOne.Y,(int)_NBallOne.X]._cellCurrentColor}\r\n");
            CheckLine(_NBallOne);

            log.WriteToLog($"\r\nStart CheckLine(_NBallTwo) [{_NBallTwo}];{_grid[(int)_NBallTwo.Y, (int)_NBallTwo.X]._cellCurrentColor}\r\n");
            CheckLine(_NBallTwo);

            log.WriteToLog($"\r\nStart CheckLine(_NBallThree) [{_NBallThree}];{_grid[(int)_NBallThree.Y, (int)_NBallThree.X]._cellCurrentColor}\r\n");
            CheckLine(_NBallThree);

            log.WriteToLog($"\r\nStart CheckLine(_cellPress) [{_cellPress}];{_grid[(int)_cellPress.Y, (int)_cellPress.X]._cellCurrentColor}\r\n");
            CheckLine(_cellPress);

            log.WriteToLog($"\r\nCheckLine(_mousePosOnCell) [{_mousePosOnCell}];{_grid[(int)_mousePosOnCell.Y, (int)_mousePosOnCell.X]._cellCurrentColor}\r\n");
            CheckLine(_mousePosOnCell);

            _SetNANCellPress();
            
            if (_listReBuild.Count >=5)
            {
                foreach (var i in _listReBuild)
                {
                    _grid[(int)i.Y, (int)i.X]._cellBall._BallSetNullWH();
                }

                _gameScore += _listReBuild.Count;
                _MW.lblScore.Content = _gameScore.ToString();
            }
            CheckLine(_NBallOne); log.WriteToLog($"\r\nend log PrintLines()  \r\n");
            GameOver();
        }

        void LinesMerge(List<Point> A, List<Point> B) 
        {
            List<Point> T = new List<Point>();
            foreach (var i in A)
            {
                if (!B.Contains(i)) { T.Add(i); };
            }
            foreach (var i in B)
            {
                if (!A.Contains(i)) { T.Add(i); };
            }
            foreach (var i in A)
            {
                if (!T.Contains(i)) { T.Add(i); };
            }
            foreach (var i in B)
            {
                if (!T.Contains(i)) { T.Add(i); };
            }
            string print = "";

            foreach (var item in T)
            {
                string pair = "";
                pair = $"[{ item.Y}:{item.X}] ";
                print += pair;
            }
            A.Clear();
            A.AddRange(T);
            log.WriteToLog($"LinesMerge -{T.Count} added {print}\r\n");
        }

        bool IsFullDesk() 
        {
            List<bool> E = new List<bool>();
            List<bool> B = new List<bool>();
            foreach (var item in _grid)
            {
                if(item._cellCurrentColor == BallColor.NoColor)
                    E.Add(true);
            }
            foreach (var item in _grid)
            {
                if (item._celRel.IsOcсupied())
                    E.Add(true);
            }
            if ((E.Count+B.Count) <=6)
            {
                foreach (var item in _grid)
                {
                    item._cellBall._BallBurst();
                }
                return true;
            }
            return false;
        }
      
    }
    
}

