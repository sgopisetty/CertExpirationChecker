# CertExpirationChecker
Parses "appcmd list site" data and outputs SSL expiration

# Prerequisites
Needs data in a source/input file. 
Run `appcmd list sites` from your server, and place the output in a text file. Specify this file as the input (`-i`) parameter.

# Usage 
`CertExpirationChecker.exe -i inputfile.txt -o output.txt`

Sample `inputfile.txt` 
```
SITE "Default Web Site" (id:1,bindings:http/*:80:,state:Stopped)
SITE "site1.com" (id:12,bindings:http/*:80:site1.com,http/*:80:www.site1.com,http/*:80:site1-binding2.org,http/*:80:www.site1-binding2.org,https/*:443:www.site1.com,https/*:443:site1.com,state:Started)
SITE "site2.org" (id:29,bindings:http/*:80:www.site2.org,http/*:80:site2.org,https/*:443:site2.org,https/*:443:www.site2.org,state:Started)
```

Sample `output.txt` 

`Site: site2.org, DNS: www.site2.org, Expiration: 7/18/2022 4:45:27 AM, Issuer: CN=GTS CA 1C3, O=Google Trust Services LLC, C=US`

