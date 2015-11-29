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
