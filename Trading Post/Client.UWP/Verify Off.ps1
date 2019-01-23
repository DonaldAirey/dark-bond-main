# Calculate the project root from the invocation.
$projectRoot = $(split-path -parent $(split-path -parent $(split-path -parent $SCRIPT:MyInvocation.MyCommand.Path)))
$sn32 = "C:\Program Files (x86)\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools\sn.exe"
$sn64 = "C:\Program Files (x86)\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools\x64\sn.exe"

# Turn off verification for 64 bit applications.
&"$sn64" -q -Vr "$projectRoot\DarkBond\bin\Development\DarkBond.dll"
&"$sn64" -q -Vr "$projectRoot\DarkBond.ClientModel\bin\Development\DarkBond.ClientModel.dll"
&"$sn64" -q -Vr "$projectRoot\DarkBond.ServiceModel\bin\Development\DarkBond.ServiceModel.dll"
&"$sn64" -q -Vr "$projectRoot\License Manager\DarkBond.LicenseManager\bin\Development\DarkBond.LicenseManager.dll"
&"$sn64" -q -Vr "$projectRoot\License Manager\DarkBond.LicenseManager.Common\bin\Development\DarkBond.LicenseManager.Common.dll"
&"$sn64" -q -Vr "$projectRoot\License Manager\DarkBond.LicenseManager.DataService\bin\Development\DarkBond.LicenseManager.DataService.dll"
&"$sn64" -q -Vr "$projectRoot\License Manager\DarkBond.LicenseManager.ImportService\bin\Development\DarkBond.LicenseManager.ImportService.dll"
&"$sn64" -q -Vr "$projectRoot\License Manager\DarkBond.LicenseManager.PersistentStore\bin\Development\DarkBond.LicenseManager.PersistentStore.dll"
&"$sn64" -q -Vr "$projectRoot\License Manager\DarkBond.LicenseManager.ServerDataModel\bin\Development\DarkBond.LicenseManager.ServerDataModel.dll"

# Turn off verification for 32 bit applications.  This is important for the Visual Studio IDE design surface.
&"$sn32" -q -Vr "$projectRoot\DarkBond\bin\Development\DarkBond.dll"
&"$sn32" -q -Vr "$projectRoot\DarkBond.ClientModel\bin\Development\DarkBond.ClientModel.dll"
&"$sn32" -q -Vr "$projectRoot\DarkBond.ServiceModel\bin\Development\DarkBond.ServiceModel.dll"
&"$sn32" -q -Vr "$projectRoot\License Manager\DarkBond.LicenseManager\bin\Development\DarkBond.LicenseManager.dll"
&"$sn32" -q -Vr "$projectRoot\License Manager\DarkBond.LicenseManager.Common\bin\Development\DarkBond.LicenseManager.Common.dll"
&"$sn32" -q -Vr "$projectRoot\License Manager\DarkBond.LicenseManager.DataService\bin\Development\DarkBond.LicenseManager.DataService.dll"
&"$sn32" -q -Vr "$projectRoot\License Manager\DarkBond.LicenseManager.ImportService\bin\Development\DarkBond.LicenseManager.ImportService.dll"
&"$sn32" -q -Vr "$projectRoot\License Manager\DarkBond.LicenseManager.PersistentStore\bin\Development\DarkBond.LicenseManager.PersistentStore.dll"
&"$sn32" -q -Vr "$projectRoot\License Manager\DarkBond.LicenseManager.ServerDataModel\bin\Development\DarkBond.LicenseManager.ServerDataModel.dll"
