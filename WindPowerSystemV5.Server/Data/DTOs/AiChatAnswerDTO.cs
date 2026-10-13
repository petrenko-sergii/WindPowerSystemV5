namespace WindPowerSystemV5.Server.Data.DTOs;

public class AiChatAnswerDTO
{
    public string Answer { get; set; } = string.Empty;

    public List<TurbineTypeLinkDTO> TurbineTypes { get; set; } = [];
}

public class TurbineTypeLinkDTO
{
    public int Id { get; set; }

    public string Manufacturer { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public float Capacity { get; set; }

    public int TurbineQty { get; set; }

    /// <summary>Relative SPA URL of the turbine type page, e.g. "/turbine-type/3".</summary>
    public string Url { get; set; } = string.Empty;
}
