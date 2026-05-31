using System;
using System.Collections.Generic;
using System.Text;

namespace Sevak.Application.DTO.Common;

public class ApiResponseDto<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }
}
