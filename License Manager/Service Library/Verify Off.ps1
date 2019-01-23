# Calculate the project root from the invocation.
$projectRoot = $(split-path -parent $(split-path -parent $(split-path -parent $SCRIPT:MyInvocation.MyCommand.Path)))
$sn32 = "C:\Program Files (x86)\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools\sn.exe"
$sn64 = "C:\Program Files (x86)\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools\x64\sn.exe"

# Turn off verification for 64 bit applications.
&"$sn64" -q -Vr "$projectRoot\DarkBond\bin\Debug\DarkBond.dll"
&"$sn64" -q -Vr "$projectRoot\DarkBond.ClientModel\bin\Debug\DarkBond.ClientModel.dll"
&"$sn64" -q -Vr "$projectRoot\DarkBond.ServiceModel\bin\Debug\DarkBond.ServiceModel.dll"
&"$sn64" -q -Vr "$projectRoot\License Manager\DarkBond.LicenseManager\bin\Debug\DarkBond.LicenseManager.dll"
&"$sn64" -q -Vr "$projectRoot\License Manager\DarkBond.LicenseManager.Common\bin\Debug\DarkBond.LicenseManager.Common.dll"
&"$sn64" -q -Vr "$projectRoot\License Manager\DarkBond.LicenseManager.DataService\bin\Debug\DarkBond.LicenseManager.DataService.dll"
&"$sn64" -q -Vr "$projectRoot\License Manager\DarkBond.LicenseManager.ImportService\bin\Debug\DarkBond.LicenseManager.ImportService.dll"
&"$sn64" -q -Vr "$projectRoot\License Manager\DarkBond.LicenseManager.PersistentStore\bin\Debug\DarkBond.LicenseManager.PersistentStore.dll"
&"$sn64" -q -Vr "$projectRoot\License Manager\DarkBond.LicenseManager.ServerDataModel\bin\Debug\DarkBond.LicenseManager.ServerDataModel.dll"

# Turn off verification for 32 bit applications.  This is important for the Visual Studio IDE design surface.
&"$sn32" -q -Vr "$projectRoot\DarkBond\bin\Debug\DarkBond.dll"
&"$sn32" -q -Vr "$projectRoot\DarkBond.ClientModel\bin\Debug\DarkBond.ClientModel.dll"
&"$sn32" -q -Vr "$projectRoot\DarkBond.ServiceModel\bin\Debug\DarkBond.ServiceModel.dll"
&"$sn32" -q -Vr "$projectRoot\License Manager\DarkBond.LicenseManager\bin\Debug\DarkBond.LicenseManager.dll"
&"$sn32" -q -Vr "$projectRoot\License Manager\DarkBond.LicenseManager.Common\bin\Debug\DarkBond.LicenseManager.Common.dll"
&"$sn32" -q -Vr "$projectRoot\License Manager\DarkBond.LicenseManager.DataService\bin\Debug\DarkBond.LicenseManager.DataService.dll"
&"$sn32" -q -Vr "$projectRoot\License Manager\DarkBond.LicenseManager.ImportService\bin\Debug\DarkBond.LicenseManager.ImportService.dll"
&"$sn32" -q -Vr "$projectRoot\License Manager\DarkBond.LicenseManager.PersistentStore\bin\Debug\DarkBond.LicenseManager.PersistentStore.dll"
&"$sn32" -q -Vr "$projectRoot\License Manager\DarkBond.LicenseManager.ServerDataModel\bin\Debug\DarkBond.LicenseManager.ServerDataModel.dll"
