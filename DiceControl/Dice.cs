using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Threading;

namespace DiceControl
{
  public partial class Dice : UserControl
  {
    private Thread _RollerThread = null;
    private Graphics _Graphics = null;
    private Stopwatch _Stopwatch = new Stopwatch();
    private static Random _Random = null;
    private int _DotMargin = 20;
    private int _DotThickness = 15;

    /// <summary>
    /// The current/last result show on the dice
    /// </summary>
    public int CurrentResult { get; private set; } = 1;

    /// <summary>
    /// The time the roll is going to last
    /// </summary>
    public double RollTime { get; set; } = 5;

    /// <summary>
    /// Set if the dice is currently inside a roll
    /// </summary>
    public bool IsRolling { get; private set; } = false;

    /// <summary>
    /// Sets if the dice border has rounded or straight corners
    /// </summary>
    public bool RoundedCorners { get; set; } = true;

    public Color DotColour { get; set; } = Color.Black;

    public Dice()
    {
      InitializeComponent();
    }

    /// <summary>
    /// <para>Starts the roll animation and result generation</para>
    /// <para>
    /// Note, it is advised to use the DiceController as the controller uses a static random generator for all dice.
    /// If a custom generator is used, ensure that the different instances are not created at the same time as the default seed for the generators use the system clock,
    /// if they are created at the same time, there is a good chance the results across the dice are going to be the same.
    /// </para>
    /// </summary>
    /// <param name="randGenerator"></param>
    public void Roll(Random randGenerator)
    {
      _Random = randGenerator;
      if (_RollerThread != null && _RollerThread.IsAlive)
        _RollerThread.Abort();
      _RollerThread = null;

      _RollerThread = new Thread(new ThreadStart(RandRoller));
      _RollerThread.Start();
      IsRolling = true;

      RollStartedEventArgs args = new RollStartedEventArgs();
      args.TimeStarted = DateTime.Now;
      OnRollStarted(args);
    }

    public void SaveDiceImage(string Path)
    {
      try
      {
        using (Bitmap bmp = new Bitmap(this.Width, this.Height))
        {
          this.DrawToBitmap(bmp, new Rectangle(0, 0, this.Width, this.Height));
          bmp.Save(Path);
        }
      }
      catch (Exception)
      {
        throw;
      }
    }

    private void RandRoller()
    {
      try
      {
        _Stopwatch.Restart();

        while (_Stopwatch.Elapsed.TotalSeconds < RollTime)
        {
          CurrentResult = _Random.Next(1, 7);
          this.Invoke(new Action(() => this.Invalidate()));
          Thread.Sleep(250);
        }
        _Stopwatch.Stop();

      }
      catch (ThreadAbortException) { }
      catch (Exception) { }

      IsRolling = false;

      RollFinishedEventArgs args = new RollFinishedEventArgs();
      args.TimeFinished = DateTime.Now;
      args.DiceResult = CurrentResult;
      this.Invoke(new Action(() => OnRollFinished(args)));
    }

    private void DrawBorder()
    {
      int penWidth = 4;
      Rectangle borderRect = new Rectangle();
      borderRect.X = penWidth - (penWidth / 2);
      borderRect.Y = penWidth - (penWidth / 2);
      borderRect.Width = this.Width - penWidth - (penWidth / 2);
      borderRect.Height = this.Height - penWidth - (penWidth / 2);

      if (RoundedCorners)
        _Graphics.DrawRoundedRectangle(new Pen(Color.Black, 4), borderRect, 3);
      else
        _Graphics.DrawRectangle(new Pen(Color.Black, 4), borderRect);
    }

    private void DrawDot(int X, int Y)
    {
      Rectangle rect = new Rectangle();
      rect.X = X - (_DotThickness / 2);
      rect.Y = Y - (_DotThickness / 2);
      rect.Width = _DotThickness;
      rect.Height = _DotThickness;

      _Graphics.DrawEllipse(new Pen(DotColour), rect);
      _Graphics.FillEllipse(new SolidBrush(DotColour), rect);
    }

    private void Dice_Load(object sender, EventArgs e)
    {

    }

    private void OnPaint(object sender, PaintEventArgs e)
    {
      DoubleBuffered = true;
      _Graphics = e.Graphics;
      _Graphics.SmoothingMode = SmoothingMode.AntiAlias;
      DrawBorder();
      switch (CurrentResult)
      {
        case 1:
          DrawDot(this.Width / 2, this.Height / 2);
          break;
        case 2:
          DrawDot(_DotMargin, _DotMargin);
          DrawDot(this.Width - _DotMargin, this.Height - _DotMargin);
          break;
        case 3:
          DrawDot(_DotMargin, _DotMargin);
          DrawDot(this.Width / 2, this.Height / 2);
          DrawDot(this.Width - _DotMargin, this.Height - _DotMargin);
          break;
        case 4:
          DrawDot(_DotMargin, _DotMargin);
          DrawDot(this.Width - _DotMargin, _DotMargin);
          DrawDot(this.Width - _DotMargin, this.Height - _DotMargin);
          DrawDot(_DotMargin, this.Height - _DotMargin);
          break;
        case 5:
          DrawDot(_DotMargin, _DotMargin);
          DrawDot(this.Width - _DotMargin, _DotMargin);
          DrawDot(this.Width - _DotMargin, this.Height - _DotMargin);
          DrawDot(_DotMargin, this.Height - _DotMargin);
          DrawDot(this.Width / 2, this.Height / 2);
          break;
        case 6:
          DrawDot(_DotMargin, _DotMargin);
          DrawDot(_DotMargin, this.Height / 2);
          DrawDot(_DotMargin, this.Height - _DotMargin);
          DrawDot(this.Width - _DotMargin, _DotMargin);
          DrawDot(this.Width - _DotMargin, this.Height / 2);
          DrawDot(this.Width - _DotMargin, this.Height - _DotMargin);

          break;
      }
    }

    private void OnLoad(object sender, EventArgs e)
    {

    }

    protected virtual void OnRollStarted(RollStartedEventArgs e)
    {
      EventHandler<RollStartedEventArgs> handler = RollStarted;
      if (handler != null)
      {
        handler(this, e);
      }
    }

    protected virtual void OnRollFinished(RollFinishedEventArgs e)
    {
      EventHandler<RollFinishedEventArgs> handler = RollFinished;
      if (handler != null)
      {
        handler(this, e);
      }
    }

    public event EventHandler<RollStartedEventArgs> RollStarted;
    public event EventHandler<RollFinishedEventArgs> RollFinished;

  }

  public class RollStartedEventArgs : EventArgs
  {
    public DateTime TimeStarted { get; set; }
  }

  public class RollFinishedEventArgs : EventArgs
  {
    public DateTime TimeFinished { get; set; }
    public int DiceResult { get; set; }
  }
}
