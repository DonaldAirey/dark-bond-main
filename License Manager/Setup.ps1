# This describes the certificate
$domain = "www.darkbond.com"
$thumbprint = "b1de5a9684149d42fc8bde5fa5e3b360a8dd52c7"

# This script must be run with administrator privileges before the service can be run in the development environment.
# Project root.
$projectRoot = $(split-path -parent $(split-path -parent $SCRIPT:MyInvocation.MyCommand.Path))

# Import the host certificate and the authority.
$password = ConvertTo-SecureString -String "Dark Bond" -Force –AsPlainText
Import-PfxCertificate –FilePath "$projectRoot\License Manager\Certificates\$domain.pfx" Cert:\LocalMachine\My -Password $password

# Grant access to user the web service certificate to any authenticated user.
$certCN = "CN=$domain,OU=Domain Control Validated"
Try
{
    $WorkingCert = Get-ChildItem CERT:\LocalMachine\My | where {$_.Subject -match $certCN} | sort $_.NotAfter -Descending | select -first 1 -erroraction STOP
    $TPrint = $WorkingCert.Thumbprint
    $rsaFile = $WorkingCert.PrivateKey.CspKeyContainerInfo.UniqueKeyContainerName
}
Catch
{
    "Error: unable to locate certificate for $($CertCN)"
    Exit
}

$keyPath = "C:\ProgramData\Microsoft\Crypto\RSA\MachineKeys\"
$fullPath=$keyPath+$rsaFile
$acl=Get-Acl -Path $fullPath
$permission="Authenticated Users","Read","Allow"
$accessRule=new-object System.Security.AccessControl.FileSystemAccessRule $permission
$acl.AddAccessRule($accessRule)
Try 
{
    Set-Acl $fullPath $acl
    "Success: ACL set on certificate"
}
Catch
{
    "Error: unable to set ACL on certificate"
    Exit
}

# Remove the previous configuration of the ports.
netsh http delete sslcert ipport=0.0.0.0:443
netsh http delete urlacl url="https://+:443/Asset Network/Market Service"
netsh http delete urlacl url="https://+:443/Asset Network/Data Service"
netsh http delete urlacl url="https://+:443/Asset Network/Import Service"
netsh http delete urlacl url="https://+:443/Asset Network""
netsh http delete urlacl url="http://+:80/Asset Network/Market Service"
netsh http delete urlacl url="http://+:80/Asset Network/Data Service"
netsh http delete urlacl url="http://+:80/Asset Network/Import Service"
netsh http delete urlacl url="http://+:80/Asset Network""

# Associate the certificate (and the application) with the SSL port.
netsh http add sslcert ipport=0.0.0.0:443 certhash=$thumbprint appid='{9E1AA0C0-D9EE-4697-83F8-DB90DB42E812}'

# To run this project in Visual Studio without Administrator privileges, grant access to the private key of the 'localhost' certificate to the
# current user.  In addition, this gives the current user permission to use the ports (80 and 443).  This script must be run with elevated
# (Administrator) privileges.
$user = [Environment]::UserName
netsh http add urlacl url="https://+:443/Asset Network/Market Service" user="$user"
netsh http add urlacl url="https://+:443/Asset Network/Data Service" user="$user"
netsh http add urlacl url="https://+:443/Asset Network/Import Service" user="$user"
netsh http add urlacl url="https://+:443/Asset Network" user="$user"
netsh http add urlacl url="http://+:80/Asset Network/Market Service" user="$user"
netsh http add urlacl url="http://+:80/Asset Network/Data Service" user="$user"
netsh http add urlacl url="http://+:80/Asset Network/Import Service" user="$user"
netsh http add urlacl url="http://+:80/Asset Network" user="$user"

# Open up the ports used by this application
netsh advfirewall firewall add rule name="Hypertext Protocol (HTTP-In)" localport=80 dir=in protocol=tcp profile=public,private,domain action=allow
netsh advfirewall firewall add rule name="Secure Socket Tunneling Protocol (HTTPS-In)" localport=443 dir=in protocol=tcp profile=public,private,domain action=allow
netsh advfirewall firewall add rule name="Transmission Control Protocol (TCP-IN)" localport=808 dir=in protocol=tcp profile=public,private,domain action=allow