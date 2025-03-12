using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiceControl
{
  /// <summary>
  /// Class that handles and controls multiple dice
  /// </summary>
  public class DiceController
  {
    /// <summary>
    /// Returns true if there is a roll currently active
    /// </summary>
    public bool IsRolling { get => _ControlThread != null && _ControlThread.IsAlive; }

    // The dictionary collection of dice
    private Dictionary<string, Dice> Dice = new Dictionary<string, Dice>();

    // Main control thread object
    private Thread _ControlThread;

    // The main random object that is used for all dice randomness
    private Random _Random = new Random();

    /// <summary>
    /// Adds a dice object to the dices dictionary
    /// </summary>
    /// <param name="dice"></param>
    /// <param name="Alias"></param>
    /// <exception cref="ArgumentException"></exception>
    public void AddDice(Dice dice, string Alias)
    {
      if (string.IsNullOrEmpty(Alias))
        throw new ArgumentException("The dice alias cannot be null or empty", "Alias");

      Dice.Add(Alias, dice);
    }

    /// <summary>
    /// Sets the dice roll time in seconds
    /// </summary>
    /// <param name="DiceAlias"></param>
    /// <param name="RollTimeSeconds"></param>
    /// <exception cref="ArgumentException"></exception>
    public void SetDiceRollTime(string DiceAlias, double RollTimeSeconds)
    {
      if (string.IsNullOrEmpty(DiceAlias))
        throw new ArgumentException("The dice alias cannot be null or empty", "DiceAlias");

      Dice[DiceAlias].RollTime = RollTimeSeconds;
    }

    /// <summary>
    /// Saves the current dice appearence to a bitmap
    /// </summary>
    /// <param name="DiceAlias"></param>
    /// <param name="Path"></param>
    /// <exception cref="ArgumentException"></exception>
    public void SaveDiceImage(string DiceAlias, string Path)
    {
      if (string.IsNullOrEmpty(DiceAlias))
        throw new ArgumentException("The dice alias cannot be null or empty", "DiceAlias");

      Dice[DiceAlias].SaveDiceImage(Path);
    }

    /// <summary>
    /// Starts the roll off for all dices in the dictionary
    /// </summary>
    public void Roll()
    {
      if (Dice.Count > 0)
      {
        if (_ControlThread != null && _ControlThread.IsAlive)
        {
          _ControlThread.Abort();
          _ControlThread = null;
        }

        _ControlThread = new Thread(new ThreadStart(RollControl));
        _ControlThread.Start();

        ControllerRollStartedEventArgs args = new ControllerRollStartedEventArgs();
        args.TimeStarted = DateTime.Now;
        OnRollStarted(args);
      }
    }

    /// <summary>
    /// Main roll control thread
    /// </summary>
    private void RollControl()
    {
      int DiceFinished = 0;
      ControllerRollFinishedEventArgs args = new ControllerRollFinishedEventArgs();

      try
      {
        foreach (KeyValuePair<string, Dice> dice in Dice)
          dice.Value.Roll(_Random);

        do
        {
          foreach (KeyValuePair<string, Dice> dice in Dice)
            if (dice.Value.IsRolling == false)
              DiceFinished++;

          if (DiceFinished > Dice.Count || DiceFinished < 0)
            DiceFinished = Dice.Count;

          Thread.Sleep(50);

        } while (DiceFinished != Dice.Count);

        args.TimeFinished = DateTime.Now;
        args.Results = new Dictionary<string, int>();

        foreach (KeyValuePair<string, Dice> dice in Dice)
          args.Results.Add(dice.Key, dice.Value.CurrentResult);

        foreach (KeyValuePair<string, Dice> dice in Dice)
          args.TotalResult += dice.Value.CurrentResult;

        args.IsDouble = false;

        if (Dice.Count == 2)
          args.IsDouble = Dice.ElementAt(0).Value.CurrentResult == Dice.ElementAt(1).Value.CurrentResult;

      }
      catch (ThreadAbortException) { }
      catch (Exception) { }

      Dice.ElementAt(0).Value.Invoke(new Action(() => OnRollFinished(args)));
    }

    /// <summary>
    /// Event that gets fired once all the rolls have started
    /// </summary>
    protected virtual void OnRollStarted(ControllerRollStartedEventArgs e)
    {
      EventHandler<ControllerRollStartedEventArgs> handler = RollStarted;
      if (handler != null)
      {
        handler(this, e);
      }
    }

    /// <summary>
    /// Event that gets fired once all the rolls have finished
    /// </summary>
    protected virtual void OnRollFinished(ControllerRollFinishedEventArgs e)
    {
      EventHandler<ControllerRollFinishedEventArgs> handler = RollFinished;
      if (handler != null)
      {
        handler(this, e);
      }
    }

    /// <summary>
    /// Event that gets fired once all the rolls have started
    /// </summary>
    public event EventHandler<ControllerRollStartedEventArgs> RollStarted;

    /// <summary>
    /// Event that gets fired once all the rolls have finished
    /// </summary>
    public event EventHandler<ControllerRollFinishedEventArgs> RollFinished;
  }

  public class ControllerRollStartedEventArgs : EventArgs
  {
    /// <summary>
    /// The time the roll started
    /// </summary>
    public DateTime TimeStarted { get; set; }
  }

  public class ControllerRollFinishedEventArgs : EventArgs
  {
    /// <summary>
    /// The time the roll finished
    /// </summary>
    public DateTime TimeFinished { get; set; }

    /// <summary>
    /// The list of dice results, available through their given alias when added to the dictionary
    /// </summary>
    public Dictionary<string, int> Results { get; set; }

    /// <summary>
    /// The sum off all dice results
    /// </summary>
    public int TotalResult { get; set; }

    /// <summary>
    /// Only set if there are two dice in the controller, set if both dice results are the same
    /// </summary>
    public bool IsDouble { get; set; }
  }
}
