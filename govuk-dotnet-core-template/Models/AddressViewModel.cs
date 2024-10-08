﻿using System.ComponentModel.DataAnnotations;

namespace govuk_dotnet_core_template.Models;

public sealed class AddressViewModel
{
    public string? SearchAddress { get; set; }
    public string? SearchPostCode { get; set; }

    [Required(ErrorMessage = "Enter address line 1")]
    public string? Address1 { get; set; }

    public string? Address2 { get; set; }

    public string? Address3 { get; set; }

    [Required(ErrorMessage = "Enter town or city")]
    public string? TownCity { get; set; }

    [Required(ErrorMessage = "Enter post code")]
    public string? PostCode { get; set; }

    public string? Country { get; set; }
}