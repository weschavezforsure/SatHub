# SatHub

Events handle uplinks, downlinks, and memories and return 1 when their update method is called and the transaction is finished.
The main [] function creates and deletes new events, decides where to put them based on "_inUse" of all the hardware components (objects), compares tags, chooses which memory location to store to and evict from, and saves latencies.

DeviceUplinkBuffers are pretty straightforward, and I decided we should change the speed of the device uplink in order to evict and replace a memory at the satellite's rate.  

DeviceDownlink Buffers are for requests, and return 1 when their update method is called and the device has latched in 16 bits.

Memory: the method _write2Bytes is called when 2 bytes is being written to and returns 1 if it is the last 2 bytes being written to.  The event adds latency for the final write, since that's the only one that really matters (links are so slow).

SatelliteUplinkBuffers update based on requests or send events.  Request events send 32 bits to the satellite, some latency is added, then the satellite downlink sends the packet.

SatelliteDownlinkBuffers update based on request events and the update returns if 2 bytes have been sent to our hub.

CsvObjs are because I am lazy and didn't want to write a parser myself.

Send Event update functions are overloaded based on if we're simply sending to memory, or we have to evict & replace.  The update functions return 1 if they are finished.
Request Event update functions are overloaded based on if the data is in memory or if the data needs to be sent to us via satellite.  The update functions return 1 if they are finished.
