# BatchPatchMaintenanceMode
Control Windows Services with BatchPatch


This is a quick and dirty .NET4.5 application for controlling Windows services.  It is called from BatchPatch and returns an exit code of 0 if everything is fine, or an exit code of 1 if something does not work. BatchPatch monitors the exit codes and can change the job queue to abort, change a color, or do something else. 

This is used to shut down all application services cleanly (and report if any failed) before patching and reboot. It is also used to startup all related services and report on any failures. The compiled exe is designed to be run from BatchPatch and takes no arguments and looks for hard coded file paths. There is no confirm prompt.

# MaintModeOn Operation
When MaintModeOn.exe is run, it runs down a list of service names in a text file (hard coded to c:\batch\service_list_mm_on.txt), one service name per line and stops then disables the windows service in that order. It reports in the Windows Event Viewer on it’s status. If any fail to shutdown or do not exist, it returns with an exit code of 1. If all good, it reports an exit code of 0. 

# MaintModeOff Operation
When MaintModeOff.exe is run, it runs down a list of service names in a text file (hard coded to c:\batch\service_list_mm_off.txt), one service name per line and sets them to auto startup, then starts each service, in the order listed. It reports in the Windows Event Viewer on it’s status. If any fail to startup or do not exist, it returns with an exit code of 1. If all good, it reports an exit code of 0. 

# Install
Source code is provided as the path will likely need to be changed. It is designed to be run from c:\batch\. It’s compiled with Visual Studio 2019 using .NET4.5 settings. Tested on Windows Server 2008 R2 to 2019.  Place the two exe files and the two txt files into c:\batch\. Edit the txt files and add services name. Then run the exe file and watch it work. Check Event Viewer for status. 

# BatchPatch Job queue
Examples of BatchPatch Job Queues are below. These will change the row red on any failures, and green if everything shutdown or started up correctly. 
![image](https://user-images.githubusercontent.com/107140997/172726245-dccd880e-5129-4efb-97b1-4355c917af9a.png)
