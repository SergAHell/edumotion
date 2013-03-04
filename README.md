edumotion
=========

Education in motion

EEG Background Info
===================
From wikipedia: http://en.wikipedia.org/wiki/Electroencephalography

Alpha is the frequency range from 8 Hz to 12 Hz. Hans Berger named the first rhythmic EEG activity he saw as the "alpha wave". This was the "posterior basic rhythm" (also called the "posterior dominant rhythm" or the "posterior alpha rhythm"), seen in the posterior regions of the head on both sides, higher in amplitude on the dominant side. It emerges with closing of the eyes and with relaxation, and attenuates with eye opening or mental exertion. The posterior basic rhythm is actually slower than 8 Hz in young children (therefore technically in the theta range). 

Beta is the frequency range from 12 Hz to about 30 Hz. It is seen usually on both sides in symmetrical distribution and is most evident frontally. Beta activity is closely linked to motor behavior and is generally attenuated during active movements.[44] Low amplitude beta with multiple and varying frequencies is often associated with active, busy or anxious thinking and active concentration. Rhythmic beta with a dominant set of frequencies is associated with various pathologies and drug effects, especially benzodiazepines. It may be absent or reduced in areas of cortical damage. It is the dominant rhythm in patients who are alert or anxious or who have their eyes open.


band pass filter eliminates drift
dc offset (500)
baseline drift
power spectrum
eliminate jitter

measure amplitude

Plan
====
Connect ANT+ device messages to Somaxis-Unity API
Build SomaxisAPI for Unity

API Design
==========

SomaxisAPI
----------
isConnected
isCalibrated
onCalibrated(function)
getSampleRate() return 320;
getSamplePoint
getAverage
_filter

enum FilterBands
 Alpha 
  min: 8
  max: 12
 Beta
  min: 13
  max 30


getAlphaSample
getBetaSample
getABRatio(int seconds)


Reading
ReadingListener
Tracking



13 2 1