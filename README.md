# 2D Dice Control
The control library is made up of two vital parts, the first being the dice itself, the second being the dice controller. 
The dice controllers job is to handle the rolling and syncronisation of multiple dice.

# Usage
### Create the dice controller
This instance of the dice controller will handle all the dice added to it

```cs
DiceController Controller = new DiceController();
```

### Add some dice
Add dice to the controller, these can be created programatically or dragged on to the designer, your choice, just make sure you add
the dice to the controller.

```cs
Controller.AddDice(dice1, "Dice1");
Controller.AddDice(dice2, "Dice2");
```
> [!IMPORTANT]
> When adding dice an Alias must be given, an argument exception will be thrown if not.

### Subscribe to the roll start and end events.
The controller provides two events, a start and finish event. 
- The RollStarted event arguments will give you the time the roll started...if thats any use.
- The RollFinished event will give you all the relevant information, here you can grab results of the dice by their alias name, grab the total result of all the dice together
  and check if the result's of two dice are the same (only applicable if there are two dice set in the controller, otherwise always false)

```cs
Controller.RollStarted += Controller_RollStarted;
Controller.RollFinished += Controller_RollFinished;

private void Controller_RollStarted(object sender, ControllerRollStartedEventArgs e)
{
  DateTime startTime = e.TimeStarted
}

private void Controller_RollFinished(object sender, ControllerRollFinishedEventArgs e)
{
  int dice1Result = e.Results["Dice1"];
  int dice2Result = e.Results["Dice2"];
  int totalResult = e.TotalResult;
  bool isResultDouble = e.IsDouble;
}
```

### Set the rolling time of the dice.
Each dice can have a different roll time, the controller will handle waiting till they are all finished for firing the roll end event. This is in seconds.

```cs
Controller.SetDiceRollTime("Dice1", 1);
Controller.SetDiceRollTime("Dice2", 2);
```

### Roll the dice
Start the roll.

```cs
if (!Controller.IsRolling)
  Controller.Roll();
```

# Example

![alt text](https://github.com/itsjoshsmith/DiceControl/blob/master/DiceControlExample.gif?raw=true)
