using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatHub 
{
    class CsvObj
    {
        public int _csvTime;
        public int _device;
        public string _operation;
	public int _size;
	public int _trDataTag;

        // CsvObj Constructor. Takes in the id number and transfer rate
        public CsvObj(int csvTime, int device, string operation, int size, int trDataTag)
        {
            _csvTime = csvTime;
            _device = device;
	    _operation = operation;
	    _size = size;
	    _trDataTag = trDataTag;
        }
    }
}
