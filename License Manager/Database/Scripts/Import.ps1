# Make sure the arguments are correct
If ($args.Count -eq 0)
{
  Write-Host "usage: Import Development|Staging|Production"
  exit
} 

# Calculate the project root from the invocation.
$slot = $args[0]
$databaseRoot = $(split-path -parent $(split-path -parent $SCRIPT:MyInvocation.MyCommand.Path))
$importer = "C:\Source\Main\License Manager\Importer\Importer\bin\$slot\DarkBond.LicenseManager.Importer.exe"

&$importer -f -i "${databaseRoot}\Data\Configuration.xml"
&$importer -i "${databaseRoot}\Data\Country.xml"
&$importer -i "${databaseRoot}\Data\License Type.xml"
&$importer -i "${databaseRoot}\Data\Province.xml"
&$importer -i "${databaseRoot}\Data\Product.xml"
&$importer -i "${databaseRoot}\Data\Customer.xml"
&$importer -i "${databaseRoot}\Data\License.xml"