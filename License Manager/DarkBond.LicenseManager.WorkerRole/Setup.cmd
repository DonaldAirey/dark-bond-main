echo "Deleting previous permissions"
netsh http delete urlacl url=https://+:443/license_manager/data_service
netsh http delete urlacl url=https://+:443/license_manager/import_service
netsh http delete urlacl url=https://+:443/license_manager
netsh http delete urlacl url=http://+:80/license_manager/data_service
netsh http delete urlacl url=http://+:80/license_manager/import_service
netsh http delete urlacl url=http://+:80/license_manager
echo "Granting access to HTTPS to NT AUTHORITY\NETWORK SERVICE"
netsh http add urlacl url=https://+:443/license_manager/data_service user="NT AUTHORITY\NETWORK SERVICE"
netsh http add urlacl url=https://+:443/license_manager/import_service user="NT AUTHORITY\NETWORK SERVICE"
netsh http add urlacl url=https://+:443/license_manager user="NT AUTHORITY\NETWORK SERVICE"
netsh http add urlacl url=http://+:80/license_manager/data_service user="NT AUTHORITY\NETWORK SERVICE"
netsh http add urlacl url=http://+:80/license_manager/import_service user="NT AUTHORITY\NETWORK SERVICE"
netsh http add urlacl url=http://+:80/license_manager user="NT AUTHORITY\NETWORK SERVICE"
exit /b 0
