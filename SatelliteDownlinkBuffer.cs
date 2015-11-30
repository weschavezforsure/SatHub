// SatelliteDownlinkBuffer.cs
// An object to represent a downlink from the satellite/data center to the satellite hub.
// 
// 11/30/15
// -Wesley Chavez
//
// Program.cs instantiates one of these.  RequestEvents can use these to receive data from
// the satellite/data center.  If the satellite uplink buffer has sent two bytes, update will return 1. 0 otherwise.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatHub 
{

    class SatelliteDownlinkBuffer
    {
        private int _timeFor2Bytes;
	private int _latency;
	private int _inUse;
	private int _startTransactionTime;
        // SatelliteDownlinkBuffer Constructor. Takes in the id number and transfer rate
        public SatelliteDownlinkBuffer(int timeFor2Bytes, int latency)
        {
            _timeFor2Bytes = timeFor2Bytes;
	    _latency = latency;
	    _inUse = 0;
	    _startTransactionTime = 0;
        }
	public void startUsing ()
	{
	    _inUse = 1;
	}
	public void stopUsing ()
	{
	    _inUse = 0;
	}
	public void startSending (int currentTime)
	{
	    _startTransactionTime = currentTime;
	}
	public int update (int currentTime)
	{
	    if ((currentTime-_startTransactionTime-_latency)%_timeFor2Bytes == 0 && currentTime != (_startTransactionTime - _latency))
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
