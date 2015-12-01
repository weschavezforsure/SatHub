// Program.cs
// Main class to keep track of and update Events in the satellite hub.
// 
// 11/30/15
// -Wesley Chavez
//
// Program.cs instantiates all the hardware, then loops through time to create and service events.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatHub 
{
    class Program
    {
	public static DeviceUplinkBuffer[] deviceUplinkBuffers;
	public static DeviceDownlinkBuffer[] deviceDownlinkBuffers;
	public static Memory[] memories;
	public static SendEvent[] sendEvents;
	public static RequestEvent[] requestEvents;
	public static CsvObj[,] csvObjs;
	public static SatelliteUplinkBuffer satelliteUplinkBuffer;
	public static SatelliteDownlinkBuffer satelliteDownlinkBuffer;


        static void Main(string[] args)
        {
	    DeviceUplinkBuffer[] deviceUplinkBuffers = new DeviceUplinkBuffer[3];
	    deviceDownlinkBuffers = new DeviceDownlinkBuffer[3];
            memories = new Memory[18];
	    satelliteUplinkBuffer = new SatelliteUplinkBuffer(1333344, 100);            
	    satelliteDownlinkBuffer = new SatelliteDownlinkBuffer(1333344, 150);            
	    sendEvents = new SendEvent[40];
	    requestEvents = new RequestEvent[40];

	    int[] csvTracker = new int[3];
	    int[,] latencies = new int[3,40];
	    int[] currentSendEvents = new int[3];  
	    int[] currentRequestEvents = new int[3];  
	    int tCurrentClock = 0;
	    int[] wait = new int[3];
	    string[] currentEventOperations = new string[3];  

	    int i;
	    int finished;
	    int j;	
	    int m;


	    for (i = 0; i < 3; i++)
	    {
		csvTracker[i] = 0;
		wait[i] = 0;
		deviceUplinkBuffers[i] = new DeviceUplinkBuffer(i,304);
		deviceDownlinkBuffers[i] = new DeviceDownlinkBuffer(i,304);
	    }	    


            for(i = 0; i < 4; i++)
            {
                memories[i] = new Memory(128, 1);
            }
            for(i = 4; i < 8; i++)
            {
                memories[i] = new Memory(512, 8);
            }
            for(i = 8; i < 18; i++)
            {
                memories[i] = new Memory(1024, 15);
            }

	    	    
	    CsvObj[,] csvObjs = new CsvObj[3,15];
		csvObjs[0,0] = new CsvObj(0,1,"SEND",128,1);
		csvObjs[0,1] = new CsvObj(150,1,"SEND",1024,4);
		csvObjs[0,2] = new CsvObj(210,1,"REQUEST",128,2);
		csvObjs[0,3] = new CsvObj(1000,1,"REQUEST",128,3);
		csvObjs[0,4] = new CsvObj(12000,1,"REQUEST",1024,10);
		csvObjs[1,0] = new CsvObj(50,2,"SEND",128,2);
		csvObjs[1,1] = new CsvObj(200,2,"REQUEST",128,1);
		csvObjs[1,2] = new CsvObj(1100,2,"REQUEST",128,3);
		csvObjs[1,3] = new CsvObj(11500,2,"REQUEST",1024,12);
		csvObjs[2,0] = new CsvObj(100,3,"SEND",128,3);
		csvObjs[2,1] = new CsvObj(300,3,"SEND",1024,5);
		csvObjs[2,2] = new CsvObj(400,3,"SEND",1024,6);
		csvObjs[2,3] = new CsvObj(500,3,"SEND",512,7);
		csvObjs[2,4] = new CsvObj(2000,3,"SEND",1024,8);
		csvObjs[2,5] = new CsvObj(3000,3,"SEND",1024,9);
		csvObjs[2,6] = new CsvObj(4000,3,"SEND",1024,10);
		csvObjs[2,7] = new CsvObj(5000,3,"SEND",1024,11);
		csvObjs[2,8] = new CsvObj(6000,3,"SEND",1024,12);
		csvObjs[2,9] = new CsvObj(7000,3,"SEND",1024,13);
		csvObjs[2,10] = new CsvObj(8000,3,"SEND",1024,14);
		csvObjs[2,11] = new CsvObj(9000,3,"SEND",1024,15);
		csvObjs[2,12] = new CsvObj(10000,3,"SEND",1024,16);
		csvObjs[2,13] = new CsvObj(11000,3,"SEND",1024,17);
		csvObjs[2,14] = new CsvObj(12100,3,"REQUEST",512,7);
            while (tCurrentClock < 5000000000)
            {
		for(i = 0; i < 3; i++)
                {
		    finished = 0;
		    if (tCurrentClock == 0)
		    {
			currentEventOperations[i] = csvObjs[i,0]._operation;
			j = whichMemoryToSend (csvObjs[i,0]._size, memories);
			sendEvents[i] = new SendEvent(csvObjs[i,csvTracker[i]]._csvTime, csvObjs[i,csvTracker[i]]._size, csvObjs[i,csvTracker[i]]._trDataTag, i, false, j);
			memories[j].startUsing();
		    }
		    if (wait[i] == 1)
		    {
			if (satelliteUplinkBuffer._inUse == 0)
			{
			    if (csvObjs[i,csvTracker[i]]._operation == "SEND")
			    {
		    		currentEventOperations[i] = "SEND";
				j = whichMemoryToEvict(csvObjs[i,csvTracker[i]]._size);
				sendEvents[i] = new SendEvent(csvObjs[i,csvTracker[i]]._csvTime, csvObjs[i,csvTracker[i]]._size, csvObjs[i,csvTracker[i]]._trDataTag, i, true, j);
				memories[j].startUsing();
				satelliteUplinkBuffer.startUsing();
			    }
			    else
			    {
		    		currentEventOperations[i] = "REQUEST";
				requestEvents[i] = new RequestEvent(csvObjs[i,csvTracker[i]]._csvTime, csvObjs[i,csvTracker[i]]._size, csvObjs[i,csvTracker[i]]._trDataTag, i, false, -1);
				satelliteUplinkBuffer.startUsing();
				satelliteDownlinkBuffer.startUsing();
			    }
			    wait[i] = 0;
			    continue;
			}
			else
			{
			    continue;	
			}
		    }
		    if (currentEventOperations[i] == "SEND")
		    {
			if (sendEvents[i]._evicting == true)
			{
			    finished = sendEvents[i].updateSendEvent(tCurrentClock, deviceUplinkBuffers[i], memories[sendEvents[i]._memoryDestination], satelliteUplinkBuffer);
		    	}
		    	else
		    	{
			    finished = sendEvents[i].updateSendEvent(tCurrentClock, deviceUplinkBuffers[i], memories[sendEvents[i]._memoryDestination]);
		    	}
		   
		    }
		    else
		    {
			if(requestEvents[i]._inMemory)
			{	
			    finished = requestEvents[i].updateRequestEvent(tCurrentClock, deviceDownlinkBuffers[i], memories[requestEvents[i]._memoryDesignation]);
			}
			else
			{
			    finished = requestEvents[i].updateRequestEvent(tCurrentClock, deviceDownlinkBuffers[i], satelliteUplinkBuffer, satelliteDownlinkBuffer);
			}
		    }
		    if (finished == 1)
		    {
			    if (currentEventOperations[i] == "SEND")
			    {
				if (sendEvents[i]._evicting == true)
				{
				    satelliteUplinkBuffer.stopUsing();
				}
				memories[sendEvents[i]._memoryDestination].stopUsing();
			    }
			    else
			    {
				if (requestEvents[i]._inMemory)
				{
				    memories[requestEvents[i]._memoryDesignation].stopUsing();
				}	
				else
				{
				    satelliteUplinkBuffer.stopUsing();
				    satelliteDownlinkBuffer.stopUsing();
				}
			    }
			    latencies[i,csvTracker[i]] = tCurrentClock - csvObjs[i,csvTracker[i]]._csvTime;
	    	    	    System.Console.WriteLine(tCurrentClock);
	    	    	    System.Console.WriteLine("-----------");
	    	    	    System.Console.WriteLine(latencies[0,0]);
	    	    	    System.Console.WriteLine(latencies[0,1]);
	    	    	    System.Console.WriteLine(latencies[0,2]);
	    	    	    System.Console.WriteLine(latencies[0,3]);
	    	    	    System.Console.WriteLine(latencies[0,4]);
	    	    	    System.Console.WriteLine("-----------");
	    	    	    System.Console.WriteLine(latencies[1,0]);
	    	    	    System.Console.WriteLine(latencies[1,1]);
	    	    	    System.Console.WriteLine(latencies[1,2]);
	    	    	    System.Console.WriteLine(latencies[1,3]);
	    	    	    System.Console.WriteLine("-----------");
	    	    	    System.Console.WriteLine(latencies[2,0]);
	    	    	    System.Console.WriteLine(latencies[2,1]);
	    	    	    System.Console.WriteLine(latencies[2,2]);
	    	    	    System.Console.WriteLine(latencies[2,3]);
	    	    	    System.Console.WriteLine(latencies[2,4]);
	    	    	    System.Console.WriteLine(latencies[2,5]);
	    	    	    System.Console.WriteLine(latencies[2,6]);
	    	    	    System.Console.WriteLine(latencies[2,7]);
	    	    	    System.Console.WriteLine(latencies[2,8]);
	    	    	    System.Console.WriteLine(latencies[2,9]);
	    	    	    System.Console.WriteLine(latencies[2,10]);
	    	    	    System.Console.WriteLine(latencies[2,11]);
	    	    	    System.Console.WriteLine(latencies[2,12]);
	    	    	    System.Console.WriteLine(latencies[2,13]);
	    	    	    System.Console.WriteLine(latencies[2,14]);
			    csvTracker[i] += 1;
			    if (csvObjs[i,csvTracker[i]]._operation == "SEND")
			    {
				currentEventOperations[i] = "SEND";
				j = whichMemoryToSend(csvObjs[i,csvTracker[i]]._size, memories);
				if (j == -1)
				{
				    j = whichMemoryToEvict(csvObjs[i,csvTracker[i]]._size);
				    wait[i] = satelliteUplinkBuffer._inUse;
				    if (wait[i] == 0)
				    {
				    	sendEvents[i] = new SendEvent(csvObjs[i,csvTracker[i]]._csvTime, csvObjs[i,csvTracker[i]]._size, csvObjs[i,csvTracker[i]]._trDataTag, i, true, j);
					memories[j].startUsing();
					satelliteUplinkBuffer.startUsing();
				    }
				    else
				    {
					continue;
				    }	
				}
				else
				{
				    sendEvents[i] = new SendEvent(csvObjs[i,csvTracker[i]]._csvTime, csvObjs[i,csvTracker[i]]._size, csvObjs[i,csvTracker[i]]._trDataTag, i, false, j);
				    memories[j].startUsing();
				}
			    }
			    else
			    {
				currentEventOperations[i] = "REQUEST";
				m = checkTags(csvObjs[i,csvTracker[i]]._trDataTag, memories);
				if (m == -1)
				{
				    wait[i] = satelliteUplinkBuffer._inUse;
				    if (wait[i] == 0)
				    {
				    	requestEvents[i] = new RequestEvent(csvObjs[i,csvTracker[i]]._csvTime, csvObjs[i,csvTracker[i]]._size, csvObjs[i,csvTracker[i]]._trDataTag, i, false, -1);
					satelliteUplinkBuffer.startUsing();
					satelliteDownlinkBuffer.startUsing();
				    }
				}
				else
				{
				    requestEvents[i] = new RequestEvent(csvObjs[i,csvTracker[i]]._csvTime, csvObjs[i,csvTracker[i]]._size, csvObjs[i,csvTracker[i]]._trDataTag, i, true, m);
				    memories[m].startUsing();
				}				
			    }
		    } 
                }
                tCurrentClock++;
            }
            
        }

	private static int checkTags (int tag, Memory[] memory)
	{
	    int i;
	    for (i = 0;i < 18; i++)
	    {
		if (memory[i]._trDataTag == tag)
		{
		    return i;
		}
	    }
	    return -1;
	}

	private static int whichMemoryToSend (int size, Memory[] memory)
	{
	    int i;
	    if (size==128)
	    {
		for (i = 0;i < 4;i++)
		{
		    if (memory[i]._trDataTag == 0 && memory[i]._usage == 0)
		    {
			return i;
		    }
		}
		return -1;
	    }	
	    if (size==512)
	    {
		for (i = 4;i < 8;i++)
		{
		    if (memory[i]._trDataTag == 0 && memory[i]._usage == 0)
		    {
			return i;
		    }
		}
		return -1;
	    }	
	    if (size==1024)
	    {
		for (i = 8;i < 18;i++)
		{
		    if (memory[i]._trDataTag == 0 && memory[i]._usage == 0)
		    {
			return i;
		    }
		}
		return -1;
	    }
	    return -2;	
	}
	
	private static int whichMemoryToEvict (int size)
	{
	    if (size==128)
	    {
		return 0;
	    }	
	    if (size==512)
	    {
		return 4;
	    }	
	    if (size==1024)
	    {
		return 8;
	    }	
	    return -2;	    
	}
    }
}
