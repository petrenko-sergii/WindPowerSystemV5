﻿using System.ComponentModel.DataAnnotations;

namespace WindPowerSystemV5.Server.Data;

public class ApiLoginRequest
{
    [Required(ErrorMessage = "Email is required.")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    public required string Password { get; set; }
}
