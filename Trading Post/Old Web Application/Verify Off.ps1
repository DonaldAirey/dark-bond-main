# Calculate the project root from the invocation.
$projectRoot = $(split-path -parent $(split-path -parent $(split-path -parent $SCRIPT:MyInvocation.MyCommand.Path)))
$sn32 = "C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.7 Tools\sn.exe"
$sn64 = "C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.7 Tools\x64\sn.exe"

# Turn off verification for 64 bit applications.
&"$sn64" -q -Vr "$projectRoot\DarkBond.Common\bin\Debug\netstandard2.0\DarkBond.Common.dll"
&"$sn64" -q -Vr "$projectRoot\DarkBond.ServiceModel\bin\Debug\netstandard2.0\\DarkBond.ServiceModel.dll"
&"$sn64" -q -Vr "$projectRoot\Trading Post\DarkBond.TradingPost.Common\bin\Debug\netstandard2.0\DarkBond.TradingPost.Common.dll"
&"$sn64" -q -Vr "$projectRoot\Trading Post\DarkBond.TradingPost.Common\bin\Debug\netstandard2.0\DarkBond.TradingPost.Common.dll"
#&"$sn64" -q -Vr "$projectRoot\Trading Post\DarkBond.TradingPost.DataService\bin\Debug\netstandard2.0\DarkBond.TradingPost.DataService.dll"
#&"$sn64" -q -Vr "$projectRoot\Trading Post\DarkBond.TradingPost.ImportService\bin\Debug\netstandard2.0\DarkBond.TradingPost.ImportService.dll"
&"$sn64" -q -Vr "$projectRoot\Trading Post\DarkBond.TradingPost.PersistentStore\bin\Debug\netstandard2.0\DarkBond.TradingPost.PersistentStore.dll"
&"$sn64" -q -Vr "$projectRoot\Trading Post\DarkBond.TradingPost.ServerDataModel\bin\Debug\netstandard2.0\DarkBond.TradingPost.ServerDataModel.dll"

# Turn off verification for 32 bit applications.  This is important for the Visual Studio IDE design surface.
&"$sn32" -q -Vr "$projectRoot\DarkBond.Common\bin\Debug\netstandard2.0\DarkBond.Common.dll"
&"$sn32" -q -Vr "$projectRoot\DarkBond.ServiceModel\bin\Debug\netstandard2.0\\DarkBond.ServiceModel.dll"
&"$sn32" -q -Vr "$projectRoot\Trading Post\DarkBond.TradingPost.Common\bin\Debug\netstandard2.0\DarkBond.TradingPost.Common.dll"
&"$sn32" -q -Vr "$projectRoot\Trading Post\DarkBond.TradingPost.Common\bin\Debug\netstandard2.0\DarkBond.TradingPost.Common.dll"
#&"$sn32" -q -Vr "$projectRoot\Trading Post\DarkBond.TradingPost.DataService\bin\Debug\netstandard2.0\DarkBond.TradingPost.DataService.dll"
#&"$sn32" -q -Vr "$projectRoot\Trading Post\DarkBond.TradingPost.ImportService\bin\Debug\netstandard2.0\DarkBond.TradingPost.ImportService.dll"
&"$sn32" -q -Vr "$projectRoot\Trading Post\DarkBond.TradingPost.PersistentStore\bin\Debug\netstandard2.0\DarkBond.TradingPost.PersistentStore.dll"
&"$sn32" -q -Vr "$projectRoot\Trading Post\DarkBond.TradingPost.ServerDataModel\bin\Debug\netstandard2.0\DarkBond.TradingPost.ServerDataModel.dll"
