# Dungeon Master

This repo contains the software and hardware needed to control a simple fighting robot arena, or "Battle box".

Planned features:
- [ ] lighting control
- [ ] "start" buttons for each team
- [ ] wireless "judge" station
- [ ] stream overlays
- [ ] scoreboard


<details>
<summary style="font-size:80%;"><i><b>Notes for me</b></i></summary>


lighting control should be modular.

We should have a series of pre-baked sequences made using software like xLight. The manager software should just send each file sequence to the arena controller on demand. (this may be too bandwidth intensive, so we may just have a series of pre-baked instructions inside the light manager that we can hit from the outside, see TSC)

There should be 3 ESP32s in play:
- Computer dongle ESP32, hosts the network and sends/gets signals from each sub-part
- Judge panel, wireless panel that allows the judge to start/pause/stop the match, and time pins
- Battle box: Controls the player ready buttons + lights + potential arena hazards
- (we may need to split the arena control and light sequencer into 2 separate ESP32s)


---

In general everything will be controlled with special `TSC`-like code that sends commands to each of the sub-parts

Arguments are implemented just like CS-TSC, including OOB access
Commands start with a `<` character. If a `<` is gotten before a command ends, it will be parsed as part of that command.
Command arguments are split by one wildcard character, but the common convention is with `:`.



```

<TGTXXXX - Target perepherial XXXX, (any commands sent after this will be forwared to the correct deivce), this code is always parsed by the dongle (this is the device's UUID, NOT type)

0: Dongle
1: Box
2: Judge Controller
3+: Dongle (OOB)



<SLTXXXX:YYYY:ZZZZ Set Lighting Targets. Tells the target what lights to address
XXXX
0: All YYYY and ZZZZ are ignored
1: Address single: YYYY, ZZZZ is ignored
2: Address range from YYYY to ZZZZ


<STSXXXX:YYYY Set light Transition speed, effects will play at this speed (taking X time to loop)
XXXX: Time in seconds
YYYY: Time in milliseconds
(total is the sum of these two parts)


<SLCXXXX:YYYY:ZZZZ Set Light Color to RGB (range for each is 0-255, values larger or smaller than this will be clamped)
XXXX: R
YYYY: G
ZZZZ: B

<SLR Set Light Rainbow, no arguments, speed set above

<SLSXXXX:YYYY:ZZZZ:AAAA:BBBB:CCCC Set Light Strobe between 2 colors, speed determined by the speed settings above
XXXX YYYY ZZZZ: RGB of first color
AAAA BBBB CCCC: RGB of second color


<WAIXXXX:YYYY WAIt xxxx seconds and yyyy milliseconds before sending the next command (not actually sent, but is used by the desktop program)


<PSH Push next command to command stack
<POP return to calling command




```

All commands can be sent to all devices,
if a command is invalid for a specific device, it will simply be ignored

When a file is loaded, TSC execution is started at "event" #0000

Other events can be called from `<PSH` and `<POP` functions


Script command `<WAI`is not sent to the perepherials; it is used by the desktop app *only*

Script command `<TGT` is handled by the base station ONLY!


---

Response packets
```
response packets are in plaintext, to be printed to the terminal window, each part of the response packet is delimited with ":"

cccc - device type
cccc - device id
cccc - response type
c - \0 - response data


device types:
dngl - dongle
valid responses:
Ok //command was gotten successfully


judg - judge remote
valid responses:
X O //button ID [x] is ON
X F //button ID [x] is OFF
Ok //command was gotten successfully

boxx - box
X O //button ID [x] is ON
X F //button ID [x] is OFF
Ok //command was gotten successfully


Button IDs:
0000 - Start match button
0001 - Pause/play button
0002 - End match button
0003 - Hold for pin
0004 - RED wins
0005 - BLUE wins
0006 - RED ready
0007 - BLUE ready
0008 - arena door is open


example of 2 response packets
judg:0003:0007 O
boxx:0002:0006 F
boxx:0002:Ok


```




---





</details>
