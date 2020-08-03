using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Lines
{


    /// <summary>
    /// Логика взаимодействия для CellGame.xaml
    /// </summary>
    public partial class CellGame : UserControl
    {
        public mcCell _cellBall;
        public CellGame(mcCell cell)
        {
            InitializeComponent();
            //BallSetNullWH();
            _cellBall = cell;
        }

        public void _BallBurst()
        {

            Ball.Fill = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(Color.FromArgb(0xFF,0xFF,0xFF,0xFF),0.164),
                    new GradientStop(Color.FromArgb(0xFF,0x00,0x88,0x92),1)
                },
                new Point(0.5, 0), new Point(0.5, 1)
            );
            _BallSetBurstHW();
        }
        public void _BallChangeColor(BallColor bc)
        {
            switch (bc)
            {
                case BallColor.White:
                    Ball.Fill = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(Color.FromArgb(0xFF,0xFF,0xFF,0xFF),0.022),
                    new GradientStop(Color.FromArgb(0xFF,0xFF,0xFF,0xFF),0.462)
                }, new Point(0.5, 0), new Point(0.5, 1));_cellBall._cellCurrentColor = BallColor.White;


                    break;
                case BallColor.Black:
                    Ball.Fill = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(Color.FromArgb(0xFF,0xFF,0xFF,0xFF),0.022),
                    new GradientStop(Color.FromArgb(0xFF,0x2A,0x2A,0x25),0.399)
                }, new Point(0.5, 0), new Point(0.5, 1)); _cellBall._cellCurrentColor = BallColor.Black;

                    break;
                case BallColor.Green:
                    Ball.Fill = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(Color.FromArgb(0xFF,0xFF,0xFF,0xFF),0.022),
                    new GradientStop(Color.FromArgb(0xFF,0x00,0xFF,0x1E),0.399)
                }, new Point(0.5, 0), new Point(0.5, 1)); _cellBall._cellCurrentColor = BallColor.Green;

                    break;
                case BallColor.Red:
                    Ball.Fill = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(Color.FromArgb(0xFF,0xFF,0xFF,0xFF),0.022),
                    new GradientStop(Color.FromArgb(0xFF,0xFF,0x1D,0x00),0.399)
                }, new Point(0.5, 0), new Point(0.5, 1)); _cellBall._cellCurrentColor = BallColor.Red;

                    break;
                case BallColor.Yellow:
                    Ball.Fill = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(Color.FromArgb(0xFF,0xFF,0xFF,0xFF),0.022),
                    new GradientStop(Color.FromArgb(0xFF,0xE2,0xFF,0x00),0.399)
                }, new Point(0.5, 0), new Point(0.5, 1)); _cellBall._cellCurrentColor = BallColor.Yellow;


                    break;
                case BallColor.Blue:
                    Ball.Fill = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(Color.FromArgb(0xFF,0xFF,0xFF,0xFF),0.022),
                    new GradientStop(Color.FromArgb(0xFF,0x06,0x00,0xFF),0.399)
                }, new Point(0.5, 0), new Point(0.5, 1)); _cellBall._cellCurrentColor = BallColor.Blue;

                    break;
                case BallColor.Magenta:
                    Ball.Fill = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(Color.FromArgb(0xFF,0xFF,0xFF,0xFF),0.022),
                    new GradientStop(Color.FromArgb(0xFF,0x9D,0x00,0xFF),0.399)
                }, new Point(0.5, 0), new Point(0.5, 1)); _cellBall._cellCurrentColor = BallColor.Magenta;

                    break;
                default:
                    _cellBall._cellCurrentColor = BallColor.NoColor;
                    break;
            }
        }
        void _BallSetBurstHW() { Ball.Width = 15; Ball.Height = 15; }
        public void _BallSetLittleHW() { Ball.Width = 10; Ball.Height = 10; _cellBall._cellBlock = mcCellBlock.Yes; }
        public void _BallSetNormalWH() { Ball.Width = 25; Ball.Height = 25; _cellBall._cellBlock = mcCellBlock.Yes; }
        public void _BallSetNullWH() { Ball.Width = 0; Ball.Height = 0; _cellBall._cellCurrentColor = BallColor.NoColor;_cellBall._cellBlock = mcCellBlock.No; }
        public bool _BallSetPressWH() 
        {
            if (!_BallPressed() && !_BallNext() && !_BallNaN())
            {
                Ball.Height = 20;
                _cellBall._celRel.MB._SetCellPress(_cellBall._coord);
                return true;
            }
            return false;
        }
        public bool _BallSetUnPressWH()
        {
            if (_BallPressed())
            {
                _BallSetNormalWH();
                if(_cellBall._coord==_cellBall._celRel.MB._mousePosOnCell)
                    _cellBall._celRel.MB._SetNANCellPress();
                return true;
            }
            return false;
        }
        public void _BallSetActive() { _BallSetNormalWH(); _cellBall._cellBlock = mcCellBlock.Yes; }
        public bool _BallPressed() 
        {
            if (Ball.Height == 20 && Ball.Width == 25) return true;
            return false;
        }
        public bool _BallNext() 
        {
            if (Ball.Width == 10 && Ball.Height == 10) return true;
            return false;
        }
        public bool _BallNaN()
        {
            if (Ball.Width == 0 && Ball.Height==0) return true;
            return false;
        }
        public bool _BallNormal() 
        {
            if (Ball.Width == 25 && Ball.Height == 25) return true;
            return false;
        }
        public bool _BallRearrange() 
        {
            if (_BallNext()) 
            {
                _cellBall._celRel.MB.Rearrenge(_cellBall, false);
                return true;
            }
            if (_BallNaN()) 
            {
                _cellBall._celRel.MB.Rearrenge(_cellBall, true);
                return true;
            }
            return false;
        }
        //---------------------

        private void Ball_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_cellBall._celRel.IsOcсupied() && _cellBall._cellBall._BallNaN())
                return;
            if (_cellBall._celRel.MB._IsCellPress()) 
            {
                if (_cellBall._celRel.MB._IsCellPress() && _BallNormal()) { return; }
                
                if (_BallSetUnPressWH()) { return; }
                if (_BallRearrange()) { return; }
                
            }
            if (!_cellBall._celRel.MB._IsCellPress()) 
            {
                if (_BallSetPressWH()) { return; }
            }
                
           
        }
        private void R1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_cellBall._celRel.IsOcсupied() && _cellBall._cellBall._BallNaN())
                return;
            if (_cellBall._celRel.MB._IsCellPress())
            {
                if (_BallRearrange()) { return; }

            }
        }

        private void CellGrid_MouseEnter(object sender, MouseEventArgs e)
        {
           _cellBall._celRel.MB._mousePosOnCell = _cellBall._coord;
        }
    }
}
