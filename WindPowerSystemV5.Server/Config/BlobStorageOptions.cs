﻿namespace WindPowerSystemV5.Server.Config;

public class BlobStorageOptions
{
    public string? ConnectionString { get; set; }
    
    public string? ContainerName { get; set; }
    
    public string? StorageAccountName { get; set; }
    
    public string? StorageAccountKey { get; set; }
}
