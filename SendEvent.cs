using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatHub 
{
    
    class SendEvent
    {   
        private int _tStartTime;
        private double _transactionSize; 
        private int _trDataTag;
	
	public int _deviceUplinkBuffer;
	public bool _evicting;
	public int _memoryDestination;
	private bool _uplinkBeingUsed;
	private int _numBytesFinished;
	private int _finishTime;
	private int _firstSendToSatTime;

        public SendEvent(int startTime, double transactionSize, int trDataTag, int deviceUplinkBuffer, bool evicting, int memoryDestination)
        {
            _tStartTime = startTime;
            _transactionSize = transactionSize;
            _trDataTag = trDataTag;
	    _deviceUplinkBuffer = deviceUplinkBuffer;
	    _evicting = evicting;
	    _memoryDestination = memoryDestination;
	    
	    _uplinkBeingUsed = true;
	    _numBytesFinished = 0;
	    _finishTime = -1;
	    _firstSendToSatTime = -1;
        }
	public int updateSendEvent(int currentTime, DeviceUplinkBuffer deviceUplinkBuffer, Memory memory)
	{
	    int finished = 0;
	    if (currentTime == _tStartTime)
	    {
        	memory.writeMemTag(_trDataTag);
		memory.use();
		return finished;
	    }
	    if (_finishTime == currentTime)
	    {
		memory.stopUsing();
		finished = 1;
		return finished;
	    }

	    int latched2BytesFromDeviceUplink;
	    latched2BytesFromDeviceUplink = deviceUplinkBuffer.update(currentTime);
	    if (latched2BytesFromDeviceUplink == 1)
	    {
		int last2Bytes;
		last2Bytes = memory.write2Bytes(_numBytesFinished);
		if (last2Bytes == 1)
		{
		    _uplinkBeingUsed = false;
		    _finishTime = currentTime + memory.getLatency();
		}
		_numBytesFinished += 2;
	    }
	    return finished;

	}    
	public int updateSendEvent(int currentTime, DeviceUplinkBuffer deviceUplinkBuffer, Memory memory, SatelliteUplinkBuffer satelliteUplinkBuffer)
	{
	    int finished = 0;
	    if (currentTime == _tStartTime)
	    {
		memory.use();
        	memory.writeMemTag(_trDataTag);
	        deviceUplinkBuffer.setTimeFor2Bytes(currentTime, satelliteUplinkBuffer.getTimeFor2Bytes());
		int dontcare;
		dontcare = memory.read2Bytes(_numBytesFinished);	
		_firstSendToSatTime = currentTime + memory.getLatency();
		return finished;
	    }
	    if (currentTime == _finishTime)
	    {
		deviceUplinkBuffer.setTimeFor2Bytes(currentTime, 291);
		memory.stopUsing();
		finished = 1;
		return finished;
	    }
	    if (currentTime < _firstSendToSatTime)
	    {
		return finished;
	    }
	    
	    int latched2BytesFromDeviceUplink;
	    latched2BytesFromDeviceUplink = deviceUplinkBuffer.update(currentTime);
	    if (latched2BytesFromDeviceUplink == 1)
	    {
		int last2Bytes;
		last2Bytes = memory.read2Bytes(_numBytesFinished);
		//Latch to SatelliteUplinkBuffer?
		last2Bytes = memory.write2Bytes(_numBytesFinished);
		if (last2Bytes == 1)
		{
		    _uplinkBeingUsed = false;
		    _finishTime = currentTime + memory.getLatency() + satelliteUplinkBuffer.getLatency();
		}
		_numBytesFinished += 2;
	    }
	    return finished;
	} 
    }
}
