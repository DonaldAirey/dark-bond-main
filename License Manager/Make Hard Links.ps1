# Calculate the project root from the invocation.
$projectRoot = $(split-path -parent $(split-path -parent $SCRIPT:MyInvocation.MyCommand.Path))

# Remove the existing files -- they are sometimes pulled out of version control.
Remove-Item -Force -ErrorAction SilentlyContinue "$projectRoot\License Manager\DarkBond.LicenseManager.DataService\DataModel.xsd"
Remove-Item -Force -ErrorAction SilentlyContinue "$projectRoot\License Manager\DarkBond.LicenseManager.ImportService\DataModel.xsd"
Remove-Item -Force -ErrorAction SilentlyContinue "$projectRoot\License Manager\DarkBond.LicenseManager.PersistentStore\DataModel.xsd"
Remove-Item -Force -ErrorAction SilentlyContinue "$projectRoot\License Manager\Database\DataModel.xsd"

# Create the links (Why is there no powershell command for this?
cmd /c mklink /h "$projectRoot\License Manager\DarkBond.LicenseManager.DataService\DataModel.xsd" "$projectRoot\License Manager\DarkBond.LicenseManager.ServerDataModel\DataModel.xsd"
cmd /c mklink /h "$projectRoot\License Manager\DarkBond.LicenseManager.ImportService\DataModel.xsd" "$projectRoot\License Manager\DarkBond.LicenseManager.ServerDataModel\DataModel.xsd"
cmd /c mklink /h "$projectRoot\License Manager\DarkBond.LicenseManager.PersistentStore\DataModel.xsd" "$projectRoot\License Manager\DarkBond.LicenseManager.ServerDataModel\DataModel.xsd"
cmd /c mklink /h "$projectRoot\License Manager\Database\DataModel.xsd" "$projectRoot\License Manager\DarkBond.LicenseManager.ServerDataModel\DataModel.xsd"
