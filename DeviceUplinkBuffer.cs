// DeviceUplinkBuffer.cs
// An object to represent wireless uplinks from devices to the satellite hub.
// 
// 11/30/15
// -Wesley Chavez
//
// Program.cs instantiates three of these, then sendEvents call their update method.
// If the device uplink buffer has latched two bytes, update will return 1. 0 otherwise.
// To facilitate a simpler method for sending data to the satellite, sendEvents can
// change the transfer rate to that of the satellite uplink in order to evict and replace memory
// at the same speed.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatHub 
{

    class DeviceUplinkBuffer
    {
        private int _id;
        private int _timeFor2Bytes;
	private int _t_changeSpeed;

        // DeviceUplinkBuffer Constructor. Takes in the id number and transfer rate
        public DeviceUplinkBuffer(int id, int timeFor2Bytes)
        {
            _id = id;
            _timeFor2Bytes = timeFor2Bytes;
	    _t_changeSpeed = 0;
        }
	public void setTimeFor2Bytes (int currentTime, int timeFor2Bytes)
	{
	    _timeFor2Bytes = timeFor2Bytes;
	    _t_changeSpeed = currentTime;
	}
	public int update (int currentTime)
	{
	    if ((currentTime - _t_changeSpeed)%_timeFor2Bytes == 0 && currentTime != 0)
	    {
		return 1;
	    }
	    else
	    {
		return 0;
	    }
	}
    }
}
