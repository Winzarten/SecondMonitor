"""
SecondMonitor SharedMemory
No licence apply.
Based on SharedMemory from CrewChief - https://github.com/mrbelowski/CrewChiefV4
"""
import mmap
import os
import struct
import functools
import ctypes
from ctypes import c_float, c_char, c_int32

class vec3(ctypes.Structure):
    _pack_ = 4
    _fields_ = [
        ('x', c_float),
        ('y', c_float),
        ('z', c_float),
        ]

class acsVehicleInfo(ctypes.Structure):
    _pack_ = 4
    _fields_ = [
        ('carId', c_int32),
        ('driverName', c_char * 64),
        ('carModel', c_char * 64),
        ('speedMS', c_float),
        ('bestLapMS', c_int32),
        ('lapCount', c_int32),
        ('currentLapInvalid', c_int32),
        ('currentLapTimeMS', c_int32),
        ('lastLapTimeMS', c_int32),
        ('worldPosition', vec3),
        ('isCarInPitline', c_int32),
        ('isCarInPit', c_int32  ),
        ('carLeaderboardPosition', c_int32),
        ('carRealTimeLeaderboardPosition', c_int32),
        ('spLineLength', c_float),
        ('isConnected', c_int32),
        ('finishStatus', c_int32),
    ]

class SPageFilesecondMonitor(ctypes.Structure):
    _pack_ = 4
    _fields_ = [
        ('numVehicles', c_int32),
        ('focusVehicle', c_int32),
        ('serverName', c_char * 512),
        ('vehicleInfo', acsVehicleInfo * 64),
        ('acInstallPath', c_char * 512),
        ('pluginVersion', c_char * 32)
    ]

class SecondMonitorShared:
    def __init__(self):
        self._acpmf_secondMonitor = mmap.mmap(0, ctypes.sizeof(SPageFilesecondMonitor),"acpmf_secondMonitor")
        self.secondMonitor = SPageFilesecondMonitor.from_buffer(self._acpmf_secondMonitor)

    def close(self):
        self._acpmf_secondMonitor.close()

    def __del__(self):
        self.close()

    def getsharedmem(self):
        return self.secondMonitor

