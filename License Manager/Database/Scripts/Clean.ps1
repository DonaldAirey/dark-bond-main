# Calculate the project root from the invocation.
$databaseRoot = $(split-path -parent $(split-path -parent $SCRIPT:MyInvocation.MyCommand.Path))
$importer = "C:\Source\Main\License Manager\Importer\Importer\bin\Debug\DarkBond.LicenseManager.Importer.exe"

&$importer -u "CN=Administrator,OU=Aspen Group,O=Dark Bond,DC=darkbond,DC=com" -p VeryLongPassword -i "${databaseRoot}\Test\Remove License.xml"
&$importer -i "${databaseRoot}\Test\Remove Customer.xml"
&$importer -i "${databaseRoot}\Test\Remove Product.xml"
&$importer -i "${databaseRoot}\Test\Remove Province.xml"
&$importer -i "${databaseRoot}\Test\Remove Country.xml"
&$importer -i "${databaseRoot}\Test\Remove License Type.xml"
&$importer -i "${databaseRoot}\Test\Remove Configuration.xml"