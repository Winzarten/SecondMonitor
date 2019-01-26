# Second Monitor

Master: [![Build status](https://ci.appveyor.com/api/projects/status/9a6pw8no49n8irip/branch/master?svg=true)](https://ci.appveyor.com/project/Winzarten/secondmonitor/branch/master)

Project: [![Build status](https://ci.appveyor.com/api/projects/status/9a6pw8no49n8irip?svg=true)](https://ci.appveyor.com/project/Winzarten/secondmonitor)

## Introduction:

Second Monitor is Timing/Car information application for racing simulators. It displays the actual session information, timing information and basic car status idication.

![Screenshot](/_githubStuff/SecondMonitor.png)

**Information provided**:
* Session Information  
* Live timing  for each driver
* Pit information - in practice/qually it is a simple "in/out", in race it shows number of pit stops, and the last pit stop information  * Absolute/Relative driver ordering.
* Absolute/relative times 
* Live detla times between your current lap and previous + personal best
* Timing Circle (ellipse :D ) / Track Map: Position of cars on the track. The app needs one fully timed lap for to be able to show the track map. 
* Car Information - Brake temperatures, tyre temperatures + pressures, tyres condition, pedal and wheel postion, oil and water temperatures, pedals and wheel position.
* Fuel Monitor - Monitoring the current fuel levels and average consumption. Offering a quick color-coded information if the actual fuel state is enough to finish the session, and what is the required fuel delta.
* Fuel Calculator - Use consumption from previous/current session for required for fuel calculation.  
* Detailed lap summer for each driver available by double-clicking on the driver name
* Session Reports - Ability to automatically export session reports in xlsx file. Files containig race summary, lap overview for each driver, race progress and detailed lap information for players laps
  
## Telemetry Viewer:
  ![ScreenshotTV](https://github.com/Winzarten/SecondMonitor/blob/master/_githubStuff/TelemetryViewer/TelemetryViewer.png)


Telemetry Viewer allows to view and analyse the telemetry data that the main second monitor application captures during a session. The data are saved per completed lap and grouped into individual sessions. The basic usage of the application is explained in the topics below:

[See the Wiki](https://github.com/Winzarten/SecondMonitor/wiki)
  
## Supported Simulators
* R3E - Works out of the box
* Automobilista - Requires the rFactorSharedMemoryMapPlugin (https://github.com/dallongo/rFactorSharedMemoryMap). Can be automaticaly installed by the app. This is the same plugin that is used (and automatically installed) by CrewChief, so if you're using that, you're good to go.
* RFactor 1 - Same as Automobilista. Wasn't tested, but it is the same engine as AMS, and the same plugin is used for data, so it should work.
* RFactor 2 - Requires the rF2SharedMemoryMapPlugin (https://github.com/TheIronWolfModding/rF2SharedMemoryMapPlugin). Can be automaticaly installed by the app. This is the same plugin that is used (and automatically installed) by CrewChief, so if you're using that, you're good to go.
* Assetto Corsa - Requires custom plugin, than should be automatically installed when the app detect Assetto Corsa running. The plugin needs to be enabled in the options settings manualy. 
* Project Cars 2 - Works out of the box. Just be sure to enable the shared memory inside Project Cars 2 [options](http://www.eksimracing.com/f-a-q/configure-project-cars-to-use-shared-memory/)
* Project Cars - Limited functionality as split times and lap times are not provided by the sim api. Splits don't work at all, and lap timing is done by the app, so the will be slight difference between what is in the app and what is in sim. Big thanks to mr_belowski for allowing me to use his project cars pit coordinates from CrewChief :)

## Known Issues
[Known Issues - Second Monitor](https://github.com/Winzarten/SecondMonitor/wiki/Known-Issues)
[Known Issues - Telemetry Viewer](https://github.com/Winzarten/SecondMonitor/wiki/Known-Issues-(Telemetry-Viewer))


## Future Plans   
 - [x] Telemetry - Allow the application to track and view advanced telemetry for individual laps.
 - [ ] Improve Telemetry Viewer
 - [ ] Client/Server - Ability to show second monitor on different computer than the simulator is running.
 - [ ] F1 2018 Support
 

## Installation

Check release tab for latests version
