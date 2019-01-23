# Calculate the project root from the invocation.
$databaseRoot = $(split-path -parent $(split-path -parent $SCRIPT:MyInvocation.MyCommand.Path))
$importer = "C:\Source\Main\License Manager\Importer\Importer\bin\Debug\DarkBond.LicenseManager.Importer.exe"

# Configuration
&$importer -u "CN=Administrator,OU=Aspen Group,O=Dark Bond,DC=darkbond,DC=com" -p VeryLongPassword -i "${databaseRoot}\Test\Channel Recovery.xml"
