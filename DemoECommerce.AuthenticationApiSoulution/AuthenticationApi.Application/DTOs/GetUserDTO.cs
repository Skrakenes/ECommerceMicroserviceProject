using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Application.DTOs;

public record GetUserDTO
(
    int Id,
    [Required] string Name,
    [Required] string TelephoneNumber,
    [Required] string Address,
    [Required, EmailAddress] string Email,
    [Required] string Role
);
