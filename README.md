FIX_BROKEN_FTDI_PID
===================

little utility that allows you to set the device PID back to what ever you want it to be (ie. 6001 for the FT232R)

SoooooooO!!!!!.. You have to do a few little things before this will work.


1. you need first go to FTDI's website, and download the older 2.10- driver version. 

2. plug in your borked FTDI and go into device manager and right click on it and UNINSTALL (not disable)
  - And yes we want it to delete the drivers as well.
  - then unplug your FTDI
  
3. Just to be safe I went into the system32 forlder and deleted the 3 or 4 ftdi dll's files (ftdiport, ftdihub, ft..... )

4. also deleted the companion (same name + GUID) folders from the System32\DriverStore\FileRepository 
(contains the .inf files REMEMBER THIS!!)

5. then install the drivers (I would disconnect your system from the internet here just to be safe and keep 
windows from auto downloading the new drivers again.) 
  - plug the FTDI back in.
  - go back to device manager and right click on your FT232 device (should have a yellow triangle)
  - click update driver, and point it to where ever you downloaded the FTDI driver (point it directly to the .inf file.
not just the folder. You may need to do this twice, once for the ftdiport.inf, and once for the ftdihub.inf)

6. Now go back to the FileRepository folder, and find the ftdiport and hub folder
  - inside each inf file you need to add the following lines
  
  right under [FtdiHw]
  %VID_0403&PID_0000.DeviceDesc%=FtdiPort.NT,FTDIBUS\COMPORT&VID_0403&PID_0000
  
  [FtdiHw.NTamd64]
  %VID_0403&PID_0000.DeviceDesc%=FtdiPort.NTamd64,FTDIBUS\COMPORT&VID_0403&PID_0000
  
  and then way down at the bottom (in the [Strings] section), add a DeviceDesc
  VID_0403&PID_0000.DeviceDesc="USB Serial Port (fixed By Jbagel2)"

7. Do step 6 again but in the other ftdi folders .inf file (up one level).

8. Disconnect your FTDI, wait a sec and plug it back in

9 if everything worked then you it should show back up the the COM/Ports list :) NOT Quite done yet

10. now for this little utility, run the EXE that is sitting the the bin/debug folder

11. have it scan (It should show your device in the text box, with some basic info, it will also automatically 
put the device serial number in the serial number text box (if you have more than one device connected and it picket the wrong one
just copy and pase the right one))

12. now click connectr (It will dump out a little bit of interesting info about your device.)

13. if you look in the text box at a field called "Product ID" Yours is probably "0" being, that is how FTDI screwed 
everyone in the most recent driver, I mean... stopped all the counterfiters..... @#%@#!@$@#$..

14. If your like most people the PID should be "6001". as a littel saftey measure you have to check the checkbox
to enable the ability change the PID... so check the box already..

15. enter 6001 into the box and click the Button button... lol... (was to lazy to change the name as 3am...)

16. it should reswet the device and force a driver reload.. and TADA!!! but. if for some reason you dont see your
device or it fails to connect after you do this. just click disconnect (or just close the app) and unplug the 
ftdi for sec, and plug it back in. reopen the app Scan -> connect. and you should see the PID back to where it should be. :)

Please feel free to message me if you have any issues. or questions.
