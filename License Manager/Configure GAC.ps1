# In order to use the ENUMs in this project with the Data Model Generator, this library needs to be registered with the GAC.  Otherwise
# the generator will throw an error that it can't find the library.
$projectRoot = $(split-path -parent $(split-path -parent $SCRIPT:MyInvocation.MyCommand.Path))
$gacutil64 = "C:\Program Files (x86)\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools\x64\gacutil.exe"

# Install the library (please build it first)
&"$gacutil64" -i "$projectRoot\License Manager\DarkBond.LicenseManager.Common\bin\Development\netstandard2.0\DarkBond.LicenseManager.Common.dll"
