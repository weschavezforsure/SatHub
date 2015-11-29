using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatHub 
{

    class SatelliteUplinkBuffer
    {
        private int _timeFor2Bytes;
        private int _latency;
        private int _sendTransactionStartTime;
        private int _requestTransactionStartTime;

	private int _inUse;
        // SatelliteUplinkBuffer Constructor. Takes in the id number and transfer rate
        public SatelliteUplinkBuffer(int timeFor2Bytes, int latency)
        {
            _timeFor2Bytes = timeFor2Bytes;
	    _latency = latency;
	    _sendTransactionStartTime = 0;
	    _inUse = 0;
        }
	public void startUsing (int sendTransactionStartTime)
	{
	    _sendTransactionStartTime = sendTransactionStartTime;
	    _inUse = 1;
	}
	public void startUsingRequest(int requestTransactionStartTime)
	{
	    _requestTransactionStartTime = requestTransactionStartTime;
	    _inUse = 1;
	}
	public void stopUsing ()
	{
	    _inUse = 0;
	}
	public int getTimeFor2Bytes()
	{
	    return _timeFor2Bytes;
	}
	public int getLatency ()
	{
	    return _latency;
	}
	public int getUsage ()
	{
	    return _inUse;
	}

	public int updateSend(int currentTime)
	{
	    if ((currentTime-_sendTransactionStartTime-_latency)%_timeFor2Bytes == 0)
	    {
		return 1;
	    }
	    else
	    {
		return 0;
	    }
	}
	public int updateRequest(int currentTime)
	{
	    if ((currentTime-_requestTransactionStartTime-_latency)%(2*_timeFor2Bytes) == 0)
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
