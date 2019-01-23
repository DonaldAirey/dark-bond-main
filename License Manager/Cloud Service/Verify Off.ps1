# Calculate the project root from the invocation.
$projectRoot = $(split-path -parent $(split-path -parent $(split-path -parent $SCRIPT:MyInvocation.MyCommand.Path)))
$sn32 = "C:\Program Files (x86)\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools\sn.exe"
$sn64 = "C:\Program Files (x86)\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools\x64\sn.exe"

# Turn off verification for 64 bit applications.
&"$sn64" -q -Vr "$projectRoot\DarkBond.Common\bin\Development\netstandard2.0\DarkBond.Common.dll"
&"$sn64" -q -Vr "$projectRoot\DarkBond.ClientModel\bin\Development\netstandard2.0\DarkBond.ClientModel.dll"
&"$sn64" -q -Vr "$projectRoot\DarkBond.ServiceModel\bin\Development\netstandard2.0\DarkBond.ServiceModel.dll"
&"$sn64" -q -Vr "$projectRoot\License Manager\DarkBond.LicenseManager.Common\bin\Development\netstandard2.0\DarkBond.LicenseManager.Common.dll"
&"$sn64" -q -Vr "$projectRoot\License Manager\DarkBond.LicenseManager.DataService\bin\Development\netstandard2.0\DarkBond.LicenseManager.DataService.dll"
&"$sn64" -q -Vr "$projectRoot\License Manager\DarkBond.LicenseManager.ImportService\bin\Development\netstandard2.0\DarkBond.LicenseManager.ImportService.dll"
&"$sn64" -q -Vr "$projectRoot\License Manager\DarkBond.LicenseManager.PersistentStore\bin\Development\netstandard2.0\DarkBond.LicenseManager.PersistentStore.dll"
&"$sn64" -q -Vr "$projectRoot\License Manager\DarkBond.LicenseManager.ServerDataModel\bin\Development\netstandard2.0\DarkBond.LicenseManager.ServerDataModel.dll"
