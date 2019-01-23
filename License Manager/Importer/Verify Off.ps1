# Calculate the project root from the invocation.
$projectRoot = $(split-path -parent $(split-path -parent $(split-path -parent $SCRIPT:MyInvocation.MyCommand.Path)))
$sn32 = "C:\Program Files (x86)\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools\sn.exe"
$sn64 = "C:\Program Files (x86)\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools\x64\sn.exe"

# Turn off verification for 64 bit applications.
&"$sn64" -q -Vr "$projectRoot\DarkBond\bin\Debug\DarkBond.dll"
&"$sn64" -q -Vr "$projectRoot\DarkBond.ClientModel\bin\Debug\DarkBond.ClientModel.dll"
&"$sn64" -q -Vr "$projectRoot\DarkBond.ServiceClient\bin\Debug\DarkBond.ServiceClient.dll"
&"$sn64" -q -Vr "$projectRoot\DarkBond.ViewModel\bin\Debug\DarkBond.ViewModel.dll"

# Turn off verification for 32 bit applications.  This is important for the Visual Studio IDE design surface.
&"$sn32" -q -Vr "$projectRoot\DarkBond\bin\Debug\DarkBond.dll"
&"$sn32" -q -Vr "$projectRoot\DarkBond.ClientModel\bin\Debug\DarkBond.ClientModel.dll"
&"$sn32" -q -Vr "$projectRoot\DarkBond.ServiceClient\bin\Debug\DarkBond.ServiceClient.dll"
&"$sn32" -q -Vr "$projectRoot\DarkBond.ViewModel\bin\Debug\DarkBond.ViewModel.dll"
