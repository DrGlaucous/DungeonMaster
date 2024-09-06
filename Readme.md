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

We should have a series of pre-baked sequences made using software like xLight. The manager software should just send each file sequence to the arena controller on demand.

There should be 3 ESP32s in play:
- Computer dongle ESP32, hosts the network and sends/gets signals from each sub-part
- Judge panel, wireless panel that allows the judge to start/pause/stop the match, and time pins
- Battle box: Controls the player ready buttons + lights + potential arena hazards
- (we may need to split the arena control and light sequencer into 2 separate ESP32s)


In general everything will be controlled with special `G`-like code that sends commands to each of the sub-parts

Components: G specifier, tells the controller what command to send
```
//NOTE: WE MAY NOT USE THIS METHOD:
//I may just specify a range (of, say, 100) for each perepherial
G0 - Target perepherial, (gcodes sent after this will be forwared to the correct deivce), this code is always parsed by the dongle
0: Dongle
1: Box
2: Judge Controller
3+: Dongle (OOB)

//ranges?:
0-999: Dongle
1000-1999: Box
2000-2999: Judge controller


G200 M0 R25 G10 B255

G200: Set box lighting. sub-sequences seen below



```





DMX control for the lights may be too bandwidth intensive.
Alternative method:

Several hard-baked sequences in each light
- `M0` Set all color `R G B`
- `M1` Set address color (ignores if address is OOB) `A R G B`
- `M2` Set address range color (truncates if OOB) `A A R G B`
- `M3` Set transition speed `S` (affects all color change operations)
- `M4` Set rainbow mode (speed set above)
- 
- 




</details>
