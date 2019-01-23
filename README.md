# Multitenancy
Lightweight, minimalistic multitenant extension at the top of ASP.NET CORE MVC platform

Lib assumed that tenants have json configuration with following structure

`{

  "Tenants":{
  
    "<Tenant-Name>":{
    
        <TenantSettingsObject>
        
        }
        
    }
    
}`
