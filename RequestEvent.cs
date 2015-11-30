using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatHub 
{
    
    class RequestEvent
    {   
        private int _tStartTime;
        private int _transactionSize; 
        private int _trDataTag;
	private int _deviceDownlinkBuffer;
	private int _numBytesFinished;
	public bool _inMemory;
	public int _memoryDesignation;
        public int _finishTime;
        private int _firstSendToDeviceTime;
	private int _waitingForSatellite;
	public RequestEvent(int startTime, int transactionSize, int trDataTag, int deviceDownlinkBuffer, bool inMemory, int memoryDesignation)
        {
            _tStartTime = startTime;
            _transactionSize = transactionSize;
            _trDataTag = trDataTag;
	    _deviceDownlinkBuffer = deviceDownlinkBuffer;
	    _inMemory = inMemory;
	    _memoryDesignation = memoryDesignation;
	    _numBytesFinished = 0;
	    _finishTime = 0;
	    _firstSendToDeviceTime = -1;
	    _waitingForSatellite = 0;
	    
        }
	public int updateRequestEvent(int currentTime, DeviceDownlinkBuffer deviceDownlinkBuffer, Memory memory )
	{
	    int finished = 0;
	    if (currentTime == _finishTime)
	    {
		memory.stopUsing();
		finished = 1;
		return finished;
	    }
	    if (currentTime == _tStartTime)
            {
		memory.use();
		int dontcare;
		dontcare = memory.read2Bytes(_numBytesFinished);	
		_firstSendToDeviceTime = currentTime + memory.getLatency();
		return finished;
            }
	    if (currentTime < _firstSendToDeviceTime)
	    {
		return finished;
	    } 
	    if (currentTime == _firstSendToDeviceTime)
	    {
		deviceDownlinkBuffer.startUsing(currentTime);
		return finished;
	    }
            int latched2BytesToDeviceDownlink;
	    latched2BytesToDeviceDownlink = deviceDownlinkBuffer.update(currentTime);
            if (latched2BytesToDeviceDownlink == 1)
            {
                int last2Bytes;
                last2Bytes = memory.read2Bytes(_numBytesFinished);
                if (last2Bytes == 1)
                {
                    _finishTime = currentTime + deviceDownlinkBuffer.getTimeFor2Bytes();
		}
                _numBytesFinished += 2;
	    }
	    return finished;
    	}
	public int updateRequestEvent(int currentTime, DeviceDownlinkBuffer deviceDownlinkBuffer, SatelliteUplinkBuffer satelliteUplinkBuffer, SatelliteDownlinkBuffer satelliteDownlinkBuffer)
	{
	    int finished = 0;
	    if (currentTime == _finishTime)
	    {
		finished = 1;
		return finished;
	    }
	    if (currentTime == _tStartTime)
            {
		satelliteUplinkBuffer.startUsingRequest(currentTime);	
		_waitingForSatellite = 1;
		return finished;
            }
	    if (_waitingForSatellite == 1)
	    {
		if (satelliteUplinkBuffer.updateRequest(currentTime) == 1)
		{
		    satelliteDownlinkBuffer.startSending(currentTime);
		    _waitingForSatellite = 0;
		}
		return finished;
	    }
	    else
	    {
        	int latched2BytesToDeviceDownlink;
		latched2BytesToDeviceDownlink = satelliteDownlinkBuffer.update(currentTime);
        	if (latched2BytesToDeviceDownlink == 1)
        	{
            	    if (_numBytesFinished == _transactionSize - 2)
                    {
                    	_finishTime = currentTime + deviceDownlinkBuffer.getTimeFor2Bytes();
		    }
		    _numBytesFinished += 2;
	        }
	    	return finished;
	    }
	    

	}
    }
}
