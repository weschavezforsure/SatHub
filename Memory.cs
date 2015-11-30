// Memory.cs
// An object to represent a memory space in the satellite hub.
// 
// 11/30/15
// -Wesley Chavez
//
// Program.cs instantiates a number of these.  SendEvents AND RequestEvents can use these to send data and to
// receive data from.  If the event is reading or writing the last 2 bytes, update will return 1. 0 otherwise.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatHub 
{
    class Memory
    {
        public int _numBytes;
	public int _trDataTag;
        public int _latency;
	public int _usage;
        public Memory(int numBytes, int latency)
        {
            _numBytes = numBytes;
            _latency = latency;
        }
        public void writeMemTag(int trDataTag)
        {
	    _trDataTag = trDataTag;
        }
        public int write2Bytes(int numBytesFinished)
        {
	    //Writing to an array is trivial. Abstraction for now.    
	    if (numBytesFinished == _numBytes - 2)
	    {	            
		return 1;	
	    }
	    else
	    {
	    	return 0;
	    }
        }
	public void use ()
	{
	    _usage = 1;
	}
	public void stopUsing()
	{
	    _usage = 0;
	}
        public int read2Bytes(int numBytesFinished)
        {
	    if (numBytesFinished == _numBytes - 2)
	    {	            
		return 1;	
	    }
	    else
	    {
	    	return 0;
	    }
	}
	public int getLatency()
	{
	    return _latency;
	}
        public bool compareTag(int trDataTag)
        {
	    if (_trDataTag == trDataTag)
	    {
		return true;
	    }
	    else
	    {
		return false;
	    }
	}
	
    }
}
