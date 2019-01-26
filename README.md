# Multitenancy  (Early alpha version, Code/Structure/API changes expected)

Lightweight, minimalistic multitenant extension at the top of ASP.NET CORE MVC platform

At the current moment this lib basically solve 2 multitenant problems:

1) Allow to configure DI for a specific tenant
2) Allow to use tenant specific settings without making any changes to code, only dealing with initial 'raw' settings data (ex: json files)

---------------------------------------

Lib assumed that tenants have json configuration with following structure

    {
    "Tenants":{  
        "<Tenant-Name>":{  
            <TenantSettingsObject>  
            }  
        }  
    }


or any other configuration format who could provide following keys for settings

`tenants:<TenantName>:<ActualConfig>`

-------------------------------------------

tenants configuration for json file should looks like this

    `{"tenantName1":"regexpForHost1", "tenantName2":"regexpForHost2", ...}`
