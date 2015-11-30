// DeviceDownlinkBuffer.cs
// An object to represent wireless downlinks from the satellite hub to the devices.
// 
// 11/30/15
// -Wesley Chavez
//
// Program.cs instantiates three of these.  RequestEvents can use these to receive
// requested data.  If the device uplink buffer has latched two bytes, update will return 1. 0 otherwise.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatHub 
{

    class DeviceDownlinkBuffer
    {
        private int _id;
        private int _timeFor2Bytes;
	private int _transactionStartTime;
	private int _inUse;

        // DeviceDownlinkBuffer Constructor. Takes in the id number and transfer rate
        public DeviceDownlinkBuffer(int id, int timeFor2Bytes)
        {
            _id = id;
            _timeFor2Bytes = timeFor2Bytes;
	    _transactionStartTime = 0;
	    _inUse = 0;
        }
        public void startUsing (int transactionStartTime)
        {
            _transactionStartTime = transactionStartTime;
            _inUse = 1;
        }
	public void stopUsing ()
	{
	    _inUse = 0;
	}
	public int getTimeFor2Bytes ()
	{
	    return _timeFor2Bytes;
	}
	public int update (int currentTime)
	{
	    if ((currentTime - _transactionStartTime)%_timeFor2Bytes == 0 && currentTime != _transactionStartTime)
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
