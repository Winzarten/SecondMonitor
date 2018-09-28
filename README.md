# Second Monitor

Master: [![Build status](https://ci.appveyor.com/api/projects/status/9a6pw8no49n8irip/branch/master?svg=true)](https://ci.appveyor.com/project/Winzarten/secondmonitor/branch/master)

Project: [![Build status](https://ci.appveyor.com/api/projects/status/9a6pw8no49n8irip?svg=true)](https://ci.appveyor.com/project/Winzarten/secondmonitor)

## Introduction:

Second Monitor is Timing/Car information application for racing simulators. It displays the actual session information, timing information and basic car status idication.

![Screenshot](/_githubStuff/SecondMonitor.png)

**Information provided**:
* Session Information:
  * Current track + layout + session type
  * Weather information (not supported for r3r as it doens't provide those)
  * Best Lap information - Driver, lap, time
  * Time/laps remaining in the session
* Live timing, for each driver:
  * Position
  * Driver name
  * Car Name
  * Completed Laps
  * Time of last lap
  * Current pace (average of last few laps)
  * Best lap
  * Current lap time
  * Sector Times
  * Pit information - in practice/qually it is a simple "in/out", in race it shows number of pit stops, and the last pit stop information - lap of the pitstop and total time
  * Timing can either by sorted as absolute (first driver always first), or relative (drivers in front of player on track will be before the player, even when lapped)
  * Times can be displayed as absolute values (whole number) or relative to the players time. So you can quickly see how is your last lap/pace to other drivers
  * Timing hilights the player, drivers in pits, personal best and session best times, cars lap back and lap in front.
  * Gap to player
  * Maximum speed
* Live detla times between your current lap and previous + personal best
* Timing Circle (ellipse :D ): Position of cars on track projected on an ellipse
* Car Information:
  * Wheel status (for each wheel):
  * Left, center and right tyre temperature
  * Brake temperature
  * Tyre pressure
  * Tyre condition  
  * Water/oil temp
* Fuel calculator : Can operate in two modes: time (displaying units pre minute) or laps (units per lap)
  *Total fuel
  *Current consumption rate
  *Last lap/minute consumption
  *Time/laps left until empty tank with current average consumption rate  
* Pedal position

* **Settings options:**
  * Ability to set UOM for Volumes (liters, US Gallons), Temperature (Celzius, Fahrenheit, Kelvin), Pressure (Kilo-pascal, bar, atm, psi)
  * Pace laps - number of laps used for pace calculation
  * Refresh rate (ms) - refresh rate of the timing datagrid
  
* **Aditional Functionality**  
  * Double click on a driver will open the drivers lap/sector times.
  * The app is able to save xlsx file after each session, containing a brief session summary and a detailed log of each players laps + sectors. Settings are in the options
  
## Supported Simulators
* R3E - Works out of the box
* Automobilista - Requires the rFactorSharedMemoryMapPlugin (https://github.com/dallongo/rFactorSharedMemoryMap). Can be automaticaly installed by the app. This is the same plugin that is used (and automatically installed) by CrewChief, so if you're using that, you're good to go.
* RFactor 1 - Same as Automobilista. Wasn't tested, but it is the same engine as AMS, and the same plugin is used for data, so it should work.
* RFactor 2 - Requires the rF2SharedMemoryMapPlugin (https://github.com/TheIronWolfModding/rF2SharedMemoryMapPlugin). Can be automaticaly installed by the app. This is the same plugin that is used (and automatically installed) by CrewChief, so if you're using that, you're good to go.
* Assetto Corsa - Requires custom plugin, than should be automatically installed when the app detect Assetto Corsa running. The plugin needs to be enabled in the options settings manualy. 
* Project Cars - Limited functionality as split times and lap times are not provided by the sim api. Splits don't work at all, and lap timing is done by the app, so the will be slight difference between what is in the app and what is in sim. Big thanks to mr_belowski for allowing me to use his project cars pit coordinates from CrewChief :)


## Known Issues

* **All:**
  * Final lap completion for AI might not be timed properly. This is a precaution, because some sims (i.e. r3e, report lap completed for AI the moment the player crosses the finish line)
  * Gap: while it is there, the implementation is very simple, just distance / speed, so it deviates a lot at higher distances. Thats why 30s +/- is the maximum it will show.
* **Assetto Corsa**
  * AC doesn't provide temperature information for water and oil temps, so these will be frozen.
  * Not all cars in AC have brake temp simulated
  * Because AC splits are little bit weird (i.e. track have 15 splits), the app is using custom splits. Each split is 1/3 of track distance.
  * If you restart session soon after start, then the app might not re-initialize property and reset the timing after the first lap is completed.
  * Tyre wear might not work correctly (WIP).
* **Project Cars 1**
  * Timing is done by app, so there might be slight differences between in-sim and in-app times
  * Pit Detection - pit detection works on comparing driver position with the entry/exit points, so if you teleport back to pits it will not detect you as in pits. There is no easy way to fix this, as the sim doesn't provide any clear indication if a driver is in pits
  * Timing will sometime reset when editing pit presets during race (should be mostly fixed)
  * Top speed is wonky :), that's because speed isn't provided by sim and has to be computed, which is prone to some error. I will try to make it less wonky ;)
  * Crr Class instead of Car Model - Pcars doesn't provide car names for other than the player driver, so I used the class for all drivers.

## Future Plans
 - [x] Little UI rework (mostly to replace the ugly gauges for something more fresh)
 - [ ] Add Race Progress and Position Percentages chrats to reports
 - [ ] Add ability to modify optimal tyre and brakes temperature for cars / tyre types
 - [ ] Rework of the timing circle, so the app can learn track layout so proper map can be displayed.
 - [ ] Increase amount of information for tyres (indicate bottoming up, dirt, slip)
## Installation

Check release tab for latests version

