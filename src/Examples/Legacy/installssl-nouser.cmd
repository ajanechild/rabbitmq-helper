::Please replace the sc.cer and sc.pfx files with your own certificate files
echo If using self-signed certificates, make sure that the CA cert is in the Trusted Root Certification Authorities or installation will fail
Release\Thycotic.RabbitMq.Helper.exe installConnector -hostname=localhost -useSsl=true -skipUserCreate=true -cacertpath=sc.cer -pfxPath=sc.pfx -pfxPw=password1