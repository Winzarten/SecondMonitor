import ac
import acsys
import sys
import os.path
import platform
import configparser

import ctypes
from ctypes import *

from smshared_mem import SecondMonitorShared

sharedMem = SecondMonitorShared()

pluginVersion = "1.0.0"

timer = 0


def updateSharedMemory():
    global sharedMem
    sharedmem = sharedMem.getsharedmem()
    sharedmem.numVehicles = ac.getCarsCount()
    sharedmem.focusVehicle = ac.getFocusedCar()
    #now we'll build the slots, so we later know every single (possible) car
    carIds = range(0, ac.getCarsCount(), 1)
    for carId in carIds:
        #first we'll check wether there is a car for this id; as soon it returns -1
        #it's over
        if str(ac.getCarName(carId)) == '-1':
            break
        else:
            sharedmem.vehicleInfo[carId].carId = carId
            sharedmem.vehicleInfo[carId].driverName = ac.getDriverName(carId).encode('utf-8')
            sharedmem.vehicleInfo[carId].carModel = ac.getCarName(carId).encode('utf-8')
            sharedmem.vehicleInfo[carId].speedMS = ac.getCarState(carId, acsys.CS.SpeedMS)
            sharedmem.vehicleInfo[carId].bestLapMS = ac.getCarState(carId, acsys.CS.BestLap)
            sharedmem.vehicleInfo[carId].lapCount = ac.getCarState(carId, acsys.CS.LapCount)
            sharedmem.vehicleInfo[carId].currentLapInvalid = ac.getCarState(carId, acsys.CS.LapInvalidated)
            sharedmem.vehicleInfo[carId].currentLapTimeMS = ac.getCarState(carId, acsys.CS.LapTime)
            sharedmem.vehicleInfo[carId].lastLapTimeMS = ac.getCarState(carId, acsys.CS.LastLap)
            sharedmem.vehicleInfo[carId].worldPosition = ac.getCarState(carId, acsys.CS.WorldPosition)
            sharedmem.vehicleInfo[carId].isCarInPitline = ac.isCarInPitline(carId)
            sharedmem.vehicleInfo[carId].isCarInPit = ac.isCarInPit(carId)
            sharedmem.vehicleInfo[carId].carLeaderboardPosition = ac.getCarLeaderboardPosition(carId)
            sharedmem.vehicleInfo[carId].carRealTimeLeaderboardPosition = ac.getCarRealTimeLeaderboardPosition(carId)
            sharedmem.vehicleInfo[carId].spLineLength = ac.getCarState(carId, acsys.CS.NormalizedSplinePosition)
            sharedmem.vehicleInfo[carId].isConnected = ac.isConnected(carId)
            if carId == '0':
                sharedmem.vehicleInfo[carId].lastSplits[0] = ac.getLastSplits(carId)[0]
                sharedmem.vehicleInfo[carId].lastSplits[1] = ac.getLastSplits(carId)[1]
                sharedmem.vehicleInfo[carId].lastSplits[2] = ac.getLastSplits(carId)[2]
                sharedmem.vehicleInfo[carId].currentSplits[0] = ac.getCurrentSplits(carId)[0]
                sharedmem.vehicleInfo[carId].currentSplits[1] = ac.getCurrentSplits(carId)[1]
                sharedmem.vehicleInfo[carId].currentSplits[2] = ac.getCurrentSplits(carId)[2]

def acMain(ac_version):
  global appWindow,sharedMem

  appWindow = ac.newApp("SecondMonitorEx")

  ac.setTitle(appWindow, "SecondMonitorEx")
  ac.setSize(appWindow, 300, 40)

  ac.log("SecondMonitor Shared memory Initialized")
  ac.console("SecondMonitor Shared memory Initialized")

  sharedmem = sharedMem.getsharedmem()
  sharedmem.serverName = ac.getServerName().encode('utf-8')
  sharedmem.acInstallPath = os.path.abspath(os.curdir).encode('utf-8')
  sharedmem.pluginVersion = pluginVersion.encode('utf-8')
  return "SecondMonitorEx"

def acUpdate(deltaT):
  global timer
  timer += deltaT
  if timer > 0.025:
      updateSharedMemory()
      timer = 0


