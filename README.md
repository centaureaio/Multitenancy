# Multitenancy

Lightweight, minimalistic multitenant extension at the top of ASP.NET CORE MVC platform

*Early alpha version, Code/Structure/API changes expected*
---------------------------------------

Lib assumed that tenants have json configuration with following structure

    {
    "Tenants":{  
        "<Tenant-Name>":{  
            <TenantSettingsObject>  
            }  
        }  
    }



