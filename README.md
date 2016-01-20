## IPUpdater
Uploads your current IP if you have changing IP addresses.

### Required paramaters: 

-u <username>  Username for the FTP access account.

-p <password>  Password for the FTP access account -- note it cannot contain odd characters.

-url <url>     URL of file on server -- format ftp://domain.com/IP.txt

### Optional paramaters

-file <filename>  Sets a specific filename to store the IP both locally and on the server. Default is "IP.txt"

-log <filename>  Sets a specific filename to store the log locally. Default is "IP.log"