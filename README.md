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
* Timing Circle (ellipse :D ) / Track Map: Position of cars on the track. The app needs one fully timed lap for to be able to show the track map. 
* Car Information:
  * Wheel status (for each wheel):
  * Left, center and right tyre temperature
  * Brake temperature
  * Tyre pressure
  * Tyre condition  
  * Water/oil temp
* Fuel Monitor - Monitoring the current fuel levels and average consumption. Offering a quick color-coded information if the actual fuel state is enough to finish the session.
  
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
* Project Cars 2 - Works out of the box. Just be sure to enable the shared memory inside Project Cars 2 [options](http://www.eksimracing.com/f-a-q/configure-project-cars-to-use-shared-memory/)
* Project Cars - Limited functionality as split times and lap times are not provided by the sim api. Splits don't work at all, and lap timing is done by the app, so the will be slight difference between what is in the app and what is in sim. Big thanks to mr_belowski for allowing me to use his project cars pit coordinates from CrewChief :)

## Known Issues
[Known Issues](https://github.com/Winzarten/SecondMonitor/wiki/Known-Issues)


## Future Plans  
 - [x] Reintroduce the fuel calculator
 - [x] Rework of the timing circle, so the app can learn track layout so proper map can be displayed.
 - [x] Improve the fuel calculator to show delta fuel. 
 - [ ] Telemetry - Allow the application to track and view advanced telemetry for individual laps.

## Installation

Check release tab for latests version
