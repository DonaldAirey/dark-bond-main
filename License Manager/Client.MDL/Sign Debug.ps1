# Calculate the project root from the invocation.
$projectRoot = "C:\Source\Main"
$sn32 = "C:\Program Files (x86)\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools\sn.exe"
$sn64 = "C:\Program Files (x86)\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools\x64\sn.exe"

# Sign the debug version of the assemblies.
&"$sn64" -q -R "$projectRoot\DarkBond\bin\Development\DarkBond.dll" "$projectRoot\Product Keys\Key Pair.snk"
&"$sn64" -q -R "$projectRoot\DarkBond.ClientModel\bin\Development\DarkBond.ClientModel.dll" "$projectRoot\Product Keys\Key Pair.snk"
&"$sn64" -q -R "$projectRoot\License Manager\DarkBond.LicenseManager.ClientDataModel\bin\Development\DarkBond.LicenseManager.ClientDataModel.dll" "$projectRoot\License Manager\Product Keys\Key Pair.snk"
&"$sn64" -q -R "$projectRoot\License Manager\DarkBond.LicenseManager.Common\bin\Development\DarkBond.LicenseManager.Common.dll" "$projectRoot\License Manager\Product Keys\Key Pair.snk"
&"$sn64" -q -R "$projectRoot\License Manager\DarkBond.LicenseManager.Infrastructure\bin\Development\DarkBond.LicenseManager.Infrastructure.dll" "$projectRoot\License Manager\Product Keys\Key Pair.snk"
&"$sn64" -q -R "$projectRoot\License Manager\DarkBond.LicenseManager.Views.MDL\bin\Development\DarkBond.LicenseManager.Views.dll" "$projectRoot\License Manager\Product Keys\Key Pair.snk"
&"$sn64" -q -R "$projectRoot\DarkBond.ViewModels\bin\Development\DarkBond.ViewModels.dll" "$projectRoot\Product Keys\Key Pair.snk"
&"$sn64" -q -R "$projectRoot\DarkBond.Views.MDL\bin\Development\DarkBond.Views.dll" "$projectRoot\Product Keys\Key Pair.snk"
