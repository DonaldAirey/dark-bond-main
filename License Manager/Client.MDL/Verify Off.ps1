# Calculate the project root from the invocation.
$projectRoot = $(split-path -parent $(split-path -parent $(split-path -parent $SCRIPT:MyInvocation.MyCommand.Path)))
$sn32 = "C:\Program Files (x86)\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools\sn.exe"
$sn64 = "C:\Program Files (x86)\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools\x64\sn.exe"

# Turn off verification for 64 bit applications.
&"$sn64" -q -Vr "$projectRoot\DarkBond\bin\Development\DarkBond.dll"
&"$sn64" -q -Vr "$projectRoot\DarkBond.ClientModel\bin\Development\DarkBond.ClientModel.dll"
&"$sn64" -q -Vr "$projectRoot\DarkBond.Views.MDL\bin\Development\DarkBond.Views.dll"
&"$sn64" -q -Vr "$projectRoot\DarkBond.ViewModels\bin\Development\DarkBond.ViewModels.dll"
&"$sn64" -q -Vr "$projectRoot\DarkBond.ViewModels\bin\Development\es\DarkBond.ViewModels.resources.dll"
&"$sn64" -q -Vr "$projectRoot\DarkBond.ViewModels\bin\Development\fr\DarkBond.ViewModels.resources.dll"
&"$sn64" -q -Vr "$projectRoot\License Manager\DarkBond.LicenseManager.Common\bin\Development\DarkBond.LicenseManager.Common.dll"
&"$sn64" -q -Vr "$projectRoot\License Manager\DarkBond.LicenseManager.Common\bin\Development\es\DarkBond.LicenseManager.Common.resources.dll"
&"$sn64" -q -Vr "$projectRoot\License Manager\DarkBond.LicenseManager.Common\bin\Development\fr\DarkBond.LicenseManager.Common.resources.dll"
&"$sn64" -q -Vr "$projectRoot\License Manager\DarkBond.LicenseManager.ClientDataModel\bin\Development\DarkBond.LicenseManager.ClientDataModel.dll"
&"$sn64" -q -Vr "$projectRoot\License Manager\DarkBond.LicenseManager.Infrastructure\bin\Development\DarkBond.LicenseManager.Infrastructure.dll"
&"$sn64" -q -Vr "$projectRoot\License Manager\DarkBond.LicenseManager.Views.MDL\bin\Development\DarkBond.LicenseManager.Views.dll"

# Turn off verification for 32 bit applications.  This is important for the Visual Studio IDE design surface.
&"$sn32" -q -Vr "$projectRoot\DarkBond\bin\Development\DarkBond.dll"
&"$sn32" -q -Vr "$projectRoot\DarkBond.ClientModel\bin\Development\DarkBond.ClientModel.dll"
&"$sn32" -q -Vr "$projectRoot\DarkBond.Views.MDL\bin\Development\DarkBond.Views.dll"
&"$sn32" -q -Vr "$projectRoot\DarkBond.ViewModels\bin\Development\DarkBond.ViewModels.dll"
&"$sn32" -q -Vr "$projectRoot\DarkBond.ViewModels\bin\Development\es\DarkBond.ViewModels.resources.dll"
&"$sn32" -q -Vr "$projectRoot\DarkBond.ViewModels\bin\Development\fr\DarkBond.ViewModels.resources.dll"
&"$sn32" -q -Vr "$projectRoot\License Manager\DarkBond.LicenseManager.Common\bin\Development\DarkBond.LicenseManager.Common.dll"
&"$sn32" -q -Vr "$projectRoot\License Manager\DarkBond.LicenseManager.Common\bin\Development\es\DarkBond.LicenseManager.Common.resources.dll"
&"$sn32" -q -Vr "$projectRoot\License Manager\DarkBond.LicenseManager.Common\bin\Development\fr\DarkBond.LicenseManager.Common.resources.dll"
&"$sn32" -q -Vr "$projectRoot\License Manager\DarkBond.LicenseManager.ClientDataModel\bin\Development\DarkBond.LicenseManager.ClientDataModel.dll"
&"$sn32" -q -Vr "$projectRoot\License Manager\DarkBond.LicenseManager.Infrastructure\bin\Development\DarkBond.LicenseManager.Infrastructure.dll"
&"$sn32" -q -Vr "$projectRoot\License Manager\DarkBond.LicenseManager.Views.MDL\bin\Development\DarkBond.LicenseManager.Views.dll"
