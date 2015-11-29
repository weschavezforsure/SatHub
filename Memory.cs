using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatHub 
{
    class Memory
    {
        private int _numBytes;
	public int _trDataTag;
        private int _latecy;
	public int _usage;
        public Memory(int numBytes, int latency)
        {
            _numBytes = numBytes;
            _latecy = latency;
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
